# SS0602
## 基本信息

**错误编号**：`SS0602`

**错误叙述**：

* **中文**：可使用 `is null` 模式匹配判断。
* **英文**：The pattern can be simplified to `is null`.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

在模式匹配里，如果书写成 `is not object` 或者是 `!(a is object)` 的话，我们会建议你改成 `is null`。

```csharp
object o = 3;

//        ↓ SS0602.
if (!(o is object))
//  ~~~~~~~~~~~~~~
{
    // ...
}
```

请使用 `is null` 模式匹配以消除分析器诊断结果。

```csharp
object o = 3;

if (o is null)
{
    // ...
}
```