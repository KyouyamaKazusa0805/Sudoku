# Sudoku

标题：*数独*

A sudoku handling SDK using brute forces and logical techniques (Update files gradually).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK（逐渐更新）。



## C# Version and IDE using

标题：*C# 版本和 IDE 使用情况*

* C# version: 8.0<br/>C# 版本：8.0
* IDE using: Visual Studio 2019 V16.4<br/>IDE 使用：Visual Studio 2019 V16.4



## How to use

标题：*如何使用*

Clone this repo, and you can take all codes!

只需要你克隆这个仓库就可以带走所有的代码了！

You can write code in your computer like this:

你可以在你的机器上使用这样的代码：

```csharp
internal static class Program
{
    private static void Main()
    {
        var solver = new Sudoku.Solving.Manual.ManualSolver
        {
            OptimizedApplyingOrder = true,
            EnableFullHouse = true,
            EnableLastDigit = true
        };
        var grid = Sudoku.Data.Meta.Grid.Parse(
            "500000482030007000000000309690085000000020000000970035102000000000100050764000008");
        var analysisResult = solver.Solve(grid);
        System.Console.WriteLine(analysisResult);
    }
}
```

And the puzzle solution and analysis result will be displayed on console screen, like this!

然后答案和分析结果都会在你的控制台界面呈现出来，就像这样！

```
Initial grid: 5.....482.3...7.........3.969..85.......2.......97..351.2.........1...5.764.....8
Solving tool: Manual
(1.2) Hidden Single (In Block): r2c7 = 5 in b3
(1.2) Hidden Single (In Block): r7c2 = 5 in b7
(1.2) Hidden Single (In Block): r5c3 = 5 in b4
(1.2) Hidden Single (In Block): r3c8 = 7 in b3
(1.2) Hidden Single (In Block): r7c4 = 7 in b8
(1.5) Hidden Single (In Row): r7c6 = 8 in r7
(2.3) Naked single: r8c2 = 8
(2.8) Claiming: 1 in r2 => r2c3 <> 1, r2c5 <> 1
(2.8) Claiming: 6 in r2 => r2c3 <> 6, r2c4 <> 6, r2c5 <> 6
(2.8) Claiming: 2 in r6 => r6c7 <> 2
(2.8) Claiming: 3 in r8 => r8c5 <> 3, r8c6 <> 3, r8c9 <> 3
(1.2) Hidden Single (In Block): r7c9 = 3 in b9
(2.8) Claiming: 9 in r8 => r8c5 <> 9, r8c6 <> 9, r8c7 <> 9
(2.8) Claiming: 1 in c6 => r1c6 <> 1, r3c6 <> 1
(3.7) Naked Triple (+): 3, 4, 6 in c4 => r9c4 <> 3, r2c4 <> 4, r3c4 <> 4, r5c6 <> 4, r6c6 <> 4, r3c4 <> 6
(3.4) Hidden Pair: 2, 4 in r6 => r6c1 <> 8, r6c2 <> 1
(2.0) Locked Pair: 2, 4 in b4 => r5c1 <> 4, r5c2 <> 4
(3.0) Naked Pair: 1, 7 in c2 => r3c2 <> 1
(3.7) Naked Triple (+): 4, 6, 9 in c5 => r3c5 <> 4, r1c5 <> 6, r3c5 <> 6, r8c6 <> 6, r1c5 <> 9, r9c5 <> 9
(5.1) Naked Quadruple (+): 2, 4, 8, 9 in b1 => r3c3 <> 8, r1c3 <> 9, r2c5 <> 9
(1.2) Hidden Single (In Block): r1c6 = 9 in b2
(1.2) Hidden Single (In Block): r7c5 = 9 in b8
(1.2) Hidden Single (In Block): r8c5 = 6 in b8
(1.2) Hidden Single (In Block): r8c6 = 4 in b8
(1.2) Hidden Single (In Block): r2c5 = 4 in b2
(1.2) Hidden Single (In Block): r7c8 = 4 in b9
(1.0) Full House: r7c7 = 6
(1.5) Hidden Single (In Row): r8c7 = 2 in r8
(1.2) Hidden Single (In Block): r4c8 = 2 in b6
(1.2) Hidden Single (In Block): r8c9 = 7 in b9
(1.5) Hidden Single (In Row): r6c6 = 6 in r6
(1.2) Hidden Single (In Block): r5c6 = 1 in b5
(1.2) Hidden Single (In Block): r1c4 = 6 in b2
(1.2) Hidden Single (In Block): r1c5 = 3 in b2
(1.2) Hidden Single (In Block): r3c5 = 1 in b2
(1.0) Full House: r9c5 = 5
(1.1) Last Digit: r3c4 = 5
(1.2) Hidden Single (In Block): r9c6 = 3 in b8
(1.0) Full House: r9c4 = 2
(1.0) Full House: r3c6 = 2
(1.0) Full House: r2c4 = 8
(1.2) Hidden Single (In Block): r2c1 = 2 in b1
(1.1) Last Digit: r6c2 = 2
(1.2) Hidden Single (In Block): r6c1 = 4 in b4
(1.2) Hidden Single (In Block): r3c2 = 4 in b1
(1.2) Hidden Single (In Block): r3c3 = 6 in b1
(1.0) Full House: r3c1 = 8
(1.2) Hidden Single (In Block): r6c3 = 8 in b4
(1.0) Full House: r6c7 = 1
(1.1) Last Digit: r5c7 = 8
(1.2) Hidden Single (In Block): r4c3 = 1 in b4
(1.2) Hidden Single (In Block): r1c2 = 1 in b1
(1.0) Full House: r1c3 = 7
(1.0) Full House: r2c3 = 9
(1.0) Full House: r5c2 = 7
(1.0) Full House: r5c1 = 3
(1.0) Full House: r8c1 = 9
(1.0) Full House: r8c3 = 3
(1.1) Last Digit: r4c4 = 3
(1.0) Full House: r5c4 = 4
(1.1) Last Digit: r4c9 = 4
(1.0) Full House: r4c7 = 7
(1.0) Full House: r9c7 = 9
(1.0) Full House: r9c8 = 1
(1.1) Last Digit: r2c9 = 1
(1.0) Full House: r2c8 = 6
(1.0) Full House: r5c8 = 9
(1.0) Full House: r5c9 = 6
Puzzle has been solved.
Puzzle solution: 517639482239847561846512379691385724375421896428976135152798643983164257764253918
Time elapsed: 00:00.00.102
Technique used:
20 * Full House
6 * Last Digit
26 * Hidden Single (In Block)
3 * Hidden Single (In Row)
1 * Locked Pair
1 * Naked single
6 * Claiming
1 * Naked Pair
1 * Hidden Pair
2 * Naked Triple (+)
1 * Naked Quadruple (+)
Total solving steps count: 68
Difficulty total: 102.3
Puzzle rating: 5.1/1.2/1.2
```


## Intro to Solution Folders

标题：*解决方案文件夹介绍*

Here displays the introduction to all folders in this whole solution.

这里陈列出所有本解决方案会使用到的文件夹的所有介绍。

* `Sudoku.Core`
    * The implementation of all core data in sudoku, such as a sudoku [grid](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core.Old/Data/Meta/Grid.cs). All extension method in use is also in here.<br/>对于数独里所有核心部件的主要实现。当然，所有在项目里使用到的扩展方法也都在这里。
* `Sudoku.Core.Old`
    * Same as `Sudoku.Core` project, but use older implementation logic. For example, this project uses LINQ to implement all SSTS (Standard sudoku technique set) step finder, and uses very simple information to describe all information in a grid, which is reduced efficiency of calculation.<br/>和 `Sudoku.Core` 项目一样，不过这个项目里的所有部件都采用了很老旧的实现方式。举个例子，这个项目使用的是 LINQ 来实现的四大基本数独技巧（排除、唯一余数、区块、数组）的查找，而使用了人类一眼就能看明白的、很简单的信息表示手段来存储一个盘面的每一个细节信息，这样就会降低计算效率。
* `Sudoku.Checking`
    * The checking module aiming to a sudoku grid, such as checking pearl puzzle, minimal puzzle, backdoors and so on.<br/>检测数独盘面的一些指定额外特性的模块，例如检测一个题目是否是珍珠题、最小题，或者是检测一个题目存在的所有后门（和魔术格）等信息。
* `Sudoku.Solving`
    * Solving module of this whole solution.<br/>控制整个解决方案完成解题操作的所有内容的模块。
* `Sudoku.Drawing`
    * Painting module of this solution, which is used for GDI+.<br/>控制整个解决方案有关绘制图形的模块，一般用于 GDI+ 上。
* `Sudoku.Diagnostics`
    * The diagnostic controlling through all over the solution. In addition, those files are used with my own custom code analyzer and fixer (But this analyzer is not included in this solution. Therefore codes has not been uploaded).<br/>控制整个解决方案执行行为和编译期间行为的项目。另外，所有的文件都会依赖于我自己实现的分析器和代码修补工具（不过这一部分代码不属于项目，所以我没有上传）。
* `Sudoku.IO`
    * I/O operations to sudoku data.<br/>控制数独文件流处理的项目。
* `Sudoku.Runtime`
    * Provides all custom runtime exceptions in this solution.<br/>为整个解决方案提供所有的自定义运行时异常类。
* `Sudoku.Solving.BruteForces.Bitwise`
    * The bitwise brute force solver to a sudoku puzzle.<br/>项目解题期间使用的位运算爆破算法（JCZSolver）的源代码。
* `Sudoku.Debugging`
    * The console program aiming to debugging codes logic of other projects.<br/>旨在解决这整个解决方案里其它项目的 bug 和调试操作的项目。
* `Sudoku.Terminal`
    * The terminal of this project. You can use console arguments (such as `--solve` to solve a grid).<br/>这整个解决方案里的终端控制部分。你可以在控制台输入比如 `--solve` 来完成对一道题的分析和解题。



## Intro to Files

标题：*文件介绍*

Here displays the introduction to files in root folder.

这里陈列出根目录下的文件的基本说明。

* `.editorconfig`
    * Editor configuration file.<br/>用户配置文件（项目整体控制编译器错误等信息的控制信息，以及控制成员名称规范的信息）。
* `Priority of operators.txt`
    * Operators priority through C# language. (P.S. I don't know why I will upload this file, maybe of vital importance?)<br/>C# 语言里的运算符的优先级表。（我也不知道为啥我要上传它，可能它很重要？）



## Grid Format and Parsing format

标题：*盘面格式和解析为盘面的字符串样式*

If you has known the whole outline of this solution, you want to know how to use grid format. First of all, I will give you some characters you should use.

如果你对这个项目有所了解的话，你肯定想知道数独盘面的输入的具体格式。首先我会给你一个表，陈列的各种字符就是你需要用到的。

| Format string<br/>格式化字符 | Meaning<br/>意思                                                                   |
| --------------------------- | ---------------------------------------------------------------------------------- |
| `.` and `0`                 | Placeholder option.<br/>占位符选项。                                                |
| `+`                         | Modifiable values option.<br/>显示可修改的数值选项。                                |
| `!`                         | Modifiable values will be regarded as given ones.<br/>把可修改数值视为提示数的选项。 |
| `:`                         | Candidates-has-been-eliminated option.<br/>显示盘面已删除的候选数的选项。            |
| `#`                         | Intelligent output option.<br/>智能输出选项。                                       |

If you write `grid.ToString("0")`, all empty cells will be replaced by character `'0'`; but if you write `grid.ToString(".")`, empty cells will be shown by `'.'`.

如果你写的是 `grid.ToString("0")` 的话，所有的空格都会使用字符 `'0'` 所占据；而如果你写的是 `grid.ToString(".")` 的话，就会是 `'.'` 字符填充空格了。

If you add modifiable-value option `'+'`, all modifiable values (if exists) will be shown by `+digit` (In default case, modifiable values will not be output). In addition, if you write `'!'` rather than `'+'`, all modifiable values will be treated as given ones, so output will not be with plus symbol (i.e. `digit` rather than `'+digit'`). **Note that you should write either `'+'` or `'!'`**. Both characters written will generate a runtime exception.

如果你使用了可修改数值的选项 `'+'` 的话，所有可修改的数值（如果存在的话）都会以 `'+数字'` 的形式被显示在盘面里（在默认情况下，这些数字都是不会显示出来的）。另外，如果你写的是 `'!'` 而不是 `'+'` 的话，所有可修改的数值都会被当作提示数看待，输出的时候不会带有 `+` 符号。注意**这两个符号不要同时使用**。同时使用它们会产生一个运行时异常。

If you want to show candidates which have been eliminated before, you should add `':'` at the tail of the format string, which means all candidates that have been eliminated before current grid status can be also displayed in the output. However, **you should add this option `':'` at the tail position only**; otherwise, generating a runtime exception.

如果你想展示盘面当前情况下的候选数被删除的样子的话，你可以在整个格式化字符串的末尾添加 `':'` 字符，这表示所有当前盘面下被删除的候选数也会被显示出来。但是，**你只能把这个字符放在整个格式化字符串的末尾**，否则它会生成一个运行时异常。

If you cannot raise a decision, you can try intelligent output option `'#'`, which will be output intelligently.

如果你无法作出决定，你可以使用 `'#'` 字符作为格式化字符串，来表示输出智能化处理（检测到有候选数排除的情况会被显示出来；检测到如果有填入的数字也会被呈现出来）。

For examples, if you write:

比如你这么写代码：

```csharp
grid.ToString("0+");
```

This format `"0+"` means that all empty cells will be shown as digit 0, givens will be shown with digit 1 to 9, and modifiable values will be shown.

这个格式化字符串 `"0+"` 表示你的空格是用 `'0'` 字符表示的，并且还会显示所有可修改的数字；而提示数则直接使用 1 到 9 显示。

And another example:

另外一个例子：

```csharp
grid.ToString(".+:");
```

Output will treat:

输出：

* empty cells as `'.'` character,<br/>空格用 `'.'` 占位；
* modifiable values as `'+digit'`,<br/>可修改数值用 `'+数字'` 形式显示；
* candidates as `':candidateList'`.<br/>候选数使用 `:候选数序列` 形式显示。



All examples are shown at the end of this part.

上面所有解释在最后都会给出例子集，可以对照。

If you want to output pencil marked grid (PM grid), you should use options below:

另外，如果你要输出这个题目的候选数盘面的话，你可以使用下面的选项：

| Format string<br/>格式化字符 | Meaning<br/>意思                                                       |
| --------------------------- | ---------------------------------------------------------------------- |
| `@`                         | Default PM grid character.<br/>默认的候选数盘面输出的格式化字符。        |
| `*`                         | Simple output option.<br/>普通格线字符输出选项。                        |
| `!`                         | Treat-modifiable-as-given option.<br/>把填入的数字视为提示数的选项。     |

These option are same or similar as normal grid (Susser format) output, so I don't give an introduction about those characters. Learn them from examples at the end of this part.

这些选项都和普通盘面输出样式的输出模式差不多，所以我就不给出解释了。你可以在例子集里找到这些东西的详细用法。

By the way, character `'*'` is for simple output. If the format has not followed by this option, the grid outline will be handled subtly. You can find the difference between two outputs:

当然了，字符 `'*'` 用作简单格线的输出。如果你的格式化字符串没有这个选项的话，那么格线看起来就没有那么“圆润”。你可以对比下面两个示例，看看区别。

```
.---------------.---------------.------------------.
| 17   128  78  | 135  <9>  35  | *4*   12378  *6* |
| *9*  128  <3> | <4>  18   *6* | 1278  1278   <5> |
| <5>  <6>  *4* | <2>  138  <7> | <9>   138    13  |
:---------------+---------------+------------------:
| <8>  13   <2> | *6*  *5*  *9* | 17    <4>    137 |
| 13   <7>  <5> | 13   *4*  *8* | <6>   <9>    *2* |
| *6*  <4>  *9* | *7*  123  23  | <5>   13     <8> |
:---------------+---------------+------------------:
| 37   *5*  <1> | <8>  23   <4> | 27    <6>    <9> |
| <2>  89   *6* | 59   *7*  <1> | <3>   58     *4* |
| *4*  389  78  | 359  <6>  235 | 1278  12578  17  |
'---------------'---------------'------------------'

+---------------+---------------+------------------+
| 17   128  78  | 135  <9>  35  | *4*   12378  *6* |
| *9*  128  <3> | <4>  18   *6* | 1278  1278   <5> |
| <5>  <6>  *4* | <2>  138  <7> | <9>   138    13  |
+---------------+---------------+------------------+
| <8>  13   <2> | *6*  *5*  *9* | 17    <4>    137 |
| 13   <7>  <5> | 13   *4*  *8* | <6>   <9>    *2* |
| *6*  <4>  *9* | *7*  123  23  | <5>   13     <8> |
+---------------+---------------+------------------+
| 37   *5*  <1> | <8>  23   <4> | 27    <6>    <9> |
| <2>  89   *6* | 59   *7*  <1> | <3>   58     *4* |
| *4*  389  78  | 359  <6>  235 | 1278  12578  17  |
+---------------+---------------+------------------+
```



Examples:

示例：

![](pic/P1.png)

```
Format（格式）:
"0"

Output（输出结果）:
800190030190007600002000000000301504000050000704906000000000900008700051040069007
```



![](pic/P2.png)

```
Format（格式）:
"0+"

Output（输出结果）:
000090+40+6+90340+600556+4207900802+6+5+90400750+4+869+2+64+9+7005080+5180406920+60+7130+4+400060000

---

Format（格式）:
".!"

Output（输出结果）:
....9.4.69.34.6..55642.79..8.2659.4..75.486926497..5.8.518.4.692.6.713.44...6....
```



![](pic/P3.png)

```
Format（格式）:
"0+:"

Output（输出结果）:
320009+100+40000+150006130040004903+6+801+60+3812+90+48+10090360004008210+106000+70000010+3+649:515 615 724 825 228 229 731 235 738 748 563 972 574 882 484 584 792 295
```



![](pic/P1.png)

```
Format（格式）:
"@"

Output（输出结果）:
.---------------------.--------------------.----------------------.
| <8>   567     567   | <1>   <9>    245   | 247    <3>     25    |
| <1>   <9>     35    | 2458  2348   <7>   | <6>    248     258   |
| 3456  3567    <2>   | 4568  348    3458  | 1478   14789   589   |
:---------------------+--------------------+----------------------:
| 269   268     69    | <3>   278    <1>   | <5>    26789   <4>   |
| 2369  12368   1369  | 248   <5>    248   | 12378  126789  23689 |
| <7>   12358   <4>   | <9>   28     <6>   | 1238   128     238   |
:---------------------+--------------------+----------------------:
| 2356  123567  13567 | 2458  12348  23458 | <9>    2468    2368  |
| 2369  236     <8>   | <7>   234    234   | 234    <5>     <1>   |
| 235   <4>     135   | 258   <6>    <9>   | 238    28      <7>   |
'---------------------'--------------------'----------------------'
```



![](pic/P2.png)

```
Format（格式）:
"@*"

Output（输出结果）:
+---------------+---------------+------------------+
| 17   128  78  | 135  <9>  35  | *4*   12378  *6* |
| *9*  128  <3> | <4>  18   *6* | 1278  1278   <5> |
| <5>  <6>  *4* | <2>  138  <7> | <9>   138    13  |
+---------------+---------------+------------------+
| <8>  13   <2> | *6*  *5*  *9* | 17    <4>    137 |
| 13   <7>  <5> | 13   *4*  *8* | <6>   <9>    *2* |
| *6*  <4>  *9* | *7*  123  23  | <5>   13     <8> |
+---------------+---------------+------------------+
| 37   *5*  <1> | <8>  23   <4> | 27    <6>    <9> |
| <2>  89   *6* | 59   *7*  <1> | <3>   58     *4* |
| *4*  389  78  | 359  <6>  235 | 1278  12578  17  |
+---------------+---------------+------------------+
```



![](pic/P3.png)

```
Format（格式）:
"@!"

Output（输出结果）:
.----------------.----------------.-----------------.
| <3>  <2>  578  | 4567  478  <9> | <1>  78    678  |
| <4>  789  78   | 26    267  <1> | <5>  3789  3678 |
| 59   <6>  <1>  | <3>   578  57  | <4>  289   278  |
:----------------+----------------+-----------------:
| 257  <4>  <9>  | 57    <3>  <6> | <8>  25    <1>  |
| <6>  57   <3>  | <8>   <1>  <2> | <9>  57    <4>  |
| <8>  <1>  27   | 457   <9>  457 | <3>  <6>   257  |
:----------------+----------------+-----------------:
| 579  357  <4>  | 679   567  <8> | <2>  <1>   35   |
| <1>  359  <6>  | 29    245  45  | <7>  358   358  |
| 257  58   2578 | <1>   57   <3> | <6>  <4>   <9>  |
'----------------'----------------'-----------------'
```



## Author

标题：*作者*

Sunnie, from Chengdu, is an original undergraduate.

小向，来自成都的一名普通大学生。
