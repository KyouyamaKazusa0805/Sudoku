## 基本信息

**错误编号**：`SS0501`

**错误叙述**：

* **中文**：解构函数至少需要两个参数。
* **英文**：Deconstruction methods should contain at least 2 parameters.

**级别**：编译器警告

**警告等级**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了解构函数，但解构函数有时候必须要至少两个参数才可生效。如果只有一个参数，甚至没有的话，参数就无法产生合适的解构。

```csharp
public void Deconstruct(int age) => age = Age; // SS0501.
```