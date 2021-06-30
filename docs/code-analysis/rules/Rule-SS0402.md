# SS0402
## 基本信息

**错误编号**：`SS0402`

**错误叙述**：

* **中文**：枚举类型的字段必须在枚举类型本身标记了 `FlagsAttribute` 特性后，必须都是 2 的幂次。
* **英文**：The `enum` field must holds a flag value if the `enum` is marked `FlagsAttribute`.

**级别**：编译器警告

**类型**：设计（Design）

## 描述

标记了 `FlagsAttribute` 特性的枚举类型，每一个字段都必须是 flag 数值，否则将产生编译器警告。除非，这个表达式是通过别的字段经过位与、位或、位取反和位异或运算得到的表达式。

```csharp
[Flags]
enum TestEnum
{
    A = 1,
    B = 2,
    C = A | B,
    D = 14 // SS0402.
}
```