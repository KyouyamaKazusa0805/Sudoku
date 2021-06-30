# **如何编译代码 (How to compile the solution)**
## Step 1: Clone the repo

第一步：克隆本代码库

Please open a terminal, and input the bash code:

请打开一个终端，然后输入如下的 Bash 代码：

```bash
git clone https://github.com/Sunnie-Shine/Sudoku.git
```

Then you just wait for the downloading finish.

然后你就等着下载完成吧。

## Step 2: Open the file `Sudoku.sln` with Visual Studio 2019

第二步：用 Visual Studio 2019 打开 `Sudoku.sln` 文件

Please note that, the file `Sudoku.sln` is located in the root path, you don't need to find it in sub-folders. In addition, only VS2019 can open it.

请注意，文件 `Sudoku.sln` 就在根目录下，你不需要进入任何子文件夹里去找。另外，只有 VS2019 可以打开。

## Step 3: Copy the required files

第三步：拷贝一些必需文件

This step is important. You should copy folders `lang` into debug folders in the project `Sudoku.Core` and `Sudoku.Windows`, and copy `tessdata` into the debug folder of the project `Sudoku.Windows`. Those two folders are both in `required` folder.

这一步非常重要。你需要拷贝 `lang` 文件夹到 `Sudoku.Core` 和 `Sudoku.Windows` 两个项目的调试文件夹里，然后把 `tessdata` 文件夹拷贝到 `Sudoku.Windows` 项目的调试文件夹里。这两个文件夹都在 `required` 文件夹下。

![1](https://user-images.githubusercontent.com/23616315/103188802-dceabc00-4904-11eb-80ff-eb964ea8dee9.png)

![2](https://user-images.githubusercontent.com/23616315/103188811-e2e09d00-4904-11eb-9ed9-bdfe66bda24a.png)

![3](https://user-images.githubusercontent.com/23616315/103188813-e5db8d80-4904-11eb-8a98-c94acd586e28.png)

## Step 4: Debug and run the solution

第五步：调试运行项目

The last step is to debug and run. Just click the compile button or select `Debug > Start Debugging` to debug. Make sure you have selected `Sudoku.Windows` as the start project.

最后一步就是调试和运行了。请点击编译按钮，或者选择 `调试 > 开始调试` 来调试程序。请确保你已经把 `Sudoku.Windows` 作为启动项目。

![4](https://user-images.githubusercontent.com/23616315/103188907-44087080-4905-11eb-8552-ebfafadfb2b8.png)

![5](https://user-images.githubusercontent.com/23616315/103188911-479bf780-4905-11eb-9d8a-a0f08895b9da.png)

Okay. Now you can enjoy the program!

行了，现在你可以用这个程序了！

English version:

英文版：

![English](https://user-images.githubusercontent.com/23616315/103188957-7dd97700-4905-11eb-8dc9-1d1bfc620004.png)

Simplified Chinese version:

简体中文版：

![Simplified Chinese](https://user-images.githubusercontent.com/23616315/103188948-7023f180-4905-11eb-81ca-778f0fdf8c54.png)
