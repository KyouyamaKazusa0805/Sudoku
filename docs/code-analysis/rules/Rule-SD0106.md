# SD0106
## 基本信息

**错误编号**：`SD0106`

**错误叙述**：

* **中文**：`Properties` 属性不可为 `null`。
* **英文**：The property `Properties` can't be `null`.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

`Properties` 属性的类型是引用类型，但我们不允许实现的时候添加可空标记 `?`，因为必须是有数值的，这样反射才能取到。

```csharp
public static TechniqueProperties? Properties { get; }
public static TechniqueProperties Properties { get; } // OK.
```

## 备注

可空标记需要在 C# 8 的基础上使用。

## 参考

[可空引用类型](Nullable-Reference-Types)（项目 Wiki）