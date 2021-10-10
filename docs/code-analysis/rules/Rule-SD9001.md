# SD9001
## 基本信息

**错误编号**：`SS9001`

**错误叙述**：

* **中文**：所有的 `ref struct` 在项目的源代码生成器里会自动生成默认不使用的方法，因此为了避免编译器报错，请使用 `partial` 关键字。
* **英文**：Please apply keyword `partial` on all `ref struct`s because the source generator will generate the source code about default useless methods on those types, which need you to apply the keyword `partial` to them in order to avoid any diagnostic warnings or errors.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

