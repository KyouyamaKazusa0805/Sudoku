﻿﻿﻿﻿﻿﻿# 项目及文件夹介绍
这个解决方案由如下项目和文件夹构成。

## 项目介绍

这些是项目的介绍。其中：

* ~~划掉~~的项目是不再维护的项目。
* **粗体**书写的项目是正在维护的项目。这些项目也会有很多更新，也不一定有更新。这个看后续需不需要添加 API。

| 项目                                                         | 框架版本          | 类型         | 描述                                    |
| ------------------------------------------------------------ | ----------------- | ------------ | --------------------------------------- |
| **[`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core)** | .NET 6            | 类库         | **数独基本元素实现的数据结构的 API**    |
| **[`Sudoku.Core.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core.CodeGen)** | .NET Standard 2.0 | 源代码生成器 | **为 `Sudoku.Core` 项目提供源代码生成** |
| **[`Sudoku.Diagnostics`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics)** | .NET 6            | 类库         | **提供反射解决方案的 API**              |
| **[`Sudoku.Diagnostics.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeGen)** | .NET Standard 2.0 | 源代码生成器 | **为其它项目提供源代码生成**            |
| ~~[`Sudoku.Drawing.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Drawing.Old)~~ | .NET 6            | 类库         | ~~用于绘制和渲染数独盘面~~              |
| ~~[`Sudoku.IO.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.IO.Old)~~ | .NET 6            | 类库         | ~~用于处理数独相关的文件操作~~          |
| **[`Sudoku.Solving`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving)** | .NET 6            | 类库         | **提供人工和计算机解题和出题的 API**    |
| ~~[`Sudoku.Solving.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Old)~~ | .NET 6            | 类库         | ~~解题和出题的项目~~                    |
| [`Sudoku.Test`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Test) | .NET 6            | 控制台       | 仅用于调试、测试代码正确性              |
| **[`Sudoku.UI`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI/Sudoku.UI)** | .NET 6            | Windows UI   | **Windows 平台使用的 UI 程序**          |
| **[`Sudoku.UI.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI.CodeGen)** | .NET Standard 2.0 | 源代码生成器 | **为 `Sudoku.UI` 项目提供源代码生成**   |
| ~~[`Sudoku.Windows.Old`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Windows.Old)~~ | .NET 6            | WPF          | ~~WPF 项目，包含 UI 界面和控件~~        |
| **[`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System)** | .NET 6            | 类库         | **扩展 .NET 内置 API 的项目**           |
| **[`System.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/System.CodeGen)** | .NET Standard 2.0 | 源代码生成器 | **为 `System` 项目提供源代码生成**      |

## 其它文件夹介绍

这些是位于项目的根目录下的文件夹的介绍。

| 文件夹                                                       | 描述                                                         |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [`docs`](https://github.com/SunnieShine/Sudoku/tree/main/src/docs) | 为整个解决方案提供文档资料（即 Wiki 内容）。                 |
| [`src`](https://github.com/SunnieShine/Sudoku/tree/main/src/src) | 整个解决方案的相关程序代码。                                 |
| [`required`](https://github.com/SunnieShine/Sudoku/tree/main/src/required) | 一些编译前的必需文件。                                       |
| [`vssnippet`](https://github.com/SunnieShine/Sudoku/tree/main/src/required/vssnippet) | 存储的是 Visual Studio 的代码片段文件，用来更快输入一些代码块。 |