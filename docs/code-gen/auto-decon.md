# `AutoDeconstructionGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成固定的解构函数 `Deconstruct` 的正确代码。

**类型名**：[`AutoDeconstructionGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoDeconstructionGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

如果类型已经被定义且无法改变的话，这样的类型也挺多，比如 .NET 库里的 API。如果我们要对这样的无法修改的类型实现解构函数的话，可以使用扩展的模块来实现。

我们需要用到 `AutoExtensionDeconstructionAttribute` 特性。该特性用于标记在程序集上，表示对指定类型生成对应的扩展解构函数。扩展解构函数就是扩展方法，然后第一个参数是 `this` 参数，而且是我们需要解构的对象的这个实例。

该特性传入两个参数，第一个参数是解构的对象类型，第二个参数则是解构的成员名称（支持的是属性、字段和无参非 `void` 返回值类型的方法）。

比如我们对程序集标记该特性的实现：

```csharp
[assembly: AutoExtensionDeconstruction(typeof(Point), nameof(Point.X), nameof(Point.Y))]
```

对 `Point` 类型实现解构，解构其中的 `X` 和 `Y` 属性，那么此时，我们只需要这么设置就可以得到完整的解构函数的代码了。

另外，该特性实例化的时候包含两个可选属性：`EmitsInKeyword` 和 `Namespace` 属性。

其中 `EmitsInKeyword` 属性控制和表示的是，是否扩展方法的 `this` 参数要标记 `in` 关键字。这是为了性能上的考虑，如果数据类型较大的话，我们可能会用到这个属性；`Namespace` 则控制的是扩展方法的所在命名空间是哪里。如果不设置的话，默认就是按照这个解构对象自己所处类型的命名空间来作为默认生成的命名空间；设置了之后命名空间会发生改变。

## 注意事项

该源代码生成器要求写入的属性必须包含 `get` 方法。如果属性只有 `set` 的话，源代码生成器将忽略该属性，因为生成的代码不会正确。
