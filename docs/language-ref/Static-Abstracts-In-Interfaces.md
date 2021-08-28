# 接口里的静态抽象成员

C# 10 开始逐渐完善和支持接口里使用抽象的静态成员，这样就可以让我们使用更灵活的代码来完成更为广泛的操作。

## 引例

考虑一种情况。我们总是喜欢对带有加减乘除运算符的对象完成一些基本的操作和行为，但 `int`、`double` 甚至 `nint` 这样的数据类型在 C# 里都带有 `+` 运算符，如果我们尝试着泛化执行逻辑，以前的 C# 还不能完成这样的任务。C# 10 开始，我们可以使用接口来完成。

首先我们让所有这样的数据类型支持 `INumber<T>` 接口：

```csharp
public interface INumber<T> where T : INumber<T>
{
    static abstract T Default { get; }

    static abstract T operator +(T left, T right);
}
```

然后，我们直接对这样的接口来完成操作：

```csharp
public static T GetSum<T>(T[] list) where T : INumber<T>
{
    var result = T.Default;

    foreach (var element in list)
        result += element;

    return result;
}
```

在这样的代码里，我们用到了 `Default` 属性和 `+` 运算符。C# 以前的接口都无法对这样的成员完成书写和实现，现在有了这样的语法后，我们就可以完美支持这一点了。

> 特别注意语法 `T.Default`。我们使用泛型参数名后直接跟上 `Default` 的方式来表达我要获取的是 `T` 这个数据类型下的 `Default` 属性成员。为什么可以这么写呢？因为 `T` 参数实现了 `INumber<T>` 接口，而这个接口里自带 `Default` 成员。因此，我写 `T.Default` 就意味着我直接取这个属性的值。

## 语法

我们只需要在接口里的、你需要的必须让类型实现的成员前面使用 `static abstract` 修饰符即可。注意，`abstract` 此时是不可缺少的。虽然我们知道 C# 里的接口默认以 `public abstract` 作为抽象的基本修饰符，但在这个特性里，我们不得不追加 `static abstract` 这个修饰符组合来同时表达它是静态成员，且必须在实现类型里完成对这个成员的实现。

稍微注意一点。我们为了让泛型参数类型可以使用运算符操作，我们需要追加一个看起来好像没用、但实际上很有用的泛型约束模式：`where T : INumber<T>`。是的，`INumber<T>` 接口是它自己，而这个泛型约束表示当前泛型参数类型 `T` 必须也实现它自己这个类型。这不是废话吗，难不成还可以不实现？

是的，它完全可以不实现，甚至是毫不相关的情况。不妨思考一下，我们在使用 `IEnumerable<T>` 接口的时候，我们自定义的集合数据类型（比如假定叫 `Class` 类型），你在声明的时候肯定不是这么写的吧：

```csharp
public class Class : IEnumerable<Class>
{
    // ...
}
```

因为 `Class` 可能会封装上一个比较小的集合类型的数据作为底层的数据类型实现，比如我里面有一个 `int[]`，那么 `GetEnumerator` 我恰好就直接把这个 `int[]` 类型的底层字段拿来取迭代器。这个时候，我们实现接口就应该是 `IEnumerable<int>`。可以看到，`Class` 类型的这个 `Class` 此时是和 `IEnumerable<int>` 里的这个 `int` 是完全不一样的两个数据类型。

而我们在为了使用泛型参数类型的运算符抽象的时候，我们写上 `where T : INumber<T>` 的目的纯粹就是为了表达“我自己这个类型就是实现这个接口的泛型参数”。如果没有这层约束的话，那么我的泛型参数 `T` 就可能和接口本身完全没有关系。想一想我们 C# 最开始运算符重载的实现规则是不是有一条这样的话：我们自定义的运算符重载，传入的运算符参数必须至少有一个和这个类型本身一致，或为它的可空类型（即 `T?`，如果这里的 `T` 是值类型的话）。

是的，如果我们不使用泛型参数的“自实现”约束的话，这个参数就可能和接口无关，因此不符合 C# 基本实现的规则，达不到约束的目的。因此 C# 10 规定，你必须得让泛型参数带上“自实现”的约束，才可以使用对泛型参数的运算符重载的抽象化行为。

有了这层约束后，接口就显得非常容易看了。我们再次把刚才的 `INumber<T>` 接口照搬过来：

```csharp
public interface INumber<T> where T : INumber<T>
{
    static abstract T Default { get; }

    static abstract T operator +(T left, T right);
}
```

这一次你再看看，是不是就没问题了？

我们现在来看一下完整的示例，给大家演示一下如何使用接口里的静态抽象成员这一特性。

```csharp
using System;

var a = new A(3);
var b = new A(2);
var c = f(a, b); // Calls that method.

Console.WriteLine(c.ToString());

// Test and simulate the case that uses the generic type to call the operator +.
T f<T>(T l, T r) where T : INumber<T> => l + r;

readonly struct A : INumber<A>
{
    private readonly int _v;

    public A(int v) => _v = v;

    public static A Default => new(0);

    public override string ToString() => _v.ToString();

    public static A operator +(A left, A right) => new(left._v + right._v);
}

interface INumber<T> where T : INumber<T>
{
    static abstract T Default { get; }

    static abstract T operator +(T left, T right);
}
```

看得懂了吗？

## 支持的成员

支持 `static abstract` 的成员，除了字段、构造器（本来接口就没有实例构造器一说）和索引器（索引器本身确实就没有 `static` 一说）以外，别的都可以：

* 属性
* 事件
* 方法
* 运算符
* 类型转换

用起来吧！

## 静态抽象成员的显式接口实现

和实例成员的实现方式一样，如果你不得不隐藏接口成员，你可以使用基本的显式接口实现的模式来完成，不过要记得带上 `static` 关键字。

```csharp
readonly struct A : INumber<A>
{
    private readonly int _v;

    public A(int v) => _v = v;

    public static A Default => new(0);
    static A INumber<A>.Default => Default;

    public override string ToString() => _v.ToString();

    public static A operator +(A left, A right) => new(left._v + right._v);
    static A INumber<A>.operator +(A left, A right) => left + right;
}
```

只不过，估计你第一次看到 `INumber<A>.operator +` 这种语法格式，可能会觉得有点奇妙。
