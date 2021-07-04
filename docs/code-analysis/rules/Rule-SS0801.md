# SS0801
## 基本信息

**错误编号**：`SS0801`

**错误叙述**：

* **中文**：内联方法需要代码里不包含循环和异常处理语句。
* **英文**：The method can't be inlined due to contained loop and exception-handling statements.

**级别**：编译器警告

**警告级别**：1

**类型**：使用（Usage）

## 描述

C# 的 JIT 在底层内联方法的时候，要求方法成员的代码里不包含循环和异常处理语句，当然代码也不能太多。分析器会提前对这段代码作出分析，如果方法里包含循环语句或者异常处理语句的话，这个方法标记了 `[MethodImpl(MethodImplOptions.AggressiveInlining)]` 也是无效的。

```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static int Method()
{
    int result = 0;
    for (int i = 0; i < 100; i++)
        result += i;
    
    return result;
}
```

