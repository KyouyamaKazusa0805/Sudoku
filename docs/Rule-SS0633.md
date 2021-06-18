## 基本信息

**错误编号**：`SS0633`

**错误叙述**：

* **中文**：模式匹配的 `case var _` 和 `default` 等价。请改成 `default`。
* **英文**：`case var _` label in a `switch` expression (or statement) is equivalent to the default case (`default:` in `switch` statement and `_ =>` in `switch` expression); try to use `default:` or `_ =>` instead.

**级别**：编译器警告

**类型**：设计（Design）

## 描述

模式匹配里可包含 `case var _` 这样的模式参与匹配。但这个写法和 `default` 可以说是没有区别，因为 `var _` 不参与类型判断，也不直接转换变量，所以和 `default` 默认情况是一样的。为了帮助编译器规范化分析，我们不建议写 `case var _:` 或 `var _ =>` 这样的东西，而是使用 `default:` 或 `_ =>`。

```csharp
object o = 100;

switch (o)
{
    ...

    case var _: // Wrong.
        Console.WriteLine(o);
}
```

分析器建议你改成 `default:`。

```csharp
object o = 100;

switch (o)
{
    ...

    default:
        Console.WriteLine(o);
}
```

需要注意的是，`switch` 分两种用法，在 `switch` 语句下，需要替换为 `default`；但在 `switch` 表达式里，会建议你改成 `_ =>`。

## 备注

要说 `default` 标签和 `var _` 模式匹配作为标签出现，它们的区别的话，其实它们是有区别的。`default:` 标签一定在最后一个执行，不管你写在 `switch` 最前面还是最后面，它都是会把别的情况都判断了、且不满足的时候，才会走 `default`；而 `case var _:` 标签是写在哪里，哪里后面的条件都无法匹配，因为 `case var _` 是永远成功匹配的一种模式匹配，它完全不判断任何情况，直接使得匹配成功。

但是问题在于，如果你把 `case var _:` 标签放在别的标签的前面的话，因为别的标签都无法得到执行和判断，因此编译器会直接给出 [CS8120](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/switch#case-labels) 的编译器错误，导致你无法编译程序，所以 `case var _` 你还只能放在末尾。而且，你都放在末尾了，就说明前面的判别条件全部失配才会到达这里，那这不是就是跟 `default:` 一样的嘛。所以，即使它们用法有区别，但仍然在使用的时候等价。