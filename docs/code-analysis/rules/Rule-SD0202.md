# SD0202
## 基本信息

**错误编号**：`SD0202`

**错误叙述**：

* **中文**：`Current` 对象只能调用 `Deserialize`、`Serialize` 和 `ChangeLanguage` 的其一。
* **英文**：The specified method can't be found and called.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

由于 `Current` 字段本身的特殊性，我们无法在编译期验证对象调用的成员是否存在和正常，所以这个检查就显得很重要。如果 `Current` 调用的方法不正确的话，就会产生编译器错误。

正确示范：

```csharp
Current.ChangeLanguage(param);
Current.Serialize(param1, param2);
Current.Deserialize(param1, param2);
```

错误示范：

```csharp
Current.GreetWith(param);
Current.DoSomething(param);
```