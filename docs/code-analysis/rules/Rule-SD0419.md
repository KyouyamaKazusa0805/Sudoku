# SD0419
## 基本信息

**错误编号**：`SS0419`

**错误叙述**：

* **中文**：调用 `[MaybeNullWhenNotDefined]` 的方法未验证返回值是否为 `null`。
* **英文**：Please check the nullability of the return value from the method whose return value is marked `[MaybeNullWhenNotDefined]`.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

如果一个方法的返回值被标记上了 `[MaybeNullWhenNotDefined]` 的话，那么这个返回值就和参数绑定起来了。如果超出访问返回的话，那么返回值就可能为 `null`。

```csharp
[return: MaybeNullWhenNotDefined("pinnedItem")]
public readonly ref readonly short GetPinnableReference(PinnedItem pinnedItem) =>
    ref pinnedItem == PinnedItem.Masks
    ? ref _grid[0]
    : ref pinnedItem == PinnedItem.CandidateMasks
        ? ref _initGrid[0]
        : ref *(short*)null;
```

比如这样的代码。如果调用此代码的话，只要参数超出 `PinnedItem` 的枚举范围的话，那么返回值可能为 `null`。

调用方：

```csharp
fixed (short* pGrid = grid, p = &grid.GetPinnableReference(PinnedItem.InitialGrid))
{
    Unsafe.CopyBlock(pGrid, p, sizeof(short) * 81);
}
```

这样的代码没有问题，但如果参数超出访问范围：

```csharp
fixed (short* pGrid = grid, p = &grid.GetPinnableReference((PinnedItem)2))
{
    Unsafe.CopyBlock(pGrid, p, sizeof(short) * 81);
}
```

`(PinnedItem)2` 表达式超出了范围，此时会在这里 `p` 变量使用的时候进行编译器报错。

