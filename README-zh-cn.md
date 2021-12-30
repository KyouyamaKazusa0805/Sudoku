<center>Language switch to: <a href="README.md">English</a></center>

# 向向的数独解决方案

## 特殊公告

如果我有点忙，或者是有东西对我来说有问题的话，我会告知你，而告知的内容我会写到这里。

> 暂无。

## 正文

### 简介

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

比如说，你可以使用如下的代码来解一道题：

```csharp
#if !NET6_0_OR_GREATER
using System;
using Sudoku.Data;
using Sudoku.Solving.Manual;
#endif

// 读取一个字符串形式的数独盘面的代码信息，并解析为 'SudokuGrid' 类型的对象。
var grid = SudokuGrid.Parse("........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........");

// 声明实例化一个 'ManualSolver' 类型的实例，用于稍后的解题。
var solver = new ManualSolver();

// 以同步的形式解题。
var analysisResult = solver.Solve(grid);
// 如果你想要异步执行，只需要改变代码成这个样子：
//var analysisResult = await solver.SolveAsync(grid, null);

// 输出分析结果。
Console.WriteLine(analysisResult.ToString());
```

> C# 10 开始使用 [`global using` 指令](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives)来按程序集级别添加命名空间的引用，因此 `#if` 和 `#endif` 包裹起来的这段代码此时是可以不写的。虽然项目是按 C# 10 语法书写的，不过这里是为了让你熟悉项目的一些简单的命名空间，我把它们以这种形式写出来了，这样也可以规避编译器产生额外的编译器警告信息。
>
> `NET6_0_OR_GREATER` 是在 .NET 6 里带有的编译符号，用于指示框架的版本是否是 .NET 6 及以上。详情请自行参考 Microsoft Docs 了解[“条件编译符号”的使用](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#conditional-compilation)。

以后，我想把这个解决方案用于**几乎所有平台**上。我可能会完成 Win10 App 项目、安卓项目、常用网络平台上的机器人（比如可能 QQ 啊，哔哩哔哩之类的）。

### 如何编译解决方案

请访问[此链接](https://sunnieshine.github.io/Sudoku/how-to/How-To-Compile-The-Solution)。

### 关于该仓库的克隆仓库（Fork）以及代码更新推并请求（Pull Requests）

当然，你可以复制这个仓库到你的账号下，然后做你想做的任何事情。你可以在基于 [MIT](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE) 开源协议的情况下做你任何想做的事情。不过，由于 Gitee 是从 GitHub 拷贝过来的，所以 Gitee 项目暂时不支持任何的代码推并请求，敬请谅解；不过这两个仓库都可以创建 issue。详情请参考下面的“基本信息”一栏的内容。

另外，这个仓库可能会更新得**非常频繁**（大概一天至少一次代码提交），而仓库备份到 Gitee 则大约是一天到两天一次。

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
| 集成开发环境   | Visual Studio 2022（17.1 预览版 1） |
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
    * [x] ~~WPF 项目（已实现，但早已过时，已被移除）~~
    * [ ] ~~UWP 项目（这个可能我不会考虑了）~~
    * [ ] Windows UI 项目
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
  * [x] ~~数独解决方案的代码分析器（Sudoku Solution Guardian，暂时删掉了）~~

### 项目开源许可证

[麻省理工开源许可证](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE)

### 数独技巧参考

我列举一些我这个解决方案里用到和参考的数独技巧网站。这些网站内容都是我自己写和出品的，所以如果你想要了解数独技巧的具体细节，你可以参考这些链接来了解它们。是中文链接。

* [标准数独技巧教程（视频）_bilibili](https://www.bilibili.com/video/BV1Mx411z7uq)
* [标准数独技巧教程（专栏）_bilibili](https://www.bilibili.com/read/readlist/rl291187)

### 作者

小向，来自成都的一名四川~~普通大学~~师范大学的本科大学生。

