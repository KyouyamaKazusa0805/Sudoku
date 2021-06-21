## 基本信息

**错误编号**：`SS0302`

**错误叙述**：

* **中文**：可使用 `Any()` 无参方法调用代替 `Count() != 0`。
* **英文**：The expression `Count() != 0` can be replaced by parameterless extension method invocation `Any()`.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 的 `Count()` 会计算序列长度，但如果列表元素过多，使用 `Count()` 会损失性能。如果判断是否为 0 的话，请使用 `Any` 方法。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

if (arr.Count() != 0) // SS0302.
{
    // ...
}
```

请改成 `Any()` 或者别的非 LINQ 的即取属性（比如 `Length` 之类的东西，如果有的话）来代替。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

if (arr.Any())
{
    // ...
}
```

另外，这种写法可能包含取反的可能。比如 `Count() == 0` 可转换为 `!Any()`。
