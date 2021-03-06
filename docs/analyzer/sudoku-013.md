## 基本信息

**错误编号**：`SUDOKU013`

**错误叙述**：

* **中文**：Current 对象调用的方法 '{0}' 不返回值，而你将其当成右值表达式。
* **英文**：The method returns void, but you make it an rvalue expression.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

`Current` 可以调用如下的三个方法：

* `Current.Serialize`；
* `Current.Deserialize`；
* `Current.ChangeLanguage`。

显然，三个方法返回值匹配才能参与执行。比如 `ChangeLanguage` 是 `void` 返回值的方法，因此我们不可将其作为右值表达式写在赋值号的右侧。

```csharp
Current.ChangeLanguage(CountryCode.EnUs); // OK.
_ = Current.ChangeLanguage(CountryCode.EnUs); // Wrong.
SomeMethod(Current.ChangeLanguage(CountryCode.EnUs)); // Wrong.
```