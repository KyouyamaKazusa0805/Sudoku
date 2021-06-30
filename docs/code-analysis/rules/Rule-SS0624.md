# SS0624
## 基本信息

**错误编号**：`SS0624`

**错误叙述**：

* **中文**：可使用扩展属性模式简化属性模式的多层级大括号。
* **英文**：Available simplification for extended property patterns.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

当属性模式需要递归书写的时候，每一个属性之间都使用逗号隔开。但是，如果每次递归书写的时候，属性模式的大括号里都只有一个成员的话，那么就可以使用扩展属性模式简化调用和代码书写。

假设我们拥有一个 `Person` 记录类，包含如下信息：

```csharp
record class Person(string Name, int Age, Gender Gender, Person Father, Person Mother);
```

那么，因为递归的关系，调用期间可能会出现递归的属性模式书写。

```csharp
if (
    zhangSan is
    {
        Name: "Zhang San",
        Age: 24,
//                   ↓ SS0624.
        Father: { Name: "Zhang 'er" },
//      ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//                 ↓ SS0624.
        Mother: { Name: "Li si" }
//      ~~~~~~~~~~~~~~~~~~~~~~~~~
    }
)
{
    Console.WriteLine("Zhang san does satisfy that condition.");
}
```

扩展属性模式将建议你改成这样：

```csharp
if (
    zhangSan is
    {
        Name: "Zhang San",
        Age: 24,
        Father.Name: "Zhang 'er",
        Mother.Name: "Li si"
    }
)
{
    Console.WriteLine("Zhang san does satisfy that condition.");
}
```