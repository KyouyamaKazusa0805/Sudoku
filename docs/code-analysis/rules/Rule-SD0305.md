# SD0305
## 基本信息

**错误编号**：`SD0305`

**错误叙述**：

* **中文**：请确认初始化器的数值是否合法。
* **英文**：The input value in this initializer is invalid.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

在 `Cells` 和 `Candidates` 结构里，我们需要给初始化器传入数值以保证初始化。不过，问题在于有时候我们可能会写错数字导致数据范围超出合法范围。按照数据结构的设计，`Cells` 的有效范围是 $[-81, -1] \cup [0, 81)$；而 `Candidates` 的有效范围是 $[-729, -1] \cup [0, 729)$。

> `Cells` 结构和 `Candidates` 结构均会使用位取反运算符来表示排除一些地方，比如 `new Cells(another) { ~1 }` 这样的形式。表达式 `~1` 表示把第 2 个单元格给去掉。

```csharp
var cells1 = new Cells { ~100 }; // Wrong.
var cells2 = new Cells { 3, 10, 100 }; // Wrong.
```