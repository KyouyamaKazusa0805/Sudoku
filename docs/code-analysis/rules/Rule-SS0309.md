## 基本信息

**错误编号**：`SS0309`

**错误叙述**：

* **中文**：`ascending` 关键字没有必要写出来，因为默认排序机制就是升序排序。
* **英文**：Keyword `ascending` is unnecessary because the default ordering case is ascending.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

LINQ 的 `orderby` 语句有两种排序机制：升序和降序排序。升序排序使用关键字 `ascending` 表示；降序则使用 `descending` 关键字。但是如果是升序排序的话，关键字 `ascending` 是可以不写的。为了避免代码内容过多降低可读性，我们建议不写出这个关键字。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = from x in arr orderby x ascending select x;
```

请删除关键字 `ascending`。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = from x in arr orderby x select x;
```

