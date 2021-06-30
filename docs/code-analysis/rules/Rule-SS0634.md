# SS0634
## 基本信息

**错误编号**：`SS0634`

**错误叙述**：

* **中文**：可使用空属性模式简化判断。
* **英文**：The expression can be simplified to use empty-brace property pattern.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

如果模式使用 `is object o` 的语法的话，可简化成 `is { } o`。因为 `is object` 等价于判断 `is not null`；而 `is not null` 外带变量声明的话，可使用 `{ }` 空属性模式判断。

```csharp
object o = 100;

if (o is object variable)
    // ...
```

分析器建议你改成 `{ } variable`。

```csharp
object o = 100;

if (o is { } variable)
    // ...
```

## 备注

如下的模式匹配都建议转换为 `{ } variable`：

* `o is object variable`
* `o is not null and var variable`
* `o is not null && o is var variable`

其中第三种因为自带的 IDE0078 诊断提供了修补工具，会自动转换为第二种类型，因此我们不对最后这种实现单独的语义检测分析。