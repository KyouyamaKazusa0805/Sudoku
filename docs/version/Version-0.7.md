# V0.7
## API 变动

### Bug 修复

* 修复分析器 bug 导致 SS9001 分析器没有诊断检查 `for` 循环外侧的变量名，以导致建议名称发生冲突而无法发现（#153）；
* 修复分析器 bug 导致 SD0413 诊断检查了 `static` lambda、匿名函数和本地函数的同名变量（#155）；
* 修复源代码生成器 bug 导致 `Sudoku.UI.WinUI` 没有识别到，没有生成相应可使用代码（#158）；
* 修复 `ref struct` 的源代码生成器 bug 导致嵌套类型的缩进错误（#160）；
* 修复读取关联 `TwoStrongLinksStepInfo` 数据类型的 `Format` 属性和字典交互失败的问题（#163）。

### 增加

* 添加 `Sudoku.UI` 和 `Sudoku.UI.WinUI` 项目，是 MAUI 的项目；
* 添加 MAUI 的 WinUI 的程序图标（#159）；
* 添加 `Sudoku.Data.Grid` 类型作为 `Sudoku.Data.SudokuGrid` 类型的替代；
* 添加了很多基本类型的 JSON 序列化和反序列化的嵌套类型提供序列化操作；
* 为 `Sudoku.Solving.Manual.StepInfo` 类型追加 `Format` 属性用于代替 `ToString` 和 `ToFullString` 方法：提供接入资源字典的该 API 可直接获取资源字典上的对应信息以多语言切换显示不同的输出结果；

### 删除

* 删除 `SUDOKU_RECOGNITION` 条件编译符号。

### 修改

* 修改部分代码的逻辑，给部分数据类型追加 `[Obsolete]` 表示类型不再使用；
* 将 `ICloneableEx` 静态类里的 `Cast` 扩展方法改名为 `As`；
* 移动 `CellParser` 类型，从 `Sudouk.Data.Extensions` 命名空间移动到 `Sudoku.Data.Parsers` 命名空间下；
* `Als` 结构里的 `StrongLinksMask` 属性的返回值类型从 `IEnumerable<short>` 改成了 `short[]`；
* 完成对 `Sudoku.Windows` UI 项目下，技巧的提示文字的汉化（并改变原始使用 `ToString` 方法的导出逻辑，而创建并改用 `Format` 属性）；
* 所有的 `StepSearcher` 派生实体类的 `init` 属性均改成 `set` 属性提供以后的属性修改和变动；
* 消除了所有的 `Extensions` 后缀的命名空间，为了确保代码的分配和风格和微软提供的库 API 一致，采用给扩展类添加 `Extensions`、`Marshal` 等后缀，并使用同类型命名空间的方式来消除自创的命名空间；
* **重大修改：所有带 `Ex` 后缀的扩展类型（带扩展方法的类型）全部从 `Ex` 改为 `Extensions`，并修改命名空间到这个类型自身的命名空间下。**；
* 提升 C# 版本到 C# 10，更进新语法。
