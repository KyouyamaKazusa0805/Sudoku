# SD0408
## 基本信息

**错误编号**：`SS0408`

**错误叙述**：

* **中文**：`[ProxyEquality]` 特性所标记的方法需要带有两个当前类型的对象的参数，才能在源代码生成器里生效。
* **英文**：The source generator will be well-working until the method marked `[ProxyEquality]` should contain a pair of parameters that is of the current type.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

`[ProxyEquality]` 标记的方法是用来判断相等性的。如果类型不匹配的话（比如不是这个类型的东西参与比较）都会影响源代码生成器生成代码。
