# SD0407
## 基本信息

**错误编号**：`SS0407`

**错误叙述**：

* **中文**：`[ProxyEquality]` 特性所标记的方法需要是返回 `bool` 类型的方法，才能在源代码生成器里生效。
* **英文**：The source generator will be well-working until the method marked `[ProxyEquality]` should return `bool`.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

`[ProxyEquality]` 标记的方法需要返回 `bool` 类型。如果返回类型不正确的话，那么比较就不可能成功，因此必须返回 `bool` 类型。

```csharp
[ProxyEquality]
public static int Equals(in SudokuGrid left, in SudokuGrid right)
{
    return 42;
}
```

这样的代码就不行。
