# `DisableParameterlessConstructorGenerator` 源代码生成器

## 基本介绍

**功能**：对值类型生成无参构造器的禁用代码。

**类型名**：[`DisableParameterlessConstructorGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/DisableParameterlessConstructorGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

对值类型而言，C# 10 允许使用无参构造器。不过值类型有些时候，类型实现起来会比较笨重，因此总是调用构造器进行实例化会导致性能的下降。

设置该源代码生成器的意图是为了防止用户错误使用无参构造器实例化对象，并使用生成的代码，告知用户应该合理使用什么成员来代替它。

该源代码生成器依赖于 `DisableParameterlessConstructorAttribute` 特性。该特性只需要标记到值类型上就可以防止用户使用无参构造器——它会生成一个无参构造器，并且标记了 `ObsoleteAttribute` 特性并提示调用此构造器的错误信息。

### `SuggestedMemberName` 属性

`SuggestedMemberName` 属性表示，提示用户应该用什么成员代替该无参构造器。该参数可以为空，不过就需要赋值另外一个可选属性了。

### `Message` 属性

`Message` 属性表示，错误的提示信息的自定义。如果该无参构造器只是为了防止用户调用，但并不打算告知用户应该用什么成员代替的话，我们可以使用该属性赋值，来自定义错误信息。
