# SS0506
## 基本信息

**错误编号**：`SS0506`

**错误叙述**：

* **中文**：解构函数的赋值语句不是单纯的变量赋值，而是表达式赋值。
* **英文**：The assignment statement isn't a simple variable one, but an expression.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

C# 7 诞生了解构函数，为了规范化处理里面的代码过程，我们建议在任何的解构函数赋值过程都是一一对应到变量上，而不是表达式赋值过程。

```csharp
public void Deconstruct(out int age) => age = 100; // SS0506.
```

比如这里的 `age` 输出参数，用了一个常量接收该数值。按解构函数的规范来说，我们应该是取出这个类型的 `Age` 属性或 `_age` 字段的数值，作为赋值方。

请改成类似如下这样的赋值方式，可以消除分析器对此的分析。

```csharp
public void Deconstruct(out int age) => age = _age;
```

> 赋值语句也可以是 `age = Age`，即用属性赋值。不过这里的 `_age` 或者 `Age` 必须存在才行。