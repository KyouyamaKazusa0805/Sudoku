# SS9005
## 基本信息

**错误编号**：`SS9005`

**错误叙述**：

* **中文**：自动属性在可变结构里会自动带有 `readonly` 修饰符，因此无需指定 `readonly` 修饰符。
* **英文**：Auto property in a non-`readonly` struct has already contained the meaning of the modifier `readonly`, so you don't specify the modifier `readonly`.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 7 里包含 `readonly` 结构修饰符，表示结构的内容不可变。之后又诞生了 `readonly` 修饰符可修饰结构类型里的非静态成员的特性。问题在于，由于自动属性是自动生成的，因此自动属性是无需为 `get` 标记 `readonly`，也无需对只读属性本身标记 `readonly` 修饰符，因为底层会自动生成修饰。

```csharp
struct S
{
    public int A { get; }
    public readonly int B { get; }
    public int C { readonly get; set; }
    public int D { get; set; }
}
```

如上，`B` 和 `C` 属性的 `readonly` 修饰符都是可以不写的。