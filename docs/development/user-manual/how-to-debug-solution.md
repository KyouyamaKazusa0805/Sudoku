> 从 Version 0.2（v20210126）开始，程序的代码里将会使用源代码生成器对部分代码进行生成。如果你的 Visual Studio 2019 不是 16.9 Preview 3 的话，可能导致编译失败。请更新版本到 Visual Studio 2019 v16.9 Preview 3 及其以上的版本。
>
> 如果不使用 16.9 Preview 3 及其以上的版本运行和编译程序的话，你将有机会获得编译器警告 [CS8032](https://cn.bing.com/search?q=CS8032+C%23+An+instance+of+analyzer+%7b0%7d+cannot+be+created+from+%7b1%7d%3a+%7b2%7d.&form=VSHELP) 一份。老实说，这个功能才出来不久，所以很多 bug 需要修复。比如第一次运行的时候，可能报的 CS8032 警告不论编译多少次程序，不论编译是不是已经成功过，这个编译器警告一直都存在（说白了就是不会刷新），只有重启了才没有；但是隔一会儿又来了，但是运行编译程序则又是成功的。

## 第一步：克隆本代码库

请打开一个终端，然后输入如下的 Bash 代码：

```bash
git clone https://github.com/Sunnie-Shine/Sudoku.git
```

然后你就等着下载完成吧。

## 第二步：用 Visual Studio 2019 打开 `Sudoku.sln` 文件

请注意，文件 `Sudoku.sln` 就在根目录下，你不需要进入任何子文件夹里去找。另外，只有 VS2019 版本才可以打开（当然，这不是说别的 IDE 不能打开，只是告诉你 VS 只有 2019 版本才可以打开，毕竟用了 .NET 5）。

## 第三步：拷贝一些必需文件

这一步非常重要。你需要拷贝 `lang` 文件夹到 `Sudoku.Core` 和 `Sudoku.Windows` 两个项目的调试文件夹里，然后把 `tessdata` 文件夹拷贝到 `Sudoku.Windows` 项目的调试文件夹里。这两个文件夹都在 `required` 文件夹下。

![1.png](https://images.gitee.com/uploads/images/2020/1228/124719_6a18eac5_1449374.png)

![2.png](https://images.gitee.com/uploads/images/2020/1228/124732_b1aa7768_1449374.png)

![3.png](https://images.gitee.com/uploads/images/2020/1228/124738_f6d6304c_1449374.png)

## 第四步：调试运行项目

最后一步就是调试和运行了。请点击编译按钮，或者选择 `调试 > 开始调试` 来调试程序。请确保你已经把 `Sudoku.Windows` 作为启动项目。

![4.png](https://images.gitee.com/uploads/images/2020/1228/124757_cbd8031f_1449374.png)

![5.png](https://images.gitee.com/uploads/images/2020/1228/124805_e0f15967_1449374.png)

行了，现在你可以用这个程序了！

英文版：
![7.png](https://images.gitee.com/uploads/images/2020/1228/124842_7fdadc3c_1449374.png)

简体中文版：
![6.png](https://images.gitee.com/uploads/images/2020/1228/124821_20e8fc76_1449374.png)
