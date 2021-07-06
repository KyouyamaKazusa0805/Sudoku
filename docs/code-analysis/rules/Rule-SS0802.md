# SS0802
## 基本信息

**错误编号**：`SS0802`

**错误叙述**：

* **中文**：标记了 `[AssemblyObsolete]` 的程序集不可被其它程序集使用。
* **英文**：The assembly can't be used by other assemblies after marked `[AssemblyObsolete]`.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

这个是一个规范。如果标记了 `[AssemblyObsolete]` 特性的程序集就不允许被别的程序集所使用了。

