## 基本信息

**错误编号**：`SUDOKU003`

**错误叙述**：

* **中文**：Properties 属性必须是静态的。
* **英文**：The property 'Properties' must be static.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

虽说它通过反射获取数据，但为什么必须是静态的呢？因为每一个信息都是对所有同类型的 `StepSearcher` 共用的，因为我们没有意义对这个实例设置成一个数据信息，而另外一个实例则不同。它们都被实例化出来，用于同样的上下文，找同样的题目。

```csharp
public TechniqueSearcher Properties { get; } // Wrong.
```