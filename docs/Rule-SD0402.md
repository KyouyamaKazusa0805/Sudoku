## 基本信息

**错误编号**：`SD0402`

**错误叙述**：

* **中文**：'{0}' 特性需要至少 {1} 个参数。
* **英文**：This attribute must contain at least the number of parameters.

**级别**：编译器警告

**警告等级**：1

**类型**：使用（Usage）

## 描述

这些特性在修饰的时候，至少需要指定个数的参数，才可以参与使用。源代码生成器并不会产生这类异常，但对于比如解构函数来说，至少两个参数是必要条件。

```csharp
[AutoDeconstruct(nameof(A.Prop1))] // Wrong.
class A
{
    public int Prop1 { get; }
    public int Prop2 { get; }
    public int Prop3 { get; }
}
```