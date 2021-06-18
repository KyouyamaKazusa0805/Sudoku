## 基本信息

**错误编号**：`SS0628`

**错误叙述**：

* **中文**：长度模式的数值不可为负数。
* **英文**：Length pattern can't match a negative value as the length.

**级别**：编译器警告

**警告等级**：1

**类型**：设计（Design）

## 描述

C# 10 提供了长度模式，但长度模式里使用负数，是不合法的。

```csharp
int[] s = ...;

//        ↓ SS0628.
if (s is [-7])
//        ~~
    // ...
```