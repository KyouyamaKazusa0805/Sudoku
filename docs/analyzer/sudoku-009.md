## 基本信息

**错误编号**：`SUDOKU009`

**错误叙述**：

* **中文**：无法在资源字典里找到键为 '{0}' 的资源。
* **英文**：The specified key can't be found in the resource dictionary.

**级别**：编译器错误

**类型**：资源字典（ResourceDictionary)

## 描述

资源字典里的数据在读取的时候显得很重要。如果资源字典本身读取的数据无法找到的话，程序就会在运行时产生异常。为了避免这一点，我们必须在编译阶段就防止此问题的发生。

```csharp
string text = TextResources.Current.Hello; // Wrong.
```

比如前文这个例子里，`Hello` 资源无法在资源字典里找到（确实没有这个字段的数据），所以编译器必须在这里给出 `SUDOKU008` 编译器错误。