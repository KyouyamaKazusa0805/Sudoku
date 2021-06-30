# SS9004
## 基本信息

**错误编号**：`SS9004`

**错误叙述**：

* **中文**：记录类包含递归成员，调用自动生成的 `ToString` 方法将导致栈溢出。
* **英文**：Due to recursive member in the `record` type, invoking synthesized method `ToString` will cause stack overflowing.

**级别**：编译器错误

**类型**：设计（Design）

## 描述

C# 9 的记录类型有一个缺陷（但也是没有办法的设计）：想象一下链表这样的递归数据结构，它的数据成员可能会包含自己这个类型的成员。这样的话，如果我们要想按照记录类型的语法设计规则来输出字符串的话，是会递归执行的。由于递归无法终止的关系，递归数据结构的 `ToString` 必然会导致栈溢出的错误。

```csharp
record C
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Z { get; init; }

    public C Inner { get; set; }
}
```

由于 `C` 类型包含 `Inner` 成员，它还是这个类型的成员，因此直接调用 `ToString` 方法就会导致栈溢出。

```csharp
var c = new C();
c.Inner = c;

//                     ↓ SS9004.
Console.WriteLine(c.ToString());
//                ~~~~~~~~~~~~
```

如果需要解决这个问题，需要自己声明 `ToString` 方法来替换递归输出的基本行为操作。分析器会检测递归成员，并检测是否包含自定义的 `ToString` 方法。如果没有 `ToString` 但包含递归成员，则会产生错误。

不过稍微注意一下的是，省略 `ToString` 的调用，在 `Console.WriteLine` 的行为里也会调用一次 `ToString`。但是如果改写成 `c` 而不是 `c.ToString` 的话，分析器由于不一定知道是否方法内部会执行 `ToString` 而不会对此产生报错信息。