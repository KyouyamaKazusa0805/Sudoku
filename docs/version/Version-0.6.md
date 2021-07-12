# V0.6
## API 变动

### Bug 修复

* `Exception.HelpLink` 的页面没有从 Gitee 迁移到 GitHub Wiki 上（#129）；
* 修复一些 Wiki 链接写错或者文字的打错；
* `RefStructDefaults` 需要忽略掉嵌套类型的源代码生成（#134）。

### 增加

* 给 `DigitCollection` 追加了 `operator &`、`operator |` 和 `operator ^` 的运算符支持；
* 添加 `CsvDocument` 类，用于读取 CSV 格式的文件；
* 为 Wiki 里添加一些例子，以帮助你使用整个解决方案的 API（#133）。

### 删除



### 修改

* 将所有源代码生成器的项目归并到同一个项目里（项目名为 `Sudoku.CodeGenerating`）；
* 删除 `Sudoku.Globalization` 项目，把项目的代码移动到 `Sudoku.Core` 项目的 `Globalization` 子文件夹下；
* 修改 `StringEx.SatisfyPattern` 方法的第二个参数，从 `string` 改为 `string?`，并追加 `[NotNullWhen(true)]` 于该参数上；
* 修改 `RecognitionServiceProvider` 类型里的 `RecognizeAsync` 异步方法的返回值从 `Task<SudokuGrid>` 改成了 `ValueTask<SudokuGrid>`；
* 将 `StringEx.SliceConcat` 方法重新命名为 `ReplaceAt`，并改变参数为 `char` 类型。
