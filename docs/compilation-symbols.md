# 条件编译符号

这里列举一些整个解决方案里使用到的单文件和项目为单位的条件编译符号。

## 单文件的条件编译符号

| 符号名[^1]                                  | 默认值[^2] | 文件[^3]                                                     | 含义                                                         |
| ------------------------------------------- | ---------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| `USE_TO_MASK_STRING_METHOD`                 | 无         | [Grid.cs](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.cs) | 表示 `Grid` 数据结构是否以底层掩码表作为输出文字显示在调试工具上。 |
| `IMPLEMENTED`                               | 无         | [UniqueRectangleStepSearcher.cs](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Solving/Manual/Searchers/DeadlyPatterns/Rectangles/UniqueRectangleStepSearcher.cs) | 表示唯一矩形技巧搜索器是否实现当前技巧的子类型的搜寻功能。   |
| `DECREASE_INITIALIZATION_MEMORY_ALLOCATION` | 有         | [StringHandler.cs](https://github.com/SunnieShine/Sudoku/blob/main/src/System/Text/StringHandler.cs) | 表示 `StringHandler` 字符串拼接器对象是否在初始化的时候减少内存分配。[^4] |
| `DISCARD_INTERPOLATION_INFO`                | 有         | [StringHandler.cs](https://github.com/SunnieShine/Sudoku/blob/main/src/System/Text/StringHandler.cs) | 表示 `StringHandler` 字符串拼接器对象是否在初始化的时候，忽略掉基本初始化信息（比如字符串内插元素数量以及总长度）。 |
| `USE_NEWER_CONSTANT_VALUES`                 | 有         | [StringHandler.cs](https://github.com/SunnieShine/Sudoku/blob/main/src/System/Text/StringHandler.cs) | 表示 `StringHandler` 字符串拼接器对象假设内插字符串只有 8 个内插部分（如果不设置此符号的话，则是 11）。 |

## 项目的条件编译符号

| 符号名                        | 默认值 | 含义                                                         |
| ----------------------------- | ------ | ------------------------------------------------------------ |
| `DEBUG`                       | 有     | 表示当前项目是否出于调试期间。只有调试期间才会使用这个符号。 |
| `TRACE`                       | 无     | 表示当前项目的一些追踪信息操作。注意，当前解决方案尚未使用此符号，但是它客观存在。 |
| `FEATURE_GENERIC_MATH`        | 有     | 表示当前编程语言和 CLR 是否支持静态抽象成员[^5]特性。        |
| `FEATURE_GENERIC_MATH_IN_ARG` | 无     | 表示当前编程语言和 CLR 是否支持静态抽象成员，而且参数是带有 `in` 关键字的。 |
| `true`                        |        | 表示无条件编译此段代码。                                     |
| `false`                       |        | 表示无条件忽略此段代码。                                     |

## 其它符号

| 符号名                      | 使用范围               | 含义                                                      |
| --------------------------- | ---------------------- | --------------------------------------------------------- |
| `NETSTANDARD2_1_OR_GREATER` | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET Standard 2.1 以及以上版本。 |
| `NET5_0_OR_GREATER`         | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET 5 以及以上版本。            |
| `NETCOREAPP3_0`             | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET Core 3.0 版本。             |
| `NETCOREAPP3_1`             | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET Core 3.1 版本。             |

[^1]: “符号名”表示该条件编译符号在代码里使用的标识符名称。
[^2]: “默认值”表示该条件编译符号程序运行和使用期间，按什么情况编译和表达。“有”表示项目或代码设置了该符号，那么使用 `#if` 了的这段代码就会被编译起来；否则会被忽略。
[^3]: “文件”表示该条件编译符号只出现在什么代码文件之中。
[^4]: 在代码实现里，`StringHandler` 为了减少内存分配以提升性能，这里故意提供了这个符号处理。如果配置了该符号，那么在初始化的时候，直接将计算得到的结果作为内存分配的大小；否则会按照“按 2 的次幂向上取整”的约定将计算得到的结果进行处理，然后将处理后的结果作为分配大小。

[^5]: **静态抽象成员**（Static Abstract Members）是 C# 的一个特性，允许在接口里定义静态的抽象成员，然后丢给实现类型进行实现的机制。这种机制需要依赖于 CLR 和语言本身的支持，因此目前还只是一个预览特性。
