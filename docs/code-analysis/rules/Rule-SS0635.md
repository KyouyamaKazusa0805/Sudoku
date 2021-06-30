# SS0635
## 基本信息

**错误编号**：`SS0635`

**错误叙述**：

* **中文**：使用弃元模式的对位子模式没有必要不写出对应名。
* **英文**：It's unnecessary to explicitly specify positional parameter in this sub-positional pattern that the pattern part is used a discard pattern.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 7 诞生了对位模式，但对位模式需要使用弃元占位表达一些不需要的对位子模式的参数信息。如果子模式的模式部分用的是下划线（即弃元模式）的话，那么名字写出来都没有意义。

```csharp
readonly struct S
{
    private readonly int _a, _b, _c, _d;
    
    public S(int a, int b, int c, int d) { _a = a; _b = b; _c = c; _d = d; }
    
    public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
}
```

如果有如下代码：

```csharp
//               ↓ SS0635.
if (s is (a: 30, b: _))
//               ~
{
    Console.WriteLine(nameof(s));
}
```

分析器将建议你删除名字。

```csharp
if (s is (a: 30, _))
{
    Console.WriteLine(nameof(s));
}
```