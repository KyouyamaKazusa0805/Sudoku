# SS9007
## 基本信息

**错误编号**：`SS9007`

**错误叙述**：

* **中文**：请将特性里属性成员的 `set` 修饰符改成 `init` 关键字。
* **英文**：Please use keyword `init` instead of `set` of the property accessor in an attribute type.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

C# 9 发明了 `init` 修饰符，不过 `init` 修饰符约束更强。C# 里的特性支持 `set` 关键字修饰特性的属性成员，但属性的 `get` 和 `set` 也只是约束它可以使用特性的初始化器赋值而已，后续是无法修改的，因为它已经被放进元数据了。

```csharp
public sealed class AAttribute : Attribute
{
    public int Property { get; set; }
}
```

请改成 `init`。

```csharp
public sealed class AAttribute : Attribute
{
    public int Property { get; init; }
}
```