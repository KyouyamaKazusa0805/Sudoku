# `AutoOverridesEqualsGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成 `Equals` 方法的重写代码，以及顺带实现一下 `IEquatable<>` 接口里的 `Equals` 那个具体类型作参数的方法。

**类型名**：[`AutoOverridesEqualsGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoOverridesEqualsGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

该源代码生成器依赖于一个叫 `AutoOverridesEqualsAttribute` 的特性。该特性要求传入一系列待比较的成员名（包括无参非 `void` 返回值类型的方法名称），用来参与比较。该源代码生成器会自动按照这些成员名逐一比较对象，来达到判等的过程。注意，如果是引用类型的话，按照 C# 的设计，引用类型在 `Equals` 重写的时候是包含可空类型标记 `?` 的，因此会多一次 `obj is not null` 的判别过程，而值类型不包含此判断。

假设我有一个 `Student` 类型，包含 `Name`、`Age` 和 `_isBoy` 三个成员，那么比较一般会这么写：

```csharp
class Student
{
    private readonly bool _isBoy;

    public string Name { get; }
    public int Age { get; }

    public override bool Equals(object? obj) => Equals(obj as Student);
    public bool Equals(Student? other)
        => other is not null && _isBoy == other._isBoy && Name == other.Name && Age == other.Age;
}
```

我们会自动实现比较方法，传入具体类型的参数。而 `Equals` 方法则默认重写基类型的时候，直接推给我们这个实现的具体类型方法来执行即可。而 `Student` 类型此时是引用类型，因此我们还会在执行具体数据判断之前，优先判断参数 `other` 是否为空。如果不空，再逐一比较每一个需要比较的成员。

现在，我们有了该特性后，可以直接标记此特性来完成比较操作，顺带实现该具体类型的 `Equals` 比较方法。首先我们需要标记此特性，然后给这个类型设置 `partial` 关键字，最后删除掉比较的两个 `Equals` 方法。

```csharp
[AutoOverridesEquals(nameof(_isBoy), nameof(Name), nameof(Age))]
partial class Student
{
    private readonly bool _isBoy;

    public string Name { get; }
    public int Age { get; }
}
```

这样就可以了。

### `UseExplicitlyImplementation` 属性

仔细看前面的实现代码，可以发现具体类型的 `Equals` 方法其实是实现的 `IEquatable<>` 接口里的那个方法签名。如果类型实现了该接口的话，我们可以试着设置该属性为 `true`，这样的话，源代码生成器将会把此时的具体类型的 `Equals` 比较代码改成显式接口实现。

对于需要带有 `in` 参数但又需要实现该接口的类型，该属性会比较有用。

### `EmitSealedKeyword` 属性

该属性表示是否对从基类型派生下来的这个 `Equals` 方法设置 `sealed` 修饰符。如果设置了该属性的话，那么生成的这个代码就被密封起来，派生类型就不可重写此 `Equals` 方法了。

注意这里说的是 `object.Equals(object?)` 这个重写，而不是 `IEquatable<T>.Equals(T?)` 这个。

### `EmitInKeyword` 属性

该属性表示是否对实现的具体类型参数的 `Equals` 方法，给参数标记 `in` 修饰符。对于大值类型来说会比较有用，拷贝起来直接拷贝引用，可以起到一定的性能提升效果。

## 注意事项

实际上，你不实现 `IEquatable<>` 接口好像也可以，源代码生成器照样可以生成合适的代码，因为源代码生成器不会去检测你的类型实现是否规范。
