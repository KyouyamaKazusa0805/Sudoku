## 基本信息

**错误编号**：`SS0311`

**错误叙述**：

* **中文**：排序语句互斥或无意义。
* **英文**：Orthogonal or unmeaningful `orderby` clauses.

**级别**：编译器警告

**警告级别**：1

**类型**：性能（Performance）

## 描述

如果前后排序的字段完全一致但还是分两次书写的话，那么排序就没有意义了。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

_ = from x in arr orderby x, x select x;
_ = from x in arr orderby x orderby x descending select x;
```

这两个例子都会产生 SS0311 错误信息。因为我们连续写了两次 `x` 字段的排序，但第一个是排序两次 `x`，第二次则是对第一次升序排序后的结果又再次降序排序。

虽然排序会得到执行，但最终排序是没有意义的，我们需要删除前者 `orderby` 语句来避免这个性能问题。

