# `WrongStepException` 异常
## 基本信息

异常类名：`WrongStepException`

命名空间：`Sudoku.Solving.Manual`

描述：如果技巧搜索器存在 bug 的话，那么记录的技巧就可能存在删数或出数的逻辑错误。此时会在运行时抛出异常提示用户，步骤错误，请联系作者解决该 bug。

## 用法

### 异常抛出

```csharp
var eliminations = step.Eliminations;
if (CheckEliminationValid(grid, eliminations))
    throw new WrongStepException(grid, step);
```

### 异常捕获

这个异常用来提示用户，因此捕获直接使用 `catch` 即可。

```csharp
try
{
    // Do something.
}
catch (WrongStepException ex)
{
#if WPF
    MessageBox.Show("Wrong", ex.ToString(), MessageBoxButtons.YesNo);
#elif DEBUG
    Debug.WriteLine(ex.ToString());
#else
    throw;
#endif
}
```