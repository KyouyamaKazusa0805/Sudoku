## 基本信息

**错误编号**：`SD0310`

**错误叙述**：

* **中文**：请为 `SudokuGrid` 调用 `ToString` 方法时指定格式化字符串，例如默认情况的 `"."` 或者智能处理的 `"#"`。
* **英文**：Please add the format string as the argument into the method invocation of `SudokuGrid`, such as `"."` for default case, or `"#"` for intelligent-handling case.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

`SudokuGrid` 有非常丰富的格式化字符串，可对对象提供固定模式的字符串格式化处理。如果不对 `ToString` 方法调用的时候传入格式化字符串的参数的话，就不一定非常清晰。分析器建议你把 `"0"`、`"."` 或者 `"#"` 这样的格式化字符串加上去。

```csharp
Console.WriteLine(grid.ToString()); // Wrong.
```

消除编译器信息的方法是，追加格式化字符串。

```csharp
Console.WriteLine(grid.ToString("#"));
```

## 补充说明

顺带一提，`new Cells(Span<int>)`、`new Candidates(Span<int>)`、`new Cells(ReadOnlySpan<int>)` 和 `new Candidates(ReadOnlySpan<int>)` 四个初始化器提供出来，是为了分片用的。当有些时候，我们把分片后的 `Span<int>` 或者 `ReadOnlySpan<int>` 当参数传入的话，就无法改写成初始化器了。
