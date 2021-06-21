## 基本信息

**错误编号**：`SS0304`

**错误叙述**：

* **中文**：请删除 `Where(x => true)` 子句。
* **英文**：Please remove redundant `Where(x => true)` clause.

**级别**：编译器警告

**警告级别**：1

**类型**：性能（Performance）

## 描述

LINQ 的 `where` 子句或者 `Where` 方法调用里传入的委托如果是永真式的话，那么条件就没有任何判别意义了。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = arr.Where(static element => true).Select(static element => element % 10); // SS0304.
```

类似 `arr.Where(static element => true)` 的调用是冗余的。请直接删除。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = arr.Select(static element => element % 10);
```

## 备注

这样的转换机制除了直接的方法调用外，还有 LINQ 的查询表达式（使用关键字的版本）。比如

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection = from element in arr where true select element % 10;
```

在这种情况下，查询表达式 `from element in arr where true select element` 的 `where true` 子句依旧可以被识别。
