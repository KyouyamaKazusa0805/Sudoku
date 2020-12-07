## Conditional Compilation Symbol

标题：**条件编译符号**

Here display all conditional compilation symbols (CCS) in this solution. CCSes are the global Boolean values that exist in some projects to indicate whether the code block with these symbols are compiled or not. The block can't be compiled until the specified symbol is set in the project file (`*.csproj`).

这里罗列本解决方案里用到的条件编译符号（简称 CCS）。CCS 是全局的布尔量，它们存在于一些项目里，用来表示某段代码块是否需要编译。这段代码块只有当我们在项目文件（`*.csproj`）里配置了符号之后，才会被编译。

Some of them are unnecessary for you perhaps, you can remove them. In addition, if you want to modify any CCSes, please search for ways online on modifying them.

其中的一些对你可能没有必要，所以你可以移除它们。另外，如果你要修改这些符号，请上网查阅修改它们的办法。

| CCS<br/>条件编译符号         | Default<br />默认值 | Usage<br/>用法                                               |
| ---------------------------- | ------------------- | ------------------------------------------------------------ |
| `DEBUG`                      | `true`              | Indicates the current environment is for debugging.<br/>表示当前是调试环境。 |
| `SUDOKU_RECOGNITION`         | `true`              | Indicates whether your machine can use OCR tools to recognize an image, and convert to a sudoku grid data structure instance.<br/>表示是否你的电脑上可以使用 OCR 识别工具来识别一个图片，并将其转换为一个数独盘面的实例对象。 |
| `AUTHOR_RESERVED`            | `true`              | Indicates the method is only used for author himself. You can delete the code surrounded with this symbol.<br/>表示这段代码只对作者来说才有意义。你完全可以删除掉这段代码，或者不使用该符号。 |
| `MUST_DOWNLOAD_TRAINED_DATA` | `false`             | Indicates whether the solution will download the trained data file `eng.traineddata` on GitHub when the local file with the same name cannot be found. Sometimes the file downloading is too slow to stand with it. If this symbol is undefined, it'll offer the user an error message window,  saying local file cannot be found.<br/>表示这个解决方案是否在本地的同名文件不存在的时候，从 GitHub 上下载该文件。有时候这个下载特别慢，以至于我们完全没办法忍受它。如果这个符号没有定义的话，我们就会在文件找不到的时候直接以错误弹窗的形式提示用户不能使用识别功能。 |
| `DOUBLE_LAYERED_ASSUMPTION`  | `false`             | Indicates whether projects will enable double-layered assumption to solve all puzzles. Double-layered assumption is a very terrible solving technique to solve all sudoku puzzles. They're copied from Sudoku Explainer project.<br/>表示项目是否需要启用双层假设来完成题目。双层假设是一个用来解决所有数独题目的非常可怕的技巧，而它们是从 Sudoku Explainer 项目里拷贝过来的。 |
| `OBSOLETE`                   | `false`             | Indicates the code block is no longer in use. You can still enable to compile them, but unstable.<br/>表示当前代码段不再使用。你可以启用编译它们，但它们不稳定。 |

