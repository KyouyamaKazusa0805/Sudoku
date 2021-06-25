## 基本信息

**错误编号**：`SS0306`

**错误叙述**：

* **中文**：可使用索引器代替 `ElementAt` LINQ 调用。
* **英文**：The LINQ expression `ElementAt` can be replaced with an indexer invocation.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 的 `ElementAt` 始终是使用循环进行迭代。如果包含索引器的话，可能会有 $O(1)$ 复杂度的取值过程，因此比 `ElementAt` 更优。建议改成索引器。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

int element = arr.ElementAt(3);
```

建议改成索引器。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

int element = arr[3];
```

