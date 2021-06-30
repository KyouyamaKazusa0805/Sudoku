# SS0102
## 基本信息

**错误编号**：`SS0102`

**错误叙述**：

* **中文**：没有必要的内插字符串前导符 '$'。
* **英文**：Unnecessary interpolation leading character '$'.

**级别**：编译器警告

**类型**：使用（Usage）

**警告等级**：1

## 描述

如果一个字符串里不包含任何的内插部分，那么字符串就不必使用内插前导符号 `$`。

```csharp
Console.WriteLine($"Hello, sunnie."); // SS0102.
```