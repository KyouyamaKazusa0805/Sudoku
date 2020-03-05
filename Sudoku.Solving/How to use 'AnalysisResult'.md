# How to use `AnalysisResult`

标题：**如何使用 `AnalysisResult`**



## Format string in `ToString`

标题：**`ToString` 方法需要的格式化字符串**

You can use all format string below to print the result.

你可以使用如下的格式化字符串进行输出。

| Format characters<br/>格式化字符 | Meaning<br/>意义                                             |
| -------------------------------- | ------------------------------------------------------------ |
| `'-'`                            | Show separators, which helps us see them more clearly.<br/>输出内容包含分隔符，将每一个子部分分隔开，看起来更清晰。 |
| `'#'`                            | Show the order of each step.<br/>输出内容包括每一个步骤的顺序编号。 |
| `'@'`                            | All logic will not be displayed. In other words, the output result will only contain the name of the technique and its eliminations (or assignments).<br/>输出内容不显示推导逻辑，即仅包括步骤的名称部分和删数（或出数）部分。 |
| `'?'`                            | Show the bottleneck step and its order after the list of solving steps. If the option `'#'` is disabled, the order will not be displayed.<br/>在解题过程列表的最后添加输出卡点步骤和对应的顺序编号。如果没有打开 `'#'` 选项，则不会输出步骤的对应编号。 |
| `'!'`                            | Show the difficulty rating of each step if the property `ShowDifficulty` of the step is `true`.<br/>此选项将展示每一个 `ShowDifficulty` 属性值为 `true` 的难度系数值；如果不是 `true`，则不会输出难度系数值，但步骤依然会输出。 |
| `'.'`                            | All technique steps after the bottleneck will not be displayed, and they will be replaced by an ellipsis `...`.<br/>Bottlenecks are the last steps that follow all single technique steps. When this step will be applied to the grid, all steps after this one will contain only Hidden Singles and Naked Singles.<br/>输出将不显示卡点后的步骤，取而代之的是 6 个小数点 `......`。<br/>卡点指的是本题目里最后一个引出出数步骤的步骤。当该步骤完成后，后续的所有步骤，直至完成题目都不再含有比排除和唯一余数外的其余任何技巧。 |
| `'a'`                            | All attributes of the puzzle will be shown here.<br/>所有的题目的特性都会显示在这里。 |
| `'b'`                            | All assigment backdoors will be shown here.<br/>所有的填数后门都会显示在这里。 |

The usage is `analysisResult.ToString(formatString)`.

使用方式为 `analysisResult.ToString(格式化字符串)`。