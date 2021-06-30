# SD0107
## 基本信息

**错误编号**：`SD0107`

**错误叙述**：

* **中文**：Properties 属性需要有默认值。
* **英文**：The property 'Properties' must contain an initializer.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

既然 `Properties` 属性限制很多（必须只能有 getter，必须是 `TechniqueProperties` 类型，必须不带可空标记 `?` 等等），那么默认数值必须存在，才能使得数值可以合理取出，毕竟是一个引用类型。

此时，我们必须要为对象赋一个默认数值才是正确的使用。

```csharp
public static TechniqueProperties Properties { get; }
```

尝试改成类似下面的这样：

```csharp
public static TechniqueProperties Properties { get; } = new(nameof(Technique.XWing), 32); // OK.
```