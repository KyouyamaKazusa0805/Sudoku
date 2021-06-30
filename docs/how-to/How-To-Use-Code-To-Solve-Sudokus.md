# 如何使用解题功能
我们如何使用程序的功能来完成一道题目？我们将介绍四种使用程序完成题目的代码。

当然，如果我们需要在使用功能之前，先引入相关的 DLL 文件才能开始完成代码。当然，你也别忘了把资源文件（指的是 `lang` 文件夹，里面带了翻译文件）拷贝过去。如果不拷贝这个文件的话，运行时就会抛出 `SudokuRuntimeException`，提示你程序尚未找到资源字典文件。

## 逻辑解法

```csharp
using System;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.Manual;

// Define a sudoku grid using the string code.
var grid = SudokuGrid.Parse("250010000001089402940700000510200000000000000000007031000001026807640100000020043");

// Now define a instance for solving the puzzle.
var solver = new ManualSolver();

// Solve this puzzle.
var analysisResult = solver.Solve(grid);

// Output the result.
Console.WriteLine(analysisResult.ToString(AnalysisResultFormattingOptions.ShowDifficulty, CountryCode.ZhCn));
```

运行结果：

```
题目：25..1......1.894.294.7.....51.2...................7.31.....1.268.764.1......2..43
解题工具：Manual
技巧使用情况：
 20 * 同区剩余
  6 * 同数剩余
 18 * 宫排除
  6 * 行排除
  1 * 列排除
  2 * 唯一余数
  1 * 宫区块
  6 * 行列区块
  1 * 隐性数对
  1 * 二阶鳍鱼
  1 * XY-Wing
  1 * 欠一数对
  2 * 隐性唯一矩形
  1 * 唯一矩形 + 3U / 2SL
  2 * 双值格环
  1 * 融合跨区数组
  9 * 不连续环
  1 * 普通链
  1 * 全双值格致死解法-双强链法则
 81个步骤
题目难度分：5.8/1.2/1.2
题目被解出。
耗时：00:00:06.818
```

## 快速爆破解法

实际上快速爆破算法的使用程序代码完全不需要你修改很多地方，你只需要把 `ManualSolver` 改成 `UnsafeBitwiseSolver` 就行了（仅需要改动这里，把解题器替换掉，其它的内容都是一样的）。

```csharp
using System;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;

// Define a sudoku grid using the string code.
var grid = SudokuGrid.Parse("250010000001089402940700000510200000000000000000007031000001026807640100000020043");

// Now define a instance for solving the puzzle.
var solver = new UnsafeBitwiseSolver();

// Solve this puzzle.
var analysisResult = solver.Solve(grid);

// Output the result.
Console.WriteLine(analysisResult.ToString(AnalysisResultFormattingOptions.ShowDifficulty, CountryCode.ZhCn));
```

运行结果：

```
题目：25..1......1.894.294.7.....51.2...................7.31.....1.268.764.1......2..43
解题工具：Bitwise (Unsafe)
题目难度分：20.0/0.0/20.0
终盘：256314978371589462948762315514238697793156284682497531435971826827643159169825743
题目被解出。
耗时：00:00:00.010
```

## LINQ 解法

LINQ 是 C# 独特的一种处理程序的机制，它的算法代码写起来也是非常优雅。其主要的处理代码是这样的：

```csharp
static List<string> Solve(string puzzle)
{
    const string values = "123456789";
    var result = new List<string> { puzzle };

    while (result.Count > 0 && result[0].IndexOf('0', StringComparison.OrdinalIgnoreCase) != -1)
    {
        result = new List<string>(
            from solution in result
            let index = solution.IndexOf('0', StringComparison.OrdinalIgnoreCase)
            let column = index % 9
            let block = index - index % 27 + column - index % 3
            from value in values
            where !(
                from i in Enumerable.Range(0, 9)
                let inRow = solution[index - column + i] == value
                let inColumn = solution[column + i * 9] == value
                let inBlock = solution[block + i % 3 + (int)Floor(i / 3f) * 9] == value
                where inRow || inColumn || inBlock
                select i
            ).Any()
            select $"{solution[0..(index + 1)]}{value}{solution[(index + 1)..]}");
    }

    return result;
}
```

下面我们将利用这个算法对程序进行解题。当然，同一个题目，使用不同算法，输出的答案肯定是一样的，只是算法执行起来有所不同。

当然，你也只需要修改解题器，改成 `OneLineLinqSolver` 就行了。

```csharp
using System;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Linqing;

// Define a sudoku grid using the string code.
var grid = SudokuGrid.Parse("250010000001089402940700000510200000000000000000007031000001026807640100000020043");

// Now define a instance for solving the puzzle.
var solver = new OneLineLinqSolver();

// Solve this puzzle.
var analysisResult = solver.Solve(grid);

// Output the result.
Console.WriteLine(analysisResult.ToString(AnalysisResultFormattingOptions.ShowDifficulty, CountryCode.ZhCn));
```

运行结果：

```
题目：25..1......1.894.294.7.....51.2...................7.31.....1.268.764.1......2..43
解题工具：One line LINQ
题目难度分：20.0/0.0/20.0
终盘：256314978371589462948762315514238697793156284682497531435971826827643159169825743
题目被解出。
耗时：00:00:02.654
```

## 经典回溯解法（无优化）

还是只需要修改解题器，改成 `BacktrackingSolver` 就行了。

```csharp
using System;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Backtracking;

// Define a sudoku grid using the string code.
var grid = SudokuGrid.Parse("250010000001089402940700000510200000000000000000007031000001026807640100000020043");

// Now define a instance for solving the puzzle.
var solver = new BacktrackingSolver();

// Solve this puzzle.
var analysisResult = solver.Solve(grid);

// Output the result.
Console.WriteLine(analysisResult.ToString(AnalysisResultFormattingOptions.ShowDifficulty, CountryCode.ZhCn));
```

运行结果：

```
题目：25..1......1.894.294.7.....51.2...................7.31.....1.268.764.1......2..43
解题工具：Backtracking
题目难度分：20.0/0.0/20.0
终盘：256314978371589462948762315514238697793156284682497531435971826827643159169825743
题目被解出。
耗时：00:00:00.107
```

