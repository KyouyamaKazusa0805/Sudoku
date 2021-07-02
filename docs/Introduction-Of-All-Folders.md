# 文件夹介绍
这个解决方案由如下文件夹构成。

## 项目介绍

| 项目                                                         | .NET 版本         | 类型 | 描述                                           | 维护状态 |
| ------------------------------------------------------------ | ----------------- | ---- | ---------------------------------------------- | -------- |
| [`Sudoku.CodeGenerating`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CodeGenerating) | .NET Standard 2.0 | DLL  | 源代码生成器项目，提供生成代码的服务。         |          |
| [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core) | .NET 6            | DLL  | 对数独基本元素的主要数据结构的实现。           |          |
| [`Sudoku.Diagnostics`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics) | .NET 6            | DLL  | 检测、诊断解决方案的项目，比如检测代码行数。   |          |
| [`Sudoku.Drawing`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Drawing) | .NET 6            | DLL  | 用于绘制和渲染数独盘面。                       | 即将弃用 |
| [`Sudoku.Globalization`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Globalization) | .NET 6            | DLL  | 封装了国际化的一些常数、常量及数据类型。       |          |
| [`Sudoku.IO`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.IO) | .NET 6            | DLL  | 用于处理数独相关的文件操作。                   |          |
| [`Sudoku.Recognition`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Recognition) | .NET 6            | DLL  | 封装了关于识别一个数独图片的相关操作。         |          |
| [`Sudoku.Solving`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving) | .NET 6            | DLL  | 解题和题目生成的项目。                         |          |
| [`Sudoku.Test`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Test) | .NET 6            | 终端 | 测试项目，仅用于调试时调试代码正确性和健壮性。 | 不维护   |
| [`Sudoku.Windows`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Windows) | .NET 6            | WPF  | WPF 项目，包含 UI 界面和控件。                 | 停止     |
| [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) | .NET 6            | DLL  | 提供 .NET 库内相关的扩展的方法、类和结构。     |          |

## 其它文件夹介绍

| 文件夹                                                       | 描述                                         | 维护状态 |
| ------------------------------------------------------------ | -------------------------------------------- | -------- |
| [`docs`](https://github.com/SunnieShine/Sudoku/tree/main/src/docs) | 为整个解决方案提供文档资料（即 Wiki 内容）。 |          |
| [`src`](https://github.com/SunnieShine/Sudoku/tree/main/src/src) | 整个解决方案的相关程序代码。                 |          |
| [`required`](https://github.com/SunnieShine/Sudoku/tree/main/src/required) | 一些编译前的必需文件。                       |          |