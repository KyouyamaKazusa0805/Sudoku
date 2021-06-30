# SD0314
## 基本信息

**错误编号**：`SD0314`

**错误叙述**：

* **中文**：在调用了 `ValueStringBuilder.ToString` 方法后，就不能再继续使用该变量了，因为变量已经被隐式地销毁了。
* **英文**：You can't invoke any operations after called `ValueStringBuilder.ToString` method because this object has been already disposed implicitly.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

在 `ValueStringBuilder.ToString` 方法的底层，会自动调用 `ValueStringBuilder.Dispose` 方法，销毁对象（比如从 `ArrayPool<char>.Shared` 里把租借的内存空间还回去啊、`Span<char>` 内存释放之类的）。正是因为如此，我们无法再次继续对已经销毁的对象继续使用，此时会引发严重的内存泄露的 bug。

```csharp
Console.WriteLine(vsb.ToString()); // Here we suppose 'vsb' is of type 'ValueStringBuilder'.

vsb.Append('!'); // Wrong.
```

在第一行里我们已经调用了 `vsb.ToString()` 方法后，就不能继续使用 `vsb` 变量了，因为它的内存已经被销毁或部分销毁。

