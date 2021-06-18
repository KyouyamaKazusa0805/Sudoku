## 基本信息

**错误编号**：`SD0108`

**错误叙述**：

* **中文**：Properties 属性只能以 new 语句进行初始化。
* **英文**：The property 'Properties' must be initialized by a new clause.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

由于算法的限制和规范性，我们必须要求你使用显式或隐式对象生成语句 `new` 来初始化该属性：

正确示范：

```csharp
public static TechniqueProperties Properties { get; } = new(nameof(Technique.XWing), 32); 
public static TechniqueProperties Properties { get; } = new TechniqueProperties(nameof(Technique.XWing), 32);
```

错误示范：

```csharp
public static TechniqueProperties Properties { get; } = null!;
public static TechniqueProperties Properties { get; } = AnotherStaticProperty;
```