# 解一道题

通过 API 来解一道题。做法很简单，只需要使用 `Grid` 类型得到一个盘面的信息，然后直接传入到 `LogicalSolver` 对象里进行参与解题即可。

```csharp
using System;
using Sudoku.Concepts;
using Sudoku.Solving.Logics;

// 这里获取一个解题的字符串代码。
// 该代码支持众多格式，详情请参考“Grid 类型支持的格式化字符串”文档页面的内容。
const string puzzle = "........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........";
// 这里也可以通过控制台输入的形式接收。比如：
// string puzzle = args[0];

// 读取一个字符串形式的数独盘面的代码信息，并解析为 'Grid' 类型的对象。
var grid = (Grid)puzzle;
// 你也可以使用 Parse 方法获取结果。
//var grid = Grid.Parse(puzzle);

// 声明实例化一个 'LogicalSolver' 类型的实例，用于稍后的解题。
var solver = new LogicalSolver();

// 以同步的形式解题。
// 返回一个对象，该对象包含解题操作的基本信息，例如题目是否被正确解出、
// 是否题目唯一解之后正常解出，还是题目过于困难导致软件提供的技巧都无法完成、
// 解题的每一个步骤信息等等。
var analysisResult = solver.Solve(grid);

// 输出分析结果。
Console.WriteLine(analysisResult);
```

