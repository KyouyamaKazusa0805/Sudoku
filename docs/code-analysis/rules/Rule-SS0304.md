## 基本信息

**错误编号**：`SS0304`

**错误叙述**：

* **中文**：没有意义的 `where true` 或者 `where false` 从句。
* **英文**：Unmeaningful `where true` or `where false` clause.

**级别**：编译器警告

**警告级别**：1

**类型**：性能（Performance）

## 描述

LINQ 的 `where` 子句或者 `Where` 方法调用里传入的委托如果是永真式的话，那么条件就没有任何判别意义了。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = from element in arr where true select element % 10;
```

`where true` 是冗余的。请直接删除。

另外，`where false` 意味着永假式，式子不可能成立，因此后面的代码不会得到判断。
