# 条件编译符号介绍
## 基本描述

条件编译符号是一种 C# 里控制代码究竟编译哪个（哪些）代码段的一种机制。我们通过条件编译指令 `#if` 和配套的结束指令 `#endif` 来确定一段代码是否在编译器编译代码的时候编译它们。

我们通过使用 `#if` 指令，然后在后面跟上条件编译符号的方式，来表达“当前符号在项目里存在的时候就参与编译”。

```csharp
#if CONSOLE
using System;
#endif

class Program
{
    private static void Main()
    {
#if CONSOLE
        Console.WriteLine("Hello!");
#endif
    }
}
```

在这个示例里，如果整个项目里包含 `CONSOLE` 符号时，代码就相当于直接输出 `Hello!` 字符串；但如果程序没有 `CONSOLE` 符号，则代码就相当于是没有执行代码的代码段；编译出来后，程序执行刚开始就结束了。

```csharp
class Program
{
    private static void Main()
    {
    }
}
```

说白了就等价于这样。

## 条件编译符号的指定和概念

条件编译符号是条件编译的核心。符号采用全大写的字母序列（当然也可以包含数字和下划线），比如刚才的 `CONSOLE` 就是符合这里说的“全大写”的要求。当没有此符号的时候，代码会被 VS 识别并显示成灰色状态，表示代码段是不可用的。

![](https://images.gitee.com/uploads/images/2021/0317/094732_995c38af_1449374.png "1.png")

设置条件编译符号的方式有两种：一种是通过设置面板上进行设置，另外一种则是通过代码控制条件编译符号。这里我推荐用第二种，也只讲第二种。第一种的配置方式过于简单，以至于无法区分是调试状态还是发布状态。

### 第一步：双击项目

点开解决方案资源管理器，双击创建的项目。如果是 .NET Core 或 .NET 的项目，就会自动打开配置文件；如果是 .NET Framework 的话，双击项目只会折叠/展开项目文件。你会得到类似这样的 XML 文件：

![](https://images.gitee.com/uploads/images/2021/0317/094739_39a6551b_1449374.png "2.png")

然后，录入下面的 XML 代码：

```xml
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <DefineConstants>DEBUG;CONSOLE</DefineConstants>
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|AnyCPU'">
    <DefineConstants>CONSOLE</DefineConstants>
</PropertyGroup>
```

![](https://images.gitee.com/uploads/images/2021/0317/094745_af274a42_1449374.png "3.png")

修改完成后退出即可。此时再次返回 C# 代码文件，就可以看到，代码的灰色就没有了：

![](https://images.gitee.com/uploads/images/2021/0317/094753_49ccbc7d_1449374.png "4.png")

这就表示代码会参与编译了。

## 条件编译符号存在的意义

你的代码如果开源，就会提供给别人使用。当别人无需使用这段代码的时候，可通过删除条件编译符号来达到效果。条件编译符号一旦删除，所有需要此符号的代码段都会变成“不可用”（即灰色）状态。代码就不会参与编译了。

## 稍作补充

为什么要把配置文件改成那样呢？这里要说一下 `csproj` 文件的一些基本知识了。`<PropertyGroup>` 标签控制的是项目里一些固有符号具体取的是什么数值。后面跟上 `Condition` 作为改标签的属性（attribute），后续跟上一个字符串，表示我们需要比较的东西到底是什么。这里写的

```
'$(Configuration)|$(Platform)'=='Debug|AnyCPU'
```

和

```
'$(Configuration)|$(Platform)'=='Release|AnyCPU'
```

就是我们这里需要用到的两种判断模式。前面这种表示我们判断当前编译环境是不是 debug（调试）环境；而后者则是判断当前环境是不是 release（发布）环境。当是 release 环境时，`DEBUG` 这个仅用于调试代码期间的符号就不应该存在，所以在这个标签里，包裹的条件编译符号里我们看不到 `DEBUG`，因此它们里面的条件编译符号会不相同。

如果有多个条件编译符号的话，我们采用的是分号分隔的方式。比如前文用的是 `DEBUG;CONSOLE`。分号前后有没有空格都不影响，但是一般书写的时候是不写这个空格的。换句话说，`DEBUG; CONSOLE` 和 `DEBUG;CONSOLE` 是一样的写法，但一般写成后者这样。

## 符号混用以及混合判断

有时候我们会对多种不同的编译符号进行混用，所以会使用新的写法：`#else` 和 `#elif` 指令。

举个例子。我们当前使用 `WPF`、`CONSOLE` 和 `DEBUG` 三种环境来控制和表达我们当前运行的是什么环境（WPF、还是控制台，还是单纯的调试）。

```csharp
#if CONSOLE
using System;
#elif WPF
using System.Windows;
#elif DEBUG
using System.Diagnostics;
#endif

class Program
{
    private static void Main()
    {
#if CONSOLE && WPF
#error You cannot use both symbols 'CONSOLE' and 'WPF' because they are always opposite.
#elif CONSOLE
        Console.WriteLine("Hello!");
#elif WPF
        MessageBox.Show("Hello!");
#elif DEBUG
        Debug.WriteLine("Hello!");
#else
#error You must contain one symbol in 'CONSOLE', 'WPF', 'DEBUG' as your debugging symbol.
#endif
    }
}

```

下面说明一下代码的意义。这段代码表示，如果我们包含 `CONSOLE` 符号，就会自动通过控制台来输出这段文字；如果没有的话，就去判断当前环境是不是 WPF。因为 WPF 项目是不能和 `CONSOLE` 混用的，所以如果用户混用 `CONSOLE` 和 `WPF` 两个符号的话，编译器会直接产生错误信息 `You cannot use both symbols 'CONSOLE' and 'WPF' because they are always opposite.`。这段文字采用 `#error` 指令来指定，这是 C# 里一个特殊的指令，专门用来显示一个编译器错误。我们添加到 `CONSOLE` 和 `WPF` 都有的编译段落里，表示“只要能够编译这段代码，就必然产生编译器错误来告知用户，这么用是不对的”。

接着，只有 `WPF` 符号的时候，我们就通过 `MessageBox` 弹窗来提示错误信息；如果是 `DEBUG` 的话，就说明不是控制台，也不是 WPF 项目，那么此处就采用 VS 自带的调试结果输出的窗体里进行输出。`Debug` 类包含了一些静态方法就是专门用来在这里输出用的。

最后，如果前文的符号全都没有，编译器会自动产生一条编译器错误，提示用户“你必须指定至少一个符号：要么 `CONSOLE`，要么 `WPF`，要么 `DEBUG`”。