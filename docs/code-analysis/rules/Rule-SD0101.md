# SD0101
## 基本信息

**错误编号**：`SD0101`

**错误叙述**：

* **中文**：从 `StepSearcher` 派生的类必须拥有 `Properties` 属性。
* **英文**：A property named `Properties` expected.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

在 `Sudoku.Solving.Manual` 命名空间里，带有 `StepSearcher` 类，这是为了给我们提供自定义技巧的。但是，这个类必须拥有一个叫做 `StepSearcher` 的静态属性，这个属性用来被反射调用，提供在设置窗体里显示技巧的调用顺序和难度。如果没有的话，窗体显示的地方就会报错，或者无法显示该技巧。

```csharp
public sealed class MyStepSearcher : StepSearcher
{
    public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
        throw new NotImplementedException();
}
```

请尝试添加这个属性后，消除此错误。