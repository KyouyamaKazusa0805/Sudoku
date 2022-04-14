# 控制台参数

本仓库有一个 [`Sudoku.CommandLine`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CommandLine) 项目，提供了一些基本的数独轻量级处理操作。如果你会使用控制台的话，可以使用这个项目来完成一些调试和处理操作来代替打开 UI 程序。

下面列举一下整个控制台项目提供的控制台参数信息。

## 根命令[^1]

下面列举一下根级别的指令。按指令使用的单词的字典序进行排序。

[**特性判别指令 `check`**](root-commands/check)

表示判别和获取指定的数独题目是否满足指定的一些特性规则。

[**格式化指令 `format`**](root-commands/format)

表示将一个数独盘面格式化处理为等价的字符串表达。

[**出题指令 `generate`**](root-commands/generate)

表示出一个数独题目。

**帮助指令 `help`**

表示获取这个程序的相关帮助信息，获取所有根指令的处理规则和用法。

[**解题指令 `solve`**](root-commands/solve)

表示用于获取一个数独题目的终盘（即它的解）。

**版本查看指令 `version`**

表示获取这个程序的版本号。

[**关于指令 `visit`**](root-commands/visit)

表示获取程序和作者的基本信息（即它的介绍网页地址）。



[^1]: **根命令**（Root Command）也称为**根级别命令**，指的是使用整个控制台项目里的基本处理操作的命令。它用于提供和区分众多操作的唯一标识符号。
