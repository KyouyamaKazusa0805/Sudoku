## 基本信息

**错误编号**：`SS0411`

**错误叙述**：

* **中文**：`[Regex]` 特性所标记的字段只能是 `string` 类型。
* **英文**：The field should be a string when marked `[Regex]`.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

`[Regex]` 特性标记的字段必须是 `string` 类型才能参与代码执行。

```csharp
public static class MyClass
{
    [Regex]
    public const int Number = 100;
}
```

