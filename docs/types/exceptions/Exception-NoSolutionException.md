# `NoSolutionException` 异常
## 基本信息

异常类名：`NoSolutionException`

命名空间：`Sudoku`

描述：当分析题目之前，会尝试对题目进行唯一性判断。如果题目无解的话，就会产生该异常的实例。

## 用法

### 异常抛出

```csharp
bool noSolution = GetWhetherGridHasNoSolution(grid);

if (noSolution)
    throw new NoSolutionException(grid);
```

### 异常捕获

当捕获此异常的时候，可能会根据调用方分成不同的处理模式。比如说控制台直接输出错误信息（实现逻辑例如 `CONSOLE` 符号内的条件编译代码块）；也可能是 WPF 项目或调试期处理，使用 `Debug.WriteLine` 来输出错误信息；如果都不是的话，也可以直接抛出原本的异常。

```csharp
try
{
    _ = GetAnalysisResult(grid);
}
catch (NoSolutionException ex)
{
#if CONSOLE
    Console.WriteLine(ex.Message);
#elif DEBUG
    Debug.WriteLine(ex.Message);
#else
    throw;
#endif
}
```

当然，你也可以根据条件进行筛选：

```csharp
try
{
    _ = GetAnalysisResult(grid);
}
catch (NoSolutionException ex) when (ex.InvalidGrid == SudokuGrid.Undefined)
{
#if CONSOLE
    Console.WriteLine("The grid is a in a invalid state.");
#elif DEBUG
    Debug.WriteLine("The grid is a in a invalid state.");
#else
    throw;
#endif
}
```