## 基本信息

**错误编号**：`SS0405`

**错误叙述**：

* **中文**：`[AutoEquality]` 特性的参数需要包含等号运算符。
* **英文**：The type of the data member as the argument in `[AutoEquality]` attribute should contain `operator ==`.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

特性在源代码生成器里是需要使用等号运算符的。如果不包含这个运算符，那么源代码生成器就会生成失败。
