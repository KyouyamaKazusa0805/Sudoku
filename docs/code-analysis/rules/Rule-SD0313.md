# SD0313
## 基本信息

**错误编号**：`SD0313`

**错误叙述**：

* **中文**：缓冲区长度过长。请改用传入 `int` 类型数据作参数的构造器。
* **英文**：The length of the buffer is too long; please use another constructor passing an `int` value.

**级别**：编译器警告

**警告级别**：1

**类型**：性能（Performance）

## 描述

一般来说缓冲区长度小于 300（大概）的话，用 `stackalloc` 来表达（调用 `Span<char>` 的那个构造器）比较合适；但是如果长度超过 300，缓冲区过大的话就不太建议了（有些时候如果栈内存小一点就会导致栈溢出的错误，毕竟还有别的栈帧，又不是只有这里 `stackalloc` 才使用栈内存）。

```csharp
var vsb = new ValueStringBuilder(stackalloc char[500]); // Wrong.
```

此时请建议使用另外一个构造器，去掉 `stackalloc` 部分：

```csharp
var vsb = new ValueStringBuidler(500);
```

这个传入 `int` 类型数据作为参数的构造器，在底层会去使用共享的数组内存空间，即 `ArrayPool<char>.Shared` 的数据，它会从数组共享缓冲区里“租借”500个字符空间的缓冲区，这样不会有额外的内存分配的同时，也能有额外的空间可用于字符串的拼接和导出，因此性能就有了提升。

## 参考

* [`ArrayPool<T>.Shared` 属性](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1.shared)

