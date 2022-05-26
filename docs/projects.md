# 项目列表

本页列举整个仓库使用到的项目，以及它的用途和功能。

| 项目名                                                       | 类型         | 介绍                                                         |
| ------------------------------------------------------------ | ------------ | ------------------------------------------------------------ |
| [`GlobalConfiguration.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/GlobalConfiguration.CodeGen) | 源生成器[^1] | 会按照 [`Directory.Build.props`](https://github.com/SunnieShine/Sudoku/blob/main/Directory.Build.props) 文件的设置自动生成全局配置代码。 |
| [`Sudoku.Bot`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Bot) | 控制台       | QQ 官方机器人项目。**正在更新中。**                          |
| [`Sudoku.Bot.Communication`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Bot.Communication) | 类库[^2]     | 提供机器人的 API 抽象。**正在更新中。**                      |
| [`Sudoku.CommandLine`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CommandLine) | 控制台       | 提供一个关于数独基本运算和操作的控制台实现，偶尔也被用来调试代码。 |
| [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core) | 类库         | 提供基本的数独相关的数据结构的实现，如数独盘面的实现 [`Grid`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.cs) 类型等。 |
| [`Sudoku.Diagnostics.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeGen) | 源生成器     | 为解决方案提供一些基本的、不必手写的源代码的功能性扩展。     |
| [`Sudoku.Drawing`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Drawing) | 类库         | 提供一种简单、轻量级的数独绘图 API。                         |
| [`Sudoku.Recognition`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Recognition) | 类库         | 提供一个基本的、数独图片识别功能的 API。                     |
| [`Sudoku.Solving.Manual`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Manual) | 类库         | 提供数独关于解题操作和技巧搜寻功能的 API。                   |
| [`Sudoku.UI`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI) | Windows UI   | 用于呈现和使用 API 提供一个具体的 UI 级别实现。**正在更新中。** |
| [`Sudoku.UI.Drawing`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI.Drawing) | 类库         | 为 `Sudoku.UI` 项目提供绘图 API。                            |
| [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) | 类库         | 为整个解决方案的别的项目提供关于 .NET 基本库 API 拓展 API 或功能代码。 |

[^1]: **源代码生成器**（Source Generator）也经常被简称为源生成器或者源码生成器，不过源代码生成器这个名字更贴近它的真正用法和作用对象。正常在看这个术语的时候，如果没有写清楚是源代码的话，读者有可能会不知道源生成器的“源”是什么。
[^2]: 类库就是生成 `*.dll` 文件的项目。
