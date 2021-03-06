## 基本信息

**错误编号**：`SUDOKU002`

**错误叙述**：

* **中文**：Properties 属性必须是 public 的。
* **英文**：The property 'Properties' must be public.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

在 [`SUDOKU001`](https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU001?sort_id=3599824) 里我们说到，一个 `StepSearcher` 类必须拥有 `Properties` 属性。如果访问修饰符不正确，依然不可以。实际上，访问修饰符必须是 `public`，其它的都不行。

```csharp
public static TechniqueProperties Properties { get; } // OK.
static public TechniqueProperties Properties { get; } // OK.
internal static TechniqueProperties Properties { get; } // Wrong.
protected static TechniqueProperties Properties { get; } // Wrong.
protected internal static TechniqueProperties Properties { get; } // Wrong.
private protected static TechniqueProperties Properties { get; } // Wrong.
private static TechniqueProperties Properties { get; } // Wrong.
static TechniqueProperties Properties { get; } // Wrong, because the default accessibility is 'private'.
```

可以从例子看出，实际上只有前两种写法是允许的。