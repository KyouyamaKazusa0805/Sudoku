## 基本信息

**错误编号**：`SS0612`

**错误叙述**：

* **中文**：使用 `var` 引出的解构模式，参数均为弃元符号是没有意义的。
* **英文**：It doesn't take any effects that all sub-patterns use discards in a `var` deconstruction pattern.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

和 [`SS0611`](Rule-SS0611) 编译器警告一样的道理，如果我们对解构模式的所有部分全部使用弃元符号的话，就没有解构意义了。

```csharp
readonly struct R
{
    private readonly int _a, _b;
    
    public R(int a, int b) { _a = a; _b = b; }
    
    public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
}
```

那么这样的代码就会产生错误。

```csharp
var r = new R(1, 2);

//            ↓ SS0612.
if (r is var (_, _))
//       ~~~~~~~~~~
{
    Console.WriteLine(r.ToString());
}
```