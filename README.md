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
* Locked Candidates: Pointing, Claiming, Almost Locked Candidates<br/>区块：宫区块、行列区块、欠一数组
* Subset: Naked Subset (Locked Subset & Partial Locked Subset), Hidden Subset<br/>数组：显性数组（死锁数组和区块数组）、隐性数组
* Fishes: Basic (Finned, Sashimi) X-Wing, Swordfish, Jellyfish<br/>鱼：（鳍、退化）二链列、三链列和四链列
* Wings: XY-Wing, XYZ-Wing, (Incompleted) WXYZ-Wing, (Incompleted) VWXYZ-Wing, W-Wing<br/>Wing 结构：XY-Wing、XYZ-Wing、（残缺）WXYZ-Wing、（残缺）VWXYZ-Wing、W-Wing
* Uniqueness: Unique Rectangle (Type 1 to 6), Hidden Rectangle, Avoidable Rectangle (Type 1 to 3),  Hidden Avoidable Rectangle, Unique Loop (Type 1 to 4), Bivalue Universal Grave (Type 1 to 4 and BUG + n)<br/>唯一性：唯一矩形（类型 1 到 6）、可规避矩形（类型 1 到 3）、唯一环（类型 1 到 4）、隐性唯一矩形、隐性可规避矩形、全双值格致死解法（类型 1 到 4 和 BUG + n）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>同数链式结构：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Subset Techniques: Sue de Coq<br/>待定数组：融合待定数组
* Last Resorts: Pattern Overlay Method, Template, Bowman's Bingo, Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、计算机试数



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。
