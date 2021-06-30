# SS0606
## 基本信息

**错误编号**：`SS0606`

**错误叙述**：

* **中文**：表达式可使用对位模式匹配。
* **英文**：The expression can be simplifiy via using positional pattern matching.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

对位模式是依赖类 `class`、结构 `struct`、接口 `interface`，以及 POCO 模型（记录结构 `record struct` 和记录类 `record class`）等类型本身实现的解构函数来产生的一种对位模式。对位模式使用小括号语法，将类型的成员通过 `out` 参数赋值的形式进行解构的一种模式匹配。

```csharp
readonly struct R
{
    internal readonly int _a, _b;

    public R(int a, int b)
    {
        _a = a;
        _b = b;
    }

    public readonly void Deconstruct(out int a, out int b)
    {
        a = _a;
        b = _b;
    }

    public readonly override string ToString() => (_a + _b).ToString();
}
```

比如这段代码，我们实现了一个直接取 `_a` 和 `_b` 字段的解构函数。接着，我们可能会有如下的代码进行数值判断：

```csharp
var r = new R(1, 3);

//            ↓ SS0606.
if (r._a == 4 && r._b == 4)
//  ~~~~~~~~~~~~~~~~~~~~~~
{
    Console.WriteLine(r);
}
```

此时，因为我们同时判断了 `_a` 和 `_b` 两个地方，所以我们可更改为对位模式。请改成 `is` 对位模式以消除该分析器对此的分析。

```csharp
if (r is (a: 4, b: 4))
{
    Console.WriteLine(r);
}
```

## 备注

由于分析器的实现算法约定，如果只给出一个等号和不等号表达式的布尔表达式不会通过分析器验证，产生该分析诊断结果。一般来说，只有一个需要判断的数值本身就只需要这样书写代码，而完全不必通过对位模式来完成数值判断：此时的对位模式是没有意义的。

分析器仅对至少两个不等或等号表达式的逻辑与运算作分析。比如上面给出的这样的例子。当然，更多的时候也行。

## 补充说明

对位模式本身应该支持的类型应该如下这些：

* 类（`class`）；
* 结构（`struct`）；
* 接口（`interface`）；
* 记录类（`record class`）；
* 记录结构（`record struct`）。

不过分析器在分析它们的时候略微有不同的地方。比如接口里包含的字段（C# 8 起支持）或属性本身不在这个接口里，而是基接口里的话，分析器可能需要去基接口里都遍历一次；而如果是记录类型的话，除了查看 `Deconstruct` 这个必需的解构函数外，还需要判断主构造器的信息，进行匹配。

而且，所有类型也均可实现对应的扩展方法。如果扩展方法本身也是合法的解构函数的话，这些方法也可参与对位模式匹配。比如对上文的 `R` 结构实现了扩展解构函数：

```csharp
static class REx
{
    public static void Deconstruct(this in R @this, out int a, out int b, out int sum, out int product)
    {
        a = @this._a;
        b = @this._b;
        sum = a + b;
        product = a * b;
    }
}
```

在这种情况下，对位模式也可使用。比如下面这种判断方式：

```csharp
if (r is (a: 1, b: 3, sum: 4, product: 3))
{
    Console.WriteLine(r);
}
```

不过，因为分析器本身无法精确确定对这种任何情况都实现了对位模式的情况（参数列表并不能一一对应到类型里的某个同名属性或同名字段上去），因此这种情况分析器不支持。

另外，在记录类型里（记录类或记录结构），会自动包含一个主构造器。主构造器虽然是构造器，但也可以参与对位模式匹配。

```csharp
record Person(string Name, int Age, Gender Gender);
```

然后假设我们有这样的判断：
```csharp
if (person.Name == "Sunnie" && person.Age == 25 && person.Gender == Gender.Boy)
{
    // ...
}
```

那么依然可以改为对位模式。

```csharp
if (person is (Name: "Sunnie", Age: 25, Gender: Gender.Boy))
{
    // ...
}
```

当然，如果不需要判断一些条件的时候，可用弃元符号 `_` 代替。
```csharp
if (person is (Name: "Sunnie", Age: _, Gender: Gender.Boy))
{
    // ...
}
```

此处的弃元符号仅仅是表达占位。因为主构造器必须要这么多参数，但这个参数正好我们不需要匹配，因此使用弃元表示这个条件永真。