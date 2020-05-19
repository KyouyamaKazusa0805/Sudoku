# Sunnie's Sudoku Solution

标题：**向向的数独解决方案 (SSS)**

> I update the files slower than the past due to the graduation approaching, which brings much more things.
>
> 这个作者目前更新文件会比以前更新得慢，因为临近毕业了，就有很多的事情。
>
> BTW, I have been studying and learning the sudoku algorithm, so some technique searchers contain bugs inevitably perhaps. If you find them, please tell me, thank you.
>
> 顺带一提，我目前一直都在学习和研究数独相关的算法，所以一些算法可能难免包含一些程序漏洞。如果你找到了，请你告诉我，万分感谢。

A sudoku handling SDK using brute forces and logical techniques (update gradually). Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK（逐渐更新）。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

> The form may be like the program Hodoku, however, using the program Hodoku is for reference only.
>
> 这个窗体可能看起来跟 Hodoku 差不多，因为我并不知道什么样子是一个漂亮的界面。使用 Hodoku 仅仅是为了参考。
>
> In addition, several little bugs will be fixed later such as `TextBox`s cannot give an error for incorrect input.
>
> 另外，一些比较细小的 bug 会在后续修复，比如文本框不会对错误的输入文本产生错误提示信息。



## Programming language and IDE using

标题：**编程语言和 IDE 使用情况**

* Programming language: C#<br/>编程语言：C#
* Language version: 8.0<br/>语言版本：8.0
* Framework: .NET Core 3.1<br/>框架：.NET Core 3.1
* Integrated development environment: Visual Studio 2019 V16.5<br/>集成开发环境：Visual Studio 2019 V16.5



## How to use

标题：**如何使用**



### Codes

标题：**代码**

Clone this repo, and you can take all codes!

只需要你克隆这个仓库就可以带走所有的代码了！

```bash
git clone https://github.com/Sunnie-Shine/Sudoku.git
```



### Compiling & Running

标题：**编译和运行**

Please check this [file](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/require/ReadMe.txt).

请参看[这个文件](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/require/ReadMe.txt)。



### Folders

标题：**文件夹**

This whole solution consists of several folders below:<br/>这个解决方案由如下文件夹构成：

* [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core): The main data structure implementation of the sudoku elementary.<br/>对数独基本元素的主要数据结构的实现。
* [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving): The generating and solving project.<br/>解题和题目生成的项目。
* [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows): The WPF project, containing UI forms and controls.<br/>WPF 项目，包含 UI 界面和控件。
* [`Sudoku.Debugging`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Debugging): The project that can be used while debugging only.<br/>这个项目仅用于调试代码时。
* [`old`](https://github.com/Sunnie-Shine/Sudoku/tree/master/old): The old projects that implemented by me or external codes.<br/>以前的由我自己实现的项目，或者是属于外部代码。
* [`ref`](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref): The profiles for sudoku for references.<br/>数独相关参考资料。
* [`pic`](https://github.com/Sunnie-Shine/Sudoku/tree/master/pic): The pictures.<br/>图片。



## Manual Technique Supports

标题：**人工技巧分析支持**

The program supports technique below at present:

目前程序支持如下的技巧项：

* Direct techniques: Hidden Single (Last Digit), Naked Single (Full House)<br/>直观技巧：排除（同数剩余）、唯一余数（同区剩余）
* Locked Candidates: Pointing, Claiming, Almost Locked Candidates<br/>区块：宫区块、行列区块、欠一数组
* Subset: Naked Subset (Locked Subset & Partial Locked Subset), Hidden Subset<br/>数组：显性数组（死锁数组和区块数组）、隐性数组
* Fishes: (Finned, Sashimi) X-Wing, Swordfish, Jellyfish, (Finned, Sashimi) Franken X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan, (Finned, Sashimi) Mutant X-Wing, Swordfish, Jellyfish, Starfish, Whale, Leviathan<br/>鱼：（鳍、退化）二链列、三链列和四链列、（鳍、退化）宫内二链列、三链列、四链列、五链列、六链列和七链列、（鳍、退化）交叉二链列、三链列、四链列、五链列、六链列和七链列
* Wings: XY-Wing, XYZ-Wing, (Uncompleted) WXYZ-Wing, (Uncompleted) VWXYZ-Wing, W-Wing, M-Wing, Split-Wing, Local-Wing, Hybrid-Wing<br/>Wing 结构：XY-Wing、XYZ-Wing、（残缺）WXYZ-Wing、（残缺）VWXYZ-Wing、W-Wing、M-Wing、Split-Wing、Local-Wing、Hybrid-Wing
* Uniqueness: Unique Rectangle (Type 1 to 6, Hidden), UR + 2, UR + 2 / 1SL, UR + 3, UR + 3 / 2SL, UR + 4 / 3SL, Unique Rectangle XY-Wing, Unique Rectangle XYZ-Wing, Unique Rectangle WXYZ-Wing, Avoidable Rectangle (Type 1, 2, 3, 5, Hidden), Avoidable Rectangle XY-Wing, Avoidable Rectangle XYZ-Wing, Avoidable Rectangle WXYZ-Wing, Extended Rectangle (Type 1 to 3), Unique Loop (Type 1 to 4), Avoidable Rectangle, Bivalue Universal Grave (Type 1 to 4 and BUG + n), BUG-XZ, Borescoper's Deadly Pattern (Type 1 to 2)<br/>唯一性：唯一矩形（类型 1 到 6、隐性唯一矩形）、唯一矩形 + 2、唯一矩形 + 2（带 1 个共轭对）、唯一矩形 + 3、唯一矩形 + 3（带 2 个共轭对）、唯一矩形 + 4（带 3 个共轭对）、UR-XY-Wing、UR-XYZ-Wing、UR-WXYZ-Wing、可规避矩形（类型 1、2、3、5、隐性可规避矩形）、AR-XY-Wing、AR-XYZ-Wing、AR-WXYZ-Wing、拓展矩形（类型 1 到 3）、唯一环（类型 1 到 4）、全双值格致死解法（类型 1 到 4 和 BUG + n）、BUG-双强链、探长致命结构（类型 1 到 2）
* Single Digit Patterns: Skyscraper, Two-string Kite, Turbot Fish, Empty Rectangle<br/>同数链式结构：摩天楼、双线风筝、多宝鱼、空矩形
* Almost Subset Techniques: Sue de Coq (Basic & Cannibalized), Empty Rectangle Intersection Pair, Extended Subset Principle, Almost Locked Sets XZ Rule, Almost Locked Sets XY-Wing, Almost Locked Sets W-Wing, Death Blossom, Stephen Kurzhal's Loop<br/>待定数组：融合待定数组（标准和自噬）、对交空矩形、伪数组、ALS-双强链、ALS-XY-Wing、ALS-W-Wing、死亡绽放、多米诺环
* Chains: Alternating Inference Chain (+ Locked Candidates), Continuous Nice Loop (+ Locked Candidates)<br/>链：普通链（+区块）、普通环（+区块）
* Last Resorts: Pattern Overlay Method, Template, Bowman's Bingo, Chute Clue Cover (Half implemented), Brute Force<br/>爆破技巧：图案叠加删减、模板、人工试数、大行列提示信息覆盖（实现了一半）、计算机试数
* Exocet: Junior Exocet, Senior Exocet<br/>飞鱼导弹：初级飞鱼导弹、高级飞鱼导弹
* Other techniques: Gurth's Symmetrical Placement<br/>其它技巧：宇宙法

> AR does not contain type 4 or 6.
>
> 可规避矩形不含有类型 4 和 6。

## Conditional Compliation Symbol

标题：**条件编译符号**

Here display all conditional compliation symbols in this solution.

这里罗列本解决方案里用到的条件编译符号。

* `DEBUG`: Indicates the current environment is for debugging. Some features rely on this symbol such as the default values for some instances in settings.<br/>表示当前是调试环境。一些特性会依赖于这个符号，诸如部分设置项的默认数值。
* `TARGET_64BIT`: Indicates your computer is 32 bits or 64 bits. If 64, please add `TARGET_64BIT` into the project [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving); otherwise, do nothing. This conditional compliation symbol is used in calling C/C++ functions in dynamic link library (DLL).<br/>指示系统是多少位的。当你的电脑是 64 位的时候，请在 [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving) 项目里添加 `TARGET_64BIT` 条件编译符号；否则就不管。这个条件编译符号用于调用位于动态链接库里的 C/C++ 函数。<br/>At present (C# 8) the syntax does not support the native integers, so this symbol will still exist a period of time.<br/>到目前为止（C# 8），语法都没有支持“适合机器的整型类型”，所以这个编译符号仍会存在一段时间。
* `SUDOKU_RECOGNIZING`: Indicates whether your machine can use OCR tools to recognize an image, and convert to a sudoku grid data structure instance. If you want to use this feature, please add this symbol to two projects [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) and [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows).<br/>表示是否你的电脑上可以使用 OCR 识别工具来识别一个图片，并将其转换为一个数独盘面的实例对象。如果你希望启用这个功能的话，需要你为 [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) 和 [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows) 这两个项目添加这个编译符号。



## Author

标题：**作者**

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。

