## 基本信息

**错误编号**：`SD0308`

**错误叙述**：

* **中文**：初始化器包含重复项。
* **英文**：The initializer contains the duplicate value.

**级别**：编译器警告

**类型**：使用（Usage）

**警告级别**：1

## 描述

`Cells` 和 `Candidates` 结构可带有额外的初始化器，传入一些初始数据进去。如果包含重复数据的话，编译器会给出提示，提示用户删除它们。

```csharp
var cells = new Cells(anotherCells）{ 19, 20, 28, 19 }; // Wrong, '19' duplicated.
```

## 备注

本分析项仅针对于初始化器进行分析。因为别的其它行为都会被分析器进行代码优化，改写成这种初始化器的格式。