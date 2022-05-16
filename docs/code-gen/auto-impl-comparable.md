# `AutoImplementsComparableGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成固定的比较操作 `CompareTo` 的正确代码，以方便且正确实现 `IComparable<>` 泛型接口。

**类型名**：[`AutoImplementsComparableGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoImplementsComparableGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

该源代码生成器将对指定的类型生成 `CompareTo` 的比较方法的模板代码，它依赖于 `AutoImplementsComparableAttribute` 特性。

`CompareTo` 方法用于比较两个自定义类型的大小。而有些时候我们在比较期间只需要绑定上一个具体的字段或属性，它自己可以使用 `CompareTo`，于是可以达到使用该成员的比较操作来代替掉整个类型的比较操作。

举个例子，我们假设实现了一个 UTF-8 格式的字符对象，它内部包含一个只读字段 `_char`，为 `byte` 类型，作为基本实现的成员。现在我要让该类型实现 `IComparable<>` 接口，可直接通过 `_char` 字段来比较。

```csharp
readonly struct Utf8Char
{
    private readonly byte _char;

    public int CompareTo(Utf8Char other) => _char.CompareTo(other._char);
}
```

此时我们可以使用该源代码生成器来自动生成对应的代码。首先我们要标记该特性，并且规定上对应的比较字段，然后给类型标记 `partial` 关键字，最后删除 `CompareTo` 方法即可。

```csharp
[AutoImplementsComparable(nameof(_char))]
readonly partial struct Utf8Char
{
    private readonly byte _char;
}
```

另外，该特性传入的参数除了支持字段、属性外，还支持无参非 `void` 返回值类型的方法，如果返回值的类型支持 `IComparable<>` 的比较操作的话，也是可以的。

### `UseExplicitImplementation` 属性

另外，该特性包含一个额外的可选赋值属性：`UseExplicitImplementation`。该属性是 `bool` 类型属性，表示是否实现该接口类型的 `CompareTo` 方法时，用显式接口实现。

显式接口实现和隐式接口实现使用不同的方法签名：

* 显式接口实现：`int IComparable<类型>.CompareTo(类型 other)`
* 隐式接口实现：`访问修饰符 int CompareTo(类型 other)`

如果对该属性赋值 `true` 的话，那么自动生成的代码使用的是显式接口实现的语法，这样可以避免自己实现的相关代码与之发生冲突。但是一般可以不用管，因此该属性保持为 `false` 就行。

## 注意事项

源代码生成器的生成过程较为敏感，因此需要按照规范进行实现。如果出现过程错误，那么生成的代码就会失效，因此源代码生成器可能会忽略掉一些情况，导致和预期不符的情况。比如说传入的特性的参数（成员名）不能匹配上类型里的具体成员的时候（比如名字写错了，大小写失配，或者是根本就没有这样的成员），该参数都会被自动忽略；而且，如果属性没有 `get` 方法的话，也会被忽略掉。

另外，该源代码生成器要求本类型在生成源代码之前就必须实现 `IComparable<>` 接口（即显式给出 `: IComparable<类型>` 的代码段，因为生成的代码会使用 `IComparable<>` 接口里给的方法 `CompareTo` 的文档注释文字，否则生成的源代码里，方法的文档注释不会正常显示出来。
