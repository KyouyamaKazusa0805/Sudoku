# SS0622
## 基本信息

**错误编号**：`SS0622`

**错误叙述**：

* **中文**：可将 `or` 属性模式簇简化为单大括号的属性模式。
* **英文**：Available simplification for property patterns connected with keyword `or` to a single property pattern.

**级别**：编译器信息

**类型**：设计（Design）

**状态**：禁用中

## 描述

如果多个属性模式使用 `or` 关键字连接的话，我们可以直接使用单个大括号表达。

```csharp
//                              ↓ SS0622.
if (o is { Prop1: 1 } or { Prop2: not 2 } or { Prop3: 3 })
//       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
{
    // ...
}
```

建议改成这样：

```csharp
if (o is not { Prop1: not 1, Prop2: 2, Prop3: not 3 })
{
    // ...
}
```