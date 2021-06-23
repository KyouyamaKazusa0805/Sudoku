## 基本信息

**错误编号**：`SS0409`

**错误叙述**：

* **中文**：`[ProxyEquality]` 特性所标记的方法只能在当前类型里有一个，才能在源代码生成器里生效。
* **英文**：The source generator will be well-working until the number of methods marked `[ProxyEquality]` should be only 1.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

`[ProxyEquality]` 标记的方法是用来判断相等性的，如果有多个方法都这么标记了的话，那么方法就会影响分析器生成代码，而后面多出来的标记的方法都是无效的。

```csharp
partial class MyClass
{
    [ProxyEquality]
    public static bool Equals1(MyClass left, MyClass right) => false;

    [ProxyEquality]
    public static bool Euqals2(MyClass left, MyClass right) => true;
}
```

比如这样的代码，类型 `MyClass` 包含两个不同的比较方法，还都标记了这个特性。这样就会触发 SS0409 警告。
