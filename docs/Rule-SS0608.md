## 基本信息

**错误编号**：`SS0608`

**错误叙述**：

* **中文**：对位模式无法使用无参或单个参数绑定的解构函数。
* **英文**：The positional pattern may not allow because bounded deconstruction method is a parameterless or single-parameter one.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了对位模式，但对位模式由于设计原则，我们无法允许无参和只需要单个参数传入的解构函数进行模式匹配，虽然我们知道这样的书写是允许的。

```csharp
//       ↓ SS0608. ↓ SS0608.
if (s is () and (a: 10) and (a: 10, b: 30))
//       ~~     ~~~~~~~
{
    Console.WriteLine(nameof(s));
}
```