Welcome to the Sudoku wiki! Some pages only support Chinese... I'm sorry for that.

欢迎来到 Sudoku 的 Wiki 界面！一些页面只支持中文，抱歉了。

> The link displayed as **bold** means the page supports English.
>
> 粗体呈现的链接是支持英文的。



## 更新 (Update Log)

This section lists the update logs. If you want to check what content was added, removed or updated, please see them below.

本节陈列更新日志。如果你要查看什么东西添加了、删掉了或者是更新了，就查看下面这些链接。

* [V0.1 (20210110)](version/Version-0.1)
* [V0.2 (20210126)](version/Version-0.2)
* [V0.3 (20210313)](version/Version-0.3)
* [V0.4 (20210414)](version/Version-0.4)
* [V0.5 (20210620)](version/Version-0.5)
* [V0.6 (20210718)](version/Version-0.6)
* [V0.7](version/Version-0.7)



## 基本内容 (Basic Links)

This section you'll find the details about this solution.

这一节你可以找到关于整个解决方案的详细内容。

* 解决方案介绍
  * [**如何编译代码 (How to compile the solution)**](how-to/How-To-Compile-The-Solution)
  * [文件夹介绍](Introduction-Of-All-Folders)
  * [数独解题技巧和算法支持](Technique-Supports)
  * [程序热键（快捷键）](Program-Hotkeys)
* API 介绍
  * 条件编译符号
    * [条件编译符号介绍](conditional-compilation-symbols/Introduction-About-Conditional-Compilation-Symbols)
    * [条件编译符号列表](conditional-compilation-symbols/List-Of-Conditional-Compilation-Symbols)
  * `SudokuGrid` 结构
    * [该类型的介绍](types/structs/How-To-Use-Struct-SudokuGrid)
    * [该类型的格式串](types/structs/Formats-Of-SudokuGrid)
  * [`Cells` 结构](types/structs/How-To-Use-Struct-Cells)
  * [`AnalysisResult` 记录类](types/classes/How-To-Use-Record-Class-AnalysisResult)
  * [`CodeCounter` 类](types/classes/How-To-Use-Class-CodeCounter)
  * 异常
    * [`AssemblyFailedToLoadException` 异常](types/exceptions/Exception-AssemblyFailedToLoadException)
    * [`FailedToFillValueException` 异常](types/exceptions/Exception-FailedToFillValueException)
    * [`InvalidPuzzleException` 异常](types/exceptions/Exception-InvalidPuzzleException)
    * [`MultipleSolutionsException` 异常](types/exceptions/Exception-MultipleSolutionsException)
    * [`NoSolutionException` 异常](types/exceptions/Exception-NoSolutionException)
    * [`PuzzleShouldHaveBeenSolvedException` 异常](types/exceptions/Exception-PuzzleShouldHaveBeenSolvedException)
    * [`RecognizerHasNotBeenInitializedException` 异常](types/exceptions/Exception-RecognizerHasNotBeenInitializedException)
    * [`ResourceCannotBeFoundException` 异常](types/exceptions/Exception-ResourceCannotBeFoundException)
    * [`TesseractException` 异常](types/exceptions/Exception-TesseractException)
    * [`WrongStepException` 异常](types/exceptions/Exception-WrongStepException)
    * [`CodeIsInvalidException` 异常](types/exceptions/Exception-CodeIsInvalidException)
* API 使用
  * [如何使用解题功能](how-to/How-To-Use-Code-To-Solve-Sudokus)
  * [数独盘面格式串支持情况](how-to/Sudoku-Grid-Format-Supports)
  * [批处理指令](how-to/Batch-Commands)
* 开发参考
  * [如何取消一个 `Task` 对象](how-to/How-To-Cancel-A-Task)
  * [如何添加一个菜单选项](how-to/How-To-Add-A-Menu-Item)
  * [如何添加一个技巧搜索器](how-to/How-To-Add-A-Technique-Searcher)
  * [如何添加一个程序设置项](how-to/How-To-Add-A-Preference-Item)
* API 使用的一些例子
  * 直接使用
    * [使用人工解题器解题](examples/Solve-Sudoku-Using-Manual-Solver)
    * [代码反射](examples/Get-Code-Reflection)
    * [解析 CSV 文档](examples/Parse-CSV-Document)
  * 老的技巧算法参考
    * [复杂鱼](technique-algorithm-ref/Old-Algorithm-Complex-Fish)
    * [死亡绽放](technique-algorithm-ref/Old-Algorithm-Death-Blossom)
* 其它
  * ~~[如何使用机器人项目](how-to/How-To-Compile-Bot-Projects)~~
  * [伪代码约定](miscellaneous/Pseudo-Code-Convention)



## 语义分析 (Code Analysis)

In this section, you'll find the code analyzers and fixers about this solution. This solution provides you with code analyzers that analyzes the code written and find the diagnostic result about this solution or .NET analyzers can't analyze on this case. Code analyzers can be separated to two parts: 1) About solution, 2) About syntax. You'll find them below.

这一节将给出整个解决方案的代码分析和修补工具。解决方案甚至贴心地给你提供了语义分析器，可对你书写的代码进行代码分析。分析类别分项目相关和语法相关两类分析诊断信息，详情请参考本节的内容。

* [什么是语义分析器](code-analysis/What-Is-A-Code-Analyzer)
* [分类表](code-analysis/Code-Analysis-Table)
* [代码分析规则集](code-analysis/Code-Analysis-Rule-List)



## C# 语法选讲 (Language Features)

This section provides you with newer C# programming language features and their details. If you don't know when or how to use them, see them below.

本节提供 C# 一些新语法的内容讲解。如果你不是很会用，或者不知道啥时候用的话，那就看链接吧。

Please note that **NOT ALL** language features will be introduced below. I'll only introduce some useful for me or this solution, or powerful features.

请注意**并不是所有的**语言特性都会介绍。我只介绍一些对我或整个解决方案有用的、或者是强大的特性。

* C# 7
  * [引用结构](language-ref/Ref-Struct)
  * [`unmanaged` 泛型约束](language-ref/Unmanaged-Generic-Type-Constraint)
* C# 8
  * [可空引用类型](language-ref/Nullable-Reference-Types)
* C# 9
  * [函数指针](language-ref/Function-Pointer)
  * [源代码生成器](language-ref/Source-Generator)
* C# 10
  * [Lambda 拓展](language-ref/Extended-Lambdas)
* 综合内容
  * [模式匹配](language-ref/Pattern-Matching)