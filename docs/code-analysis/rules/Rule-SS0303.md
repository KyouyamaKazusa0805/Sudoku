## 基本信息

**错误编号**：`SS0303`

**错误叙述**：

* **中文**：简化 LINQ 调用表达式。
* **英文**：Available simplification of LINQ expression.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 有 `where` 关键字和 `Where` 筛选方法，但这样的方法在调用之后还使用 `Any` 等方法的话，显然会导致调用产生冗余，因为底层会产生 `Where` 的迭代器和 `Any` 的迭代器两处实例化，导致性能损失。为了避免性能损失，可以合二为一。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

if (arr.Where(static element => (element & 1) != 0).Any()) // SS0302.
{
    // ...
}
```

类似 `arr.Where(element => (element & 1) != 0).Any()` 的调用是冗余的。请使用 `Any` 传入委托的方法来避免编译器分析。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

if (arr.Any(static element => (element & 1) != 0))
{
    // ...
}
```

## 备注

这样的转换机制除了直接的方法调用外，还有 LINQ 的查询表达式（使用关键字的版本）。比如

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

if (
    (
        from element in arr
        where (element & 1) != 0
        select element
    ).Any() // SS0302.
)
{
    // ...
}
```

在这种情况下，查询表达式 `from element in arr where (element & 1) != 0 select element` 依旧可以被识别，并简化为 `Any` 的调用。但是，如果查询表达式过于复杂（比如带有 `group by into` 语句，或者 `orderby ascending` 语句）的话，这样的 LINQ 查询表达式可能就无法直接转换了。理论上是可以转换，但需要很复杂的处理，分析器暂时不考虑这么多。
