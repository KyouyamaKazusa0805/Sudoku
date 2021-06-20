## API 变动

### Bug 修复

* 对于 BUG + n 技巧，无法显示出真数的信息（#113）；
* 分析器 bug 导致解构函数的代码分析器不起作用（#115）；
* 分析器 bug 导致 SS0506 分析失效（#116）；
* `ToCamelCase` 和 `ToPascalCase` 方法会导致内存泄漏的严重问题（#118）；
* 分析器 bug 导致无法分析位数值的枚举类型（#119）；
* 分析器 bug 导致 SS0301 的一部分比较麻烦的条件无法识别（#120）；
* 分析器 bug 导致括号表达式无法被识别，导致 SS0301 无法确认调用序列是不是 `IEnumerable<int>` 类型的（#121）；
* 分析器 bug 导致无法识别常量字段或常量，导致 SS9001 的条件判断的右侧计算值无法消去警告（#122）；
* 分析器 bug 导致无法确定 `null` 字面量的类型，如果和另外一个引用类型的表达式参与计算比较的话，那么必然会抛出异常（#123）；
* 分析器 bug 导致忘记分析处理 `ToString` 方法输出文字的时候，需要处理递归成员的问题（#124）；
* 分析器 bug 导致无法对值类型和引用类型进行分析（#125）；
* 分析器 bug 导致 SS0615 分析情况异常（#126）；
* 分析器 bug 导致如果触发 `if (expr is null) { 多语句 }` 的部分会引起 `CompoundNullCoalescingAnalyzer` 类的代码崩溃（#127）；
* 分析器 bug 导致无法正确载入资源字典用于判断数据（#128）。

### 增加

* 为 `SudokuGrid` 结构添加 `IsEmpty`、`IsUndefined` 和 `IsDebuggerUndefined` 属性，可用于模式匹配；
* 添加 `Sudoku.CodeGen.PrimaryConstructor` 项目，用来给一些类自动生成主构造器；
* 添加 `Sudoku.CodeGen.Deconstruction` 项目，用来给非递归嵌套的类型自动生成解构函数；
* 添加 `Sudoku.CodeGen.Equality` 项目，用来自动生成相等性比较函数 `Equals`；
* 添加 `Sudoku.CodeGen.HashCode` 项目，用来自动生成哈希码计算函数 `GetHashCode`；
* 添加 `Sudoku.CodeGen.RefStructDefaults` 项目，用来自动为引用结构生成无意义的重写方法（比如 `Equals` 方法）；
* 添加 `Sudoku.CodeGen.GetEnumerator` 项目，用于自动生成 `GetEnumerator` 的相关执行方法；
* 添加 `Sudoku.CodeGen.DiagnosticInfo` 项目，专门为了提供项目分析器的 ID 和分类的类；
* 添加 `Sudoku.CodeGen.CodeAnalyzerDefaults` 项目，专门为了提供项目分析器和修补器的默认代码生成；
* 增加 `Sudoku.CodeGen.Formattable` 项目用来生成 `ToString` 相关方法。

### 删除

* 删除 `Bot` 和 `Bilibili` 相关的源代码。你放心，这些代码并没有真正删除，只是暂时从这个解决方案里删掉了，我还保存了一份在本地。等待 .NET 6 发布后，我会考虑继续更新到 .NET 6 后再重新放进项目里来；
* 删除 `SUDOKU_GRID_LINQ` 条件编译符号和所有有关 `SudokuGrid` 的 LINQ 相关迭代器的文件；
* 删除 `Sudoku.DocComments` 这个只是为了提供文档注释用的无意义的项目。

### 修改

* **重大修改：所有的项目不再支持 CLS 公共语言标准**（因为各个源代码生成器需要依赖变长参数初始化的特性，而这个东西是不符合 CLS 要求的）；
* 将注入式语义分析器修改为 VSIX 插件式语义分析器；
* 把原始 `BitOperations` 的相关代码生成改成用源代码生成器 `Sudoku.CodeGen.BitOperations` 项目自动生成；
* 将 `Sudoku.CodeGen` 项目里的内容移动到 `Sudoku.CodeGen.KeyedTuple` 项目里；
* 对所有引用了 `Sudoku.DocComments` 文档注释的成员全部更改了文档注释内容（要么删掉要么重新更新内容）；
* 合并 `Sudoku.Solving` 和 `Sudoku.Generating` 项目。