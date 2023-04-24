# 解一道题

通过 API 来解一道题。做法很简单，只需要使用 `Grid` 类型得到一个盘面的信息，然后直接传入到 `Analyzer` 对象里进行参与解题即可。

```csharp
using System;
using Sudoku.Analytics;
using Sudoku.Concepts;

// 这里获取一个解题的字符串代码。
// 该代码支持众多格式，详情请参考“Grid 类型支持的格式化字符串”文档页面的内容。
const string puzzle = "........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........";
// 这里也可以通过控制台输入的形式接收。比如：
// string puzzle = args[0];

// 读取一个字符串形式的数独盘面的代码信息，并解析为 'Grid' 类型的对象。
var grid = (Grid)puzzle;
// 你也可以使用 Parse 方法获取结果。
//var grid = Grid.Parse(puzzle);

// 声明实例化一个 'Analyzer' 类型的实例，用于稍后的解题。
var analyzer = new Analyzer();

// 以同步的形式解题。
// 返回一个对象，该对象包含解题操作的基本信息，例如题目是否被正确解出、
// 是否题目唯一解之后正常解出，还是题目过于困难导致软件提供的技巧都无法完成、
// 解题的每一个步骤信息等等。
var analyzerResult = analyzer.Analyze(grid);

// 输出分析结果。
Console.WriteLine(analyzerResult);
```

如果你要复杂一点的解题过程，可以这么写：

```csharp
var grid = (Grid)"..38......8..16...5....29..9.....23..2..3..4..46.....7..72....1...46..8......15..";

var solver = CommonLogicalSolvers.Suitable;

const char finishedChar = '■', unfinishedChar = '□';
var width = Console.WindowWidth - 2;
var progress = new Progress<double>(printProgress);

var result = solver.Solve(grid, progress);

Console.Clear();
lock (SyncRoot)
{
    switch (result)
    {
        case { UnhandledException: { } ex }:
        {
            Terminal.WriteLine(ex, ConsoleColor.Red);
            break;
        }
        case { IsSolved: true }:
        {
            Terminal.WriteLine(result);
            break;
        }
    }
}

void printProgress(double percent)
{
    lock (SyncRoot)
    {
        Console.Clear();
        Console.WriteLine($"系统正在解题……进度：{percent:P}");
        Console.WriteLine($"{new string(finishedChar, (int)(percent * width))}{new string(unfinishedChar, (int)((1 - percent) * width))}");
    }
}

file static partial class Program
{
    private static readonly object SyncRoot = new();
}
```

