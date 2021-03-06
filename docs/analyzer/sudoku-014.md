## 基本信息

**错误编号**：`SUDOKU014`

**错误叙述**：

* **中文**：成员 '{0}' 不能被调用，因为它是被保留的。
* **英文**：The member can't be invoked because they are reserved.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

这个说起来比较绕。在提供的 API 里，有一少部分的成员被暴露出来给用户看，原因却仅仅是因为这个数据对我们调用别的成员的时候，显得很重要。比如这里的 `SudokuGrid` 结构里的 `RefreshingCandidates` 和 `ValueChanged` 字段。这两个字段是静态只读的字段，它们是分别用来控制刷新候选数和修改数据的时候，自动修改相关单元格里的其余候选数的函数指针。

这两个函数指针仅可以在 `SudokuGrid` 结构里随意调用和使用，但出了这个结构就不可以调用了，因为 `SudokuGrid` 已经封装好了，我们也完全没有必要在外部调用这两个函数指针；如果故意调用此方法，就会导致数据结构的封装被破坏，进而暴露出数据结构本身封装设计不够良好（非良构类型）的潜在问题，即使你知道，函数指针需要启用 `unsafe` 来使用，这也是不行的。

如下的例子给出了产生该编译器错误的情况。

```csharp
using Sudoku.Data;

// Global statement, which means the code is in the another file that contains an implicit class 'Program'.
unsafe
{
    var grid = SudokuGrid.Empty;
    SudokuGrid.ValueChanged(ref grid, new()); // SUDOKU013.
    SudokuGrid.RefreshingCandidates(ref grid); // SUDOKU013.
}
```