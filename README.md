# Sunnie's Sudoku Solution (向向的数独解决方案)

## Bulletin for Specials (特殊公告)

> No items here. Wait for appending.
>
> 暂无。等待添加。

## Content (正文)

### Introduction (简介)

A sudoku handling SDK using brute forces and logical techniques. Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

For example, you can use the code like this to solve a puzzle:

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

In the future, I'd like to apply this solution to **almost everything**. I may finish the Win10 app project, android app project, bot on common online platforms (QQ, Bilibili and so on).

以后，我想把这个解决方案用于**几乎所有东西**上。我可能会完成 Win10 APP 项目、安卓项目、常用网络平台上的机器人（比如可能 QQ 啊，哔哩哔哩之类的）。

If I'm busy or something goes bad for me, I'll make a notice to you, whose content will be put into the section "Bulletin for Specials".

如果我有点忙，或者是有东西对我来说有问题的话，我会告知你，而告知的内容我会写到上方的特殊公告栏里。

### Basic Information (基本信息)

| Solution sites<br />项目地址 |                                                             | P.S.<br />备注                                               |
| ---------------------------- | ----------------------------------------------------------- | ------------------------------------------------------------ |
| GitHub                       | [SunnieShine/Sudoku](https://github.com/SunnieShine/Sudoku) |                                                              |
| Gitee                        | [SunnieShine/Sudoku](https://gitee.com/SunnieShine/Sudoku)  | This project is copied & sync'd from GitHub as a backup.<br />这个项目从 GitHub 拷贝和同步过来的，是一个备份项目。 |

| Intro about how to compile the solution<br />关于如何编译解决方案的介绍 |                                                              |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| Chinese & English<br />中英双语介绍                          | [GitHub](https://github.com/SunnieShine/Sudoku/issues/83)    |
| Chinese only<br />中文介绍                                   | [Gitee](https://gitee.com/SunnieShine/Sudoku/wikis/%E5%A6%82%E4%BD%95%E5%90%AF%E5%8A%A8%E5%92%8C%E8%B0%83%E8%AF%95%E9%A1%B9%E7%9B%AE?sort_id=3330593) |

| Wiki<br />百科页面         |                                                           |
| -------------------------- | --------------------------------------------------------- |
| Chinese only<br />中文介绍 | [Gitee](https://gitee.com/SunnieShine/Sudoku/wikis/pages) |

> I'm sorry that I haven't created wiki in English, because it's too complex to me. I have been working for English for many years, but it's so hard to me for some description (especially expression of some detail) to translate into English still.
>
> 我很遗憾我并未创建英文版的 Wiki 内容，因为工程量太大了。我学了很多年的英语，但是对于一些描述（尤其是细节的表达）要翻译成英语仍然有点困难。

| Coding Information<br />编码信息                     |                                                              |
| ---------------------------------------------------- | ------------------------------------------------------------ |
| Programming language<br />编程语言                   | C#                                                           |
| Language version<br />编程语言版本                   | 9.0                                                          |
| Framework<br />框架                                  | .NET 5                                                       |
| Indenting<br />缩进                                  | Tabs (`\t`)<br />Tab                                         |
| Integrated development environment<br />集成开发环境 | Visual Studio 2019 (Version 16.10 Preview)<br />Visual Studio 2019（16.10 预览版） |
| Natural languages support<br />自然语言支持          | English, Simplified Chinese<br />英语、简体中文              |

> You can also use JetBrains Rider as your IDE. However, disappointing, some features contain bugs still now. :(
>
> 当然，你也可以使用 JetBrains 的 Rider 作为你的 IDE 来开发，不过有点难受的地方是，有一些特性目前还有 bug。（不开心脸）
>
> In addition, the framework and IDE version may update in the future; in other words, they aren't changeless. The information is **for reference only**.
>
> 另外，框架和 IDE 使用版本可能在以后会继续更新。换句话说，它们并非一直都不变。这些信息**仅供参考**。

### Author (作者)

Sunnie, from Chengdu, is a normal undergraduate from Sichuan Normal University.

小向，来自成都的一名四川<del>普通大学</del>师范大学的本科大学生。

