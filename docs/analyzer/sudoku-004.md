## 基本信息

**错误编号**：`SUDOKU004`

**错误叙述**：

* **中文**：Properties 属性必须是只读的。
* **英文**：The property 'Properties' must be read-only.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

[`SUDOKU001`](https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU001?sort_id=3599824) 错误告诉我们，必须要拥有 `Properties` 属性。如果属性存在，但不是只读的，即可修改，则会产生该编译器错误。

```csharp
public static TechniqueProperties Properties { get; } // OK.
public static TechniqueProperties Properties { get; set; } // Wrong.
public static TechniqueProperties Properties { get; init; } // Wrong.
public static TechniqueProperties Properties { set; } // Wrong.
public static TechniqueProperties Properties { init; } // Wrong.
```