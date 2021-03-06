## 基本信息

**错误编号**：`SUDOKU021`

**错误叙述**：

* **中文**：请使用 '{0}' 只读字段以避免对象实例化分配内存空间。
* **英文**：Please use the default-value field to avoid instantiation.

**级别**：编译器警告

**类型**：性能（Performance）

**警告等级**：1

## 描述

对于大的值类型对象来说，使用默认的构造器 `new()` 可以实例化一个对象出来（当然，使用 `default(T)` 也是一样的效果）。不过，带有 `Empty` 字段的对象会检测的只有 `Cells`、`Candidates` 和 `SudokuGrid` 三个对象；另外，`SudokuGrid` 类型的默认结果是 `SudokuGrid.Undefined` 而不是 `Sudoku.Empty`，而 `Cells` 和 `Candidates` 的默认数值是 `Cells.Empty` 和 `Candidates.Empty`。

```csharp
using Sudoku.Data;

var grid = new SudokuGrid(); // SUDOKU021.
var cells = new Cells(); // SUDOKU021.
var cells2 = new Cells { 3, 10, 40 };
var candidates = new Candidates(); // SUDOKU021.
var candidates2 = new Candidates { 3, 10, 40 };
SudokuGrid grid2 = new(); // SUDOKU021.
Cells cells3 = new(); // SUDOKU021.
Candidates candidates3 = new(); // SUDOKU021.
Candidates candidates4 = new() { 1, 10 };
SudokuGrid grid3 = default; // SUDOKU021.
var grid4 = default(SudokuGrid); // SUDOKU021.
var candidates5 = default(Candidates); // SUDOKU021.
Candidates candidates6 = default; // SUDOKU021.
```