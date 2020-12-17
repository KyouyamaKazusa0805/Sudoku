# 向向的数独解决方案

English introduction: [Click me!](https://sunnieshine.github.io/Sudoku/)



一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

> 这个窗体可能看起来跟 [Hodoku](http://hodoku.sourceforge.net/en/index.php) 差不多，因为我并不知道什么样子是一个漂亮的界面。使用 Hodoku 差不多的窗体仅仅是为了参考。



## 编程语言和 IDE 使用情况

* 编程语言：C#
* 语言版本：9.0
* 框架：.NET 5
* 缩进：原生 Tab（`\t`）
* 集成开发环境：Visual Studio 2019 V16.9 预览版
* 语言支持：英语、简体中文



## 如何使用

### 代码

只需要你克隆这个仓库就可以带走所有的代码了！

```bash
git clone https://github.com/Sunnie-Shine/Sudoku.git
```

当然，你也可以下载 zip 文件。



### 编译和运行

在运行程序的时候，你需要提前下载一些文件（或者是确认一些文件是否存在）。请参看这个[文件夹](https://github.com/Sunnie-Shine/Sudoku/tree/master/required)下的这个[文件](https://github.com/Sunnie-Shine/Sudoku/blob/master/required/ReadMe.txt)以获取更多相关信息。



### 文件夹

这个解决方案由如下文件夹构成：

| 项目                                                         | 描述                                                         |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) | 对数独基本元素的主要数据结构的实现。                         |
| [`Sudoku.Diagnostics`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Diagnostics) | 这个项目封装了诊断整个解决方案的操作，比如检查代码的行数。   |
| [`Sudoku.DocComments`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.DocComments) | 我只想说，这个项目除了给别的项目提供文档注释以外，就没别的用了。由于一些比较难说的原因，我把用来提供文档注释的 XML 文件都删掉了，改成了真正的实体对象。 |
| [`Sudoku.Drawing`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Drawing) | 这个项目用于绘制和渲染数独盘面。                             |
| [`Sudoku.Globalization`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Globalization) | 这个项目封装了国际化交互的操作和一些常数。                   |
| [`Sudoku.IO`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.IO) | 这个项目用于处理数独相关的 IO 操作。                         |
| [`Sudoku.Recognition`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Recognition) | 这个项目封装了关于识别一个数独图片的相关操作。               |
| [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving) | 解题和题目生成的项目。                                       |
| [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows) | WPF 项目，包含 UI 界面和控件。                               |
| [`Sudoku.Debugging`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Debugging) | 这个项目仅用于调试代码时。                                   |
| [`System`](https://github.com/Sunnie-Shine/Sudoku/tree/master/System) | 这个项目提供 .NET 库内相关的扩展的方法、类和结构。           |
| [`docs`](https://github.com/SunnieShine/Sudoku/tree/master/docs) | 提供文档或者是跟项目有关的帮助文件，抑或是数独本身的内容。   |
| [`required`](https://github.com/Sunnie-Shine/Sudoku/tree/master/required) | 数独相关参考资料。另外，一些帮助我们执行和运行的文件也在此文件夹下。 |
| [`pic`](https://github.com/Sunnie-Shine/Sudoku/tree/master/pic) | 图片。                                                       |



## 其它关于该项目的介绍

请访问[此链接](https://sunnieshine.github.io/Sudoku/index)以获取更多信息。



## 作者

小向，来自成都的一名普通大学生。

