## 基本信息

**错误编号**：`SS0703`

**错误叙述**：

* **中文**：不必使用的不可空类型的空值条件运算符 `?`。
* **英文**：Unnecessary null-conditional operator `?`.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 6 诞生了两个运算符 `?.` 和 `?[]`，不过实际上我个人觉得这还不如直接说成是 `?` 后缀运算符比较合适。

这个运算符用在前面是可空类型上，表示如果为空，立即阻断点运算符的后续调用过程。但是，如果不可空类型使用这个运算符就显得冗余了，分析器需要对这种情况报告。

```csharp
P p = new();
//           ↓ SS0703.
string? q = p?.ToString();
//           ~
Console.WriteLine(q);

class P
{
    public override string ToString() => string.Empty;
}
```

请直接删除此时的空值禁用运算符。

```csharp
P p = new();
string q = p.ToString();
Console.WriteLine(q);

class P
{
    public override string ToString() => string.Empty;
}
```