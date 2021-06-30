# SS9001
## 基本信息

**错误编号**：`SS9001`

**错误叙述**：

* **中文**：可前置的 `for` 循环迭代条件表达式。
* **英文**：Available prepositional iteration condition expression.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

有些时候，`for` 循环的迭代条件会使用表达式来表现。此时，我们可提供优化处理，将表达式前置，避免每次迭代的时候都计算一次表达式。

```csharp
//                       ↓ SS9001.
for (int i = 0; i < arr.Length; i++)
//                  ~~~~~~~~~~
{
    Console.WriteLine(arr[i]);
}
```

请将其修改成这样以通过编译器检查：

```csharp
for (int i = 0, length = arr.Length; i < length; i++)
{
    Console.WriteLine(arr[i]);
}
```