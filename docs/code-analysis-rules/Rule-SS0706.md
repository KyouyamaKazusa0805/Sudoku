## 基本信息

**错误编号**：`SS0706`

**错误叙述**：

* **中文**：如果使用 `object` 的等号和不等号比较的话，项目使用的是 `ReferenceEquals`。请使用 `ReferenceEquals` 方法来进行。
* **英文**：If you want to check the whether two references from two objects are equal, please use `ReferenceEquals` instead of the operators `==` or `!=` .

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

这是一个习惯。因为等号和不等号本身很难确定运算重载是否比较引用，因此如果刻意判断引用的话，可以使用 `ReferenceEquals` 代替。

```csharp
object o = new(), p = new();

if (o == p) // Here.
{
    // ...
}
```

请改成 `ReferenceEquals`。

```csharp
object o = new(), p = new();

if (ReferenceEquals(o, p))
{
    // ...
}
```