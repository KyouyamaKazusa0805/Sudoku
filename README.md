# Sunnie's Sudoku Solution

标题：**向向的数独解决方案（SSS）**

A sudoku handling SDK using brute forces and logical techniques (Update files gradually). Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK（逐渐更新）。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。



## Programming language and IDE using

标题：**编程语言和 IDE 使用情况**

* Programming language: C#<br/>编程语言：C#
* Language version: 8.0<br/>语言版本：8.0
* Framework: .NET Core 3.1<br/>框架：.NET Core 3.1
* Integrated development environment: Visual Studio 2019 V16.5<br/>集成开发环境：Visual Studio 2019 V16.5



## How to use

标题：**如何使用**

Clone this repo, and you can take all codes!

只需要你克隆这个仓库就可以带走所有的代码了！

```bash
Username@ComputerName MINGW64 ~/Desktop
$ git clone https://github.com/Sunnie-Shine/Sudoku.git
```

This whole solution consists of several folders below:<br/>这个解决方案由如下文件夹构成：
* Sudoku.Core: The main data structure implementation of the sudoku elementary.<br/>对数独基本元素的主要数据结构的实现。
* Sudoku.Solving: The generating and solving project.<br/>解题和题目生成的项目。
* Sudoku.Windows: The WPF project, containing UI forms and controls.<br/>WPF 项目，包含 UI 界面和控件。
* Sudoku.Debugging: The project that can be used while debugging only.<br/>这个项目仅用于调试代码时。
* old: The old projects that implemented by me or external codes.<br/>以前的由我自己实现的项目，或者是属于外部代码。
* ref: The profiles for sudoku for references.<br/>数独相关参考资料。
* pic: The pictures.<br/>图片。


## Demo

标题：**演示**

You can write code in your computer like this:

你可以在你的机器上使用这样的代码：

```csharp
// Create a solver.
var solver = new ManualSolver();

// Parse a string and convert it to a sudoku grid instance.
var grid = Grid.Parse("006800009000020300700001080010900200000040008008750040030006001900080030002000500");

// Solve it.
var analysisResult = solver.Solve(grid);

// Print the result onto the console screen.
Console.WriteLine(analysisResult.ToString(null));
```

And the puzzle solution and analysis result will be displayed on console screen, like this!

然后答案和分析结果都会在你的控制台界面呈现出来，就像这样！

```
Puzzle: ..68....9....2.3..7....1.8..1.9..2......4...8..875..4..3...6..19...8..3...2...5..
Solving tool: Manual
Technique used:
 20 * Full House
  5 * Last Digit
 25 * Hidden Single (In Block)
  2 * Hidden Single (In Row)
  3 * Hidden Single (In Column)
  1 * Naked single
  2 * Pointing
  5 * Claiming
  1 * Naked Pair
  4 * Hidden Pair
  1 * Finned X-Wing
  2 * Sashimi X-Wing
  2 * Swordfish
  1 * Two-string Kite
  1 * XY-Wing
  1 * XYZ-Wing
  1 * Almost Locked Pair
  1 * Unique Rectangle Type 4
  1 * Hidden Unique Rectangle 
  1 * Discontinuous Nice Loop
  7 * Almost Locked Sets XZ Rule
 87 steps in total
Puzzle rating: 5.5/1.2/1.2
Puzzle solution: 146835729589427316723691485314968257257143968698752143435276891971584632862319574
Puzzle has been solved.
Time elapsed: 00:00.12.281
```

> Format strings in the analysis result is shown in the description of file *How-to-use-analysis-result.md*.
>
> 分析结果的格式化字符串可以参照“How-to-use-analysis-result.md”文件里呈现的描述。



## Manual Technique Supports

标题：**人工技巧分析支持**

The program supports technique below at present:

目前程序支持如下的技巧项：

* Direct techniques: Hidden Single (Last Digit), Naked Single (Full House)<br/>直观技巧：排除（同数剩余）、唯一余数（同区剩余）
* Locked Candidates: Pointing, Claiming, Almost Locked Candidates<br/>区块：宫区块、行列区块、欠一数组
* Subset: Naked Subset (Locked Subset & Partial Locked Subset), Hidden Subset<br/>数组：显性数组（死锁数组和区块数组）、隐性数组
* Fishes: (Finned, Sashimi) X-Wing, Swordfish, Jellyfish, (Finned, Sashimi) Franken X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan, (Finned, Sashimi) Mutant X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan<br/>鱼：（鳍、退化）二链列、三链列和四链列、（鳍、退化）宫内二链列、三链列、四链列、五链列、六链列和七链列、（鳍、退化）交叉二链列、三链列、四链列、五链列、六链列和七链列
* Wings: XY-Wing, XYZ-Wing, (Uncompleted) WXYZ-Wing, (Uncompleted) VWXYZ-Wing, W-Wing, M-Wing, Split-Wing, Local-Wing, Hybrid-Wing<br/>Wing 结构：XY-Wing、XYZ-Wing、（残缺）WXYZ-Wing、（残缺）VWXYZ-Wing、W-Wing、M-Wing、Split-Wing、Local-Wing、Hybrid-Wing
* Uniqueness: Unique Rectangle (Type 1 to 6, Hidden), Avoidable Rectangle (Type 1 to 3, Hidden), Extended Rectangle (Type 1 to 2), Unique Loop (Type 1 to 4), Avoidable Rectangle, Bivalue Universal Grave (Type 1 to 4 and BUG + n), Borescoper's Deadly Pattern (Type 1 to 2)<br/>唯一性：唯一矩形（类型 1 到 6）、隐性唯一矩形、可规避矩形（类型 1 到 3）、隐性可规避矩形、拓展矩形（类型 1 到 3）、唯一环（类型 1 到 4）、全双值格致死解法（类型 1 到 4 和 BUG + n）、探长致命结构（类型 1 到 2）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>同数链式结构：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Subset Techniques: Sue de Coq, Extended Subset Principle, Almost Locked Sets XZ Rule, Almost Locked Sets XY-Wing, Almost Locked Sets W-Wing, Death Blossom<br/>待定数组：融合待定数组、伪数组、ALS-双强链、ALS-XY-Wing、ALS-W-Wing、死亡绽放
* Chains: Alternating Inference Chain (+ Locked Candidates), Continuous Nice Loop (+ Locked Candidates)<br/>链：普通链（+区块）、普通环（+区块）
* Last Resorts: Pattern Overlay Method, Template, Bowman's Bingo, Chute Clue Cover (Half implemented), Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、大行列提示信息覆盖（实现了一半）、计算机试数
* Other techniques: Gurth's Symmetrical Placement<br/>其它技巧：宇宙法



## Conditional Compliation Symbol

标题：**条件编译符号**

Here display all conditional compliation symbols in this solution.

这里罗列本解决方案里用到的条件编译符号。

* `TARGET_64BIT`: Indicates your computer is 32 bits or 64 bits. If 64, please add `TARGET_64BIT` into the solution; otherwise, do nothing. This conditional compliation symbol is used in calling C/C++ functions in dynamic link library (DLL).<br/>指示系统是多少位的。当你的电脑是 64 位的时候，请添加 `TARGET_64BIT` 条件编译符号；否则就不管。这个条件编译符号用于调用位于动态链接库里的 C/C++ 函数。
* `SUDOKU_RECOGNIZING`: Indicates whether your machine can use OCR tools to recognize an image, and convert to a sudoku grid data structure instance. If you want to use this feature, please add this symbol to two projects `Sudoku.Core` and `Sudoku.Windows`.<br/>表示是否你的电脑上可以使用 OCR 识别工具来识别一个图片，并将其转换为一个数独盘面的实例对象。如果你希望启用这个功能的话，需要你为 `Sudoku.Core` 和 `Sudoku.Windows` 这两个项目添加这个编译符号。



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。

