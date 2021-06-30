# SS0614
## 基本信息

**错误编号**：`SS0614`

**错误叙述**：

* **中文**：属性模式的空大括号模式 `{ }` 仅对引用类型和内联变量的声明模式有匹配含义，因此不建议对不可空类型使用空大括号模式判断；请改成 `var` 模式或别的模式，或删除这个子句。
* **英文**：The empty-brace property pattern `{ }` will take effects only in reference types or value types with variable designations, so we don't suggest you use the pattern to judge non-nullable types; please change the clause to `var` pattern or other valid patterns, or just remove it.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 8 的空大括号模式匹配的是可空类型里不为 `null` 的对象。但是，如果对整数这样的不可空类型使用空大括号匹配的话，是没有意义的。

假设我们有如下的记录结构类型。

```csharp
record struct R(int A, double B, float C, string D);
```

假设我们使用空大括号的属性模式判断：

```csharp
var o = new R(1, 2D, 3F, "4");

//        ↓ SS0614.
if (o is { })
//       ~~~
{
    Console.WriteLine(o);
}
```

因为 `o` 此时是记录结构类型，所以是不可能为 `null` 的情况。因此这样的书写格式等价于 `o is not null`，是显然没有判定意义的。

请删除这段子句以取消信息。

```csharp
var o = new R(1, 2D, 3F, "4");

Console.WriteLine(o);
```