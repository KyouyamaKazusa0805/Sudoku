## 基本信息

**错误编号**：`SS0636`

**错误叙述**：

* **中文**：如果对位子模式里使用了非弃元的模式，那么建议把对应的参数名写出增强可读性。
* **英文**：We suggest you explicitly specify the parameter name to enhance the readability when the sub-pattern in the positional pattern has used the non-discard one.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

如题，为了增强可读性，建议写出来参数名。

```csharp
record struct Person(string Name, int Age, Gender Gender);
```

如果有如下代码：

```csharp
//                        ↓ SS0636.
if (person is (_, _, Gender.Male))
//                   ~~~~~~~~~~~
{
    // ...
}
```

分析器将建议你添加上参数名。

```csharp
if (person is (_, _, Gender: Gender.Male))
{
    // ...
}
```