# V0.6
## API 变动

### Bug 修复

* `Exception.HelpLink` 的页面没有从 Gitee 迁移到 GitHub Wiki 上（#129）；
* 修复一些 Wiki 链接写错或者文字的打错；
* `RefStructDefaults` 需要忽略掉嵌套类型的源代码生成（#134）；
* 修复 SS9005 分析器的 bug 导致忘记分析非自动实现的 get 属性导致的假阳性现象（#135）。

### 增加

* 给 `DigitCollection` 追加了 `operator &`、`operator |` 和 `operator ^` 的运算符支持；
* 添加 `CsvDocument` 类，用于读取 CSV 格式的文件；
* 为 Wiki 里添加一些例子，以帮助你使用整个解决方案的 API（#133）；
* 添加 `FileScopedNamespaceSyntaxReplacer` 类型，为即将的 C# 10 特性“基于文件的命名空间”（File-scoped namespace）提供代码语法的转换操作。

### 删除

* 批量删除一些标记性注释（这些注释对读代码来说用处不大，只是为了辅助编译器修改变更代码风格）。

### 修改

* 将所有源代码生成器的项目归并到同一个项目里（项目名为 `Sudoku.CodeGenerating`）；
* 删除 `Sudoku.Globalization` 项目，把项目的代码移动到 `Sudoku.Core` 项目的 `Globalization` 子文件夹下；
* 修改 `StringEx.SatisfyPattern` 方法的第二个参数，从 `string` 改为 `string?`，并追加 `[NotNullWhen(true)]` 于该参数上；
* 修改 `RecognitionServiceProvider` 类型里的 `RecognizeAsync` 异步方法的返回值从 `Task<SudokuGrid>` 改成了 `ValueTask<SudokuGrid>`；
* 将 `StringEx.SliceConcat` 方法重新命名为 `ReplaceAt`，并改变参数为 `char` 类型；
* 所有 XML 文档注释里使用到的转义符号改用 `<![CDATA[]]>` 表达式块存储，避免出现转义符号；
* 禁用掉了 SS0621 和 SS0622 的分析器代码，因为有 bug 不是很方便去修复。
