## 基本信息

**错误编号**：`SS0504`

**错误叙述**：

* **中文**：解构函数必须是 `public` 的。
* **英文**：Deconstruction methods must be public.

**级别**：编译器警告

**警告等级**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了解构函数，但因为要可以使用，我们允许且必须用 `public` 关键字修饰。

```csharp
void Deconstruct(int age) => age = 100; // SS0504, because the default accessibility is 'private'.
```