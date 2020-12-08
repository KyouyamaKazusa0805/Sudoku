# Intro

A sudoku handling SDK using brute forces and logical techniques. Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

> The window may be like a program called [Hodoku](http://hodoku.sourceforge.net/en/index.php). However, the base window of Hodoku is only for reference.
>



## Programming language and IDE using

* Programming language: C#
* Language version: 9.0
* Framework: .NET 5
* Indenting: Tabs（`\t`）
* Integrated development environment: Visual Studio 2019 V16.9 Preview
* Language Support: English, Simplified Chinese



## How to use

### Codes

Clone this repo, and you can take all codes!

```bash
git clone https://github.com/Sunnie-Shine/Sudoku.git
```

Of course, you can also download the zip file.



### Compiling & Running

Before running the program, you should download some files ahead of time (or check the existence of some files). Please check [this file](https://github.com/Sunnie-Shine/Sudoku/blob/master/ref/require/ReadMe.txt) in [this folder](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref/require) for more information.



### Folders

This whole solution consists of several folders below:

| Project                                                      | Description                                                  |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [`Sudoku.Core`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Core) | The main data structure implementation of the sudoku elementary. |
| [`Sudoku.Diagnostics`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Diagnostics) | This project encapsulates operations for diagnosing the whole solution, such as checking the number of code lines. |
| [`Sudoku.DocComments`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.DocComments) | I just want to say this project is useless but for providing other projects with document comments. Due to some reason hard to say, I've deleted the XML files used for providing with comments and using real instances to offer comments. |
| [`Sudoku.Drawing`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Drawing) | The project that can be used for drawing and rendering sudoku grids. |
| [`Sudoku.Globalization`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Globalization) | This project encapsulates the operation and constants for globalization interactions. |
| [`Sudoku.IO`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.IO) | The project handling IO operations over sudoku.              |
| [`Sudoku.Recognition`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Recognition) | This project encapsulates the operations about recognition of a sudoku picture. |
| [`Sudoku.Solving`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Solving) | The generating and solving project.                          |
| [`Sudoku.Windows`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Windows) | The WPF project, containing UI forms and controls.           |
| [`Sudoku.Debugging`](https://github.com/Sunnie-Shine/Sudoku/tree/master/Sudoku.Debugging) | The project that can be used while debugging only.           |
| [`System`](https://github.com/Sunnie-Shine/Sudoku/tree/master/System) | The project provides the extension methods, classes and structures in use. |
| [`docs`](https://github.com/SunnieShine/Sudoku/tree/master/docs) | Provides the document or helper files of this solution or sudoku itself. |
| [`ref`](https://github.com/Sunnie-Shine/Sudoku/tree/master/ref) | The profiles for sudoku for references. In addition, some necessary files to help us compile and run the whole project are also in this folder. |
| [`pic`](https://github.com/Sunnie-Shine/Sudoku/tree/master/pic) | The pictures.                                                |



## Other introduction

Please visit [this page](https://sunnieshine.github.io/Sudoku/index) to get more information.



## Author

Sunnie, from Chengdu, is an normal undergraduate from normal university.

