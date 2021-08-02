# V0.7
## API 变动

### Bug 修复

* 修复分析器 bug 导致 SS9001 分析器没有诊断检查 `for` 循环外侧的变量名，以导致建议名称发生冲突而无法发现（#153）；
* 修复分析器 bug 导致 SD0413 诊断检查了 `static` lambda、匿名函数和本地函数的同名变量（#155）；
* 修复源代码生成器 bug 导致 `Sudoku.UI.WinUI` 没有识别到，没有生成相应可使用代码（#158）；
* 修复 `ref struct` 的源代码生成器 bug 导致嵌套类型的缩进错误（#160）。

### 增加

* 添加 `Sudoku.UI` 和 `Sudoku.UI.WinUI` 项目，是 MAUI 的项目；
* 添加 MAUI 的 WinUI 的程序图标（#159）；
* 添加 `Sudoku.Data.Grid` 类型作为 `Sudoku.Data.SudokuGrid` 类型的替代；
* 添加了很多基本类型的 JSON 序列化和反序列化的嵌套类型提供序列化操作。

### 删除



### 修改

* 修改部分代码的逻辑，给部分数据类型追加 `[Obsolete]` 表示类型不再使用；
* 将 `ICloneableEx` 静态类里的 `Cast` 扩展方法改名为 `As`。
