## 基本信息

**错误编号**：`SS0609`

**错误叙述**：

* **中文**：对位模式判断冗余或冲突。
* **英文**：The positional sub-pattern is redundant or contradictory.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了对位模式，但很多时候，对位模式判断是没有意义的。比如现在有一个 `S` 结构。

```csharp
readonly struct S
{
    private readonly int _a, _b, _c, _d;
    
    public S(int a, int b, int c, int d) { _a = a; _b = b; _c = c; _d = d; }
    
    public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
    public void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }
    public void Deconstruct(out int a, out int b, out int c, out int d) { a = _a; b = _b; c = _c; d = _d; }
}
```

显然，对位模式调用第一个解构函数和第二个解构函数都会解构同样的变量。如果我们使用 `and` 或 `or` 连接对位模式，并判断同一个信息的话，那么判断的信息是没有意义的。

```csharp
//                            ↓ SS0609.
if (s is (a: 10, b: _) and (a: 10, b: 30, c: 50))
//                          ~~~~~
{
    Console.WriteLine(nameof(s));
}
```

如果两个判断的成员对象是同一个，而且对象判别的数值不一致的时候，我们也会给出错误信息。

```csharp
//                            ↓ SS0609.
if (s is (a: 10, b: _) and (a: 100, b: 30, c: 50))
//                          ~~~~~~
{
    Console.WriteLine(nameof(s));
}
```