## 基本信息

**错误编号**：`SD0203`

**错误叙述**：

* **中文**：Current 对象调用的方法 '{0}' 的参数必须是 '{1}' 个，而调用时有 '{2}' 个。
* **英文**：The number of arguments mismatched in this dynamically invocation.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

`Current` 可以调用如下的三个方法：

* `Current.Serialize`；
* `Current.Deserialize`；
* `Current.ChangeLanguage`。

显然，三个方法调用的参数必须匹配才能参与执行。如果参数个数不匹配，则会产生该编译器错误。

```csharp
Current.ChangeLanguage();
```

请添加对应的合适的参数。

```csharp
Current.ChangeLanguage(CountryCode.EnUs); // OK.
```