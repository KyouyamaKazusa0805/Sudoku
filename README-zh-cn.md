<center>Language switch to: <a href="README.md">English</a></center>



# 向向的数独解决方案

## 特殊公告

如果我有点忙，或者是有东西对我来说有问题的话，我会告知你，而告知的内容我会写到这里。

> 目前没有。

## 正文

### 简介

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

比如说，你可以使用如下的代码来解一道题：

```csharp
using System;
using Sudoku.Data;
using Sudoku.Solving.Manual;

// Parse a puzzle from the string text.
var grid = SudokuGrid.Parse("........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........");

// Declare a manual solver that uses techniques used by humans to solve a puzzle.
var solver = new ManualSolver();

// To solve a puzzle synchonously.
var analysisResult = solver.Solve(grid);
// If you want to solve the puzzle asynchonously, just change the code to:
//var analysisResult = await solver.SolveAsync(grid, null);

// Output the analysis result.
Console.WriteLine(analysisResult.ToString());
```

以后，我想把这个解决方案用于**几乎所有平台**上。我可能会完成 Win10 APP 项目、安卓项目、常用网络平台上的机器人（比如可能 QQ 啊，哔哩哔哩之类的）。

### 如何编译解决方案

请访问[此链接](https://sunnieshine.github.io/Sudoku/how-to/How-To-Compile-The-Solution)。

### 关于该仓库的克隆仓库（Fork）以及代码更新推并请求（Pull Requests）

当然，你可以复制这个仓库到你的账号下，然后做你想做的任何事情。你可以在基于[ MIT 开源协议](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE)的情况下做你任何想做的事情。不过，由于 Gitee 是从 GitHub 拷贝过来的，所以 Gitee 项目暂时不支持任何的代码推并请求，敬请谅解；不过这两个仓库都可以创建 issue。详情请参考下面的“基本信息”一栏的内容。

另外，这个仓库可能会更新得**非常频繁**（大概一天至少一次代码提交）。

### 基本信息

请查看下面的表格获取更多信息。

| 项目地址 |                                                             | 备注                                                 |
| -------- | ----------------------------------------------------------- | ---------------------------------------------------- |
| GitHub   | [SunnieShine/Sudoku](https://github.com/SunnieShine/Sudoku) |                                                      |
| Gitee    | [SunnieShine/Sudoku](https://gitee.com/SunnieShine/Sudoku)  | 这个仓库从 GitHub 拷贝和同步过来的，是一个备份仓库。 |

| 百科页面 |                                                |
| -------- | ---------------------------------------------- |
| 中文介绍 | [GitHub](https://sunnieshine.github.io/Sudoku) |

| 编码信息       |                                       |
| -------------- | ------------------------------------- |
| 编程语言和版本 | C# 10                                  |
| 框架           | .NET 6                                |
| 缩进           | Tab `\t`                              |
| 集成开发环境   | Visual Studio 2022（17.0 预览版 3） |
| 自然语言支持   | 英语、简体中文                        |

> 我很遗憾我并未创建英文版的 Wiki 内容，因为工程量太大了。我学了很多年的英语，但是对于一些描述（尤其是细节的表达）要翻译成英语仍然有点困难。
>
> 当然，你也可以使用 JetBrains 的 Rider 作为你的 IDE 来开发。随便你用什么都行，甚至是记事本（大笑）
>
> 另外，框架和 IDE 使用版本可能在以后会继续更新。换句话说，它们并非一直都不变。这些信息**仅供参考**。

### 完成列表

* [x] API
* [ ] UI 项目
  * [ ] 桌面项目
    * [x] WPF 项目
    * [ ] ~~UWP 项目（这个可能我不会考虑了）~~
    * [ ] WinUI 3 项目
    * [x] ~~Winform 项目（已实现，但早已过时，已被移除）~~
  * [ ] MAUI 项目
    * [ ] 安卓项目
    * [ ] iOS 项目
    * [ ] Mac Catalyst
    * [ ] 其它
* [ ] 一些常见平台的机器人
  * [ ] 哔哩哔哩（这个可能需要网站自己提供 API 给我们用才行）
  * [x] ~~QQ（这个已经实现了，不过我删掉了……之后再说吧）~~
  * [ ] 微信
  * [ ] 其它内容……
* [ ] Wiki 文档
  * [x] 基本文档
  * [ ] 数独教程
* [x] Visual Studio 数独相关插件
  * [x] 数独解决方案的代码分析器（Sudoku Solution Guardian）

### 项目开源许可证

[麻省理工开源许可证](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE)

### 作者

小向，来自成都的一名四川~~普通大学~~师范大学的本科大学生。

