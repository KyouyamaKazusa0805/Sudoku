# SS0302
## 基本信息

**错误编号**：`SS0302`

**错误叙述**：

* **中文**：平凡的筛选语句。没有 `where` 的语句，且 `select` 语句使用的转换表达式就是 `from` 的范围变量的话，建议直接省去迭代。
* **英文**：The LINQ expression is unmeaningful, where the `where` clause doesn't exist, and the conversion expression in `select` clause is equal to the range variable in `from` clause; please omit the iteration.

**级别**：编译器警告

**类型**：性能（Performance）

**警告等级**：1

## 描述

LINQ 里如果出现了类似 `from a in list select a` 这样的语句的话，是没有意义的，因为它和下面这样的代码等价：

```csharp
foreach (var a in list)
    yield return a;
```

但 `list` 变量本身就是可迭代的变量，那么为什么不直接作为返回数值返回，或者直接赋值给别的变量继续使用呢？因此，这样的迭代是没有意义的。

```csharp
var selection = from a in list select a;
```

