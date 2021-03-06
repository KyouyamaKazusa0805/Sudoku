# Version 0.3

## API 变动

### Bug 修复

* JE 里，在检查共轭对和 AHS 的时候，条件位置写错导致所有输入都永真；（[#I39YE9](https://gitee.com/SunnieShine/Sudoku/issues/I39YE9)）
* 在导出序列的时候，由于忘记初始化导致序列里的数据被取出垃圾数据，进而导致输出出现内存的严重错误；（[#I39YK6](https://gitee.com/SunnieShine/Sudoku/issues/I39YK6)）
* `Candidates` 类的 `operator ~` 运算符的运算 bug 导致 `Count` 属性计算错误，进而使得严重的内存溢出问题。（[#I3AAW1](https://gitee.com/SunnieShine/Sudoku/issues/I3AAW1)）

### 添加

* 添加 `Sudoku.Bot` 和 `Sudoku.Bot.Console` 项目，使用 Mirai 来完成机器人自动回复群消息，并完成数独相关的任务和操作；
* 添加 `Sudoku.CodeAnalysis` 项目，以代码注入的方式来检测和分析 API 是不是需要额外的约束；
* 添加 `TechniqueTags` 枚举，并为所有的技巧信息记录（`StepInfo`）追加 `TechniqueTags` 属性，以标记技巧属于哪些标签；
* 添加 `StepInfo.HasTag` 方法，确认 `TechniqueFlags` 属性是否包含该枚举 flag 字段；
* 添加 `Cells.RemoveAll(Predicate<int>)` 方法，用来去掉实例里所有满足条件的单元格。

### 删除

* 删除 `DisabledReason.Deprecated` 枚举字段；
* 删除 `TechniqueProperties.FromType<TTechniqueSearcher>` 方法；
* 删除 `string TextResources.GetValue(string)` 方法；
* 删除 `Candidates.GetPinnableReference` 方法；
* 删除所有有关复杂鱼（Complex Fish）有关的设置项。因为新算法不需要限制和关心这些条件，所以没有必要需要它们了（[#I2P1WB](https://gitee.com/Sunnie-Shine/Sudoku/issues/I2P1WB)）。

### 修改

* 修改 `string TextResources.TryGetValue(string)` 方法的访问修饰符为 `private`，一些联动的方法也改成了 `private` 或 `internal`，提取了部分实例方法提供给 `Current` 对象使用（详情请参看别的项）；
* 修改 `Sudoku.Data.Collections.DigitCollection..ctor(short)` 构造器的访问修饰符为 `public`；
* 将 `SudokuGridEx.Deconstruct(this in SudokuGrid, out Cells, out Cells, out Cells[], out Cells[])` 改成非扩展实例方法；
* 将 `SudokuGridEx.Exists(this in SudokuGrid, int, int)` 改成非扩展实例方法；
* 修改执行逻辑，将 `ComplexFishStepSearcher` 改为只有快速解题模式下才启用（[#I2NUU5](https://gitee.com/Sunnie-Shine/Sudoku/issues/I2NUU5)）；
* 把 `Algorithm.Swap<T>(T*, T*)` 移动到 `Pointer.Swap<T>(T*, T*)`；
* 修改 `short Algorithm.CreateBitsInt16(int[])` 的访问修饰符为 `private`；
* 修改 `short Algorithm.CreateBitsInt16(short[])` 的访问修饰符为 `private`；
* 移动 `Algorithms.GetMaskSubsets` 方法到 `Sudoku.Solving.Extensions.Algorithms` 类里；
* 修改 `TextResources` 类为非静态类，并继承自 `DynamicObject` 抽象类（[#I2P8H9](https://gitee.com/Sunnie-Shine/Sudoku/issues/I2P8H9)）；
* 修改 `AnalysisResult` 记录的 `Additional` 属性接收类型为 `object?`，原本是 `string?`；
* 修改 `ManualSolver` 抛出异常的时候的信息：原本 `ManualSolver` 在产生错误信息的时候，会导致异常，此时录入的 `AnalysisResult` 对象会把错误步骤的字符串录入，现在改成对象本身；
* 变动 `Candidates` 数据结构底层的实现：原本用的是一个 `fixed` 数组，现在改成 `long` 的一系列字段。这样可以避免调试器无法识别缓冲区导致无法正确显示结果的问题；
* 修改 `SudokuHandlingException<,>` 类型在错误码为 201（题目步骤处理错误）的时候，第二个参数 `Arg2` 显示和呈现的条件：条件原本设定为 `is string`，现在改成 `is not null`。

## 窗体变动

### 添加

* 添加 `ErrorInfoWindow` 窗体，用来显示错误步骤信息，并呈现步骤对应图片和错误步骤的文本；
* 添加一个新的选项 `ShowLightRegion`，用于显示区域的时候，直接使用线条和方框的方式来表达渲染一个区域，还是用经典的填涂背景色的形式来渲染一个区域。如果是前者（线条），那么看起来就不太美观，但看起来很清晰。比如在绘制鱼的时候，使用经典背景色填涂的模式会使得区域无法或不好区分，前者更容易看清楚和区分清楚具体的定义域和删除域。

### 修改

* 修改一些显示错误信息的呈现方式：错误信息呈现的时候原本用的是 `MessageBox`，但现在改成新添加的 `ErrorInfoWindow` 来呈现，可以显示图片和复制错误信息的文本。