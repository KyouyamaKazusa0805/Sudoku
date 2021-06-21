## 基本信息

**错误编号**：`SS0305`

**错误叙述**：

* **中文**：重复调用的 `OrderBy` 语句，后面的 `OrderBy` 语句请全部换成 `ThenBy` 语句。
* **英文**：As multiple `OrderBy` method invocation expressions, non-first `OrderBy`s should be replaced with `ThenBy` clause.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 的 `orderby` 子句重复调用的话，会重复产生 `OrderBy` 相关的迭代器，会影响性能。为了避免性能消耗，建议改成 `ThenBy` 语句来解决。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = arr.OrderBy(x => x).OrderBy(x => x); // SS0305.
```

类似 `arr.OrderBy(x => x)` 的调用会产生别的迭代器，导致性能消耗。请改成 `ThenBy`。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = arr.OrderBy(x => x).ThenBy(x => x);
```

## 备注

这样的机制包含如下的情况：

* `OrderBy(筛选条件)`；
* `OrderByDescending(筛选条件)`；
* `orderby` 关键字表达的查询表达式，重复出现的 `orderby` 子句。

分别转换为 `ThenBy(筛选条件)`、`ThenByDescending(筛选条件)` 和归并 `orderby` 语句。比如说，

```csharp
var selection = arr.OrderBy(x => x).OrderBy(x => x);
```

可转换为

```csharp
var selection =
    from x in arr
    orderby x ascending
    orderby x ascending
    select x;
```

这样的表达式建议改写成

```csharp
var selection =
    from x in arr
    orderby x ascending, x ascending
    select x;
```

