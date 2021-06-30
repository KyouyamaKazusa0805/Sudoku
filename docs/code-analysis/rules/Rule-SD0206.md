# SD0206
## 基本信息

**错误编号**：`SD0206`

**错误叙述**：

* **中文**：无法动态调用 '{0}' 的方法，因为它不存在。
* **英文**：The specified dynamic invocation method doesn't exist.

**级别**：编译器错误

**类型**：资源字典（ResourceDictionary)

## 描述

资源字典里的数据在读取的时候显得很重要。如果资源字典本身读取的数据无法找到的话，程序就会在运行时产生异常。为了避免这一点，我们必须在编译阶段就防止此问题的发生。

```csharp
string text = TextResources.Current.MethodThatDoesNotExist();
```

比如前文这个例子里，`MethodThatDoesNotExist` 资源无法在资源字典里找到（确实没有这个字段的数据），所以编译器必须在这里给出 `SD0206` 编译器错误。