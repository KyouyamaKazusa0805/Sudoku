# V0.2 (20210126)
# 下载链接

https://github.com/SunnieShine/Sudoku/releases/tag/v2021.1.26-beta

# 内容

## API 变动

### Bug 修复

* 修复了鱼这个技巧的计算 bug。

### 增加

* 添加 `Sudoku.CodeGen` 项目，用于源代码生成；
* 添加 `Sudoku.Windows.Media.ColorPalette` 类，并把原本的 `DiffColor` 属性改成字段移动到该静态类下；
* 添加 `Sudoku.Solving.Manual.Extensions.Int32Ex.ToRegion(this int, Sudoku.Data.RegionLabel)` 方法；
* 添加 `Sudoku.Solving.Manual.Exocets.Elimination` 结构，用来存储 JE 和 SE 的删数，表示一种特定删数规则下的删数；
* 添加 `Sudoku.Solving.Manual.Exocets.EliminatedReason` 枚举，用来表示删数的规则；
* 添加 `Sudoku.Solving.Extensions.BatchRater` 静态类，专门用来批量对文件里的题库作分析，然后导出题目和对应题目的分析结果；
* 添加 `Sudoku.Data.Cells..ctor(int*, int)` 构造器，用一个以指针指代的数组进行初始化；
* 添加 `Sudoku.Data.Candidates..ctor(int*, int)` 构造器，用一个以指针指代的数组进行初始化；
* 添加 `Sudoku.Drawing.Converters.CellsJsonConverter` 类，用来提供对 `Cells` 结构的序列化和反序列化转换的支持；
* 添加 `Sudoku.Drawing.Converters.DirectLineJsonConverter` 类，用来提供对 `View` 和 `MutableView` 里的 `DirectLines` 属性的序列化和反序列化转换的支持；
* 添加 `Sudoku.Drawing.Converters.DrawingInfoJsonConverter` 类，用来提供对 `DrawingInfo` 结构的序列化和反序列化转换的支持；
* 添加 `Sudoku.Drawing.Converters.LinkJsonConverter` 类，用来提供对 `Link` 结构的序列化和反序列化转换的支持；
* 添加 `Sudoku.Drawing.Converters.ViewJsonConverter` 类，用来提供对 `View` 类的序列化和反序列化转换的支持；
* 添加 `Sudoku.Drawing.Converters.MutableViewJsonConverter` 类，用来提供对 `MutableView` 类的序列化和反序列化转换的支持；
* 为 `Sudoku.Solving.Manual.StepInfo` 添加 `Acronym` 属性，表示技巧的缩写信息（缩写只提供英文的）；
* 为 `Sudoku.Solving.Manual.StepInfo` 添加 `NameAlias` 属性，表示技巧的别名（技巧别名区分中英文）；
* 为 `Sudoku.Drawing.View` 和 `Sudoku.Drawing.MutableView` 类添加了 `IEquatable<>` 接口的实现，并实现了：
  * `Equals` 方法；
  * `GetHashCode` 方法；
  * `operator ==` 和 `operator !=` 运算符重载；
  * `ToJson` 方法，用来导出 JSON 序列化的字符串信息。
* 为 `Sudoku.Data.SudokuGrid` 添加了一些基本的 LINQ 方法，可以提供遍历和搜索。添加了的方法有：
  * `let`（`Select` 方法实现）；
  * `where`（`Where` 方法实现）；
  * `select`（`Select` 方法实现）；
  * 笛卡尔积（`from`-`from`，`SelectMany` 方法实现）；
  * `group-by`（`GroupBy` 方法实现）。

### 删除

* 删除 `SudokuGridEx.BitwiseOrMasks(this SudokuGrid, in Cells)`；
* 删除 `SudokuGridEx.BitwiseAndMasks(this SudokuGrid, in Cells)`；
* 删除 `Sudoku.Solving.Manual.Exocets.Eliminations.*` 里的所有的类型；
* 删除 `Cells.Overlaps(in Cells)` 方法；
* 删除 `Sudoku.Data.Extensions.RegionLabelEx.ToRegion(this Sudoku.Data.RegionLabel, int)` 方法。 

### 修改

* 微调排除法的计算优先次序：排除法优先找宫排除，然后才是行列排除；当然，原本的算法（先以数字迭代，然后再以宫、行、列的顺序进行区域迭代）也保留了，你依然可以通过修改代码编译来调整算法计算的优先次序。
* 微调输出解析的算法，将图片保存大小和图片在 Word 文档里的大小分开表达。在 Word 里的每一个图片都固定为 550 * 9525 **[1]** 单位大小；图片保存则是 1000 像素单位大小；
* 调整算法，现在 `SudokuGrid` 迭代器将返回盘面的**所有候选数**；在 0.2 之前的版本里，`SudokuGrid` 的迭代器将每一个 `short` 的掩码数值迭代返回；
* 为了配合 `SudokuGrid` 的用法，迭代器将 `ref struct` 改成普通的 `struct` 以避免无法添加委托字段的问题。

* 位置变动
  * 方法变动
    * `GetSubsets<T>(this IReadOnlyList<T>, int)`：`System.Extensions.IReadOnlyListEx` -> `System.Algorithms`；
    * `Sudoku.Solving.Extensions` 命名空间下的所有方法都移动到 `Sudoku.Solving.Manual.Extensions` 命名空间下；
    * `Sudoku.Data.Stepping.UndoableGrid` 里的 `GetCandidateMask` 方法改名为 `GetCandidates`（配合 `SudokuGrid` 的名称）。
  * 类型变动
    * `TechniqueCode` -> `Technique`；
    * `ColorPicker` -> `ColorPickerWindow`；
    * `ColorPickerControl` -> `ColorPicker`；
    * `ColorSwatch` -> `ColorSample`；
    * `ColorSwatchItem` -> `ColorInfo`；
    * `Resources` -> `TextResources`。
  * 命名空间变动
    * `TechniqueCode` 枚举移动到 `Sudoku.Techniques` 命名空间下；
    * `SdcStepInfo` 记录移动到 `Sudoku.Solving.Manual.RankTheory` 命名空间下；
    * `SdcStepSearcher` 类移动到 `Sudoku.Solving.Manual.RankTheory` 命名空间下；
    * `BivalueOddagonStepInfo` 记录移动到 `Sudoku.Solving.Manual.RankTheory` 命名空间下；
    * `BivalueOddagonStepSearcher` 类移动到 `Sudoku.Solving.Manual.RankTheory` 命名空间下；
    * `DifficultyInfo` 记录移动到 `Sudoku.Windows.Data` 命名空间下；
    * `ColorInfo` 类移动到 `Sudoku.Windows.Media` 命名空间下；
    * `DrawingInfo` 结构移动到 `Sudoku.Models` 命名空间下；
    * `Resources` 类移动到 `Sudoku.Resources` 命名空间下；
    * `ColorJsonConveter` 类移动到 `Sudoku.Drawing.Converters` 命名空间下；
    * `Sudoku.Solving.BruteForces.*`**[2]** 移动到 `Sudoku.Solving.BruteForces` 命名空间下，但不改名；
    * `Sudoku.Windows.Tooling.*` 移动到 `Sudoku.Windows.CustomControls` 命名空间下，但不改名（特例请参看其它项）；
    * `Sudoku.Windows.Data.*` 移动到 `Sudoku.Windows.Converters` 命名空间下，但不改名。

## 窗体变动

### Bug 修复

* 在撤销一个填数步骤的时候，候选数还原不正确（#89）
* 每次我按 Escape 想取消选中步骤信息的时候，总是会返回到第一步去（#73）

### 增加

* 为窗体添加命令行参数处理，可以提供参数让程序在打开的时候自动打开该题目；
* 窗体支持画链：设计了两个按钮：一个是触发开始画链的环境，一个是结束画链的环境。开始之后，鼠标在盘面上点击并拖动鼠标位置；鼠标起点和结束点就是链的位置了；删除链的时候将鼠标放到起点，然后按键盘的 `/` 按键（[`Key.Oem2`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key)）即可；
* 支持保存和读取自定义的绘图的数据。

### 修改

* 窗体调色板的倒数第二个颜色改成 `#FF7684`（大概是桃红色）；
* 调整了绘制区域的时候的鼠标点击行为：鼠标现在是拖动选中一个区域，用的是鼠标左键而不是鼠标右键。

## 文字说明

### [1]

9525 是 NPOI 插入图片时，图片换算的换算因子。图片是 550 像素时，需要传入的参数结果需乘以 9525 后才可以。代码大致如下：

```csharp
var runPic = paraPic.CreateRun();
runPic.AddPicture(
    picStream,
    (int)(pictureFileType switch
    {
        PictureFileType.Jpg => PictureType.JPEG,
        PictureFileType.Png => PictureType.PNG,
        PictureFileType.Bmp => PictureType.BMP,
        PictureFileType.Gif => PictureType.GIF
    }),
    curPictureName, TargetSize * Emu, TargetSize * Emu);
```

这里的 `Emu` 就是这个换算因子 9525。


### [2]

`*` 是通配符，表示这个命名空间下的所有类型；以后都使用同样的表达描述，以后就不再解释了。