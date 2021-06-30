# SS0603
## 基本信息

**错误编号**：`SS0603`

**错误叙述**：

* **中文**：可使用 `is not null` 模式匹配判断。
* **英文**：The pattern can be simplified to `is not null`.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

在模式匹配里，如果我们书写成 `is object` 的话，我们会建议你改成 `is not null`。

```csharp
object o = 3;

//         ↓ SS0603.
if (o is object)
//       ~~~~~~
{
    // ...
}
```

请改写成 `is not null` 以消除分析器对此的诊断结果。

```csharp
object o = 3;

if (o is not null)
{
    // ...
}
```

你也可以改成 `!(o is null)` 这种写法。不过我们不建议这么写，因为 C# 9 有取反模式，所以不必使用小括号表达。实际上，.NET 自带的分析器可以对这种带小括号的手动取反模式匹配作出分析和诊断，因此在这个分析器里，我们没有对这种手动取反的模式匹配进行分析和诊断。