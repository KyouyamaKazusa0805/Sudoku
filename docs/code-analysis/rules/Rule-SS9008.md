## 基本信息

**错误编号**：`SS9008`

**错误叙述**：

* **中文**：请为只读属性追加 `readonly` 修饰符。
* **英文**：Please append keyword `readonly` into the read-only properties.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 8 里的只读成员尚未对一个直接返回一个数值的属性（即只读属性）自动标记 `readonly` 修饰符。但是这样的过程是纯（Pure）的，因为它只会执行产生结果，而不会因为返回数据而修改变动数据，因此建议增加 `readonly` 修饰符到这个属性上。

```csharp
struct Sample
{
    int a;

    public int A => a; // SS9008.
}
```

比如这里的 `a`，虽然 `a` 字段本身可变，但在返回过程里，它是不可变的，所以可以对属性追加标记。

## 备注

另外，唯一一种属性返回结果不能追加 `readonly` 修饰符的情况是，它返回了一个非只读的方法（比如当前结构里没有标注 `readonly` 的方法，或者是别的类型的方法）。因为这样的方法我们无法保证方法是否修改对象，因此这样的返回不能加 `readonly` 修饰符到属性上，比如 `public int Prop => Method();` 里的 `Method` 方法如果是非 `readonly` 方法的话，那么就不能加。

