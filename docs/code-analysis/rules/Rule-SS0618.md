# SS0618
## 基本信息

**错误编号**：`SS0618`

**错误叙述**：

* **中文**：`not null` 在模式匹配里是冗余判断，请删除它。
* **英文**：Pattern `not null` is redundant; please remove it.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

如果一个对象需要使用 `a is not null and ...` 这样的判断的话，因为 `is` 一定会作一次隐式 `null` 的可空性判断，因此我们根本不必写出来。

```csharp
object? a = 30;

//             ↓ SS0618.
if (a is not null and > 10)
//       ~~~~~~~~~~~~
{
    // ...
}
```

显然，因为 `a` 在模式匹配里使用了 `not null` 和 `> 10` 两个模式匹配语句。但是 `a` 即使为 `null`，这样的判别也是不必写的，因为 `is` 会帮我们隐式完成判断 `null` 的过程，避免程序出现异常。因此，我们不必写出来。

改法很简单，删除 `not null` 模式，和 `and` 关键字即可。

```csharp
if (a is > 10)
{
    // ...
}
```