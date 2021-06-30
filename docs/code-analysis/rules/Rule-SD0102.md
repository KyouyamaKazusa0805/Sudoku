# SD0102
## 基本信息

**错误编号**：`SD0102`

**错误叙述**：

* **中文**：`Properties` 属性必须是 `public` 的。
* **英文**：The property `Properties` must be `public`.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

在 [`SD0101`](Rule-SD0101) 里我们说到，一个 `StepSearcher` 类必须拥有 `Properties` 属性。如果访问修饰符不正确，依然不可以。实际上，访问修饰符必须是 `public`，其它的都不行。

正确示范：

```csharp
public static TechniqueProperties Properties { get; }
static public TechniqueProperties Properties { get; }
```

错误示范：

```csharp
internal static TechniqueProperties Properties { get; }
protected static TechniqueProperties Properties { get; }
protected internal static TechniqueProperties Properties { get; }
private protected static TechniqueProperties Properties { get; }
private static TechniqueProperties Properties { get; }
static TechniqueProperties Properties { get; } // Because the default accessibility is 'private'.
```