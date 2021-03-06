这个解决方案由如下文件夹构成：

| 文件夹/项目                                                  | 框架              | 类型 | 描述                                                         |
| ------------------------------------------------------------ | ----------------- | ---- | ------------------------------------------------------------ |
| [`Sudoku.Bot`](https://gitee.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Bot) | .NET  5           |      | 机器人项目。这个项目提供了数独相关插件的主要实现模型。       |
| [`Sudoku.Bot.Console`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Bot.Console) | .NET  5           | 终端 | 控制台项目。这个项目用来和 Mirai 交互，达到机器人发送消息的目的。 |
| [`Sudoku.CodeAnalysis`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.CodeAnalysis) | .NET Standard 2.0 |      | 源代码生成器项目，提供代码规范性的检测。                     |
| [`Sudoku.CodeGen`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.CodeGen) | .NET Standard 2.0 |      | 源代码生成器项目，生成必需的代码用。                         |
| [`Sudoku.Core`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Core) | .NET 5            |      | 对数独基本元素的主要数据结构的实现。                         |
| [`Sudoku.Debugging`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Debugging) | .NET 5            | 终端 | 仅用于调试代码时。                                           |
| [`Sudoku.Diagnostics`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Diagnostics) | .NET 5            |      | 封装了诊断整个解决方案的操作，比如检查代码的行数。           |
| [`Sudoku.DocComments`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.DocComments) | .NET 5            |      | 仅用于给其它项目提供文档注释。                               |
| [`Sudoku.Documents`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Documents) | SHFB 文档生成     | /    | 用来生成解决方案文档注释的项目。                             |
| [`Sudoku.Drawing`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Drawing) | .NET 5            |      | 用于绘制和渲染数独盘面。                                     |
| [`Sudoku.Generating`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Generating) | .NET 5            |      | 用于出题的项目。                                             |
| [`Sudoku.Globalization`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Globalization) | .NET 5            |      | 封装了国际化交互的操作和一些常数。                           |
| [`Sudoku.IO`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.IO) | .NET 5            |      | 用于处理数独相关的 IO 操作。                                 |
| [`Sudoku.Painting`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Painting) | .NET 5            |      | 用于绘制和渲染数独盘面，是 `Sudoku.Drawing` 的代替项目（用于 `Sudoku.UI`）。 |
| [`Sudoku.Recognition`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Recognition) | .NET 5            |      | 封装了关于识别一个数独图片的相关操作。                       |
| [`Sudoku.Solving`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Solving) | .NET 5            |      | 解题和题目生成的项目。                                       |
| [`Sudoku.UI`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.UI) | .NET 5            | WPF  | 打算用来代替 `Sudoku.Windows` 的新项目。不过这个项目暂时没有完成，还在开发中。 |
| [`Sudoku.Windows`](https://gitee.com/SunnieShine/Sudoku/tree/main/Sudoku.Windows) | .NET 5            | WPF  | WPF 项目，包含 UI 界面和控件。                               |
| [`System`](https://gitee.com/SunnieShine/Sudoku/tree/main/System) | .NET 5            |      | 提供 .NET 库内相关的扩展的方法、类和结构。                   |
| [`required`](https://gitee.com/SunnieShine/Sudoku/tree/main/required) |                   | /    | 一些帮助我们执行和运行的文件也在此文件夹下。                 |