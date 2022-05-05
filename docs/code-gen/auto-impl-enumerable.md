# `AutoImplementsEnumerableGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成固定的比较操作 `GetEnumerator` 的正确代码，以方便且正确实现 `IEnumerable<>` 泛型接口。

**类型名**：[`AutoImplementsEnumerableGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoImplementsEnumerableGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

该源代码生成器绑定 `AutoImplementsEnumerableAttribute` 特性的使用。假设我有一个数据类型需要实现基本的迭代规则和过程，我们可以使用 `AutoImplementsEnumerableAttribute` 特性标记来自动完成生成代码的过程。

假设我有一个 `Cells` 类型，包含一些单元格信息。我想让对象可以迭代，我们的工作是使用 `Offsets` 属性来完成迭代。

```csharp
struct Cells
{
    private int[] Offsets => ...;

    public IEnumerable<int> GetEnumerator() => Offsets.GetEnumerator();
}
```

那么我们现在可以使用该特性来完成这一点。首先我们需要给类型标记 `AutoImplementsEnumerableAttribute` 特性，传入 `Offsets` 名称，以及迭代的每一个成员自身的类型（本例子里是 `typeof(int)`），然后给类型标记 `partial` 关键字，并删去该代码，即可。

```csharp
[AutoImplementsEnumerable(typeof(int), nameof(Offsets))]
partial struct Cells
{
    private int[] Offsets => ...;
}
```

### `UseExplicitImplementation` 属性

该特性包含一个 `UseExplicitImplementation` 属性，是可选赋值的属性。该属性表示是否实现 `IEnumerable<>` 接口的派生代码的时候使用显式接口实现。如果设置为 `true`，则 `GetEnumerator` 就会被实现为显式接口的规则。

此时请你注意，由于 C# 对于 `IEnumerable<>` 接口的设计（它从 `IEnumerable` 接口派生），因此你在实现的时候还要求实现 `IEnumerable` 接口里的非泛型 `GetEnumerator` 方法。该方法也会被源代码生成器自动实现，但它不论泛型版本的实现是显式的还是隐式的，这个非泛型版本的 `GetEnumerator` 方法都会按显式接口实现来实现，这是在设置此属性的时候改不了的行为。

### `ConversionExpression` 属性

该属性用于控制你返回表达式的具体表达式执行。一般来说，我们直接返回实例的 `GetEnumerator` 方法即可完成，不过有些时候这样的实现是不够灵活的，因此此属性可控制返回表达式的具体形式。

该属性支持 C# 的基本代码作为返回表达式的字符串，还支持感叹号 `!`、星号 `*` 和艾特符号 `@` 的简化表达式书写规则。其中：

* 感叹号 `!` 会被展开为具体元素的类型；
* 星号 `*` 会被展开为 `GetEnumerator` 方法的调用；
* 艾特符号 `@` 会被展开为我们传入的那个用来迭代的对象名称。

举个例子，我们前文提到的 `Offsets.GetEnumerator()` 调用可以被折叠为 `@.*`。其中的 `@` 代替的是 `Offsets` 属性名，而 `*` 代替的是 `GetEnumerator` 方法的调用。因为完整书写起来太长了，因此表达式允许这样进行简化。该属性的默认值是 `"*"`，即直接展开成 `GetEnumerator` 方法调用。

另外，该属性支持使用竖线 `|`（即管道符）来分隔为两个表达式。竖线左侧的表达式用于替换到泛型版本的返回值表达式中，而竖线右侧的表达式用于替换到非泛型版本的返回值表达式里。如果没有这个竖线，我们书写的这个表达式会被视为泛型版本的返回值表达式，而非泛型版本会自动的表达为 `GetEnumerator` 的调用。

举个例子，`((IEnumerable<!>)@[..Count]).*|((IEnumerable<!>)this).*` 表达式可按竖线拆解为两个表达式：

1. `((IEnumerable<!>)@[..Count]).*`
2. `((IEnumerable<!>)this).*`

按照竖线的使用规则，左边的替换到泛型版本的 `GetEnumerator` 方法实现上作为返回值结果，而右侧的则替换到非泛型版本的方法上。其中感叹号会被展开为具体的迭代的元素的类型，艾特符号会被展开为迭代的成员名称，而星号展开为 `GetEnumerator` 方法调用，因此这两个表达式会被源代码生成器直接展开为这样：

```csharp
// 非泛型。
IEnumerator IEnumerable.GetEnumerator()
    => ((IEnumerable<元素类型>)this).GetEnumerator();

// 泛型。
public IEnumerator<元素类型> GetEnumerator()
    => ((IEnumerable<元素类型>)迭代对象[..Count]).GetEnumerator();
```

按照设计和使用规则的话，表达式 `((IEnumerable<!>)@).*` 经常被用于作为此属性的值，这是一种习惯。

## 注意事项

该源代码生成器不支持识别类型的基本实现。换句话说，如果你自己实现了一个 `GetEnumerator` 方法的话，该源代码生成器不会去识别它。如果此时你仍使用显式接口实现的话，就会产生生成代码和自己写的代码的冲突（签名一致）。此时你必须设置 `UseExplicitImplementation` 属性为 `true` 来避免源代码生成器生成错误代码；另外，非泛型版本的 `GetEnumerator` 方法会被无条件生成，因此如果你自己写了一个的话，请删除它，才能使得该源代码生成器正常工作。
