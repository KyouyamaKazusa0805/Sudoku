这个解决方案由如下文件夹构成：

| 文件夹/项目                                                  | 框架              | 类型   | 描述                                                         |
| ------------------------------------------------------------ | ----------------- | ------ | ------------------------------------------------------------ |
| [`Sudoku.CodeGen.BitOperations`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.BitOperations) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成 `BitOperationsEx` 类的基本操作（扩展方法）。 |
| [`Sudoku.CodeGen.CodeAnalyzerDefaults`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.CodeAnalyzerDefaults) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动为代码分析器和代码修补器生成固定代码。 |
| [`Sudoku.CodeGen.Deconstruction`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.Deconstruction) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成解构函数。                     |
| [`Sudoku.CodeGen.DiagnosticInfo`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.DiagnosticInfo) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成分析器的诊断 ID 和分类信息。                     |
| [`Sudoku.CodeGen.Equality`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.Equality) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成 `Equals` 相等性比较函数。     |
| [`Sudoku.CodeGen.Formattable`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.Formattable) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成 `ToString` 相关函数。     |
| [`Sudoku.CodeGen.GetEnumerator`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.GetEnumerator) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成 `GetEnumerator` 的迭代器函数。 |
| [`Sudoku.CodeGen.HashCode`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.HashCode) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成 `GetHashCode` 哈希码计算函数。 |
| [`Sudoku.CodeGen.KeyedTuple`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.KeyedTuple) | .NET Standard 2.0 |        | 源代码生成器项目，给项目提供 `KeyedTuple` 记录。             |
| [`Sudoku.CodeGen.PrimaryConstructor`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.PrimaryConstructor) | .NET Standard 2.0 |        | 源代码生成器项目，用来给部分类生成主构造器。                 |
| [`Sudoku.CodeGen.RefStructDefaults`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGen.RefStructDefaults) | .NET Standard 2.0 |        | 源代码生成器项目，用于自动生成引用结构里那些写不写都没事的、没有意义的重写方法（比如 `bool Equals(object?)`）。 |
| [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core) | .NET 5            |        | 对数独基本元素的主要数据结构的实现。                         |
| [`Sudoku.Diagnostics`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics) | .NET 5            |        | 封装了诊断整个解决方案的操作，比如检查代码的行数。           |
| [`Sudoku.Drawing`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Drawing) | .NET 5            |        | 用于绘制和渲染数独盘面。                                     |
| [`Sudoku.Globalization`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Globalization) | .NET 5            |        | 封装了国际化交互的操作和一些常数。                           |
| [`Sudoku.IO`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.IO) | .NET 5            |        | 用于处理数独相关的 IO 操作。                                 |
| [`Sudoku.Recognition`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Recognition) | .NET 5            |        | 封装了关于识别一个数独图片的相关操作。                       |
| [`Sudoku.Solving`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving) | .NET 5            |        | 解题和题目生成的项目。                                       |
| [`Sudoku.Test`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Test) | .NET 5            | 终端   | 仅用于调试代码时。                                           |
| [`Sudoku.Windows`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Windows) | .NET 5            | WPF    | WPF 项目，包含 UI 界面和控件。**已停止维护**               |
| [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) | .NET 5            |      | 提供 .NET 库内相关的扩展的方法、类和结构。                   |
| [`required`](https://github.com/SunnieShine/Sudoku/tree/main/src/required) |                   | /    | 一些帮助我们执行和运行的文件也在此文件夹下。                 |