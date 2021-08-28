﻿# Lambda 拓展
C# 10 开始扩展 Lambda 表达式。C# 3 诞生 Lambda 表达式，基本够用。不过极少数时候不得不需要一些特殊情况的时候，Lambda 表达式就不够用了。比如

* 给 Lambda 表达式的参数和返回值添加特性；
* 给 Lambda 指定返回值类型，以便直接调用 Lambda。

## 为 Lambda 指定参数和返回值类型

为了明确 Lambda 的参数和返回值类型，我们发明了一种新的语法。原本的语法规则只能指定参数类型，但返回值类型无法固定。返回值类型直接使用和普通方法的语法一样的、先类型后参数表列的方式表示：

```csharp
var f = string () => null;
```

比如这样。返回值是 `string` 类型。

至于参数类型的声明格式，这个是 Lambda 本来的语法，我就不多说了。因为 Lambda 没有方法名，是即时声明即时使用的一种语法，所以如果你记不住写法，可以尝试在参数表列前面补充一个方法名，还原成一个完整的方法声明签名的模式，来看是不是语法写对了。

## 为 Lambda 指定特性

Lambda 在升级写法之后，还可以为参数、返回值或 Lambda 方法本身标记特性了。下面我们来用法和代码。

```csharp
var f = [A] () => { };
```

从这个例子我们看到，我们给 `() => { }` 这个 Lambda 追加了特性 `AAttribute`，这样的话，Lambda 则自动对应到不返回、无参数的方法签名。

请注意，我们直接添加的 `[A]` 特性语法，这表示将整个 Lambda 表达式底层生成的方法标记 `A` 特性。如果想对参数标记特性的话，需要一对小括号：

```csharp
var f = ([A] ref int a) => a++;
```

我们对 `a` 这个参数标记 `[A]` 特性，为了避免和 Lambda 表达式的标记方式冲突，需要一对小括号。

如果标记返回值上的话：

```csharp
var f = [return: A] int (ref int a) => a++;
```

## 一些分析器语法分析的小问题

请注意如下语法：

```csharp
var f = condition ? [A] () => { } : anotherMethod;
```

请问这个写法有没有问题？是的，这个写法是有语法冲突的。C# 6 有 `?[` 这样的运算符，注意 `b ? [A] () => { }` 语法，问号后直接跟了一个开中括号，所以会产生语法冲突。因此，我们不得不需要使用小括号才可以允许：

```csharp
var f = condition ? ([A] () => { }) : anotherMethod;
```

## 自然签名

我们约定，把 Lambda 表达式声明可以推出来的签名称为 Lambda 表达式的自然签名。可问题就在于，如果我们无法知道 Lambda 的参数，就不可能得到它的自然类型。因此，我们给大家五个例子，让大家明白自然签名的推断方式。

```csharp
var f1 = () => default;       // Error
var f2 = x => { };            // Error
var f3 = x => x;              // Error
var f4 = () => 1;             // System.Func<int>
var f5 = string () => null;   // System.Func<string>
```

应该从例子看得出来，前面三个因为无法暗示出参数类型和返回值类型，我们无法得到明确的自然签名；但是后面两个可以。比如 `f4` 变量的 Lambda：`() => 1`，因为返回值是 `int` 类型的字面量，而参数是空的，所以它自动对应无参数返回 `int` 类型的 `Func<int>` 类型；最后一个因为带有返回值类型 `: string`，因此自动对应 `Func<string>`。

## 直接调用 Lambda

Lambda 表达式以前不支持直接调用，现在可以用 `var` 关键字表示其类型了，因此……还是不行 :(

这个主要是因为会增加代码复杂度，降低可读性，以及分析器分析的复杂度等等，所以 C# 10 里仍然没有实现这个功能。不过现在有了 `var` 来表达类型，就不必那么复杂了：

```csharp
var f = static (string s) => Console.WriteLine(s);

f("30");
```

之类的。

## Lambda 对 `Delegate` 和 `object` 类型的隐式转换规则

因为之前说过，Lambda 最终是赋值给一个 `Action` 或者 `Func` 这样的委托类型的，而委托类型默认从 `Delegate` 类型派生，因此所有的委托对象都可以赋值给 `Delegate` 类型（多态）。

可是问题在于，我们无法确定它的类型，就无法直接赋值，比如前面的 `() => default` 语法。下面我们来几个例子。

```csharp
Delegate d1 = 1.GetHashCode; // Ok
Delegate d2 = 2.ToString;    // Error: Due to multiple 'ToString' methods
object o1 = (int x) => x;    // Ok
object o2 = x => x;          // Error: Due to no natural type
```

第一个例子 `1.GetHashCode` 和第三个例子 `(int x) => x` 因为确定了返回值和参数类型，因此包含自然签名，可以转换；但是如果使用 `x => x` 这样的语法，因为没有自然签名，就无法转换。

然后是第二个例子和第一个例子的书写。可能你看着别扭。这是什么意思呢？因为 `GetHashCode` 和 `ToString` 此时是实例方法，需要默认代入一个实例才可参与计算，所以在使用的时候，我们需要优先指定实例是什么。比如这里的 `1.GetHashCode`。`1` 是调用 `GetHashCode` 的实例，而 `GetHashCode` 自身又可以明确确定签名，所以赋值是可以成功的；不过，`2.ToString` 就不行了。因为 `ToString` 调用的实例是 `int` 类型（字面量 `2` 是 `int` 类型的），但 `int` 类型的 `ToString` 方法包含重载，所以无法确定到底是哪个调用，因此也会出错。

特别说明一下 `GetHashCode` 和 `ToString` 这样连参数表列都不带的书写方式。这一点 Lambda 里在 C# 5 开始就有。如果调用的方法（显式方法名，比如 `ToString` 这样带名字的方法，而不是 Lambda 这种没名字的方法）的签名和调用时候需要的变量（参数）本身给定的签名一致，就允许直接写名字：

```csharp
using System;

Func<int> f1 = C.M;
Func<int> f2 = new C().N;

_ = f1();
_ = f2();

class C
{
    public int Value { get; init; }
    
    public static int M() { return 42; }
    public int N() { return Value; }
}
```

比如这样。

## Lambda 对重载方法的最优选取

有些时候，Lambda 现在可以暗示签名之后就会出现更多的转换的问题。

```csharp
static void Invoke(Func<string> f) { }
static void Invoke(Delegate d) { }
static void Invoke(Expression e) { }

static string GetString() => "";
static int GetInt() => 0;

Invoke(GetString); // Invoke(Func<string>)
Invoke(GetInt);    // Invoke(Delegate)

Invoke(() => "");  // Invoke(Func<string>)
Invoke(() => 0);   // Invoke(Expression)
```

比如这样。`Invoke` 方法直接有三个重载，分别是 `Func<string>`、`Delegate` 和 `Expression` 类型接收 Lambda 作为参数。可问题就在于，如果我们直接传入方法名称后，就无法确定执行的方法的类型了。

C# 3 出现 LINQ 的时候，就已经开始允许 Lambda 表达式直接赋值给 `Expression` 类型。`Expression` 类型表示的是这个表达式自身的语法树结构。也就是说，C# 3 允许 Lambda 赋值给 `Expression` 类型，是为了直接把 Lambda 转换成语法树，方便 LINQ 里的语义转换。

那么，既然三种类型都可接收 Lambda 的话，现在调用的方法究竟又是如何选择的呢？很简单，找匹配即可。为了规避 C# 3 语义上给定的语法规则冲突，如果自然签名类型契合的话，自然是选择最优的；但是如果不契合的话，如果是方法名，就选取 `Delegate`；如果是 Lambda，就选取 `Expression`，比如上面这样的例子。因为 C# 3 的 `Expression` 赋值 Lambda 是一个已经存在的语法，所以不能改变；而 `GetInt` 方法既可以赋值给 `Delegate` 类型又可以赋值给 `Expression` 类型的话，会选择 `Delegate` 一边，因为这也是更合适合理的传递参数的规则。

