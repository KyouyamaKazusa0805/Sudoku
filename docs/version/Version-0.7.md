# V0.7
## API 变动

### Bug 修复

* 修复分析器 bug 导致 SS9001 分析器没有诊断检查 `for` 循环外侧的变量名，以导致建议名称发生冲突而无法发现（#153）；
* 修复分析器 bug 导致 SD0413 诊断检查了 `static` lambda、匿名函数和本地函数的同名变量（#155）；
* 修复源代码生成器 bug 导致 `Sudoku.UI.WinUI` 没有识别到，没有生成相应可使用代码（#158）；
* 修复 `ref struct` 的源代码生成器 bug 导致嵌套类型的缩进错误（#160）；
* 修复读取关联 `TwoStrongLinksStepInfo` 数据类型的 `Format` 属性和字典交互失败的问题（#163）；
* `UrStepSearcher` 里有一个 `private` 的方法的代码是错误的执行行为（#165）；
* 修复探长致命结构类型 4 的 bug 导致删数没有找全（#166，如图 1 所示）。

### 增加

* ~~添加 `Sudoku.UI` 和 `Sudoku.UI.WinUI` 项目，是 MAUI 的项目；~~
* ~~添加 MAUI 的 WinUI 的程序图标（#159）；~~
* 添加 `Sudoku.Data.Grid` 类型作为 `Sudoku.Data.SudokuGrid` 类型的替代；
* 添加了很多基本类型的 JSON 序列化和反序列化的嵌套类型提供序列化操作；
* 为 `Sudoku.Solving.Manual.StepInfo` 类型追加 `Format` 属性用于代替 `ToString` 和 `ToFullString` 方法：提供接入资源字典的该 API 可直接获取资源字典上的对应信息以多语言切换显示不同的输出结果；
* 添加一些代码生成器用来服务 `Sudoku.UI` 项目，简化代码。

### 删除

* 删除 `SUDOKU_RECOGNITION` 条件编译符号；
* 删除本来应该在 `Sudoku.Solving.Old` 项目拓展，但没有使用机会的内容（#176）；
* 删除 `MemberInfoExtensions` 类型，删除 `TypeExtensions.IsSubclassOf<>` 方法；
* 删除 `UnsafeExtensions` 类型。

### 修改

* 修改部分代码的逻辑，给部分数据类型追加 `[Obsolete]` 表示类型不再使用；
* 将 `ICloneableEx` 静态类里的 `Cast` 扩展方法改名为 `As`；
* 移动 `CellParser` 类型，从 `Sudouk.Data.Extensions` 命名空间移动到 `Sudoku.Data.Parsers` 命名空间下；
* `Als` 结构里的 `StrongLinksMask` 属性的返回值类型从 `IEnumerable<short>` 改成了 `short[]`；
* 完成对 `Sudoku.Windows` UI 项目下，技巧的提示文字的汉化（并改变原始使用 `ToString` 方法的导出逻辑，而创建并改用 `Format` 属性）；
* 所有的 `StepSearcher` 派生实体类的 `init` 属性均改成 `set` 属性提供以后的属性修改和变动；
* 消除了所有的 `Extensions` 后缀的命名空间，为了确保代码的分配和风格和微软提供的库 API 一致，采用给扩展类添加 `Extensions`、`Marshal` 等后缀，并使用同类型命名空间的方式来消除自创的命名空间；
* 将源代码生成器里的 `KeyedTuple` 泛型类型从 `record` 改成 `readonly record struct`；
* 删除 `StepInfoExtensions` 类型，但把它们内部的方法用属性的形式直接放进了 `StepInfo` 记录类型里；
* 将一些项目的所有类型的命名空间添加上后缀 `Old`，自己也添加 `Old` 后缀，比如 `Sudoku.Drawing.Old`，暗示项目不再被别的项目引用；
* 提升 C# 版本到 C# 10，更进新语法；
* 所有带 `Ex` 后缀的扩展类型（带扩展方法的类型）全部从 `Ex` 改为 `Extensions`，**并修改命名空间到这个类型自身的命名空间下**；
* 代码重构，修改源代码生成器的基本代码，使用面向 LINQ 的源代码生成器，使得代码更具可读性。

## UI 变动

* 添加教程和 Wiki 链接到 `Sudoku.Windows.Old`（原 `Sudoku.Windows` 项目）的“关于”窗体里（#167）。

# 图片

![图 1](https://user-images.githubusercontent.com/23616315/130937553-88ed0036-dd28-4c62-b52e-78c06aba3abe.png "图 1.png")
