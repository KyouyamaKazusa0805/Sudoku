很多时候你可能会选择开发一个自己的技巧，如果需要基于项目的 SDK 开发的话，你需要完成如下的一些内容，才能在程序里使用上该技巧。

## 第一步：添加技巧搜索类和技巧信息记录

请规划一下你即将完成的技巧本体需要被分配在哪个技巧类别下。比如我需要创建一个技巧叫做**死环**（Bivalue Oddagon）。如果你实在不清楚应该放在哪里的时候，就划分为无分类的技巧。

想好了之后，在命名空间 `Sudoku.Solving.Manual` 下找到对应的文件夹，然后创建对应的技巧搜索器类和技巧信息记录。

举个例子，刚才的技巧我就不知道分在哪里，我就放在 `Miscellaneous` 文件夹下。接着创建 `BivalueOddagonStepSearcher` 类和 `BivalueOddagonStepInfo` 记录。声明格式必须类似于下面的这样：

```csharp
// BivalueOddagonStepSearcher.cs

using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Solving.Manual.Miscellaneous
{
    public sealed class BivalueOddagonStepSearcher : StepSearcher
    {
        public static TechniqueProperties Properties { get; } = new(70, nameof(TechniqueCode.BivalueOddagonType1))
        {
            DisplayLevel = 3,
            OnlyEnableInAnalysis = true
        };


        public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
        {
            // TODO: Implement this.
        }
    }
}
```

注意 `Properties` 静态属性必须要存在，否则会在打开设置窗体时将产生运行时异常。另外，请你注意，特别是添加的名称书写格式需要类似我刚才的写法：

![1](https://images.gitee.com/uploads/images/2020/1229/202704_d0eec5f1_1449374.png)

这一点之所以需要必须这样，是因为这个名称（使用 `nameof(TechniqueCode.BivalueOddagonType1)` 的格式）在资源字典里需要拥有名称一致的匹配项。稍后我会继续给出匹配项的书写格式。

接着是技巧信息记录。

```csharp
// BivalueOddagonStepInfo.cs

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Miscellaneous
{
    public sealed record BivalueOddagonStepInfo(
        IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
        : StepInfo(Conclusions, Views)
        {
            // TODO: Implement this.
            public override decimal Difficulty => throw new NotImplementedException();

            // TODO: Implement this.
            public override TechniqueCode TechniqueCode => throw new NotImplementedException();

            // TODO: Implement this.
            public override DifficultyLevel DifficultyLevel => throw new NotImplementedException();
        }
}
```

然后开始完善程序的执行逻辑。

> 请注意你创建的技巧搜索类和技巧信息记录的继承关系。如果你不是 `Miscellaneous` 文件夹下的技巧的话，技巧需要从对应技巧的基类型进行派生。

## 第二步：在 `TechiqueCode` 枚举里添加对应技巧的枚举字段

在完成了逻辑之后，你需要添加技巧的枚举字段。

```csharp
public enum TechniqueCode
{
    ...
        
    /// <summary>
    /// Indicates the bi-value oddagon type 1.
    /// </summary>
    BivalueOddagonType1,

    /// <summary>
    /// Indicates the bi-value oddagon type 2.
    /// </summary>
    BivalueOddagonType2,

    /// <summary>
    /// Indicates the bi-value oddagon type 3.
    /// </summary>
    BivalueOddagonType3,

    /// <summary>
    /// Indicates the bi-value oddagon type 4.
    /// </summary>
    BivalueOddagonType4,
    
    ...
}
```

技巧枚举添加后如图所示：

![2-1](https://images.gitee.com/uploads/images/2021/0101/152602_6c3a9a81_1449374.png "2-1.png")

![2-2](https://images.gitee.com/uploads/images/2021/0101/152615_38658d5d_1449374.png "2-2.png")

## 第三步：在 `ManualSolverEx` 类里添加技巧的实例

这一步是为了保证技巧的搜索器生效，可以在运行的时候找寻该技巧是否存在。请在 `Sudoku.Solving.Extensions` 命名空间下找到 `ManualSolverEx` 类，在一大堆技巧的地方，按照自己技巧的难度，预估一个位置，插入到技巧序列里。

![3-1](https://images.gitee.com/uploads/images/2020/1229/202522_0d5bfaa4_1449374.png)

![3-2](https://images.gitee.com/uploads/images/2020/1229/202545_8b3c42cb_1449374.png)

## 第四步：修改 `Sudoku.Windows` 里的资源字典 `xaml` 文件

这一步是为了保证和显示分组情况。技巧在前面添加了枚举项后，还需要修改这两个文件，来保证“已经实现的技巧”窗体下显示的技巧本身的分类划分和显示正确。

![4-1](https://images.gitee.com/uploads/images/2020/1229/202939_9565f097_1449374.png)

![4-2](https://images.gitee.com/uploads/images/2020/1229/202950_afbdd509_1449374.png)

![4-3](https://images.gitee.com/uploads/images/2020/1229/202957_51617d48_1449374.png)

请注意此处的添加项。添加项目的 `x:Key` 属性的值，是四个子类型的名称，前面带有 `Group` 前缀。`Group` 前缀是为了确保在代码处理的时候能够取得正确的数值。这个前缀专门保证各个技巧在技巧窗体里正常显示分类。当然，这个 `Group` 前缀也是为了区别于普通的元素：如果真有一个一模一样的 `x:Key` 就无法添加了。

## 第五步：修改本地资源字典 `dic` 文件

这一步是为了本地调取名称的汉化和英文单词的时候可以正常找到对应表达。如果没有这个词语的话，会产生运行时异常提示该字典不存在指定名称的技巧。

![5-1](https://images.gitee.com/uploads/images/2020/1229/203207_3c412ca4_1449374.png)

![5-2](https://images.gitee.com/uploads/images/2020/1229/203216_fb474dc9_1449374.png)

![5-3](https://images.gitee.com/uploads/images/2020/1229/203224_e7ea64d9_1449374.png)

![5-4](https://images.gitee.com/uploads/images/2020/1229/203232_14c35b30_1449374.png)

这里我们需要为两个字典都添加对应的 `Progress技巧名` 和 `技巧名` 两处的文字信息。前者是提供给提示技巧搜索器搜索进度的文字用的，后者则是显示技巧名称用。

当你打开程序的时候，你可能会针对其中某一个盘面下，搜索当前盘面可以搜索到的所有数独技巧。此时搜索器可能会很慢，于是会弹窗提示用户搜索的进度。此时我们搜索进度的显示文字的格式是 `Progress技巧名` 作为固定格式来提供的。另外，这个格式是不是很眼熟呢？是的，前文第一步里我们创建的技巧搜索器类里有一个静态属性 `Properties`，这个属性的构造器传参的第二个参数就是这个数值的技巧名称部分。

我们可以知道，`nameof(字段)` 输出的结果则是字段本身的字符串字面量的表示形式。举个例子，在资源字典里写的是 `ProgressBivalueOddagonType1`，而我们在 `Properties` 静态属性的第二个参数里传入的恰好是 `BivalueOddagonType1` 的字面量写法，所以这一点来说，是匹配的。程序也确实会这么去寻找名称。当名称不匹配的时候，将会产生运行时异常。

## 第六步：运行和调试

这一步应该我不用多说了，编译程序然后运行，调试你的技巧是否实现正确。实现正确需要保证两点：第一是技巧没有 bug；第二是技巧本身是否在当前盘面下找全了。如果有漏找的话，技巧依然是存在 bug 的。

![6](https://images.gitee.com/uploads/images/2020/1229/203257_c6bd9374_1449374.png)

## 最后

如果你前面调试完成并且运行成功的话，那么恭喜你，技巧就实现完成了。