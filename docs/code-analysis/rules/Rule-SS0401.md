## 基本信息

**错误编号**：`SS0401`

**错误叙述**：

* **中文**：无法使用运算符 {0} 因为这个类型是一个闭合类型。
* **英文**：Can't apply the operator here because the type is closed `enum`.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

我们定义一个闭合的枚举类型，只能使用比较运算符，其它任何的运算符均不可使用，包括强制转换。

```csharp
[Closed]
enum TestEnum { A, B, C }

for (var f = TestEnum.A; f < TestEnum.C; f++) // SS0401.
{
    Console.WriteLine(f);
}
```