## 基本信息

**错误编号**：`SUDOKU007`

**错误叙述**：

* **中文**：Properties 属性需要有默认值。
* **英文**：The property 'Properties' must contain an initializer.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

既然 `Properties` 属性限制很多（必须只能有 getter，必须是 `TechniqueProperties` 类型，必须不带可空标记 `?` 等等），那么默认数值必须存在，才能使得数值可以合理取出，毕竟是一个引用类型。

此时，我们必须要为对象赋一个默认数值才是正确的使用。

```csharp
public static TechniqueProperties Properties { get; } // Wrong.
public static TechniqueProperties Properties { get; } = new(nameof(Technique.XWing), 32); // No SUDOKU006.
```