`AnalysisResult` 记录是专门表达一个题目在解题后的结果信息的类型。这个记录类型里带有很多必需属性，和一些可选属性。在实例化的时候，可以自己选择进行添加。

```csharp
var analysisResult = new AnalysisResult(SolverName, grid, false, stopwatch.Elapsed)
{
    Steps = steps,
    StepGrids = stepGrids
};
```



## `ToString` 方法需要的格式化字符串

你可以使用如下的格式化字符串进行输出。

| 格式化字符 | 意义                                                         |
| ---------- | ------------------------------------------------------------ |
| `'-'`      | 输出内容包含分隔符，将每一个子部分分隔开，看起来更清晰。     |
| `'#'`      | 输出内容包括每一个步骤的顺序编号。                           |
| `'@'`      | 输出内容不显示推导逻辑，即仅包括步骤的名称部分和删数（或出数）部分。 |
| `'?'`      | 在解题过程列表的最后添加输出卡点步骤和对应的顺序编号。如果没有打开 `'#'` 选项，则不会输出步骤的对应编号。 |
| `'!'`      | 此选项将展示每一个 `ShowDifficulty` 属性值为 `true` 的难度系数值；如果不是 `true`，则不会输出难度系数值，但步骤依然会输出。 |
| `'.'`      | 输出将不显示卡点后的步骤，取而代之的是 6 个小数点 `......`。 卡点指的是本题目里最后一个引出出数步骤的步骤。当该步骤完成后，后续的所有步骤，直至完成题目都不再含有比排除和唯一余数外的其余任何技巧。 |
| `'a'`      | 所有的题目的特性都会显示在这里。                             |
| `'b'`      | 所有的填数后门都会显示在这里。                               |
| `'d'`      | 所有的技巧分类的详情信息会被显示出来。比如，这个技巧名的所有技巧的最小值、难度系数总和将显示在该技巧统计信息的对应左侧。 |
| `'l'`      | 展示所有技巧信息。                                           |

使用方式为 `analysisResult.ToString(格式化字符串)`。

## `AnalysisResultFormattingOptions` 枚举

输出的时候，可能你不一定能够记得所有的格式化字符串。此时我们需要使用枚举来搞定这一点。

| 枚举值                     | 格式化字符串 |
| -------------------------- | ------------ |
| `ShowSeparators`           | `'-'`        |
| `ShowStepLabel`            | `'#'`        |
| `ShowSimple`               | `'@'`        |
| `ShowBottleneck`           | `'?'`        |
| `ShowDifficulty`           | `'!'`        |
| `ShowStepsAfterBottleneck` | `'.'`        |
| `ShowAttributes`           | `'a'`        |
| `ShowBackdoors`            | `'b'`        |
| `ShowStepDetail`           | `'d'`        |
| `ShowSteps`                | `'l'`        |

当我们使用的时候，和格式化字符串类似，叠加枚举数值即可：

```csharp
string result = analysisResult.ToString(
    AnalysisResultFormattingOptions.ShowSeparators |
    AnalysisResultFormattingOptions.ShowStepLabel |
    AnalysisResultFormattingOptions.ShowDifficulty
);
```

