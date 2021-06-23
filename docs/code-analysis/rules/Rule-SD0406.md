## 基本信息

**错误编号**：`SS0406`

**错误叙述**：

* **中文**：`[ProxyEquality]` 特性所标记的方法需要是静态方法，才能在源代码生成器里生效。
* **英文**：The source generator will be well-working until the method marked `[ProxyEquality]` is `static`.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

`[ProxyEquality]` 标记的方法需要使用 `类名.方法名` 进行引用，这是源代码生成器里规定的。如果方法本身不是静态的的话，那么源代码生成器就不会对这个类型进行对应的相等性比较方法的生成。

```csharp
[ProxyEquality]
public bool Equals(in SudokuGrid left, in SudokuGrid right)
{
    fixed (short* pThis = left, pOther = right)
    {
        int i = 0;
        for (short* l = pThis, r = pOther; i < Length; i++, l++, r++)
        {
            if (*l != *r)
            {
                return false;
            }
        }

        return true;
    }
}
```

比如这样。这个方法没有 `static` 标记。就会导致分析器给出 SD0406 警告。你需要给方法加上 `static` 修饰符才可以。
