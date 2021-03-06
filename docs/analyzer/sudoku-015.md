## 基本信息

**错误编号**：`SUDOKU015`

**错误叙述**：

* **中文**：为了完整书写函数指针的签名，请添加 'managed' 关键字。
* **英文**：For more readability and completeness, please add the keyword 'managed' into the function pointer type.

**级别**：编译器警告

**类型**：风格（Style）

**警告等级**：1

## 描述

函数指针是 C# 9 出来的一种新的概念，可以允许我们直接操作底层的 `calli` 和 `ldftn` 指令来获取函数的地址。函数指针允许 `unmanaged` 和 `managed` 两种，前者用于和 C/C++ 交互，并带有比如 `Cdecl` 这样的函数调用转换模式。当如果没有任何的调用转换的时候，默认就是 `managed` 的函数指针，并用于专门指向 C# 里定义的静态函数。

该项将建议你补充 `managed` 关键字，为了声明格式的完整性和可读性，就好像是在声明类型、方法的时候，都建议用于自动补充访问修饰符一样。

```csharp
private readonly Cells[] GetMap(delegate*<in SudokuGrid, int, int, bool> predicate)
{
    var result = new Cells[9];
    for (int digit = 0; digit < 9; digit++)
    {
        ref var map = ref result[digit];
        for (int cell = 0; cell < 81; cell++)
        {
            if (predicate(this, cell, digit))
            {
                map.AddAnyway(cell);
            }
        }
    }

    return result;
}
```

例如代码里第一行，`delegate*<in SudokuGrid, int, int, bool>` 将建议你改成 `delegate* managed<in SudokuGrid, int, int, bool>`。