# `solve` 指令

**介绍**：`solve` 指令用于解题，获取一个题目的终盘，即它的解。

**用法**：`solve --grid <盘面> --method <解题方法名|简记字母|索引>`

## 参数

### `--grid` 参数（简写 `-g`）

`--grid` 参数需要给出用于解题的数独题目。参数支持各种 `Grid` 可以解析的字符串类型。

### `--method` 参数（简写 `-m`）

`--method` 参数指定的是解题使用的算法。在 [`Sudoku.Core` 项目](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core/Solving)里指定了很多种不同类型的解题算法。这些算法都是用于支持使用的解题算法名。
