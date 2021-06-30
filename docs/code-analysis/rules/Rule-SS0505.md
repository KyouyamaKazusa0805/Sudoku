# SS0505
## 基本信息

**错误编号**：`SS0505`

**错误叙述**：

* **中文**：解构函数的参数必须用 `out` 修饰。
* **英文**：All parameters in deconstruction methods should be `out` parameters.

**级别**：编译器警告

**警告等级**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了解构函数，但因为要可以使用，我们允许且必须用 `out` 关键字修饰参数才可以。

```csharp
public void Deconstruct(int age) => age = 100; // SS0505.
```