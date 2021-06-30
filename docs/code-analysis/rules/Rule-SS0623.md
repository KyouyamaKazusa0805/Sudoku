# SS0623
## 基本信息

**错误编号**：`SS0623`

**错误叙述**：

* **中文**：重复出现了相同路径的属性模式。它们应使用 `and` 关键字整合成一个子模式。
* **英文**：Repeated property path; try to integrate to one using keyword `and` as the connection.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

```csharp
//                          ↓ SS0623.
if (a is { A: < 4, B: 20, A: > 0 })
//                        ~~~~~~
{
    // ...
}
```

分析器将建议你改成这样：

```csharp
if (a is { A: > 0 and < 4, B: 20 })
{
    // ...
}
```