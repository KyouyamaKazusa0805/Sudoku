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
				//EnableBruteForce = false,
			};
			var grid = Grid.Parse(@"
.---------------------------------.---------------------------------.---------------------------------.
|  45679      3          567      |  129        129        269      |  479        8          249      |
|  2          169        68       |  4          7          689      |  5          139        139      |
|  479        1479       78       |  3          5          289      |  479        1249       6        |
:---------------------------------+---------------------------------+---------------------------------:
|  356        8          2356     |  1259       4          2359     |  369        7          139      |
|  3456       456        9        |  15         13         7        |  2          1346       8        |
|  1          47         237      |  8          6          239      |  349        349        5        |
:---------------------------------+---------------------------------+---------------------------------:
|  35679      2          1        |  579        8          3459     |  3469       34569      349      |
|  8          59         35       |  6          239        23459    |  1          23459      7        |
|  35679      5679       4        |  579        239        1        |  8          23569      239      |
'---------------------------------'---------------------------------'---------------------------------'
", GridParsingType.PencilMarkedTreatSingleAsGiven);
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:m}");
		}
	}
}

```

And the puzzle solution and analysis result will be displayed on console screen, like this!

然后答案和分析结果都会在你的控制台界面呈现出来，就像这样！

```
Puzzle: .3.....8.2..47.5.....35...6.8..4..7...9..72.81..86...5.21.8....8..6..1.7..4..18..:119 294
Solving tool: Manual
Solving steps:
 (5.0) Sue de Coq: { r4c6, r6c6 }(2359) - ({ r1c6, r2c6, r3c6 }(2689) and r5c45(135)) => r4c4 <> 1, r8c6 <> 2, r4c4 <> 5, r7c6 <> 9, r8c6 <> 9
 (1.5) Hidden Single (In Row): r4c9 = 1 in r4
 (2.6) Pointing: 2 in b8\c5 => r1c5 <> 2
(20.0) Brute Force: r5c5 = 1
 (1.2) Hidden Single (In Block): r1c4 = 1 in b2
 (1.5) Hidden Single (In Column): r4c4 = 2 in c4
 (1.2) Hidden Single (In Block): r6c3 = 2 in b4
 (1.2) Hidden Single (In Block): r6c2 = 7 in b4
 (2.3) Naked single: r1c5 = 9
 (2.3) Naked single: r5c4 = 5
 (2.6) Pointing: 4 in b4\r5 => r5c8 <> 4
 (2.6) Pointing: 7 in b7\c1 => r1c1 <> 7, r3c1 <> 7
 (2.6) Pointing: 3 in b5\c6 => r7c6 <> 3, r8c6 <> 3
 (2.8) Claiming: 5 in c2\b7 => r7c1 <> 5, r8c3 <> 5, r9c1 <> 5
 (2.3) Naked single: r8c3 = 3
 (1.2) Hidden Single (In Block): r9c5 = 3 in b8
 (1.0) Full House: r8c5 = 2
 (3.0) Naked Pair: 6, 8 in r2 => r2c2 <> 6
 (3.4) Finned X-Wing: 9 in r28\c28 (With a fin cell: r2c9) => r3c8 <> 9
 (3.7) Naked Triple (+): 1, 4, 9 in b1 => r1c1 <> 4, r3c78 <> 4
...
----------
Technique used:
19 * Full House
7 * Last Digit
18 * Hidden Single (In Block)
1 * Hidden Single (In Row)
1 * Hidden Single (In Column)
4 * Naked single
4 * Pointing
1 * Claiming
1 * Naked Pair
1 * Finned X-Wing
1 * Naked Triple (+)
1 * Sashimi Swordfish
1 * Sue de Coq
1 * Brute Force
----------
Total solving steps count: 61
Difficulty total: 112.9
Puzzle rating: 20.0/5.0/5.0
Puzzle solution: 537196482216478593948352716385249671469517238172863945721985364893624157654731829
Puzzle has been solved.
Time elapsed: 00:00.01.907
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
