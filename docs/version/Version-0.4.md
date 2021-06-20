# 下载链接

https://github.com/SunnieShine/Sudoku/releases/tag/v2021.4.14-beta

# 内容

## API 变动

### Bug 修复

* `MathEx.Min` 算法的代码写错了（#117）；
* 修复部分文档注释的失效链接、错误描述或者单词打错，以及一些没有意义的描述；
* 由于调用方法的问题，导致导出分析文本的过程中，不论你默认是什么语言，都会被自动切换成英语（#110）；
* 在解析数独盘面的代码的时候，如果盘面是无法解析成功的话，会直接抛出 `ArgumentOutOfRangeException` 异常（原本期望情况是返回 `SudokuGrid.Undefined`）（#111）。

### 增加

* 增加所有可代替 `SudokuHandlingException` 的子情况的异常类型；
* 添加一些关于 `Cells` 的构造器；
* 为袋鼠技巧添加专门用来写未知数的渲染操作和对应的属性，并淡入淡出袋鼠格子的候选数信息；
* 添加技巧“UR + 袋鼠”；
* 给 `PresentationData` 添加了专门表示袋鼠（代数未知数）的图层字段，也实现了对应的 JSON 读写操作；
* 给 `AnalysisResult` 添加了索引器，用于获取 `AnalysisResult` 存储的人工解题步骤；
* 增加 `Cells.Parse`、`Cells.TryParse`、`Candidates.Parse` 和 `Candidates.TryParse` 方法；
* 增加 `TechniqueGroup` 枚举类型，用来合并呈现技巧；
* 为 `StepInfo` 添加 `TechniqueGroup` 枚举的属性，用来表示技巧所属的分类；
* 添加对 Open-sudoku 格式的数独文本解析和导出操作。

### 删除

* 删除 `SudokuHandlingException`；
* 删除 `Cells operator -(in Cells, IEnumerable<int>)` 和 `Cells operator -(IEnumerable<int>, in Cells)`；
* 删除 `Cells operator +(in Cells map, int cell)` 和 `Cells operator -(in Cells map, int cell)`。

### 修改

* 害，最近没有空，我把以前的死亡绽放的代码放上来了。代码最近没时间找 bug；
* 修改渲染逻辑，允许袋鼠的过程填数使得当前单元格的候选数和呈现格颜色淡出；
* 将类名 `TechniqueStrings` 改成 `TechniqueNaming`；
* `Cells.operator *(in Cells, in Cells)` 改成了 `Cells.operator %(in Cells, in Cells)`。

## 窗体变动

### 增加

* 增加对 Open-sudoku 格式的支持选项，支持复制 Open-sudoku 的文本。