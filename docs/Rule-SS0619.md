## 基本信息

**错误编号**：`SS0619`

**错误叙述**：

* **中文**：`null` 常量模式和 `or` 组合在模式匹配里是冗余判断，请删除它。
* **英文**：Combination constant pattern `null` and the keyword `or` is redundant; please remove it.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

如果一个对象需要使用 `a is null or ...` 这样的判断的话，因为 `is` 一定会作一次隐式 `null` 的可空性判断，因此我们根本不必写出来；为了简化代码，我们需要使用“双重否定”的书写模式，来避免 `null or` 的组合。

```csharp
List<int>? list = null;

// ...

//             ↓ SS0619.
if (list is null or { Count: 3 })
//          ~~~~~~~
{
    // ...
}
```

显然，因为 `a` 在模式匹配里使用了 `null or` 组合，但显然是没有必要写 `null` 的，因此我们需要改写成“双重否定”的书写规则来避免使用 `null or` 组合。

```csharp
if (list is not { Count: not 3 })
{
    // ...
}
```

双重否定的书写规则比较难理解，你可以先看后面的模式 `{ Count: not 3 }`，然后再把这个模式取反。比如这个例子下，判断模式是 `{ Count: not 3 }`，这表示判断的 `Count` 属性不能是 3；然后再看 `is not` 组合，这表示匹配项目只要不让 `{ Count: 3 }` 这个模式成立的所有情况就行。所以，这包含了如下两种情况：

* 对象自身就是 `null`；
* 对象自身不是 `null`，但它的 `Count` 属性不是 3。

因此，这种写法已经包含了 `null or` 的匹配组合的匹配含义。