# MAUI 项目

## 介绍

MAUI 是一个比较新的 UI 框架，一次编译就可以在多个不同的平台上运行同一个程序：

* Windows（Win10 及更高版本）
* 安卓
* iOS
* Mac
* Tizen（三星系统，可选）

## 补充

如果你要运行该项目的话，你需要先去还原一下该项目的 NuGet 包，用下面的命令来还原：

```bash
cd .\src\Sudoku.Maui
dotnet restore .\Sudoku.Maui.csproj
```

这似乎是 Visual Studio 目前 MAUI 项目模板里的一个 bug。
