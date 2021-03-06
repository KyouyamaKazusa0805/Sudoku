## 基本信息

**错误编号**：`SUDOKU018`

**错误叙述**：

* **中文**：使用 'IsEmpty' 代替 'Count == 0'。
* **英文**：Replace 'Count == 0' with 'IsEmpty'.

**级别**：编译器警告

**类型**：使用（Usage）

**警告等级**：1

## 描述

`Cells` 和 `Candidates` 结构里都有 `IsEmpty` 属性，它们专门拿来表示是不是集合里没有元素。我们没有必要使用 `Count == 0` 来计算集合是否有元素。