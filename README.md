# Sudoku

标题：**数独**

A sudoku handling SDK using brute forces and logical techniques (Update files gradually).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK（逐渐更新）。



## C# Version and IDE

标题：**C# 版本和 IDE 使用情况**

* C# version: 8.0<br/>C# 版本：8.0
* IDE using: Visual Studio 2019 V16.4<br/>IDE 使用：Visual Studio 2019 V16.4



## How to use

标题：**如何使用**

Clone this repo, and you can take all codes!

只需要你克隆这个仓库就可以带走所有的代码了！



## Demo

标题：**演示**

You can write code in your computer like this:

你可以在你的机器上使用这样的代码：

```csharp
using System;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual;

internal static class Program
{
    private static void Main()
    {
        // Manual solver tester.
        var solver = new ManualSolver
        {
            CheckMinimumDifficultyStrictly = true
        };
        var grid = Grid.Parse("800000050740500900002897000000001400090070020005400000000246100008009063010000004");
        var analysisResult = solver.Solve(grid);
        Console.WriteLine(analysisResult);
    }
}
```

And the puzzle solution and analysis result will be displayed on console screen, like this!

然后答案和分析结果都会在你的控制台界面呈现出来，就像这样！

```
Puzzle: 8......5.74.5..9....2897........14...9..7..2...54........2461....8..9.63.1......4
Solving tool: Manual
Solving steps:
 (1.2) Hidden Single (In Block): r1c6 = 4 in b2
 (1.2) Hidden Single (In Block): r3c8 = 4 in b3
 (1.2) Hidden Single (In Block): r8c1 = 4 in b7
 (1.1) Last Digit: r5c3 = 4
 (1.2) Hidden Single (In Block): r1c3 = 9 in b1
 (1.2) Hidden Single (In Block): r4c4 = 9 in b5
 (1.5) Hidden Single (In Column): r2c3 = 1 in c3
 (1.5) Hidden Single (In Row): r3c9 = 1 in r3
 (1.2) Hidden Single (In Block): r6c8 = 1 in b6
 (1.2) Hidden Single (In Block): r5c1 = 1 in b4
 (1.2) Hidden Single (In Block): r6c9 = 9 in b6
 (2.6) Pointing: 3 in b8\r9 => r9c1 <> 3, r9c3 <> 3
 (2.6) Pointing: 8 in b8\r9 => r9c7 <> 8, r9c8 <> 8
 (2.8) Claiming: 8 in c7\b6 => r4c8 <> 8, r4c9 <> 8, r5c9 <> 8
 (2.8) Claiming: 2 in c9\b3 => r1c7 <> 2
 (4.1) Two-string Kite: 6 in r2\c4 => r5c9 <> 6
 (2.3) Naked single: r5c9 = 5
 (1.2) Hidden Single (In Block): r4c5 = 5 in b5
 (1.2) Hidden Single (In Block): r9c6 = 5 in b8
 (1.2) Hidden Single (In Block): r8c7 = 5 in b9
 (1.2) Hidden Single (In Block): r9c7 = 2 in b9
 (1.2) Hidden Single (In Block): r8c2 = 2 in b7
 (1.2) Hidden Single (In Block): r9c5 = 8 in b8
 (1.2) Hidden Single (In Block): r9c4 = 3 in b8
 (1.2) Hidden Single (In Block): r8c4 = 7 in b8
 (1.0) Full House: r8c5 = 1
 (1.1) Last Digit: r1c4 = 1
 (1.0) Full House: r5c4 = 6
 (1.5) Hidden Single (In Row): r4c1 = 2 in r4
 (1.5) Hidden Single (In Row): r4c2 = 8 in r4
 (4.1) Two-string Kite: 3 in r5\c8 => r2c6 <> 3
 (2.3) Naked single: r2c6 = 2
 (1.2) Hidden Single (In Block): r1c9 = 2 in b3
 (1.1) Last Digit: r6c5 = 2
 (1.2) Hidden Single (In Block): r1c7 = 7 in b3
 (1.5) Hidden Single (In Row): r6c2 = 7 in r6
 (2.8) Claiming: 6 in c2\b1 => r3c1 <> 6
 (4.2) XY-Wing: 3, 6, 7 in r4c3 with r4c8, r9c3 => r9c8 <> 7
 (1.5) Hidden Single (In Row): r9c3 = 7 in r9
 (1.2) Hidden Single (In Block): r9c1 = 6 in b7
 (1.0) Full House: r9c8 = 9
 (1.1) Last Digit: r7c1 = 9
 (1.2) Hidden Single (In Block): r7c2 = 5 in b7
 (1.0) Full House: r7c3 = 3
 (1.0) Full House: r4c3 = 6
 (1.0) Full House: r6c1 = 3
 (1.0) Full House: r3c1 = 5
 (1.2) Hidden Single (In Block): r5c6 = 3 in b5
 (1.0) Full House: r6c6 = 8
 (1.0) Full House: r5c7 = 8
 (1.0) Full House: r6c7 = 6
 (1.0) Full House: r3c7 = 3
 (1.0) Full House: r3c2 = 6
 (1.0) Full House: r1c2 = 3
 (1.0) Full House: r1c5 = 6
 (1.0) Full House: r2c5 = 3
 (1.1) Last Digit: r4c8 = 3
 (1.0) Full House: r4c9 = 7
 (1.1) Last Digit: r2c9 = 6
 (1.0) Full House: r2c8 = 8
 (1.0) Full House: r7c8 = 7
 (1.0) Full House: r7c9 = 8
----------
Technique used:
19 * Full House
6 * Last Digit
21 * Hidden Single (In Block)
1 * Hidden Single (In Column)
5 * Hidden Single (In Row)
2 * Naked single
2 * Pointing
3 * Claiming
2 * Two-string Kite
1 * XY-Wing
----------
Total solving steps count: 62
Difficulty total: 90.4
Puzzle rating: 4.2/1.2/1.2
Puzzle solution: 839164752741532986562897341286951437194673825375428619953246178428719563617385294
Puzzle has been solved.
Time elapsed: 00:00.00.272
```



## Manual Technique Supports

标题：**人工技巧分析支持**

The program supports technique below at present:

目前程序支持如下的技巧项：

* Direct techniques: Hidden Single (Last Digit), Naked Single (Full House)<br/>直观技巧：排除（同数剩余）、唯一余数（同区剩余）
* Locked Candidates: Pointing, Claiming<br/>区块：宫区块、行列区块
* Subset: Naked Subset (Locked Subset & Partial Locked Subset), Hidden Subset<br/>数组：显性数组（死锁数组和区块数组）、隐性数组
* Fishes: (Finned, Sashimi) X-Wing, Swordfish, Jellyfish<br/>鱼：（带鱼鳍、退化）二链列、三链列和四链列
* Wings: XY-Wing, XYZ-Wing, (Incompleted) WXYZ-Wing, (Incompleted) VWXYZ-Wing, W-Wing<br/>Wing 结构：XY-Wing、XYZ-Wing、（残缺）WXYZ-Wing、（残缺）VWXYZ-Wing、W-Wing
* Uniqueness: Unique Rectangle (Type 1 to 6), Hidden Rectangle, Unique Loop (Type 1 to 4), Bivalue Universal Grave (Type 1 to 4 and BUG + n)<br/>唯一性：唯一矩形（类型 1 到 6）、唯一环（类型 1 到 4）、隐性唯一矩形、全双值格致死解法（类型 1 到 4 和 BUG + n）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>双强链：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Locked Set Techniques: Almost Locked Candidates, Sue de Coq<br/>待定数组：欠一数组、融合待定数组
* Last Resorts: Pattern Overlay Method, Template, Bowman Bingo, Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、计算机试数



## Intro to Solution Folders

标题：**解决方案文件夹介绍**

Here displays the introduction to all folders in this whole solution.

这里陈列出所有本解决方案会使用到的文件夹的所有介绍。

* `Sudoku.Core`
    * The implementation of all core data in sudoku, such as a sudoku [grid](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core/Data/Meta/Grid.cs). All extension method in use is also in here.<br/>对于数独里所有核心部件的主要实现，比如[数独盘面类](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core/Data/Meta/Grid.cs)。当然，所有在项目里使用到的扩展方法也都在这里。
* `Sudoku.Core.Old` *
    * Same as [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) project, but use older implementation logic. For example, this project uses LINQ to implement all SSTS (Standard sudoku technique set) step finder, and uses very simple information to describe all information in a grid, which is reduced efficiency of calculation. One of implementations of step finders is [here](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core.Old/Solving/Subsets/SubsetStepFinder.cs#L27).<br/>和 [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) 项目一样，不过这个项目里的所有部件都采用了很老旧的实现方式。举个例子，这个项目使用的是 LINQ 来实现的四大基本数独技巧（排除、唯一余数、区块、数组）的查找，而使用了人类一眼就能看明白的、很简单的信息表示手段来存储一个盘面的每一个细节信息，这样就会降低计算效率。其中一个使用 LINQ 实现的步骤搜索类的执行思路可以点击[此链接](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core.Old/Solving/Subsets/SubsetStepFinder.cs#L27)查看。
* `Sudoku.Solving`
    * Solving module of this whole solution.<br/>控制整个解决方案完成解题操作的所有内容的模块。
* `Sudoku.Generating`
	* Provides generating operations of sudoku.<br/>为整个数独项目提供出题的模块。
* `Sudoku.Drawing`
    * Painting module of this solution, which is used for GDI+.<br/>控制整个解决方案有关绘制图形的模块，一般用于 GDI+ 上。
* `Sudoku.Diagnostics`
    * The diagnostic controlling through all over the solution.<br/>控制整个解决方案执行或调试行为的项目。
* `Sudoku.Solving.BruteForces.Bitwise` *
    * The bitwise brute force solver to a sudoku puzzle.<br/>项目解题期间使用的位运算爆破算法（JCZSolver）的源代码。
* `Sudoku.Debugging`
    * The console program aiming to debugging codes logic of other projects.<br/>旨在解决这整个解决方案里其它项目的 bug 和调试操作的项目。

Note that all projects marked star `*` is an old or unused project in the whole solution. They are only for reference.

需要注意的是，所有标注了星号 `*` 的项目都是整个解决方案里用不上的项目，它们仅作参考。

## Intro to Files

标题：**文件介绍**

Here displays the introduction to files in root folder.

这里陈列出根目录下的文件的基本说明。

* `.editorconfig`
    * Editor configuration file.<br/>用户配置文件（项目整体控制编译器错误等信息的控制信息，以及控制成员名称规范的信息）。
* `Priority of operators.txt`
    * Operators priority through C# language. (P.S. I don't know why I will upload this file, maybe of vital importance?)<br/>C# 语言里的运算符的优先级表。（我也不知道为啥我要上传它，可能它很重要？）



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。
