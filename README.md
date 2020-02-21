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

namespace Sudoku.Debugging
{
    /// <summary>
    /// The class aiming to this console application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main function, which is the main entry point
        /// of this console application.
        /// </summary>
        private static void Main()
        {
            // Manual solver tester.
            var solver = new ManualSolver
            {
                AnalyzeDifficultyStrictly = true,
                EnableBruteForce = true,
            };
            var grid = Grid.Parse("201000008000003000000000062603007000052094700700300100400006051900500040007010000");
            var analysisResult = solver.Solve(grid);
            Console.WriteLine($"{analysisResult}");
        }
    }
}
```

And the puzzle solution and analysis result will be displayed on console screen, like this!

然后答案和分析结果都会在你的控制台界面呈现出来，就像这样！

```
Puzzle: 2.1.....8.....3..........626.3..7....52.947..7..3..1..4....6.519..5...4...7.1....
Solving tool: Manual
Solving steps:
 (1.2) Hidden Single (In Block) => r2c8 = 1
 (1.2) Hidden Single (In Block) => r8c2 = 1
 (1.2) Hidden Single (In Block) => r5c1 = 1
 (1.2) Hidden Single (In Block) => r4c4 = 1
 (1.1) Last Digit => r3c6 = 1
 (1.2) Hidden Single (In Block) => r9c4 = 4
 (1.2) Hidden Single (In Block) => r9c1 = 5
 (1.2) Hidden Single (In Block) => r8c9 = 7
 (1.2) Hidden Single (In Block) => r1c8 = 7
 (1.5) Hidden Single (In Column) => r3c1 = 3
 (1.0) Full House => r2c1 = 8
 (1.2) Hidden Single (In Block) => r1c7 = 3
 (1.5) Hidden Single (In Row) => r8c5 = 3
 (1.5) Hidden Single (In Row) => r7c2 = 3
 (1.2) Hidden Single (In Block) => r9c2 = 2
 (1.2) Hidden Single (In Block) => r8c3 = 6
 (1.0) Full House => r7c3 = 8
 (1.5) Hidden Single (In Column) => r9c7 = 6
 (2.6) Pointing => r6c6 <> 8
 (2.8) Claiming => r2c5 <> 5, r3c5 <> 5
 (2.8) Claiming => r4c7 <> 2
 (3.6) Naked Triple => r2c25 <> 4, r2c24 <> 9
 (5.3) Borescoper's Deadly Pattern 3 Digits (Type 1) => r4c7 <> 4, r4c7 <> 5, r4c7 <> 9
...
----------
Technique used:
21 * Full House
6 * Last Digit
23 * Hidden Single (In Block)
2 * Hidden Single (In Column)
2 * Hidden Single (In Row)
1 * Naked single
1 * Pointing
2 * Claiming
1 * Naked Triple
1 * Borescoper's Deadly Pattern 3 Digits (Type 1)
----------
Total solving steps count: 60
Difficulty total: 80.6
Puzzle rating: 5.3/1.2/1.2
Puzzle solution: 261945378875623419349781562693157824152894736784362195438276951916538247527419683
Puzzle has been solved.
Time elapsed: 00:00.00.253
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
* Uniqueness: Unique Rectangle (Type 1 to 6), Hidden Rectangle, Avoidable Rectangle (Type 1 to 3),  Hidden Avoidable Rectangle, Unique Loop (Type 1 to 4), Bivalue Universal Grave (Type 1 to 4 and BUG + n), Borescoper's Deadly Pattern (Type 1 to 4)<br/>唯一性：唯一矩形（类型 1 到 6）、可规避矩形（类型 1 到 3）、唯一环（类型 1 到 4）、隐性唯一矩形、隐性可规避矩形、全双值格致死解法（类型 1 到 4 和 BUG + n）、探长致命结构（类型 1 到 4）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>同数链式结构：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Subset Techniques: Sue de Coq<br/>待定数组：融合待定数组
* Last Resorts: Pattern Overlay Method, Template, Bowman's Bingo, Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、计算机试数
* Other techniques: Gurth's Symmetrical Placement<br/>其它技巧：宇宙法



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。
