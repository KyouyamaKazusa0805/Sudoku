# SS0314
## 基本信息

**错误编号**：`SS0314`

**错误叙述**：

* **中文**：不要在 LINQ 语句里的 `where` 条件或模式匹配子句里定义变量。
* **英文**：Don't declare new variables in the `where` clause or pattern matching in the LINQ expression.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

因为 LINQ 里的语句，变量互相都无法识别，因此如果在 `where` 语句里定义变量，后续的语句仍然识别不了该变量。

```csharp
int[] a = { 1, 2, 3, 4, 5, 6 };

var selection1 =
    from e in a
    select e + 1 into f
    where f is var g // Wrong.
    select f;

var selection2 =
    from e in a
    let f = e + 1
    where f is var g && g % 2 == 0
    select f;
```

如代码所示，第一个 LINQ 语句里，`g` 变量无法被后续代码里使用，因此不建议直接在里面定义；但是第二个 LINQ 语句里，因为在同一个筛选语句里使用了 `g` 变量，因此是合法的调用行为。
