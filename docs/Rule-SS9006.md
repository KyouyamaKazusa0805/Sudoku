## 基本信息

**错误编号**：`SS9006`

**错误叙述**：

* **中文**：`: object` 没有意义：因为它是客观事实。
* **英文**：You don't need to write `: object` because this is always a truth.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 设计约定所有类型都从 `object` 派生，而类是可以写基类型的继承关系的，而 C# 没有禁止 `: object` 显式书写的方式。这种写法是客观成立的，因此没有必要写出来。

```csharp
class I : object
{
}
```

请直接删除 `: object` 部分。

```csharp
class I
{
}
```