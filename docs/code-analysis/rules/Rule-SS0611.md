## 基本信息

**错误编号**：`SS0611`

**错误叙述**：

* **中文**：`var` 模式下使用弃元符号将没有任何匹配效果。
* **英文**：The discard in the `var` pattern may not take any effects.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 7 的 `var` 模式匹配如果后面紧跟 `_` 的话，是没有意义的。唯一使用 `var _` 组合有意义的地方只有 `out` 参数接收变量的地方，但这种写法也可以使用 `out _` 而不是 `out var _`，因此 `var _` 组合是没有意义的。

顺带一提，就算 `out var _` 是有实际意义的，但 `out var _` 里 `var _` 也并不是 `var` 模式，而只是一个普通的变量声明而已。

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

//         ↓ SS0611.
if (r is var _)
//       ~~~~~
{
    Console.WriteLine(r.ToString());
}
```