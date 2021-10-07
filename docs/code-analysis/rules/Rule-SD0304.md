# SD0304
## 基本信息

**错误编号**：`SD0304`

**错误叙述**：

* **中文**：请使用属性 '{0}.{1}' 简化调用。
* **英文**：Please use the property to simplify the invocation.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

有三个类型 `SudokuGrid`、`Cells` 和 `Candidates` 是具有特殊判断的属性的。当我们需要简化调用判断的时候，我们可能会用上它们。

比如我们拿 `SudokuGrid` 来说。`SudokuGrid` 类型里包含三个特殊属性 `IsEmpty`、`IsUndefined` 和 `IsDebuggingUndefined`。这三个属性专门用来表示和判断对象是不是为空盘、为无效盘和调试期无效盘。其中，`SudokuGrid` 实例要使得 `IsUndefined` 属性为 `true` 值的唯一情况就是等于 `new()`、`default(SudokuGrid)` 这样的数值。但是，这样的等价显然是没有必要写出来的，因此编译器会建议你改成 `SudokuGrid.IsUndefined`。

```csharp
using Sudoku.Data;

var grid = SudokuGrid.Empty;

if (grid == default) // Wrong.
{
    // ...
}
```