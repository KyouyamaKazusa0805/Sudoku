# SS0610
## 基本信息

**错误编号**：`SS0610`

**错误叙述**：

* **中文**：啥都没判断的对位模式。如果你想要判断对象是否不为 `null`，请使用空属性模式 `{ }` 或 `not null` 模式代替。
* **英文**：The positional pattern judges nothing. If you want to judge whether the object is not `null`, please use empty property pattern `{ }` or `not null` instead.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了对位模式，但如果解构函数调用后，所有元素全都不判断的话，我们不如使用空属性模式判断。

```csharp
readonly struct S
{
    private readonly int _a, _b, _c, _d;
    
    public S(int a, int b, int c, int d) { _a = a; _b = b; _c = c; _d = d; }
    
    public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
}
```

假设我们使用对位模式调用第一个解构函数，但所有 `out` 参数都没有使用，此时我们将建议你改成 `{ }` 或 `not null` 来代替。

```csharp
//          ↓ SS0610.
if (s is (_, _))
//       ~~~~~~
{
    Console.WriteLine(nameof(s));
}
```