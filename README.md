# Sunnie's Sudoku Solution

标题：**向向的数独解决方案（SSS）**

A sudoku handling SDK using brute forces and logical techniques (Update files gradually). Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking.

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK（逐渐更新）。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证。



## C# Version and IDE

标题：**C# 版本和 IDE 使用情况**

* C# version: 8.0<br/>C# 版本：8.0
* Framework using: .NET Core 3.1<br/>框架使用：.NET Core 3.1
* IDE using: Visual Studio 2019 V16.4<br/>IDE 使用：Visual Studio 2019 V16.4



## How to use

标题：**如何使用**

Clone this repo, and you can take all codes! This solution does not provide you with GUI at present because DNC may not support the form designer for some internal bugs. I may write forms part in later version of IDE.

只需要你克隆这个仓库就可以带走所有的代码了！这个解决方案目前还不支持可视化界面，因为 .NET Core 框架由于一些内部 bug，暂时不支持窗体设计器。我可能将在下一个 IDE 更新版本才会写窗体部分。



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
            var solver = new ManualSolver();
            var grid = Grid.Parse("700000020000300405006000097000006070035708140060400000690000700801005000070000001");
            var analysisResult = solver.Solve(grid);
            Console.WriteLine($"{analysisResult:-#!.}");
        }
    }
}
```

And the puzzle solution and analysis result will be displayed on console screen, like this!

然后答案和分析结果都会在你的控制台界面呈现出来，就像这样！

```
Puzzle: 7......2....3..4.5..6....97.....6.7..357.814..6.4.....69....7..8.1..5....7......1
Solving tool: Manual
Solving steps:
   1,  (1.2) Hidden Single (In Block): r2c8 = 1 in b3
   2,  (1.2) Hidden Single (In Block): r9c1 = 5 in b7
   3,  (1.5) Hidden Single (In Column): r3c1 = 3 in c1
   4,  (1.5) Hidden Single (In Column): r4c1 = 4 in c1
   5,  (1.5) Hidden Single (In Column): r6c1 = 1 in c1
   6,  (1.2) Hidden Single (In Block): r7c8 = 5 in b9
   7,  (1.2) Hidden Single (In Block): r5c9 = 6 in b6
   8,  (1.2) Hidden Single (In Block): r1c7 = 6 in b3
   9,  (1.2) Hidden Single (In Block): r1c9 = 3 in b3
  10,  (1.0) Full House: r3c7 = 8
  11,  (1.2) Hidden Single (In Block): r2c5 = 6 in b2
  12,  (1.2) Hidden Single (In Block): r2c6 = 7 in b2
  13,  (1.2) Hidden Single (In Block): r6c3 = 7 in b4
  14,  (1.1) Last Digit: r8c5 = 7
  15,  (2.6) Pointing: 8 in b2\r1 => r1c23 <> 8
  16,  (2.6) Pointing: 9 in b2\r1 => r1c3 <> 9
  17,  (2.3) Naked single: r1c3 = 4
  18,  (1.2) Hidden Single (In Block): r8c2 = 4 in b7
  19,  (1.2) Hidden Single (In Block): r7c9 = 4 in b9
  20,  (1.2) Hidden Single (In Block): r9c8 = 8 in b9
  21,  (1.2) Hidden Single (In Block): r8c8 = 6 in b9
  22,  (1.0) Full House: r6c8 = 3
  23,  (1.2) Hidden Single (In Block): r4c5 = 3 in b5
  24,  (1.2) Hidden Single (In Block): r4c4 = 1 in b5
  25,  (1.5) Hidden Single (In Row): r8c7 = 3 in r8
  26,  (1.2) Hidden Single (In Block): r6c5 = 5 in b5
  27,  (1.2) Hidden Single (In Block): r4c7 = 5 in b6
  28,  (1.1) Last Digit: r9c4 = 6
  29,  (1.5) Hidden Single (In Row): r6c9 = 8 in r6
  30,  (2.8) Claiming: 2 in r2\b1 => r3c2 <> 2
  31,  (2.6) Pointing: 2 in b7\c3 => r2c3 <> 2, r4c3 <> 2
  32,  (4.0) Finned Swordfish: 2 in c467\r369 (With fin cells: { r7c46, r8c4 }) => r9c5 <> 2
  33,  (4.0) Finned Swordfish: 9 in c467\r169 (With a fin cell: r8c4) => r9c5 <> 9
  34,  (2.3) Naked single: r9c5 = 4
  35,  (1.1) Last Digit: r3c6 = 4
  36,  (3.2) X-Wing: 9 in r69\c67 => r1c6 <> 9
......
----------
Technique used:
20 * Full House
6 * Last Digit
21 * Hidden Single (In Block)
3 * Hidden Single (In Column)
2 * Hidden Single (In Row)
3 * Naked single
3 * Pointing
1 * Claiming
1 * X-Wing
2 * Finned Swordfish
----------
Total solving steps count: 62
Difficulty total: 88.0
Puzzle rating: 4.0/1.2/1.2
Puzzle solution: 754981623928367415316524897489136572235798146167452938692813754841275369573649281
Puzzle has been solved.
Time elapsed: 00:00.00.154
```

> Format strings in the analysis result is shown in the description of file *How to use 'AnalysisResult'.md*.
>
> 分析结果的格式化字符串可以参照“How to use 'AnalysisResult'.md”文件里呈现的描述。



## Manual Technique Supports

标题：**人工技巧分析支持**

The program supports technique below at present:

目前程序支持如下的技巧项：

* Direct techniques: Hidden Single (Last Digit), Naked Single (Full House)<br/>直观技巧：排除（同数剩余）、唯一余数（同区剩余）
* Locked Candidates: Pointing, Claiming, Almost Locked Candidates<br/>区块：宫区块、行列区块、欠一数组
* Subset: Naked Subset (Locked Subset & Partial Locked Subset), Hidden Subset<br/>数组：显性数组（死锁数组和区块数组）、隐性数组
* Fishes: (Finned, Sashimi) X-Wing, Swordfish, Jellyfish, (Finned, Sashimi) Franken X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan, (Finned, Sashimi) Mutant X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan<br/>鱼：（鳍、退化）二链列、三链列和四链列、（鳍、退化）宫内二链列、三链列、四链列、五链列、六链列和七链列、（鳍、退化）交叉二链列、三链列、四链列、五链列、六链列和七链列
* Wings: XY-Wing, XYZ-Wing, (Uncompleted) WXYZ-Wing, (Uncompleted) VWXYZ-Wing, W-Wing<br/>Wing 结构：XY-Wing、XYZ-Wing、（残缺）WXYZ-Wing、（残缺）VWXYZ-Wing、W-Wing
* Uniqueness: Unique Rectangle (Type 1 to 6, Hidden), Avoidable Rectangle (Type 1 to 3, Hidden), Extended Rectangle (Type 1 to 2), Unique Loop (Type 1 to 4), Avoidable Rectangle, Bivalue Universal Grave (Type 1 to 4 and BUG + n), Borescoper's Deadly Pattern (Type 1 to 2)<br/>唯一性：唯一矩形（类型 1 到 6）、隐性唯一矩形、可规避矩形（类型 1 到 3）、隐性可规避矩形、拓展矩形（类型 1 到 2）、唯一环（类型 1 到 4）、全双值格致死解法（类型 1 到 4 和 BUG + n）、探长致命结构（类型 1 到 2）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>同数链式结构：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Subset Techniques: Sue de Coq<br/>待定数组：融合待定数组
* Last Resorts: Pattern Overlay Method, Template, Bowman's Bingo, Chute Clue Cover, Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、大行列提示信息覆盖、计算机试数
* Other techniques: Gurth's Symmetrical Placement<br/>其它技巧：宇宙法



## Conditional Compliation Symbol

标题：**条件编译符号**

Here display all conditional compliation symbols in this solution.

这里罗列本解决方案里用到的条件编译符号。

* `TARGET_64BIT`: Indicates your computer is 32 bits or 64 bits. If 64, please add `TARGET_64BIT` into the solution; otherwise, do nothing. This conditional compliation symbol is used in calling C/C++ functions in dynamic link library (i.e. DLL).<br/>指示系统是多少位的。当你的电脑是 64 位的时候，请添加 `TARGET_64BIT` 条件编译符号；否则就不管。这个条件编译符号用于调用位于动态链接库里的 C/C++ 函数。



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。

