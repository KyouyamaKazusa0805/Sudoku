# SD0315
## 基本信息

**错误编号**：`SD0315`

**错误叙述**：

* **中文**：不要对 `ValueStringBuilder` 对象的实例使用 `using` 声明。
* **英文**：Don't use `using` statement onto the instance of type `ValueStringBuilder`.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

这是因为 `ValueStringBuilder` 数据类型本身的机制的问题。确实在底层 `ValueStringBuilder` 类型实现了 `Dispose` 方法可以释放内存，不过因为它是受到自动调用，因此强烈不建议手动调用，比如 `using` 声明。

```csharp
using var sb = new ValueStringBuilder(stackalloc char[10]);

...;
```

这样就是一个典型的错误使用。
