# SD0309
## 基本信息

**错误编号**：`SD0309`

**错误叙述**：

* **中文**：`stackalloc` 或 `new` 表达式是不必要的。请使用对象初始化器。
* **英文**：`stackalloc` or `new` expression is unnecessary; Please use object initializer instead.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

`Cells` 和 `Candidates` 结构有一个构造器，允许传入一个 `stackalloc` 表达式。按照 C# 的规则约定，`stackalloc` 表达式允许用于任何地方；而超出范围的其余地方将被认为是一个 `Span<T>` 的表达式。因此，`new Cells(stackalloc int[])` 等价于在调用 `new Cells(Span<int>)` 这个构造器。

但是，如果是平凡的使用的话，我们还是建议你直接改成普通的初始化器，来避免产生不必要的内存分配。

```csharp
var cells = new Cells(stackalloc[] { 1, 2, 3, 4 }); // Wrong.
```

消除编译器信息的方法是，改成初始化器。

```csharp
var cells = new Cells { 1, 2, 3, 4 };
```

## 补充说明

顺带一提，`new Cells(Span<int>)`、`new Candidates(Span<int>)`、`new Cells(ReadOnlySpan<int>)` 和 `new Candidates(ReadOnlySpan<int>)` 四个初始化器提供出来，是为了分片用的。当有些时候，我们把分片后的 `Span<int>` 或者 `ReadOnlySpan<int>` 当参数传入的话，就无法改写成初始化器了。
