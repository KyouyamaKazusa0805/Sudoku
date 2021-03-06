## 基本信息

**错误编号**：`SUDOKU012`

**错误叙述**：

* **中文**：Current 对象调用的方法 '{0}' 的参数 '{1}' 必须是 '{2}' 类型，而实际传入的是 '{3}' 类型。
* **英文**：The argument type dismatched in this dynamically invocation.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

`Current` 可以调用如下的三个方法：

* `Current.Serialize`；
* `Current.Deserialize`；
* `Current.ChangeLanguage`。

显然，三个方法调用的参数必须匹配才能参与执行。如果参数类型不匹配，则会产生该编译器错误。

```csharp
Current.ChangeLanguage(CountryCode.EnUs); // OK.
Current.ChangeLanguage("Test"); // Wrong.
```

另外，注入式分析器甚至不允许你传入 `ref` 和 `out` 关键字，毕竟本来就不需要它们。
```csharp
var someValue = CountryCode.EnUs;
Current.ChangeLanguage(ref someValue); // Still wrong.
```