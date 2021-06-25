## 基本信息

**错误编号**：`SS0307`

**错误叙述**：

* **中文**：请将包含 `Where` 和 `OrderBy` 组合的 LINQ 表达式，交换顺序改成先 `Where` 后 `OrderBy` 方法。
* **英文**：If the `Where` and `OrderBy` clauses both exist in a same LINQ expression, please write `Where` firstly.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 的 `OrderBy` 调用会对对象进行自定义排序。但是自定义排序和筛选比起来，`Where` 筛选后的元素一般会比没有筛选之前更少一些，因此排序的性能消耗就没有那么多。如果先 `OrderBy` 的话，排序元素就会较多，即使使用 $O(\log{n})$ 的排序算法，但是因为产生迭代器的关系，也会消耗较多性能。因此建议先筛选后排序。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

int element = from x in arr orderby x / 2 ascending where (x & 1) != 0 select x; 
```

建议交换 `orderby` 和 `where` 的计算顺序。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

int element = from x in arr where (x & 1) != 0 orderby x / 2 ascending select x; 
```

