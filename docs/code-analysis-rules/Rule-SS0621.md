## 基本信息

**错误编号**：`SS0621`

**错误叙述**：

* **中文**：可将 `and` 属性模式簇简化为单大括号的属性模式。
* **英文**：Available simplification for property patterns connected with keyword `and` to a single property pattern.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

如果多个属性模式使用 `and` 关键字连接的话，我们可以直接使用单个大括号表达。

```csharp
//                               ↓ SS0621.
if (o is { Prop1: 1 } and { Prop2: not 2 } and { Prop3: 3 })
//       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
{
    // ...
}
```

建议改成这样：

```csharp
if (o is { Prop1: 1, Prop2: not 2, Prop3: 3 })
{
    // ...
}
```