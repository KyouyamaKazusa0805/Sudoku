# SS0616
## 基本信息

**错误编号**：`SS0616`

**错误叙述**：

* **中文**：可空引用类型请使用 `{ }` 或 `not null` 模式匹配语法来表达判断类型是否可空。
* **英文**：Please use `{ }` or `not null` instead of `!= null` in nullable reference types.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

因为运算符是可以重载的，所以如果我们一旦没有处理可空的情况，就会造成程序抛出 `NullReferenceException` 以及类似的异常。为了避免这点，C# 使用 `is null` 和 `is not null` 来代替它们。

```csharp
//     ↓ SS0616.
if (o == null)
//  ~~~~~~~~~
{
    // ...
}
```

请改成 `is not { }` 或 `is null` 语法来取消该信息提示。

```csharp
if (o is null)
{
    // ...
}
```