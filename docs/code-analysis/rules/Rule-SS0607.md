## 基本信息

**错误编号**：`SS0607`

**错误叙述**：

* **中文**：可以去掉的对位模式弃元。
* **英文**：This discard can be omitted in the current positional pattern.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 7 诞生了对位模式，可使用解构函数将对象的字段或属性的数值提取出来。

当一个对象使用了对位模式后，对象可使用弃元符号 `_` 来表示这个对应位置上的值是我们不需要的。但如果这个所属类型本身具有多个解构函数，且有更少参数的解构函数作为选择的时候，这个弃元符号是可能可以被省略的。

举个例子。假设我现在有一个 `R` 结构，它里面包含两个解构函数，一个解构函数是获取字段 `_a` 和 `_b` 两个成员；一个解构函数则是解构 `_a`、`_b` 和 `_c` 三个成员。假设满足条件的实现如下：

```csharp
readonly struct S
{
    private readonly int _a, _b, _c;


    public S(int a, int b, int c)
    {
        _a = a;
        _b = b;
        _c = c;
    }


    public void Deconstruct(out int a, out int b)
    {
        a = _a;
        b = _b;
    }

    public void Deconstruct(out int a, out int b, out int c)
    {
        a = _a;
        b = _b;
        c = _c;
    }
}
```

如果我书写对位模式的时候，代码可能会长这样：

```csharp
//                     ↓ SS0607.
if (r is (a: 1, b: 2, c: _))
//                    ~~~~
{
    // ...
}
```

此时，由于我们拥有一个只需要 `_a` 和 `_b` 就可以参与解构的解构函数，因此对字段 `_c` 的解构的弃元符号是没有必要书写的，因此分析器会对这种情况分析并产生诊断信息。你需要做的，就是删除这个子句。

```csharp
if (r is (a: 1, b: 2))
{
    // ...
}
```