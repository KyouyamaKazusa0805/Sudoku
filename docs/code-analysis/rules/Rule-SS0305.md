## 基本信息

**错误编号**：`SS0305`

**错误叙述**：

* **中文**：重复调用的 `orderBy` 语句可归并到一起。
* **英文**：Multiple `orderby` LINQ expressions can be merged to one.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 的 `orderby` 子句重复调用的话，会重复产生 `orderby` 相关的 LINQ 排序用的迭代器，这也会影响性能。为了避免性能消耗，建议改成 `ThenBy` 语句来解决。底层的 `ThenBy` 可使用逗号拼接的语法归并到同一个 `orderby` 语句上，这也就可以避免重复调用 `orderby` 导致性能影响。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection =
    from x in arr
    orderby x ascending
    orderby x ascending // SS0305.
    select x;
```

这样的表达式建议改写成

```csharp
var selection =
    from x in arr
    orderby x ascending, x ascending
    select x;
```

