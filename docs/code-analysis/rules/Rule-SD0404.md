# SD0404
## 基本信息

**错误编号**：`SD0404`

**错误叙述**：

* **中文**：如果一个技巧搜索器 `DirectSearcherAttribute` 特性，那么这个技巧搜索器就必须在该类型的实例声明语句之前调用 `FastProperties.InitializeMaps`。
* **英文**：If a step searcher doesn't mark `DirectSearcherAttribute` attribute, the searcher can't be initialized unless the variable declaration above contains a method invocation `FastProperties.InitializeMaps`.

**级别**：编译器警告

**警告等级**：1

**类型**：使用（Usage）

## 描述

这是特性的一个机制。为了在窗体上启用类型的数据处理和技巧在处理过程上的差异，我们使用 `DirectSearcherAttribute` 特性标记到一个技巧搜索器的类类型上，表示这个类型不需要使用快速属性计算处理内容。

> 所谓的快速属性（请参看 [`FastProperties` 类型](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/FastProperties.cs)的代码），是为了快速处理每个盘面状态下的所有双值格信息啊、空格信息啊之类的，为了快速遍历和迭代，所以创建了这样的类型来临时缓存这个盘面状态的具体信息。

但是，如果这个搜索器类型本身没有标记 `[DirectSearcher]` 特性的话，那么这个搜索器就可能会依赖快速属性。如果不初始化快速属性信息的话，我们直接使用这个类型的话，就会造成信息执行不匹配甚至运行时错误的问题。所以，编译器会给出错误。

假设有这么一个声明：

```csharp
class TestStepSearcher : StepSearcher
{
    // ...
}
```

那么如果我们使用这个类型的实例来完成搜索的话：

```csharp
var grid = ...;

var searcher = new TestStepSearcher(); // SD0404.
var list = new List<StepInfo>();
searcher.GetAll(list, grid);
```

因为 `TestStepSearcher` 里可能包含快速属性，因此分析器告知你这个初始化前面需要追加初始化操作：

```csharp
var grid = ...;
FastProperties.InitializeMaps(grid);

var searcher = new TestStepSearcher();
var list = new List<StepInfo>();
searcher.GetAll(list, grid);
```

这样的话，就不会出现问题。分析器就会消去警告信息。