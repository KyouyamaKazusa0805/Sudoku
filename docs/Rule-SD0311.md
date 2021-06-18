## 基本信息

**错误编号**：`SD0311`

**错误叙述**：

* **中文**：`SudokuGrid.ToString` 方法的格式化字符串不正确。
* **英文**：Invalid format string in `SudokuGrid.ToString`.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

`SudokuGrid` 的格式化字符串非常丰富，因此也有一定的书写规则，比如不能把 `+` 和 `!` 符号混用，因为 `+` 用来输出和显示可修改变动的数值；而 `!` 直接将所有确定值全部显示。所以它们输出格式的逻辑之间存在矛盾。

```csharp
Console.WriteLine(grid.ToString("!+")); // Wrong.
```