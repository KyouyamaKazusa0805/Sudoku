## 基本信息

**错误编号**：`SD0312`

**错误叙述**：

* **中文**：`ValueStringBuilder` 的 `Span<char>` 参数，请使用 `stackalloc` 语句表达。
* **英文**：Please use `stackalloc` clause to create a buffer of type `Span<char>` as the argument of the constructor of type `ValueStringBuilder`.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

值类型的字符串构造类型 `ValueStringBuilder` 是一个比 `StringBuilder` 更高效且近乎零 GC 的数据类型，用于字符串的拼接和构建。它有一个构造器是传入一个 `Span<char>` 类型的缓冲内存块，但这里我们一般建议调用使用 `stackalloc` 语句来表达。如果传入别的 `Span<char>` 类型的对象的话，因为长度不好确定而导致编译器无法分析，就非常不方便。

```csharp
var vsb = new ValueStringBuilder(new char[50]); // Wrong.
```

请直接改用 `stackalloc` 语句。`new` 语句大家其实都知道，它会自动生成 50 个字节的空间用来表示缓冲内存块，一维数组是直接可以转换为 `Span<T>` 和 `ReadOnlySpan<T>` 类型的，但也没有 `stackalloc` 写法更合适。C# 8 的“nested `stackalloc`”特性发明出来就是为了表示这一点的。

