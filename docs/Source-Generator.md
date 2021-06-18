在数独项目里，我们注意到有一个项目的文件目录是 `Sudoku.CodeGen`。这个目录包含若干源代码生成器项目。因为概念相对于别的内容比较新，所以需要单独介绍一下。

**源代码生成器**（Source Generator）是一种机制，它允许我们使用 C# 代码在编译前产生别的、自定义的代码，以达到代码注入的效果。



## 何为代码注入？

那什么是**代码注入**（Code Injection）呢？我举一个稍微偏一点但是比较形象的例子。在多个人比赛之前，裁判就已经定下了一些比赛期间的“黑幕”：比如我给谁谁谁比赛选手做一个标记，裁判在比赛之前要是看到了这个标记，裁判就知道我给他说过，让裁判故意判他赢。

这个例子里，标记好比是编译前用户写代码时给的特性之类的类型标记，而裁判就是这个编译器。在编译程序的时候，一旦发现这个东西，就立刻作出一些额外的补充。我再举个代码层面的例子：

```csharp
[TranslateIntoRecords("int X, int Y")]
public readonly struct Point
{
}
```

假如我有这么一个类型 `Point`，这个类型我不给出任何的实现，但给了一个特性标记：`TranslateIntoRecordsAttribute`。是的，这个特性是我随手写的，库文件里不存在。假定这个特性用来表示，让编译器知道，我一会儿要自动追加一个 `X` 一个 `Y` 这两个属性到项目里，并自动实现诸如 `operator ==`、`operator !=`、`Equals`、`GetHashCode` 之类的方法，而我不从这里直接实现（因为写起来比较复杂，我懒得写，于是就想使用代码注入的方式，把工作交给编译器）。

再举个例子。假如，我实现了一个类型，这个类型底层用了一个数组来封装，比如这样：

```csharp
public unsafe ref struct ValueArray<T> where T : unmanaged
{
    private T* _array;
    
    
    public ValueArray(int length)
    {
        var arr = stackalloc T[length];
        _array = arr;
        Length = length;
    }
    
    
    public readonly int Length { get; }
    
    
    public T this[int index]
    {
        readonly get => _array[index];
        
        set => _array[index] = value;
    }
}
```

可以发现一个显著的问题。索引器 `this[int]` 的参数可能随用户传入负数进去。但我在实现的时候并没有写这句话。我可以考虑使用代码注入的方式，给 getter 和 setter 标记特性（或者直接给属性标记诸如 `ParamInRangeAttribute`），以表示参数必须在合理的范围内，否则抛出异常。这样我也懒得写这些代码了，也可以防止我有些时候代码写错了或者漏写了：

```csharp
public T this[[ParamInRange(0, MaxParamName = nameof(Length))] int index]
{
    readonly get => _array[index];

    set => _array[index] = value;
}
```

然后写上合适的生成代码，最后编译器可以自动加上注入代码：

```csharp
public T this[int index]
{
    readonly get
    {
        if (index < 0 || index >= Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        return _array[index];
    }

    set
    {
        if (index < 0 || index >= Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        _array[index] = value;
    }
}
```

当然，这我们压根没给出实现这样的行为的代码，我也就这么说说，让你明白这个代码注入到底有多强。



## 源代码生成器和 T4 模板

下面我们回到话题，来说一下，如何构造一个源代码生成器。

前文说到，源代码生成器的作用之一，就是用来代码注入。接下来我们为了让大家入门，我们来说另外一种用法：代替 T4 模板。

> 前文的内容还是对入门的朋友们来说太难了，所以这里我们讲简单一点。如果你想学习前文的代码注入的过程，你可以参考 [Source Generator Cookbook](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.cookbook.md) 一节的内容，但有点多，而且全英文，所以需要你多多努力。

T4 模板是一种控制复杂的、生成类似变长泛型参数这种无法从 C# 语言本身做到，但又有规律性的代码。当然，我知道 C++ 里已经有了变长泛型参数这个概念和特性了，但 C# 官方不出意外是不会添加这个特性了，因为没有必要。T4 模板可以生成这样的代码。那么做法很简单，按照 T4 模板的语法规则，将基本的代码的文本写出来，把变化的部分用 `<%= %>`、`<% %>` 这样的标记来替换掉。但是，这样的代码控制缩进非常困难。稍不注意，标记位置写得不对，就会导致代码生成的缩进有问题，而且就算是没问题了，T4 模板代码本身也会很丑。

那么，源代码生成器就这么诞生了。



## 使用源代码生成器

下面我们来介绍一下源代码生成器是怎么用的。

### 第一步：创建动态链接库项目

怎么用呢？我们先创建一个生成动态链接库的项目。

> 在此之前，请确保你的 Visual Studio 是 2019 v16.9 Preview 3 及其以上版本的，因为从这个版本开始，Visual Studio 才开始支持源代码生成器的使用和显示。
>
> 从这个数独项目的 Version 0.2（v20210126）开始，程序的代码里将会使用源代码生成器对部分代码进行生成。如果你的 Visual Studio 2019 不是 16.9 Preview 3 的话，可能导致编译失败。
>
> 如果不使用 16.9 Preview 3 及其以上的版本运行和编译程序的话，你将 100% 获得编译器警告 [CS8032](https://cn.bing.com/search?q=CS8032+C%23+An+instance+of+analyzer+%7b0%7d+cannot+be+created+from+%7b1%7d%3a+%7b2%7d.&form=VSHELP) 一份。
>
> > 老实说，这个功能才出来不久，所以很多 bug 需要修复。比如第一次运行的时候，可能报的 CS8032 警告不论编译多少次程序，不论编译是不是已经成功过，这个编译器警告一直都存在（说白了就是不会刷新），只有重启了才没有；但是隔一会儿又来了，但是运行编译程序则又是成功的。

![步骤 1-1](https://images.gitee.com/uploads/images/2021/0126/170154_7c609a42_1449374.png "第一步.png")

然后选择“Class Library”，点“Next"。

![步骤 1-2](https://images.gitee.com/uploads/images/2021/0126/170209_3f7aaa2b_1449374.png "第二步.png")

这里是让你创建一个项目，给项目取名。项目随便取名，比如 `SourceGenerator` 就好。也继续点击“Next”。

![步骤 1-3](https://images.gitee.com/uploads/images/2021/0228/163222_d804cf69_1449374.png "第三步.png")

最后，注意这一步。**我们需要选择的是 .NET Standard 2.0**。请不要选择 .NET 或 .NET Core、.NET Framework 或其它内容（也不要选择 .NET Standard 2.1），这一点很重要。



### 第二步：创建一个生成器的类，并且实现 `ISourceGenerator` 接口和标记 `GeneratorAttribute` 特性

当我们创建完成了基本的项目后，项目会默认带一个空的类 `Class1`。这个时候随便改个名字就行，比如叫 `HelloWorldGenerator`。

这个时候，请写这样的代码：

```csharp
namespace SourceGenerator
{
    [Generator]
    public sealed class HelloWorldGenerator : ISourceGenerator
    {
    }
}
```

![步骤 2-1](https://images.gitee.com/uploads/images/2021/0126/170236_25566f92_1449374.png "第四步.png")

代码上会报错。很明显的原因：`GeneratorAttribute` 和 `ISourceGenerator` 并不存在。这个时候我们点击弹出的黄色灯泡图标（或者按 <kbd>Alt+Enter</kbd>），可以弹出如下的内容：

![步骤 2-2](https://images.gitee.com/uploads/images/2021/0126/170249_628a0c3d_1449374.png "第五步.png")

此时，请选择“Install package 'Microsoft.CodeAnalysis.Common'”就可以了。然后等待 VS 自动安装。

安装完成后，接口和特性就都会成功导入。但是，此时会有这样的错误信息：

![步骤 2-3](https://images.gitee.com/uploads/images/2021/0126/170300_652bd6f7_1449374.png "第六步.png")

当然，因为我们没实现接口呢。然后我们照着这个实现接口。

```csharp
using Microsoft.CodeAnalysis;

namespace SourceGenerator
{
    [Generator]
    public sealed class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // Code here.
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // This method is unnecessary at present. It's okay to keep it empty.
        }
    }
}
```

这样，接口就不报错了。请注意代码里写的这个注释。这是我加的，因为我们一会儿只在 `Execute` 方法里给实现，而不是在 `Initialize` 方法里。下面这个方法我们这一次用不到，留空就行。

> 留空不是抛 `NotImplementedException`。留空就是保持空代码块就行，不写代码，而不是抛异常。



### 第三步：照着我抄就行了，看我怎么写代码的

是的，看标题就明白我的意思了：

```csharp
using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerator
{
    [Generator]
    public sealed class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // Create a file, where the code is:
            var sb = new StringBuilder();
            sb.Append($@"using System;

namespace GeneratedNamespace
{{
    internal static class GeneratedClass
    {{
        internal static void Hello(string who) => Console.WriteLine($""Hello, {{who}}!"");
    }}
}}");

            // Create the a new file named 'GeneratedFile.cs'.
            context.AddSource("GeneratedFile", sb.ToString());

            // The code above is equivalent to the following one, you can also write as this:
            //   - SourceText: Microsoft.CodeAnalysis.Text
            //   - Encoding: System.Text
            //context.AddSource("GeneratedFile", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
```

行了。抄上去就行。



### 第四步：创建一个控制台程序项目

接着，创建一个测试项目。当然了，这个测试项目那肯定得是可以运行的，对吧。

![步骤 4-1](https://images.gitee.com/uploads/images/2021/0126/170346_e01dc600_1449374.png "第七步.png")

这里随便取名。

![步骤 4-2](https://images.gitee.com/uploads/images/2021/0126/170355_09dccd61_1449374.png "第八步.png")

然后是这里。控制台程序可以是任何版本的，只要不低于 .NET Standard 2.1（就是刚才第一个项目里，你选择的那一项）。

点击“Create”就行了。



### 第五步：修改项目配置文件

这里是整个创建过程的难点。请打开这个新建项目的配置文件（`*.csproj` 的那个文件）。然后添加这样的内容：

```xml
<ItemGroup>
    <ProjectReference Include="..\SourceGenerator\SourceGenerator.csproj"
                      OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```

![步骤 5-1](https://images.gitee.com/uploads/images/2021/0126/170405_f49085ba_1449374.png "第九步.png")

现在，配置文件长这样。然后关闭这个文件。记得保存，而且别写错了。

然后隔一会儿，你就可以发现，你刚添加的这个源代码生成器项目已经添加到了测试项目里了，而且是以一个分析器的形式添加进来的（看显示的位置，是在 Dependencies 的 Analyzers 下面的。这俩单词啥意思呢？依赖和分析器的意思）。

![步骤 5-2](https://images.gitee.com/uploads/images/2021/0126/170417_5bc90161_1449374.png "第十步.png")

> 就这个显示分析器的功能，只有 16.9 Preview 3 才开始有的。这也就是为什么我告诉你必须要这个版本及其以上的才可以的真正原因：分析器都不显示，你从哪里查看生成器和源代码呢？



### 第六步：更替代码

这里就需要我们更新源代码了。我们把刚创建的控制台的程序的文件改成这样：

```csharp
using GeneratedNamespace;

GeneratedClass.Hello(who: "Sunnie");
```

是的，就这两句话。这是 C# 9 提供的新特性：全局 `Main` 方法。当你只写了执行逻辑的时候，编译器会自动创建一个 `Program` 类，并带一个 `Main` 方法；然后这里的执行代码就是被一起塞进 `Main` 方法里的代码了。这个方法自带一个 `string[] args` 参数。所以你压根不用担心你写的代码会出问题，或者没办法用命令行参数。

当然，这里不是讲 C# 新语法的。写完这段代码的时候，可能你会看到报错信息：提示你 `GeneratedNamespace` 命名空间不存在啊，或者 `GeneratedClass` 这个类也不存在啊，这样类似的错误。

![步骤 6-1](https://images.gitee.com/uploads/images/2021/0126/170431_62e92c32_1449374.png "第十一步.png")

别担心，你的源代码生成器已经完成了，所以代码一会儿会自动在编译的时候生成，因此它一会儿就是存在的了。



### 第七步：然后重启 Visual Studio

这一步应该是目前 Source Generator 和 Visual Studio 2019 交互的 bug。我不知道以后 VS 会不会修复这一个问题，但目前必须要这么做才能运行成功。

请重启 VS。

> 你可能会问，这有什么用呢？我也不知道，应该是 bug，而且目前必须这么做才行。毕竟，我手头也没有 VS 的源代码，我也没办法给你解释为什么必须这样；就算我有 VS 的代码，我也看不懂 :D



### 最后一步：运行和调试代码

在前文描述的过程里，我们已经大概表达了所有需要编译和运行一个带有源代码生成器的程序，到底应该怎么写东西。现在是最后一步了。那自然是调试代码。

> 前文如果尝试完成后，发现依旧失败的话，请尝试把代码生成器项目的标准改成 .NET Standard 2.0。.NET Standard 2.1 可以用，但可能第一次需要 2.0 版本的标准进行编译才可以成功。

![步骤 8-1](https://images.gitee.com/uploads/images/2021/0126/170445_5cc4e899_1449374.png "第十二步.png")

这个时候，你就会发现，类名已经不再带有红色波浪线了，而是正确的类的语义着色，这就说明，这个类成功编译出来了。

我们可以点进去看看。

![步骤 8-2](https://images.gitee.com/uploads/images/2021/0126/170453_617a557c_1449374.png "第十三步.png")

你将获得原本我们刚才写的那串代码，最后生成的东西。

> 上面的文字说：“这个文件是被 `SourceGenerator.HelloWorldGenerator` 这个生成器自动生成的，且你无法对其进行修改”。

下面我们来运行一下。

很高兴，我们得到了想要的结果。

![步骤 8-3](https://images.gitee.com/uploads/images/2021/0126/170503_bb0c36c2_1449374.png "第十四步.png")

那么，整个项目我们就完成了。



## 总结

源代码生成器的主要作用是用来在编译前注入别的代码（产生代码）来达到额外的、必须在编译前要完成的功能。源代码生成器有些时候格外重要，但不能乱用。请勿滥用代码注入。

另外，我查了很多的资料，至于我们能否改成现在出的 .NET 5，很抱歉，不能。

![](https://images.gitee.com/uploads/images/2021/0228/162916_cdf98347_1449374.png "Snipaste_2021-02-28_16-26-14.png")

可以参考[这个回答](https://stackoverflow.com/a/65480017/13613782)（原问题是说，我想把 .NET Standard 2.0 这个设定改成 .NET 5，可以吗）。