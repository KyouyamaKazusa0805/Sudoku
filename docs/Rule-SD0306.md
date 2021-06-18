## 基本信息

**错误编号**：`SD0306`

**错误叙述**：

* **中文**：构造器 '{0}.{0}()' 的初始化器里，删除表达式是无效的。
* **英文**：The remove expression in the initializer following with the constructor has no effect.

**级别**：编译器警告

**类型**：使用（Usage）

**警告级别**：1

## 描述

由于 `Cells` 和 `Candidates` 是结构，因此带有无参构造器。该无参构造器在使用的时候，一般是配合初始化器使用。但是，无参构造器配合初始化器的时候，删除表达式（即带有位取反运算符的表达式）是没有意义的，因为空列表里不可能再删除别的序列了）。

```csharp
var cells = new Cells { ~10 }; // Wrong.
```