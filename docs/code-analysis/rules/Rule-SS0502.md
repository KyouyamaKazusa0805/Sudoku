# SS0502
## 基本信息

**错误编号**：`SS0502`

**错误叙述**：

* **中文**：解构函数必须是实例方法。
* **英文**：Deconstruction methods must be instance ones.

**级别**：编译器警告

**警告等级**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了解构函数，但解构函数要想生效，必须绑定一个单独的对象。如果方法是静态的话，除非方法是扩展解构函数，否则会产生编译器警告。

```csharp 
public static void Deconstruct(int age) => age = 100; // SS0502.
```