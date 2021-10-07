# SD0307
## 基本信息

**错误编号**：`SD0307`

**错误叙述**：

* **中文**：可简化成如下删除表达式：'{0}'。
* **英文**：The expression can be simplified.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

`Cells` 和 `Candidates` 结构可带有额外的初始化器，传入一些初始数据进去。不过，有些时候我们可能会使用别的表达式来代替，诸如 `-14`（即 `~13`），主要是为了意图和数值表达更清晰。

```csharp
var cells = new Cells(anotherCells）{ -10 }; // Wrong.
```

编译器将会建议你改成

```csharp
var cells = new Cells(anotherCells) { ~9 };
```