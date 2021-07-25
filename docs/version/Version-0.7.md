# V0.7
## API 变动

### Bug 修复

* 修复分析器 bug 导致 SD0413 诊断检查了 `static` lambda、匿名函数和本地函数的同名变量（#155）；
* 修复源代码生成器 bug 导致 `Sudoku.UI.WinUI` 没有识别到，没有生成相应可使用代码（#158）；
* 修复 `ref struct` 的源代码生成器 bug 导致嵌套类型的缩进错误（#160）。

### 增加

* 添加 `Sudoku.UI` 和 `Sudoku.UI.WinUI` 项目，是 MAUI 的项目；
* 添加 MAUI 的 WinUI 的程序图标（#159）。

### 删除



### 修改

