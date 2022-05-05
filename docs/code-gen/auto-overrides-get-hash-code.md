# `AutoOverridesGetHashCodeGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成 `GetHashCode` 方法的重写代码。

**类型名**：[`AutoOverridesGetHashCodeGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoOverridesGetHashCodeGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

该源代码生成器依赖于 `AutoOverridesGetHashCodeAttribute` 特性。该特性设置到类型上之后，源代码生成器会对这个类型生成 `GetHashCode` 方法的重写代码，因此不要标记了该特性还自己实现 `GetHashCode` 的代码，否则会导致代码冲突，编译失败。

从 .NET 5 开始，包含 `HashCode` 结构类型。该类型可以提供一种比较简便的哈希码计算过程。比如我要计算一个对象的哈希码，假设我找到了该对象的核心属性或字段，只需要把它们传入到参数里，即可得到哈希码结果。

```csharp
int hashCode = HashCode.Combine(Name, Age, _isBoy);
```

`HashCode.Combine` 方法是纯函数。换句话说，不管你何时何地计算，都会得到相同的结果，只要参数的数值是保持原来的一致的。而内部会启用五个固定的质数（即素数），通过比特移位运算以及异或运算来得到哈希码结果，因此不必手写它们。

源代码生成器生成的代码就是使用的此特性来完成的。稍微注意一下的是，如果参数超过 8 个，`HashCode.Combine` 就不支持了，此时需要实例化一个 `HashCode` 类型的实例，并使用 `Add` 方法添加成员进去。最后使用 `ToHashCode` 方法就可以得到结果了。而源代码生成器是支持这种情况的识别的。换而言之，你可以传入超过 8 个成员。少于 8 个成员（或刚好 8 个成员名称）的时候，生成的代码会自动定位到 `HashCode.Combine` 方法上；超过 8 个成员的时候，生成的代码会自动实例化 `HashCode` 类型对象，并使用 `Add` 来逐个成员的添加，然后调用 `ToHashCode` 得到结果。

```csharp
[AutoOverridesGetHashCode(nameof(_char))]
readonly partial struct Utf8Char
{
}
```

### `EmitSealedKeyword` 属性

该属性表示，是否我给实现的 `GetHashCode` 方法上追加 `sealed` 修饰符来防止对象派生时重写此方法。对于引用类型的话会比较好用，值类型显得比较没有意义：因为值类型没办法使用自定义的派生继承机制，因此 `sealed` 修饰符对于值类型来说没有特殊含义，甚至有编译器错误。

### `Pattern` 属性

该属性控制的是结果表达式。一般默认的话，是使用 `HashCode` 的调用。但是也可以不使用此机制。如果你需要自定义一种更为简单的调用过程的话，可以使用此属性来控制返回表达式。

该属性传入一个字符串，该字符串是满足 C# 基本的语法规则的代码的片段，并且支持如下的记号：

* 星号 `*`：表示 `GetHashCode` 的调用过程；
* 中括号索引运算 `[索引]`：表示获取该特性在传入的参数序列里的第几个成员。举个例子，假设我要给 `Student` 类型的三个成员 `Name`、`Age`、`_isBoy` 来参与计算哈希码。那么我可以使用 `[2]` 表达式来表示 `_isBoy` 字段的引用。

通过此点，我们可以直接控制哈希码的计算表达式。比如 `(int)[0]` 表达式表示将第一个成员强制转换为 `int` 类型，并将结果作为哈希码返回出去。

## 注意事项

不必多说。至少你的中括号索引运算不能超过成员总数量吧。对吧。
