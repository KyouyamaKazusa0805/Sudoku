## 基本信息

**错误编号**：`SS0613`

**错误叙述**：

* **中文**：属性模式的弃元符号没有意义，请删除此判断子句。
* **英文**：The discard pattern in this property pattern doesn't judge anything. Please remove this clause.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 8 的属性模式可以允许我们递归完成判断，所以属性模式也称递归模式。不过，因为属性模式判断下，我们对某个属性使用弃元模式匹配的话，是没有任何意义的，因此分析器会建议你删除这个子句。

```csharp
record R(int A, double B, float C, string D);
```

假设我们使用属性模式判断：

```csharp
object o = new R(1, 2D, 3F, "4");

//                  ↓ SS0613.    ↓ SS0613.
if (o is R { A: 1, B: _, C: 3F, D: _ })
//                 ~~~~         ~~~~
{
    Console.WriteLine(o);
}
```

请删除这段子句以取消信息。

```csharp
object o = new R(1, 2D, 3F, "4");

if (o is R { A: 1, C: 3F })
{
    Console.WriteLine(o);
}
```