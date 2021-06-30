# `unmanaged` 泛型约束
C# 的泛型的底层算是 C++ 模板和 Java 类型擦除的泛型的折中处理机制，它比 Java 的类型擦除要复杂，但比 C++ 模板要简单。C# 泛型是支持指针的，但指针并不安全，所以 C# 7 之前都不允许我们直接对泛型类型处理和操作指针。

C# 7 诞生了一种机制，允许我们通过 `unmanaged` 约束来保证一个类型可直接使用指针。

## 引例

假设我们写一个对普通变量的交换函数。

```csharp
public static void Swap<T>(ref T left, ref T right) where T : struct
{
    var temp = left;
    left = right;
    right = temp;
}
```

这么做没有问题，但问题在于不够通用。如果结构体里包含引用类型字段的话，交换了也是没有完全交换成功的。所以，我们干脆限制成“非托管类型”，而不是 `struct` 约束，即值类型。

非托管类型是什么？非托管类型就是那些可以用指针处理的类型。只要类型本身只包含非 `object` 和 `string` 这两个引用类型以外的剩余的所有内置类型作为数据成员（字段或属性），那么这样的类型实例化后，赋值就会全数据成员挨个赋值，两个对象完全独立了。

这样的类型称为非托管类型。但是，我们知道我们无法使用泛型处理，因为这样的非托管类型无法使用任何之前学到过的泛型约束来完成。所以，`unmanaged` 约束就诞生了。

我们使用 `unmanaged` 约束约束类型即可。

```csharp
public static unsafe void Swap<T>(T* left, T* right) where T : unmanaged
{
    var temp = *left;
    *left = *right;
    *right = temp;
}
```

我们直接使用 `where T : unmanaged` 约束就可以对泛型类型使用指针了。这样的话，交换函数就不必使用 `ref` 了（当然用 `ref` 也可以），然后这里改成指针，C# 就允许了。

## `unmanaged` 约束允许的操作

`unmanaged` 约束允许如下两种操作：

* 指针使用（使用这个泛型类型作为基本类型，创建指针类型对象指向这样的数据类型）；
* `sizeof(T)` 表达式使用。

后面这个 `sizeof(T)` 我们说说是什么东西。C# 的引用类型在内存存储里是相当复杂的，除了从 C/C++ 沿用下来的内存对齐机制，还有一些别的规则，因此往往这样的大小我们是无法直接获取的。但是值类型的特例：非托管类型不同。非托管类型完全不需要 GC 就可以内存分配和释放，它的内存大小就没有那么多复杂的机制。C# 允许使用 `sizeof` 运算符计算一个非托管类型的内存大小，针对于内置类型来说，它们是常量；但对于其它自定义的非托管类型的话，就不是常量了。因此，它需要 `unmanaged` 的约束才可使用，然后底层就可以安全计算到正确的结果。

## `sizeof(T)` 泛型用例

考虑计算更快的 `Enum.HasFlag` 方法的 API。我们如果要自己写一个计算更快的、不需要反射处理的机制完成的方法的话，就需要指针来完成。恰好，`Enum` 是基于整数类型的；而 `Enum` 刚好也确实都是 `unmanaged` 类型的对象，因此可以直接对枚举类型的对象使用这种约束。

```csharp
public static unsafe bool HasFlag<T>(this T @this, T another) where T : unmanaged, Enum
{
    switch (sizeof(T))
    {
        case 1:
        case 2:
        case 4:
            int i = Unsafe.As<T, int>(ref other);
            return (Unsafe.As<T, int>(ref @this) & i) == i;
        case 8:
            long l = Unsafe.As<T, long>(ref other);
            return (Unsafe.As<T, long>(ref @this) & l) == l;
        default:
            throw new ArgumentException(
                "The parameter should be one of the values 1, 2, 4 or 8.",
                nameof(@this)
            );
    }
}
```

甚至你可以使用 `switch` 语句来完成：

```csharp
public static unsafe bool HasFlag<T>(this T @this, T another) where T : unmanaged, Enum =>
    sizeof(T) switch
    {
        1 or 2 or 4 when Unsafe.As<T, int>(ref other) is var i => (Unsafe.As<T, int>(ref @this) & i) == i,
        8 when Unsafe.As<T, long>(ref other) is var l => (Unsafe.As<T, long>(ref @this) & l) == l,
        _ => throw new ArgumentException(
            "The parameter should be one of the values 1, 2, 4 or 8.",
            nameof(@this)
        )
    };
```

很方便，对吧。只需要加一个 `unmanaged` 约束到 `Enum` 约束的后面就可以了。

## 为什么 `Enum` 约束的语义不直接包含 `unmanaged` 呢？

问题问得好。既然所有的枚举类型都是整数类型互相转换，底层也是由整数类型来实现的，它们也都直接就是 `unmanaged` 约束，那么为什么我们还要自己写上 `unmanaged` 约束呢？

这是因为 C# 语法设计上，`Enum` 只是类型的一种约束，也就是说，`Enum` 约束只约束这个对象从 `Enum` 类型派生下来，而并不能自动包含 `unmanaged` 这样的语义，所以我们才需要自己加。

