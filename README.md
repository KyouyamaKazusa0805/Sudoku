# Sunnie's Sudoku Solution (SSS)

标题：**向向的数独解决方案 (SSS)**



A sudoku handling SDK using brute forces and logical techniques. Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

> The window may be like a program called [Hodoku](http://hodoku.sourceforge.net/en/index.php). However, the base window of Hodoku is only for reference.
>
> 这个窗体可能看起来跟 [Hodoku](http://hodoku.sourceforge.net/en/index.php) 差不多，因为我并不知道什么样子是一个漂亮的界面。使用 Hodoku 差不多的窗体仅仅是为了参考。
>



## Programming language and IDE using

标题：**编程语言和 IDE 使用情况**

* Programming language: C#<br/>编程语言：C#
* Language version: 9.0<br/>语言版本：9.0
* Framework: .NET 5<br/>框架：.NET 5
* Indenting: Tabs（`\t`）<br/>缩进：原生 Tab（`\t`）
* Integrated development environment: Visual Studio 2019 V16.9 Preview<br/>集成开发环境：Visual Studio 2019 V16.9 预览版
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

Please download [this](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/Sudoku%20API%20references.chm) file.

请下载[这个](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/Sudoku%20API%20references.chm)文件。



### Folders

标题：**文件夹**

This whole solution consists of several folders below:<br/>这个解决方案由如下文件夹构成：

| Project<br/>项目                                             | Description<br/>描述                                         |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [`Sudoku.Bot`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Bot) | The project that implement the bot.<br/>All code files in this project are surrounded with the conditional compilation symbol `AUTHOR_RESERVED`. If you wouldn't like to use this project, just delete it!<br />实现机器人的项目。<br />该项目的所有代码文件都使用条件编译符号`AUTHOR_RESERVED` 包裹。如果你不想使用这个项目，删掉就行！ |
| [`Sudoku.Bot.Console`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Bot.Console) | The project that use the bot.<br/>All code files in this project are surrounded with the conditional compilation symbol `AUTHOR_RESERVED`. If you wouldn't like to use this project, just delete it!<br />使用机器人的项目。<br />该项目的所有代码文件都使用条件编译符号`AUTHOR_RESERVED` 包裹。如果你不想使用这个项目，删掉就行！ |
| [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) | The main data structure implementation of the sudoku elementary.<br/>对数独基本元素的主要数据结构的实现。 |
| [`Sudoku.Diagnostics`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Diagnostics) | This project encapsulates operations for diagnosing the whole solution, such as checking the number of code lines.<br/>这个项目封装了诊断整个解决方案的操作，比如检查代码的行数。 |
| [`Sudoku.DocComments`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.DocComments) | I just want to say this project is useless but for providing other projects with document comments. Due to some reason hard to say, I've deleted the XML files used for providing with comments and using real instances to offer comments.<br/>我只想说，这个项目除了给别的项目提供文档注释以外，就没别的用了。由于一些比较难说的原因，我把用来提供文档注释的 XML 文件都删掉了，改成了真正的实体对象。 |
| [`Sudoku.Drawing`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Drawing) | The project that can be used for drawing and rendering sudoku grids.<br/>这个项目用于绘制和渲染数独盘面。 |
| [`Sudoku.Globalization`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Globalization) | This project encapsulates the operation and constants for globalization interactions.<br/>这个项目封装了国际化交互的操作和一些常数。 |
| [`Sudoku.IO`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.IO) | The project handling IO operations over sudoku.<br/>这个项目用于处理数独相关的 IO 操作。 |
| [`Sudoku.Recognition`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Recognition) | This project encapsulates the operations about recognition of a sudoku picture.<br/>这个项目封装了关于识别一个数独图片的相关操作。 |
| [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving) | The generating and solving project.<br/>解题和题目生成的项目。 |
| [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows) | The WPF project, containing UI forms and controls.<br/>WPF 项目，包含 UI 界面和控件。 |
| [`Sudoku.Debugging`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Debugging) | The project that can be used while debugging only.<br/>这个项目仅用于调试代码时。 |
| [`ref`](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref) | The profiles for sudoku for references. In addition, some necessary files to help us compile and run the whole project are also in this folder.<br/>数独相关参考资料。另外，一些帮助我们执行和运行的文件也在此文件夹下。 |
| [`pic`](https://github.com/Sunnie-Shine/Sudoku/tree/master/pic) | The pictures.<br/>图片。                                     |



## Manual Technique Supports

标题：**人工技巧分析支持**

The program supports technique below at present:

目前程序支持如下的技巧项：

| Techniques<br />技巧类别                    | Supported Techniques<br />已支持的技巧                       | Difficulty Level<br />技巧难度级别    |
| ------------------------------------------- | ------------------------------------------------------------ | ------------------------------------- |
| **Direct techniques<br />直观技巧**         | Hidden Single（排除）<br />Last Digit（同数剩余）<br />Naked Single（唯一余数）<br />Full House（同区剩余） | Easy<br />容易                        |
| **Locked Candidates<br />区块**             | Pointing（宫区块）<br />Claiming（行列区块）                 | Moderate<br />一般                    |
| **Subset<br />数组/链数**                   | Naked Subset（显性数组）<br />Naked Subset (+)（区块数组）<br />Locked Subset（死锁数组）<br />Hidden Subset（隐性数组） | Moderate<br />一般                    |
| **Fishes<br />鱼/链列**                     | (Finned, Sashimi) Fishes（普通鱼）<br />(Finned, Sashimi) Franken Fishes（宫内鱼）<br />(Finned, Sashimi) Mutant Fishes（交叉鱼） | Hard<br />困难                        |
| **Wings<br />短链类结构**                   | XY-Wing<br />XYZ-Wing<br />WXYZ-Wing<br />VWXYZ-Wing<br />W-Wing | Hard<br />困难                        |
| **Uniqueness<br />唯一性技巧/致命结构**     | Unique Rectangle（唯一矩形）<br />Avoidable Rectangle（可规避矩形）<br />Extended Rectangle（拓展矩形）<br />Unique Loop（唯一环）<br />Bivalue Universal Grave（全双值格致死解法）<br />Borescoper's Deadly Pattern（探长致命结构）<br />Qiu's Deadly Pattern（淑芬致命结构）<br />Unique Square（唯一矩阵） | Hard - Fiendish<br />困难 - 极难      |
| **Single Digit Patterns<br />同数链式结构** | Skyscraper（摩天楼）<br />Two-string Kite（双线风筝）<br />Turbot Fish（多宝鱼）<br />Empty Rectangle（空矩形）<br />Guardians（守护者） | Hard - Fiendish<br />困难 - 极难      |
| **Almost Locked Sets<br />待定数组**        | Almost Locked Candidates（欠一数组）<br />Empty Rectangle Intersection Pair（对交空矩形）<br />Extended Subset Principle（伪数组）<br />Sue de Coq（融合待定数组）<br />3-Dimension Sue de Coq（三维融合待定数组）<br />Almost Locked Sets XZ Rule（待定数组-双强链）<br />Almost Locked Sets XY-Wing（待定数组-XY-Wing）<br />Almost Locked Sets W-Wing（待定数组-W-Wing）<br />Death Blossom（死亡绽放） | Hard - Nightmare<br />困难 - 地狱     |
| **Chains<br />链**                          | Alternating Inference Chain（普通链）<br />Continuous Nice Loop（连续环）<br />Cell Forcing Chains（单元格强制链）<br />Region Forcing Chains（区域强制链）<br />Dynamic Cell Forcing Chains（动态单元格强制链）<br />Dynamic Region Forcing Chains（动态区域强制链）<br />Dynamic Contradiction Forcing Chains（动态矛盾强制链）<br />Dynamic Double Forcing Chains（动态逆反矛盾强制链） | Fiendish - Nightmare<br />极难 - 地狱 |
| **Exocet<br />飞鱼导弹**                    | Junior Exocet（初级飞鱼导弹）<br />Senior Exocet（高级飞鱼导弹） | Nightmare<br />地狱                   |
| **Generalized Locked Sets<br />广义数组**   | Stephen Kurzhal's Loop（多米诺环）<br />Multi-sector Locked Sets（网） | Nightmare<br />地狱                   |
| **Last Resorts<br />爆破技巧**              | Pattern Overlay Method（图案叠加删减）<br />Template（模板）<br />Bowman's Bingo（人工试数）<br />Brute Force（计算机试数） | Nightmare<br />地狱                   |
| **Symmetrical Techniques<br />对称性技巧**  | Gurth's Symmetrical Placement（宇宙法）                      | Fiendish<br />极难                    |



## Conditional Compilation Symbol

标题：**条件编译符号**

Here display all conditional compilation symbols (CCS) in this solution. CCSes are the global Boolean values that exist in some projects to indicate whether the code block with these symbols are compiled or not. The block can't be compiled until the specified symbol is set in the project file (`*.csproj`).

这里罗列本解决方案里用到的条件编译符号（简称 CCS）。CCS 是全局的布尔量，它们存在于一些项目里，用来表示某段代码块是否需要编译。这段代码块只有当我们在项目文件（`*.csproj`）里配置了符号之后，才会被编译。

Some of them are unnecessary for you perhaps, you can remove them. In addition, if you want to modify any CCSes, please search for ways online on modifying them.

其中的一些对你可能没有必要，所以你可以移除它们。另外，如果你要修改这些符号，请上网查阅修改它们的办法。

| CCS<br/>条件编译符号         | Usage<br/>用法                                               |
| ---------------------------- | ------------------------------------------------------------ |
| `DEBUG`                      | Indicates the current environment is for debugging.<br/>表示当前是调试环境。 |
| `SUDOKU_RECOGNITION`         | Indicates whether your machine can use OCR tools to recognize an image, and convert to a sudoku grid data structure instance.<br/>表示是否你的电脑上可以使用 OCR 识别工具来识别一个图片，并将其转换为一个数独盘面的实例对象。 |
| `AUTHOR_RESERVED`            | Indicates the method is only used for author himself. You can delete the code surrounded with this symbol.<br/>表示这段代码只对作者来说才有意义。你完全可以删除掉这段代码，或者不使用该符号。 |
| `MUST_DOWNLOAD_TRAINED_DATA` | Indicates whether the solution will download the trained data file `eng.traineddata` on GitHub when the local file with the same name cannot be found. Sometimes the file downloading is too slow to stand with it. If this symbol is undefined, it'll offer the user an error message window,  saying local file cannot be found.<br/>表示这个解决方案是否在本地的同名文件不存在的时候，从 GitHub 上下载该文件。有时候这个下载特别慢，以至于我们完全没办法忍受它。如果这个符号没有定义的话，我们就会在文件找不到的时候直接以错误弹窗的形式提示用户不能使用识别功能。 |
| `OBSOLETE`                   | Indicates the code block is no longer in use. You can still enable to compile them, but unstable.<br/>表示当前代码段不再使用。你可以启用编译它们，但它们不稳定。 |



## Author

标题：**作者**

Sunnie, from Chengdu, is an normal undergraduate from normal university.

小向，来自成都的一名普通大学生。

