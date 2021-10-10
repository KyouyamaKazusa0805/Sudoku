# SD0401
## 基本信息

**错误编号**：`SD0401`

**错误叙述**：

* **中文**：请使用 `nameof` 表达式修饰参数。
* **英文**：Please use `nameof` expression instead.

**级别**：编译器警告

**警告等级**：1

**类型**：使用（Usage）

## 描述

在使用源代码生成器的时候，我们会使用到对应的自动生成的标记特性，比如 `AutoDeconstructAttribute`。在源代码生成器里，我们需要对参数使用 `nameof` 表达式，才可对应到对应的参数上去。

```csharp
[AutoDeconstruct(nameof(A.Prop1), nameof(A.Prop2), "Prop3")] // Wrong.
class A
{
    public int Prop1 { get; }
    public int Prop2 { get; }
    public int Prop3 { get; }
}
```