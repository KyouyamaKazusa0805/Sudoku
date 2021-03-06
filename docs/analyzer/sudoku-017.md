## 基本信息

**错误编号**：`SUDOKU017`

**错误叙述**：

* **中文**：请不要将 '{0}..ctor(void*, int)' 的结果作为方法的返回值。
* **英文**：The result of the expression '{0}..ctor(void*, int)' can't be the return value as any methods.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

C# 7 引入了 `Span<T>` 和 `ReadOnlySpan<T>` 这两个数据类型。由于这两个数据类型是 `ref struct`，因此只能作为栈内存存储的数据信息。不过，C# 的设计团队考虑到了作为返回值的这一点，因此，`ref struct` 结果是可以被允许作为某个方法的返回值的，这样的话，这个数据的返回值将被放在调用方的栈帧上。

问题来了。`Span<T>` 和 `ReadOnlySpan<T>` 的实现非常特殊，即使我们传入的是一个 `void*` 作为参数，但这块连续的内存空间是作为地址直接赋值到 `Span<T>` 和 `ReadOnlySpan<T>` 的底层的，因此，我们不可以这么做——因为返回到调用方的时候，被调用方栈帧内存已经被清除，所以 `void*` 此时就变成了垂悬指针。

如下的例子，将展示出一个隐藏的 `SUDOKU017` 错误：

```csharp
using System.Numerics; // BitOperations.

public static unsafe ReadOnlySpan<int> GetAllSets(byte val)
{
    if (val == 0)
    {
        return ReadOnlySpan<int>.Empty;
    }

    int length = BitOperations.PopCount(val);
    int* ptrArrResult = stackalloc int[length];
    for (byte i = 0, p = 0; i < 8; i++, val >>= 1)
    {
        if ((val & 1) != 0)
        {
            ptrArrResult[p++] = i;
        }
    }

    return new(ptrArrResult, length); // SUDOKU017 raised.
}
```

> 如果不知道引用结构（`ref struct`）是什么的话，请参考 Wiki 页面“[引用结构](https://gitee.com/SunnieShine/Sudoku/wikis/%E5%BC%95%E7%94%A8%E7%BB%93%E6%9E%84?sort_id=3476777)”。