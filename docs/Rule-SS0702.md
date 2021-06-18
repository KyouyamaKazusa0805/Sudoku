## 基本信息

**错误编号**：`SS0702`

**错误叙述**：

* **中文**：可简化的空值传播 `??` 表达式。
* **英文**：The expression can be simplified to using null-coalescing expression `??`.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 6 里发明了一种运算符，叫做空值传播运算符。写法是 `a ?? b`，用法等价于 `a is null ? b : a`。

```csharp
int? p = 30;
int q = 40;

//               ↓ SS0702.
int? r = p is null ? q : p;
//       ~~~~~~~~~~~~~~~~~
```

请改成 `??` 运算符表达式。

```csharp
int? p = 30;
int q = 40;
int? r = p ?? q;
```