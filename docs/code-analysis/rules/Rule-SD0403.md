# SD0403
## 基本信息

**错误编号**：`SD0403`

**错误叙述**：

* **中文**：`DirectSearcherAttribute` 特性只能用在技巧搜索器的类类型上。
* **英文**：`DirectSearcherAttribute` attribute can be only used for step searchers.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

这是特性的一个机制。为了在窗体上启用类型的数据处理和技巧在处理过程上的差异，我们使用 `DirectSearcherAttribute` 特性标记到一个技巧搜索器的类类型上，表示这个类型不需要使用快速属性计算处理内容。

> 所谓的快速属性（请参看 [`FastProperties` 类型](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/FastProperties.cs)的代码），是为了快速处理每个盘面状态下的所有双值格信息啊、空格信息啊之类的，为了快速遍历和迭代，所以创建了这样的类型来临时缓存这个盘面状态的具体信息。

但是，如果不使用在技巧搜索器的类类型上使用，而是用在别的什么类类型上，是无效的处理机制。

```csharp
[DirectSearcher]
record RecordType;

[DirectSearcher]
class A { }
```

如上的两处类型声明，由于底层都是类，因此处理上是一样的；因为没有从 `StepSearcer` 类型派生，因此编译器会对此进行报错。