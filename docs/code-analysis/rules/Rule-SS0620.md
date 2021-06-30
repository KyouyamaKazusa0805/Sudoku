# SS0620
## 基本信息

**错误编号**：`SS0620`

**错误叙述**：

* **中文**：数据比较模式下的 `not` 模式不建议这么用；请改写为运算符取反的格式。
* **英文**：Keyword `not` followed with relation pattern is redundant; please negate the operator directly instead.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

在数据大小比较的模式下，如果我们重叠 `not` 和数据比较运算符两种判别方式的话，这样是不建议的，因为它会减小可读性。

```csharp
object? o = 13;

// ...

//           ↓ SS0620.
if (o is not >= 3)
//       ~~~~~~~~
{
    // ...
}
```

显然，`not >= 3` 和 `< 3` 是等价的写法，所以建议改成 `< 3`。

```csharp
if (o is < 3)
{
    // ...
}
```