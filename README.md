<center>语言切换：<a href="README-zh-cn.md">简体中文</a></center>

# Sudoku Studio[^1]

[![stars](https://img.shields.io/github/stars/SunnieShine/Sudoku?color=097abb)](https://github.com/SunnieShine/Sudoku/stargazers)
[![issues](https://img.shields.io/github/issues/SunnieShine/Sudoku?color=097abb)](https://github.com/SunnieShine/Sudoku/issues)
[![license](https://img.shields.io/github/license/SunnieShine/Sudoku?color=097abb)](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE)

![](https://img.shields.io/badge/Programming%20Language-C%23%2012%20Preview-%23178600)
![](https://img.shields.io/badge/Framework-.NET%208-blueviolet)
![](https://img.shields.io/badge/Indenting-Tabs-lightgrey)
![](https://img.shields.io/badge/IDE-Visual%20Studio%202022%20v17.8%20Preview%204-%23cf98fb?logo=Visual%20Studio)
![](https://img.shields.io/badge/Language-English%2C%20Simplified%20Chinese-success)
[![](https://img.shields.io/badge/UI%20Project-SudokuStudio-%230d1117)](https://github.com/SunnieShine/Sudoku/tree/main/src/SudokuStudio/SudokuStudio)

[![](https://img.shields.io/badge/GitHub%20repo-Here!-%230d1117?logo=github)](https://github.com/SunnieShine/Sudoku)
[![](https://img.shields.io/badge/Gitee%20repo-Here!-%230d1117?logo=gitee)](https://gitee.com/SunnieShine/Sudoku)
[![](https://img.shields.io/badge/Wiki-Here!-%230d1117)](https://sunnieshine.github.io/Sudoku/)

[![bilibili](https://img.shields.io/badge/dynamic/json?color=%23fb7299&label=bilibili&logo=bilibili&query=%24.data.follower&suffix=%20followers&url=https%3A%2F%2Fapi.bilibili.com%2Fx%2Frelation%2Fstat%3Fvmid%3D23736703)](https://space.bilibili.com/23736703)

## Introduction

A sudoku handling SDK using brute forces and logical techniques. Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

### Sample

Here I give you [a sample](https://sunnieshine.github.io/Sudoku/usages/en/solve-by-manual) to demonstrate how to solve a puzzle using manual sudoku techniques.

### Extra description

In the future, I'd like to apply this solution to **almost every platform**. I may finish the Win10 app project, android app project, bot on common on-line platforms (QQ, Bilibili and so on).

Please note that the programming language version is always used as 'preview', which means some preview features are still used in this solution.

You can also use JetBrains Rider as your IDE. Use whatever you want to use, even Notepad :D Although C# contains some syntaxes that are only allowed in Visual Studio (e.g. keyword `__makeref`), this repo doesn't use them. Therefore, you can use other IDEs to develop the code in this repo liberally.

In addition, the framework and IDE version may update in the future; in other words, they aren't changeless. The information is **for reference only**.

### Repository Positioning

This repository is created for the following kinds of users:

* **who likes sudoku algorithms**: If you like to study for sudoku techniques and its backing implementation and running mechanism, you can find answer in this repository.
* **who wants to learn about C#-related features**: This repository uses some C# newer features and .NET-related features such as Windows UI, which can help you learn more about it as practicing.
* **myself**: The codes are very important for me to learn sudoku and algorithms.

## Prefaces

### Main Page

![](docs/pic/winui.png)

### Puzzle Library Page

![](docs/pic/puzzle-lib.png)

### Technique Gallery Page

![](docs/pic/gallery.png)

## Technique Supports

This solution supports many kinds of human-friendly techniques. Here I will list them.

* Singles
  * Naked Single, Full House
  * Hidden Single, Last Digit
* Candidate-Related Techniques
  * Locked Candidates Family
    * LC (Locked Candidates)
    * ALC (Almost Locked Candidates)
    * Firework Subsets (Hanabi)
  * Subsets
    * Naked Subsets
      * Locked Subsets (Naked, Hidden)
      * Semi-Locked Subsets
      * Normal Naked Subsets
    * Hidden Subsets
  * Fishes (including finned types)
    * Normal Fishes
    * Complex Fishes (Franken, Mutant)
  * Single-Digit Structures
    * Two-Strong-Links (Non-grouped, Grouped)
    * Empty Rectangle
  * Wings
    * Regular Wing (XY-Wing, XYZ-Wing, etc.)
    * Irregular Wing[^2]
      * W-Wing (Basic, Grouped, Multi-Branch)
      * M-Wing
      * Local-Wing
      * Split-Wing
      * Hybrid-Wing
      * Purple Cow
  * Uniqueness
    * UR (Unique Rectangle)
    * UL (Unique Loop)
    * XR (Extended Rectangle, Including fit & fat types)
    * Borescoper's Deadly Pattern
    * Qiu's Deadly Pattern
    * Unique Matrix
    * BUG (Bivalue Universal Grave)
    * Reverse BUG (Reverse Bivalue Universal Grave)
  * Chains
    * Forcing Chains / Bidirectional Cycles
    * Multiple Forcing Chains
    * Dynamic Forcing Chains
    * Blossom Loop
  * Almost Locked Sets
    * Chaining ALSes (Including ALS-XZ, ALS-XY-Wing, ALS-W-Wing)
    * Empty Rectangle Intersection Pair
  * Rank Logics
    * Positive or 0 -Rank Logics
      * SDC (Sue de Coq, Including basic type, isolated digit type and cannibalistic type)
      * 3D SDC (3-Dimensional Sue de Coq)
      * DL (Domino Loop)
      * Exocets
        * JE (Junior Exocet)
        * SE (Senior Exocet)
      * MSLS (Multi-Sector Locked Sets)
    * Negative Rank Logics
      * Guardian
      * Bi-value Oddagon
      * Chromatic Pattern (i.e. Tri-value Oddagon)
  * Symmetrical Placements
    * GSP (Gurth's Symmetrical Placements, Normal & Anti Type)
  * Permutations
    * Aligned Exclusion
  * Last Resorts
    * Bowman's Bingo
    * Computer Algorithms
      * Pattern Overlay
      * Templating
      * Brute Force

Some other techniques will be implemented later, such as senior exocets, baba grouping, death blossom, forcing chains and dynamic chains.

## Forks & PRs (Pull Requests) for This Repo

Of course you can fork my repo and do whatever you want. You can do whatever you want to do under the [MIT license](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE). However, due to the copy of the GitHub repo, Gitee repo doesn't support any PRs. I'm sorry. But you can create the issue on both two platforms. Please visit the following part "Basic Information" for learning about more details.

In addition, this repo may update **frequently** (At least 1 commit in a day).

## Open source license

### Codes

The codes in this repository uses **MIT license**, which means you should mention the copyright of the author of the repository if you want to use code snippet in this repository.

### Special project: `Sudoku.Recognition`

This project uses some APIs and source code from repo [EmguCV](https://github.com/emgucv/emgucv). Therefore, the project uses a standalone open-source license. For more information please visit the file `LICENSE` in that project in source code.

### Document project: `SudokuTutorial`

This project use **CC-BY-4.0 license** to constraint your usage and copyright statements on this repository. You should mention the copyright of the author in this repository and give the state changes after modified the content in the copied one if you want to use the Wiki pages.

### Wiki pages

The repository also lists many wiki pages, being stored in the folder `docs`. In addition, files `README.md` and `README-zh-cn.md` (as Chinese translation version) are also included as wiki pages. Those pages uses **CC-BY-4.0 license** to constraint your usage and copyright statements on this repository. You should mention the copyright of the author in this repository and give the state changes after modified the content in the copied one if you want to use the Wiki pages.

## Code of Conduct

Please see file [CODE_OF_CONDUCT](CODE_OF_CONDUCT).

## Sudoku Technique References

Here we list some websites about sudoku techniques that I used and referenced. The contents are constructed by myself, so if you want to learn more about sudoku techniques that this solution used and implemented, you can visit the following links[^4] to learn about more information.

* [标准数独技巧教程 (Video)_bilibili](https://www.bilibili.com/video/BV1Mx411z7uq)
* [标准数独技巧教程 (Text, New)_bilibili](https://www.bilibili.com/read/readlist/rl717501)[^5]

## Author

Sunnie, from Chengdu, is a normal undergraduate from Sichuan Normal University. I mean, a normal university (Pun)

Please visit file [CONTACTS](CONTACTS) to contact me if you want.

[^1]: The old name of the repository is "Sunnie's Sudoku Solution".

[^2]: W-Wing is implemented by a single technique searcher type, while other irregular wing types are implemented by AIC searchers.

[^3]: Because only type 2 contains a valid test example.

[^4]: I'm sorry that those pages are only written in Chinese. However, I may create pages written in other languages in the future.

[^5]: Under construction. Please wait with patience.
