# SS0312
## 基本信息

**错误编号**：`SS0312`

**错误叙述**：

* **中文**：重复调用的 `where` 语句可归并到一起。
* **英文**：Multiple `where` LINQ expressions can be merged to one.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 的 `where` 子句重复调用的话，会重复产生 `where` 相关的 LINQ 排序用的迭代器，这也会影响性能。为了避免性能消耗，建议改成同一个 `where` 语句的调用，并使用 `&&` 运算符连接。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

_ = from x in arr where x / 2 == 0 where x >= 8 select x;
```

建议合并。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

_ = from x in arr where x / 2 == 0 && x >= 8 select x;
```

