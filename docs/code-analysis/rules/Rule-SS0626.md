# SS0626
## 基本信息

**错误编号**：`SS0626`

**错误叙述**：

* **中文**：`not` 关键字重复。成对的 `not` 关键字可直接去掉。
* **英文**：Keyword `not` repeats; A pair of keyword `not` can be omitted.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

成对出现两次的 `not` 会影响可读性。

```csharp
object? o = 13;

// ...

//          ↓ SS0626.
if (o is not not 3)
//       ~~~~~~~
{
    // ...
}
```

删除即可。

```csharp
if (o is 3)
{
    // ...
}
```