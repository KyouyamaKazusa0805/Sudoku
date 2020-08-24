> **I have no time to update the project recently... If you find any bugs, please raise a issue or tell me directly. Thx!**
>
> **我最近没有时间更新项目……如果你找到了 bug，请提 issue 或者直接告诉我。感谢！**

# Sunnie's Sudoku Solution

标题：**向向的数独解决方案 (SSS)**

A sudoku handling SDK using brute forces and logical techniques (update gradually). Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK（逐渐更新）。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

> The form may be like the program Hodoku. However, using the program Hodoku is for reference only.
>
> 这个窗体可能看起来跟 Hodoku 差不多，因为我并不知道什么样子是一个漂亮的界面。使用 Hodoku 仅仅是为了参考。
>
> In addition, several little bugs will be fixed later.
>
> 另外，一些比较细小的 bug 会在后续修复。



## Programming language and IDE using

标题：**编程语言和 IDE 使用情况**

* Programming language: C#<br/>编程语言：C#
* Language version: 9.0<br/>语言版本：9.0
* Framework: .NET Core 3.1<br/>框架：.NET Core 3.1
* Integrated development environment: Visual Studio 2019 V16.7<br/>集成开发环境：Visual Studio 2019 V16.7
* Language Support: English, Simplified Chinese<br/>语言支持：英语、简体中文



## How to use

标题：**如何使用**



### Codes

标题：**代码**

Clone this repo, and you can take all codes!

只需要你克隆这个仓库就可以带走所有的代码了！

```bash
git clone https://github.com/Sunnie-Shine/Sudoku.git
```

Of course, you can also download the zip file.

当然，你也可以下载 zip 文件。



### Compiling & Running

标题：**编译和运行**

Please check [this file](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/require/ReadMe.txt) in [this folder](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref/require).

请参看这个[文件夹](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref/require)下的这个[文件](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/require/ReadMe.txt)。



### API Docs

标题：**API 文档**

Please download [this]([https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/Sudoku%20API%20references.chm](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/Sudoku API references.chm)) file.

请下载[这个]([https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/Sudoku%20API%20references.chm](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/Sudoku API references.chm))文件。



### Folders

标题：**文件夹**

This whole solution consists of several folders below:<br/>这个解决方案由如下文件夹构成：

* [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core): The main data structure implementation of the sudoku elementary.<br/>对数独基本元素的主要数据结构的实现。
* <font color="red">*</font> [`Sudoku.Core.Old`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core.Old): The old project of sudoku implementations, including the sudoku metadata and solving techniques (only singles, locked candidates and subsets).<br/>以前的数独项目实现，包含数独的相关元素以及一些数独技巧（只有排除、唯一余数、区块和数组）。
* [`Sudoku.Documents`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Documents): The project for parsing doc comment files and generating Markdown files.<br/>解析文档注释文件并生成 Markdown 的项目。
* [`Sudoku.Drawing`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Drawing): The project that can be used for drawing and rendering sudoku grids.<br/>这个项目用于绘制和渲染数独盘面。
* [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving): The generating and solving project.<br/>解题和题目生成的项目。
* <font color="red">*</font> [`Sudoku.Solving.BruteForces.Bitwise`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving.BruteForces/Bitwise): The native code for solving sudokus, which is used for speed up the calculation.<br/>解数独题目的 native 代码，用它是为了加快计算速度。
* [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows): The WPF project, containing UI forms and controls.<br/>WPF 项目，包含 UI 界面和控件。
* [`Sudoku.Debugging`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Debugging): The project that can be used while debugging only.<br/>这个项目仅用于调试代码时。
* [`ref`](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref): The profiles for sudoku for references.<br/>数独相关参考资料。
* [`pic`](https://github.com/Sunnie-Shine/Sudoku/tree/master/pic): The pictures.<br/>图片。

> The project with <font color="red">*</font> means the project is not important for the whole solution, they may not join in compiling.
>
> 带有 <font color="red">*</font> 符号的项目对整个解决方案都不重要，它们不会参与编译。



## Manual Technique Supports

标题：**人工技巧分析支持**

The program supports technique below at present:

目前程序支持如下的技巧项：

* Direct techniques: Hidden Single (Last Digit), Naked Single (Full House)<br/>直观技巧：排除（同数剩余）、唯一余数（同区剩余）
* Locked Candidates: Pointing, Claiming, Almost Locked Candidates<br/>区块：宫区块、行列区块、欠一数组
* Subset: Naked Subset (Locked Subset & Partial Locked Subset), Hidden Subset<br/>数组：显性数组（死锁数组和区块数组）、隐性数组
* Fishes: (Finned, Sashimi) X-Wing, Swordfish, Jellyfish, (Finned, Sashimi) Franken X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan, (Finned, Sashimi) Mutant X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan<br/>鱼：（鳍、退化）二链列、三链列和四链列、（鳍、退化）宫内二链列、三链列、四链列、五链列、六链列和七链列、（鳍、退化）交叉二链列、三链列、四链列、五链列、六链列和七链列
* Wings: XY-Wing, XYZ-Wing, (Incomplete) WXYZ-Wing, (Incomplete) VWXYZ-Wing, W-Wing, M-Wing, Split-Wing, Local-Wing, Hybrid-Wing<br/>Wing 结构：XY-Wing、XYZ-Wing、（残缺）WXYZ-Wing、（残缺）VWXYZ-Wing、W-Wing、M-Wing、Split-Wing、Local-Wing、Hybrid-Wing
* Uniqueness: Unique Rectangle (Type 1 to 6, Hidden, -XY-Wing, -XYZ-Wing, -WXYZ-Wing, + 2(/1SL), + 3(/2SL), + 4(/3SL)), Avoidable Rectangle (Type 1, 2, 3, 5, Hidden, -XY-Wing, -XYZ-Wing, -WXYZ-Wing), Extended Rectangle (Type 1 to 4), Unique Loop (Type 1 to 4), Bivalue Universal Grave (Type 1 to 4 and BUG + n, -XZ, BUG + n with Forcing Chains), Borescoper's Deadly Pattern (Type 1 to 4), Qiu's Deadly Pattern (Type 1 to 4, Locked), Unique Square (Type 1 to 4)<br/>唯一性：唯一矩形（类型 1 到 6、隐性、-XY-Wing、-XYZ-Wing、-WXYZ-Wing、+ 2（带 1 个共轭对）、+ 3（带 2 个共轭对）、+ 4（带 3 个共轭对））、可规避矩形（类型 1、2、3、5、隐性可规避矩形、-XY-Wing、-XYZ-Wing、-WXYZ-Wing）、拓展矩形（类型 1 到 4）、唯一环（类型 1 到 4）、全双值格致死解法（类型 1 到 4 和 BUG + n、双强链、BUG + n + 强制链）、探长致命结构（类型 1 到 4）、淑芬致命结构（类型 1 到 4、死锁）、唯一矩阵（类型 1 到 4）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>同数链式结构：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Locked Sets: Empty Rectangle Intersection Pair, Extended Subset Principle, Almost Locked Sets XZ Rule, Almost Locked Sets XY-Wing, Almost Locked Sets W-Wing, Death Blossom<br/>待定数组：对交空矩形、伪数组、ALS-双强链、ALS-XY-Wing、ALS-W-Wing、死亡绽放
* Chains: Alternating Inference Chain, Continuous Nice Loop, Cell Forcing Chains, Region Forcing Chains<br/>链：普通链、普通环、单元格强制链、区域强制链
* Exocet: Junior Exocet, Senior Exocet, Complex Senior Exocet<br/>飞鱼导弹：初级飞鱼导弹、高级飞鱼导弹、复杂高级飞鱼导弹
* Generalized Locked Sets: Sue de Coq (Basic & Cannibalized), 3-Dimension Sue de Coq, Stephen Kurzhal's Loop, Multi-sector Locked Sets<br/>广义数组：融合待定数组（标准和自噬）、2自由度融合待定数组、多米诺环、网
* Last Resorts: Pattern Overlay Method, Template, Bowman's Bingo, Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、计算机试数
* Other techniques: Gurth's Symmetrical Placement<br/>其它技巧：宇宙法



## Conditional Compilation Symbol

标题：**条件编译符号**

Here display all conditional compilation symbols in this solution.

这里罗列本解决方案里用到的条件编译符号。

> Some of them are unnecessary for you perhaps, you can remove them.
>
> 其中的一些对你可能没有必要，所以你可以移除它们。

* `DEBUG`: Indicates the current environment is for debugging. Some features rely on this symbol such as the default values for some instances in settings.<br/>表示当前是调试环境。一些特性会依赖于这个符号，诸如部分设置项的默认数值。
* `SUDOKU_RECOGNIZING`: Indicates whether your machine can use OCR tools to recognize an image, and convert to a sudoku grid data structure instance. If you want to use this feature, please add this symbol to two projects [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) and [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows).<br/>表示是否你的电脑上可以使用 OCR 识别工具来识别一个图片，并将其转换为一个数独盘面的实例对象。如果你希望启用这个功能的话，需要你为 [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) 和 [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows) 这两个项目添加这个编译符号。
* `ADVANCED_PICTURE_SAVING`: Indicates whether the solution will use another picture saving way to save pictures. This symbol will be used only in the file [`PictureSavingPreferencesWindow.xaml.cs`](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Windows/PictureSavingPreferencesWindow.xaml.cs).<br/>表示是否解决方案使用另外一种保存图片的办法去保存图片。这个符号只用在文件 [`PictureSavingPreferencesWindow.xaml.cs`](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Windows/PictureSavingPreferencesWindow.xaml.cs) 里面。
* `AUTHOR_RESERVED`: Indicates the method is only used for author himself. You can delete the code surrounded with this symbol.<br/>表示这段代码只对作者来说才有意义。你完全可以删除掉这段代码，或者不使用 `AUTHOR_RESERVED` 符号。
* `CSHARP_9_PREVIEW`: Indicates the current feature is only used for C# 9 (preview). If the C# 9 is released version, this symbol will be removed.<br/>表示代码只在 C# 9 预览版里存在。当变作 C# 9 的时候，这段代码将会消失。



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。

