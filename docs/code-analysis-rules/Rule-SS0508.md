## 基本信息

**错误编号**：`SS0508`

**错误叙述**：

* **中文**：此类型已经具有同样参数个数的解构函数，此解构函数将不发生效。
* **英文**：The type has already contained a deconstruction method that holds the same number of parameters of this method, so this method won't work.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 7 诞生了解构函数，但是解构函数因为语言设计规则约定，因此我们无法定义同样长度参数的、同类型的解构函数，否则在解构函数使用的时候将会无法定位。举个例子，假设我有一个类型 `S` 包含一个成员 `a` 和 `b` 的解构函数，另外一个包含成员 `c` 和 `d` 的解构函数。我们经常会书写如下的代码：

```csharp
var (a, b) = instance;
```

可是这样写出来，我们就无法断定到底解构函数应该调用哪个。因此，编译器会产生错误信息告知你无法这么使用。

为了避免这样的书写问题，我们提前对这样的解构函数声明作出分析，避免使用出问题。

```csharp
public void Deconstruct(out string name, out int age)
{
    name = _name;
    age = _age;
}

public void Deconstruct(out string name, out Gender gender) // SS0508.
{
    name = _name;
    gender = _gender;
}
```