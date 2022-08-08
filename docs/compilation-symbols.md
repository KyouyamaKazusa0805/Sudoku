# 条件编译符号

这里列举一些整个解决方案里使用到的单文件和项目为单位的条件编译符号。

## 单文件的条件编译符号

| 符号名[^1]                                  | 含义                                                         |
| :------------------------------------------ | :----------------------------------------------------------- |
| `IMPLEMENTED`                               | 表示唯一矩形技巧搜索器是否实现当前技巧的子类型的搜寻功能。   |
| `DECREASE_INITIALIZATION_MEMORY_ALLOCATION` | 表示 `StringHandler` 字符串拼接器对象是否在初始化的时候减少内存分配。[^2] |
| `DISCARD_INTERPOLATION_INFO`                | 表示 `StringHandler` 字符串拼接器对象是否在初始化的时候，忽略掉基本初始化信息（比如字符串内插元素数量以及总长度）。 |
| `USE_NEWER_CONSTANT_VALUES`                 | 表示 `StringHandler` 字符串拼接器对象假设内插字符串只有 8 个内插部分（如果不设置此符号的话，则是 11）。 |
| `GRID_SERIALIZE_STRINGS`                    | 表示序列化数独盘面信息是按照字符串输出的格式进行序列化的方式。 |
| `GRID_SERIALIZE_RAW_DATA`                   | 表示序列化数独盘面信息是按照底层的原始掩码表进行序列化的方式。 |
| `USE_EQUALITY_COMPARER`                     | 表示是否使用 `EqualityComparer<T>` 类型来给对象进行比较运算操作。 |
| `VISIT_SITE_DIRECTLY`                       | 表示是否直接弹出浏览器显示指定的链接。如果不设置该符号的话，那么就只会输出网址信息到控制台。 |

完整的符号引用树状图如下：

```mermaid
graph LR
    subgraph 项目引用关系
        Z(Sudoku 解决方案) -->|派生| P1(Sudoku.Core)
        Z -->|派生| P3(Sudoku.CommandLine)
        Z -->|派生| P4(Sudoku.Solving.Manual)
        Z -->|派生| P2(SystemExtensions)
        style Z fill:#FFF
        click Z "https://github.com/SunnieShine/Sudoku" _blank
        click P1 "https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core" _blank
        click P2 "https://github.com/SunnieShine/Sudoku/tree/main/src/System" _blank
        click P3 "https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CommandLine" _blank
        click P4 "https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Manual" _blank
        click P5 "https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.UI" _blank
    end

    subgraph 文件和符号对应关系
        P1 -->|文件| A(Grid.cs)
        P1 -->|文件| D(BitwiseSolver.cs)
        P2 -->|文件| C(Bag.cs)
        P2 -->|文件| G(StringHandler.cs)
        P3 -->|文件| F(MainMethod.cs)
        P4 -->|文件| H(UniqueRectangleStepSearcher.cs)
        style P1 fill:#FFF
        style P2 fill:#FFF
        style P3 fill:#FFF
        style P4 fill:#FFF
        style A fill:#0F8
        style B fill:#0F8
        style C fill:#0F8
        style D fill:#0F8
        style F fill:#0F8
        style G fill:#0F8
        style H fill:#0F8
        click A "https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.cs" _blank
        click C "https://github.com/SunnieShine/Sudoku/blob/main/src/System/Collections/Generic/Bag.cs" _blank
        click D "https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Solving/BitwiseSolver.cs" _blank
        click F "https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CommandLine/MainMethod.cs" _blank
        click G "https://github.com/SunnieShine/Sudoku/blob/main/src/System/Text/StringHandler.cs" _blank
        click H "https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Solving/Manual/Searchers/DeadlyPatterns/Rectangles/UniqueRectangleStepSearcher.cs" _blank

        A -->|嵌套类文件| B(Grid.JsonConverter.cs)
        B -->|符号| S3(GRID_SERIALIZE_STRINGS)
        B -->|符号| S4(GRID_SERIALIZE_RAW_DATA)
        C -->|符号| S5(USE_EQUALITY_COMPARER)
        F -->|符号| S7(VISIT_SITE_DIRECTLY)
        G -->|符号| S8(DECREASE_INITIALIZATION_MEMORY_ALLOCATION)
        G -->|符号| S9(DISCARD_INTERPOLATION_INFO)
        G -->|符号| S10(USE_NEWER_CONSTANT_VALUES)
        H -->|符号| S11(IMPLEMENTED)
        style S3 fill:#CCC
        style S4 fill:#EEE
        style S5 fill:#CCC
        style S7 fill:#CCC
        style S8 fill:#EEE
        style S9 fill:#EEE
        style S10 fill:#EEE
        style S11 fill:#CCC
        click B "https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.JsonConverter.cs" _blank
    end
```

如图所示，深灰色的条件编译符号没有，而浅灰色的条件编译符号有。绿色为文件名，白色的则是项目名。

## 项目的条件编译符号

| 符号名  | 默认值 | 含义                                                         |
| ------- | ------ | ------------------------------------------------------------ |
| `DEBUG` | 有     | 表示当前项目是否出于调试期间。只有调试期间才会使用这个符号。 |
| `TRACE` | 无     | 表示当前项目的一些追踪信息操作。注意，当前解决方案尚未使用此符号，但是它客观存在。 |
| `true`  |        | 表示无条件编译此段代码。                                     |
| `false` |        | 表示无条件忽略此段代码。                                     |

## UI 特性专用符号

部分符号只用于测试新功能，作为增量内容呈现出来。这些符号只在 UI 窗体项目（以及引用的 UI 类库项目）里才会使用和启用。这部分的条件编译符号一般都带有“FEATURE\_”或“AUTHOR_FEATURE\_”的前缀，意味着这些部分的内容都是作者保留的功能和特性。目前尚未包含这样的符号。

| 符号                          | 默认值 | 含义                               |
| ----------------------------- | ------ | ---------------------------------- |
| `DISABLE_XAML_GENERATED_MAIN` | 有     | 表示是否禁用生成器自动生成主方法。 |

## 其它符号

这些符号是 .NET 库文件和官方库 API 提供和预带的条件编译符号，它们多用于区分项目所使用的库 API 的版本。它们多以“NET”开头。

| 符号名                      | 使用范围               | 含义                                                      |
| --------------------------- | ---------------------- | --------------------------------------------------------- |
| `NETSTANDARD2_1_OR_GREATER` | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET Standard 2.1 以及以上版本。 |
| `NET5_0_OR_GREATER`         | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET 5 以及以上版本。            |
| `NET7_0_OR_GREATER`         | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET 7 以及以上版本。            |
| `NETCOREAPP3_0`             | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET Core 3.0 版本。             |
| `NETCOREAPP3_1`             | 源代码生成器的生成代码 | 表示当前 .NET 框架是否为 .NET Core 3.1 版本。             |

[^1]: “符号名”表示该条件编译符号在代码里使用的标识符名称。
[^2]: 在代码实现里，`StringHandler` 为了减少内存分配以提升性能，这里故意提供了这个符号处理。如果配置了该符号，那么在初始化的时候，直接将计算得到的结果作为内存分配的大小；否则会按照“按 2 的次幂向上取整”的约定将计算得到的结果进行处理，然后将处理后的结果作为分配大小。

