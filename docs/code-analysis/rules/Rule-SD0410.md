# SD0410
## 基本信息

**错误编号**：`SS0410`

**错误叙述**：

* **中文**：`[ProxyEquality]` 特性所标记的方法只能有一个参数。
* **英文**：The source generator will be well-working until the method marked `[ProxyEquality]` should contain 2 parameters.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

`[ProxyEquality]` 标记的方法是用来判断相等性的，因为是静态方法的关系，那么这个方法必须包含两个参数。

```csharp
partial class MyClass
{
    [ProxyEquality]
    public static bool Equals(MyClass left) => false;
}
```

比如这样的代码，类型 `MyClass` 的这个 `Equals` 方法只有一个参数。
