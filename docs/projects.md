# 项目列表

本页列举整个仓库使用到的项目，以及它的用途和功能。

## 类库项目

类库项目指的是它们自身不包含任何主入口点来直接运行起来的项目。类库只提供 API 的封装，包裹众多 API，将其打包成一个项目的项目类型。

属于该类型的项目有：

* [`Sudoku.Analytics`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Analytics)：提供分析数独基本技巧或搜索技巧序列的 API；
* [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core)：提供基本的数独相关的数据结构的实现，如数独盘面的实现 [`Grid`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.cs) 类型等；
* [`Sudoku.Gdip`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Gdip)：提供一种简单、轻量级的数独绘图 API；
* [`Sudoku.Recognition.Behaviors`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Recognition.Behaviors)：提供识别一个用户在游玩产生的盘面对比，并识别一些信息的 API；
* [`Sudoku.Recognition.Imaging`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Recognition.Imaging)：提供识别一个图片里的数独题目的基本 API（实现得非常简单）；
* [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System)：为整个解决方案的别的项目提供关于 .NET 基本库 API 拓展 API 或功能代码。

## 源代码生成器项目

**源代码生成器**（Source Generator）也经常被简称为源生成器或者源码生成器，不过源代码生成器这个名字更贴近它的真正用法和作用对象。正常在看这个术语的时候，如果没有写清楚是源代码的话，读者有可能会不知道源生成器的“源”是什么。

属于该类型的项目有：

* [`RootLevelSourceGeneration`](https://github.com/SunnieShine/Sudoku/tree/main/src/RootLevelSourceGeneration)：为根级别的源代码生成器。它生成一些东西，专门提供给派生下去的源代码生成器项目使用，也提供一些 API；
* [`Sudoku.SourceGeneration`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.SourceGeneration)：为解决方案提供一些基本的、不必手写的源代码的功能性扩展。

## 解决方案项目

这些项目将对底层的 API 进行调用，实现对于控制台、UI 界面框架等的用户交互解决方案。

属于该类型的项目有：

* [`SudokuStudio`](https://github.com/SunnieShine/Sudoku/tree/main/src/SudokuStudio)（UI）：提供一个关于数独基本运算和操作的 UI 级别实现。
* [`SudokuTutorial`](https://github.com/SunnieShine/Sudoku/tree/main/src/SudokuTutorial)（共享项目）：是数独教程的源文件。这些文件虽然放在 Shared Project 模板内，但大部分文件都不是 C# 源代码。

