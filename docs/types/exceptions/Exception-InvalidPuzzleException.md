# `InvalidPuzzleException` 异常
## 基本信息

异常类名：`InvalidPuzzleException`

命名空间：`Sudoku`

描述：有一些时候，题目需要先验证，并且当题目通过验证之后，才能进行下一步操作。如果题目本身没有通过验证，就可以尝试触发此异常来表示题目不合法。

## 用法

### 异常抛出

```csharp
bool isValid = CheckGridIsValid(grid);

if (!isValid)
    throw new InvlidPuzzleException(grid);
```

### 异常捕获

当捕获此异常的时候，可能会根据调用方分成不同的处理模式。比如说控制台直接输出错误信息（实现逻辑例如 `CONSOLE` 符号内的条件编译代码块）；也可能是 WPF 项目或调试期处理，使用 `Debug.WriteLine` 来输出错误信息；如果都不是的话，也可以直接抛出原本的异常。

```csharp
try
{
    // Do something that uses 'grid'.
}
catch (InvalidPuzzleException ex)
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
    // Do something that uses 'grid'.
}
catch (InvalidPuzzleException ex) when (ex.Reason is not null)
{
#if CONSOLE
    Console.WriteLine($"The grid is invalid because {ex.Reason}.");
#elif DEBUG
    Debug.WriteLine($"The grid is invalid because {ex.Reason}.");
#else
    throw;
#endif
}
```