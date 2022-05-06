# `RefStructOverridensGenerator` 源代码生成器

## 基本介绍

**功能**：对 `ref struct` 类型生成固定的、从 `ValueType` 派生下来的方法的默认重写规则。

**类型名**：[`RefStructOverridensGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/RefStructOverridensGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

对于 `ref struct` 类型而言，由于类型设计特殊，因此它们不能以任何形式装箱拆箱，因此很多时候，它们从 `ValueType` 类型派生下来的代码是无法使用的，例如 `Equals` 方法。`Equals` 方法从基类型派生下来是要求传入一个 `object?` 类型的参数，但由于要参与相等性比较的话，`ref struct` 类型不能以任何形式装/拆箱，因此该参数无论如何都不可能和 `ref struct` 类型的实例相等。

虽然我们可以默认设置返回 `false` 的结果，但该方法此时已经没有任何用途和意义了，因此我们更建议的方式是抛出异常，并设置 `ObsoleteAttribute` 防止用户调用这类型的方法。而该源代码生成器自动提供了生成这种代码的操作，因此无需我们手动实现它们。

从 `ValueType` 派生下来默认有三个方法：

* `Equals`
* `GetHashCode`
* `ToString`

其中后面两个可以重写掉，因为它们可能在某些时候有一定的意义，但第一个没有意义。因此我们建议在不需要它们的时候，不要显式给出相关的重写代码。

该项目会自动对项目里**所有**的 `ref struct`（包括嵌套级别的 `ref struct`）自动生成这三个方法的重写代码。因此，对于该项目而言，你必须对所有的 `ref struct` 类型全部标记上 `partial` 关键字，因为源代码生成器会生成这样的代码。

另外，你可以手动重写掉 `GetHashCode` 和 `ToString` 方法，这样源代码生成器就不会针对于这两个方法生成对应的默认代码了；但是，如果不重写的话，源代码生成器仍然会重写掉它们，而且仍然是抛异常和防止用户调用的默认行为。

该源代码生成器不依赖任何特性或其它类型。
