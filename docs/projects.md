# 项目列表

本页列举整个仓库使用到的项目，以及它的用途和功能。

| 项目名                                                       | 项目类型     | 含义                                                         |
| ------------------------------------------------------------ | ------------ | ------------------------------------------------------------ |
| [`GlobalConfiguration.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/GlobalConfiguration.CodeGen) | 源代码生成器 | 会按照 [`Directory.Build.props`](https://github.com/SunnieShine/Sudoku/blob/main/Directory.Build.props) 文件的设置自动生成全局代码配置的代码。 |
| [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core) | DLL          | 提供基本的数独相关的数据结构的实现，比如数独盘面的基本实现 [`Grid`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.cs) 类型等。 |
| [`Sudoku.Core.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core.CodeGen) | 源代码生成器 | 会生成给 [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core) 项目提供的初始化的不可变代码。 |
| [`Sudoku.Diagnostics.CodeAnalysis`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeAnalysis) | 源代码生成器 | 提供整个解决方案里其它项目的轻量级代码分析功能，能够更方便更有用地约束 API 使用规范和代码风格。 |
| [`Sudoku.Diagnostics.CodeAnalysis.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeAnalysis.CodeGen) | 源代码生成器 | 会生成跟代码分析项目有用的配置和功能代码。                   |
| [`Sudoku.Diagnostics.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeGen) | 源代码生成器 | 为解决方案提供一些基本的、不必手写的源代码的功能性扩展扩展。 |
| [`Sudoku.Solving`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving) | DLL          | 提供数独关于解题操作和技巧搜寻功能的 API。                   |
| [`Sudoku.UI`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI) | Windows UI   | 用于呈现和使用 API 提供一个具体的 UI 级别实现。正在更新中。  |
| [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) | DLL          | 为整个解决方案的别的项目提供关于 .NET 基本库 API 拓展、扩展的相关 API 和功能代码。 |
| [`System.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/System.CodeGen) | 源代码生成器 | 用于生成关于 [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) 项目有关的 API。 |

