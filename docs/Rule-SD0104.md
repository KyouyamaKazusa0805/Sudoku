## 基本信息

**错误编号**：`SD0104`

**错误叙述**：

* **中文**：`Properties` 属性必须是只读的。
* **英文**：The property `Properties` must be read-only.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

[`SD0101`](Rule-SD0101) 错误告诉我们，必须要拥有 `Properties` 属性。如果属性存在，但不是只读的，即可修改，则会产生该编译器错误。


正确示范：

```csharp
public static TechniqueProperties Properties { get; } // OK.
```

错误示范：

```csharp
public static TechniqueProperties Properties { get; set; }
public static TechniqueProperties Properties { get; init; }
public static TechniqueProperties Properties { set; }
public static TechniqueProperties Properties { init; }
```