# SD0413
## 基本信息

**错误编号**：`SS0413`

**错误叙述**：

* **中文**：标记了 `[Discard]` 的参数无法被任何地方使用，除非是 `nameof` 表达式。
* **英文**：The parameter marked` [Discard]` can't be used wherever unless a `nameof` expression.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

`[Discard]` 专门用来表示一些 C# 方法上不支持的弃元参数。当这些内容一旦被标记弃元后，这个参数就不可再以任何方式被使用了，除非 `nameof` 表达式。因为 `nameof` 表达式自身不会用到变量本身的信息和数据，只是取这个参数的名称的字符串表达而已。
