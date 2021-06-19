## 基本信息

**错误编号**：`SS0503`

**错误叙述**：

* **中文**：解构函数返回值必须为 `void` 类型。
* **英文**：Deconstruction methods must return `void`.

**级别**：编译器警告

**警告等级**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了解构函数，但解构函数要想生效，必须返回 `void`。

```csharp
public bool Deconstruct(int age) // SS0503.
{
    age = 100;

    return true;
}
```