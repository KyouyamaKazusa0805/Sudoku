# SD0316
## 基本信息

**错误编号**：`SD0316`

**错误叙述**：

* **中文**：不要显式调用 `ValueStringBuilder.Dispose` 方法。
* **英文**：Don't call `ValueStringBuilder.Dispose` method explicitly.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

和 [`SD0315`](Rule-SD0315.md) 的原因一样，因为它自身会自动在 `ToString` 里调用 `Dispose` 方法，因此我们不建议手动调用此方法。
