# How to use `AnalysisResult`

标题：**如何使用 `AnalysisResult`**



## Format string in `ToString`

标题：**`ToString` 方法需要的格式化字符串**

You can use all format string below to print the result.

你可以使用如下的格式化字符串进行输出。

| Format characters<br/>格式化字符 | Meaning<br/>意义                                             |
| -------------------------------- | ------------------------------------------------------------ |
| `'s'`                            | All logic will not be displayed. In other words, the output result will only contain the name of the technique and its eliminations (or assignments).<br/>输出内容不显示推导逻辑，即仅包括步骤的名称部分和删数（或出数）部分。 |
| `'m'`                            | All technique steps after the bottleneck will not be displayed, and they will be replaced by an ellipsis `...`.<br/>Bottlenecks are the last steps that follow all single technique steps. When this step will be applied to the grid, all steps after this one will contain only Hidden Singles and Naked Singles.<br/>输出将不显示卡点后的步骤，取而代之的是三个小数点 `...`。<br/>卡点指的是本题目里最后一个引出出数步骤的步骤。当该步骤完成后，后续的所有步骤，直至完成题目都不再含有比排除和唯一余数外的其余任何技巧。 |

The usage is `analysisResult.ToString(formatString)`.

使用方式为 `analysisResult.ToString(格式化字符串)`。