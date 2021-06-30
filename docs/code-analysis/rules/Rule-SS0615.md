# SS0615
## 基本信息

**错误编号**：`SS0615`

**错误叙述**：

* **中文**：可空值类型请使用 `{ }` 或 `not null` 模式匹配语法来表达判断类型是否可空。
* **英文**：Please use `{ }` or `not null` instead of `!= null` or `HasValue` in nullable value types.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

对于值类型来说，使用 `{ }` 语法最为简单。

```csharp
//       ↓ SS0615.
if (o.HasValue)
//    ~~~~~~~~
{
}
```

请改成 `is { }` 语法来取消该信息提示。

```csharp
if (o is { })
{
}
```