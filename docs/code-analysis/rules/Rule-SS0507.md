# SS0507
## 基本信息

**错误编号**：`SS0507`

**错误叙述**：

* **中文**：解构函数的参数应对应到对象本身的某个实例字段或实例属性上去。
* **英文**：The parameter should be corresponded to a certain instance field or instance property in this type.

**级别**：编译器信息

**类型**：使用（Usage）

## 描述

C# 7 诞生了解构函数，但按照解构函数的规范，我们应一一将参数对应到对象本身的某个字段或属性上去。如果对应不上，那么分析器就会产生错误。

```csharp
public void Deconstruct(out int age) // SS0507.
{
    // ...
}
```

假设这个类型没有 `_age` 或者 `Age` 成员的话，分析器就会对此产生一个信息提示这一点。消除这个信息的唯一办法是为这个类型也添加一个 `age` 匹配的字段 `_age` 或属性 `Age`，再在代码里对此进行赋值。