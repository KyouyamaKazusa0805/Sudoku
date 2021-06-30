# SS0403
## 基本信息

**错误编号**：`SS0403`

**错误叙述**：

* **中文**：枚举类型的字段必须在枚举类型本身标记了 `FlagsAttribute` 特性后，建议把数值显式写出来。
* **英文**：The `enum` field must holds a explicitly-wroten value if the `enum` is marked `FlagsAttribute`.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

标记了 `FlagsAttribute` 特性的枚举类型，每一个字段的对应整数数值都必须是 2 的幂次。除非这个字段是别的字段的计算得到的结果。

我们通常是建议把这些字段显式给出的，便于调试和发现问题。

```csharp
[Flags]
enum TestEnum
{
    A = 1,
    B, // SS0403.
}
```

即使我们知道确实 `B` 此时的数值是 2，也是满足条件的数值。