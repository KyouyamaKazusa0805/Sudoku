﻿﻿# 文件夹介绍
这个解决方案由如下文件夹构成。

## 项目介绍

|        | 项目                                                         | .NET 版本         | 类型  | 描述                                 | 维护状态 |
| ------ | ------------------------------------------------------------ | ----------------- | ----- | ------------------------------------ | -------- |
| 默认值 |                                                              | .NET 6            | 类库  |                                      | 正在维护 |
|        | [`Sudoku.CodeGenerating`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGenerating) | .NET Standard 2.0 |       | 提供生成源代码的服务。               |          |
|        | [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core) |                   |       | 对数独基本元素的主要数据结构的实现。 |          |
|        | [`Sudoku.Diagnostics`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics) |                   |       | 反射解决方案本身内容的存在。         |          |
|        | [`Sudoku.Drawing.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Drawing.Old) |                   |       | 用于绘制和渲染数独盘面。             | 弃用     |
|        | [`Sudoku.IO.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.IO.Old) |                   |       | 用于处理数独相关的文件操作。         | 弃用     |
|        | [`Sudoku.Solving.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Old) |                   |       | 解题和出题的项目。                   | 弃用     |
|        | [`Sudoku.Test`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Test) |                   | 终端  | 仅用于调试代码正确和健壮性。         | 不维护   |
|        | [`Sudoku.Windows.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Windows.Old) |                   | WPF   | WPF 项目，包含 UI 界面和控件。       | 弃用     |
|        | [`Sudoku.UI`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI/Sudoku.UI) |                   | WinUI | WinUI 项目，提供给桌面端的 UI 内容。 |          |
|        | [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) |                   |       | 提供 .NET 库相关的扩展类型和操作。   |          |

## 其它文件夹介绍

| 文件夹                                                       | 描述                                                         |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [`docs`](https://github.com/SunnieShine/Sudoku/tree/main/src/docs) | 为整个解决方案提供文档资料（即 Wiki 内容）。                 |
| [`src`](https://github.com/SunnieShine/Sudoku/tree/main/src/src) | 整个解决方案的相关程序代码。                                 |
| [`required`](https://github.com/SunnieShine/Sudoku/tree/main/src/required) | 一些编译前的必需文件。                                       |
| [`vssnippet`](https://github.com/SunnieShine/Sudoku/tree/main/src/required/vssnippet) | 存储的是 Visual Studio 的代码片段文件，用来更快输入一些代码块。 |