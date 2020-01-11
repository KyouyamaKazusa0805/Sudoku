# Sudoku

A sudoku solver through brute forces and logical techniques (Update files gradually).



## C# Version and IDE using

* C# version: 8.0
* IDE using: Visual Studio 2019 V16.4



## How to use

Clone this repo, and you can take all codes!

You can write code like this:

```csharp
using System;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual;

internal static class Project
{
    private static void Main(string[] args)
    {
        var grid = Grid.Parse("800190030190007600002000000000301504000050000704906000000000900008700051040069007");
        var solver = new ManualSolver();
        var analysisResult = solver.Solve(grid);
        
        Console.WriteLine(analysisResult);
    }
}
```

And the puzzle solution and analysis result will be displayed on console screen!


## Intro to Solution Folders

Here displays the introduction to all folders in this whole solution.

* `Sudoku.Core.Old`
    * The implementation of the sudoku meta data, such as sudoku [grid](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core.Old/Data/Meta/Grid.cs) class and so on. Also provides some extension methods in this project, such as [string.Match([Pattern] string)](https://github.com/Sunnie-Shine/Sudoku/blob/master/Sudoku.Core.Old/Data/Extensions/StringEx.cs#L10).
* `Sudoku`
    * The newer implementation of sudoku meta data. Different than `Sudoku.Core.Old`, all files are re-implemented by newer logic. For example, older implementation of grid class uses `CellInfo` struct to describe all information in a cell, which is a "vivid" description; while newer one uses bitwise operations (using 12 bits to represent a cell information, 3 bits for cell status and other 9 bits are candidates status).
* `Sudoku.Debugging`
    * The console program aiming to debugging codes logic of other projects.
* `Sudoku.Diagnostics`
    * The diagnostic controlling through all over the solution. In addition, those files are used with my own custom code analyzer and fixer (But this analyzer is not included in this solution. Therefore codes has not been uploaded).
* `Sudoku.IO`
    * I/O operations to sudoku data (This project has not been implemented).
* `Sudoku.Solving.Bf.Bitwise`
    * The bitwise brute force solver to a sudoku puzzle.
* `Sudoku.Terminal`
    * The terminal of this project. You can use console arguments (such as `--solve` to solve a grid). However, this project has not been implemented also.



## Intro to Files

Here displays the introduction to files in root folder.

* `.editorconfig`
    * Editor configuration file.
* `Priority of operators.txt`
    * Operators priority through C# language. (P.S. I don't know why I will upload this file, maybe of vital importance?)



## Grid Format and Parsing format

If you has known the whole outline of this solution, you want to know how to use grid format. First of all, I will give you some characters you should use.

| Format string | Meaning                                           |
| ------------- | ------------------------------------------------- |
| `.` and `0`   | Placeholder option.                               |
| `+`           | Modifiable values option.                         |
| `!`           | Modifiable values will be regarded as given ones. |
| `:`           | Candidates-has-been-eliminated option.            |
| `#`           | Intelligent output option.                        |

If you write `grid.ToString("0")`, all empty cells will be replaced by character `'0'`; but if you write `grid.ToString(".")`, empty cells will be shown by `'.'`.

If you add modifiable-value option `'+'`, all modifiable values (if exists) will be shown by `+digit` (In default case, modifiable values will not be output). In addition, if you write `'+'` rather than `'!'`, all modifiable values will be treated as given ones, so output will not be with plus symbol (i.e. `digit` rather than `'+digit'`). **Note that you should write either `'+'` or `'!'`**. Both characters written will generate a runtime exception.

If you want to show candidates which have been eliminated before, you should add `':'` at the tail of the format string, which means all candidates that have been eliminated before current grid status can be also displayed in the output. However, **you should add this option `':'` at the tail position only**; otherwise, generating a runtime exception.

If you cannot rise a decision, you can try intelligent output option `'#'`, which will be output intelligently.

All options above can be used with other ones. For examples, if you write:

```csharp
grid.ToString("0+");
```

This format `"0+"` means that all empty cells will be shown as digit 0, givens will be shown with digit 1 to 9, and modifiable values will be shown.

And another example:

```csharp
grid.ToString(".+:");
```

Output will treat:

* empty cells as `'.'` character,
* modifiable values as `'+digit'`,
* candidates as `'candidateList'`.

All examples are shown at the end of this part.

If you want to output pencil marked grid (PM grid), you should use options below:

| Format string | Meaning                           |
| ------------- | --------------------------------- |
| `@`           | Default PM grid character.        |
| `*`           | Simple output option.             |
| `!`           | Treat-modifiable-as-given option. |

These option are same or similar as normal grid (Susser format) output, so I don't give an introduction about those characters. Learn them from examples at the end of this part.

By the way, character `*` is for simple output. If the format has not followed by this option, the grid outline will be handled subtly. You can find the difference between two outputs:

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

![](pic/P1.png)

```
Format:
"0"

Output:
800190030190007600002000000000301504000050000704906000000000900008700051040069007
```



![](pic/P2.png)

```
Format:
"0+"

Output:
000090+40+6+90340+600556+4207900802+6+5+90400750+4+869+2+64+9+7005080+5180406920+60+7130+4+400060000
```



![](pic/P3.png)

```
Format:
"0+:"

Output:
320009+100+40000+150006130040004903+6+801+60+3812+90+48+10090360004008210+106000+70000010+3+649:515 615 724 825 228 229 731 235 738 748 563 972 574 882 484 584 792 295
```



![](pic/P1.png)

```
Format:
"@"

Output:
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
Format:
"@*"

Output:
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
Format:
"@!"

Output:
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

Sunnie, a student from Chengdu, China... An original undergraduate.
