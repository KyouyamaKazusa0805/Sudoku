# `check` 指令

**介绍**：`check` 指令用于判断一个数独盘面是否满足一些基本的特性规则，比如唯一解之类的。

**用法**：`check --grid <盘面> --type <特性名>`

## 参数

### `--grid` 参数（简写 `-g`）

`--grid` 参数需要给出用于判断的数独题目。参数支持各种 `Grid` 可以解析的字符串类型。

### `--type` 参数（简写 `-t`）

`--type` 参数主要给出的是判断的题目的类型。所有支持的类型可以前往 [`CheckType` 类型](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.CommandLine/RootCommands/CheckType.cs)的相关代码进行查看。
