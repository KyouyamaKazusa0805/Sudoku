## 基本信息

**错误编号**：`SUDOKU016`

**错误叙述**：

* **中文**：请为内插部分 '{0}' 补充 'ToString' 方法以避免装箱、拆箱操作。
* **英文**：Please add 'ToString' method invocation to the interpolation part in order to prevent any box and unbox operations.

**级别**：编译器警告

**类型**：性能（Performance）

**警告级别**：1

## 描述

C# 6 引入了内插字符串的概念，这使得代码写起来比单纯的字符串加法更加容易和可读。但是，由于内插字符串的实现是使用 `string.Format` 方法，而后面的参数均是 `object` 类型而不是泛型参数，因此在传参的时候，都会使得值类型隐式转换的装箱操作。

为了避免问题产生，请添加 `ToString` 方法来避免装箱，因为 `ToString` 一般是由用户自己实现，或由 C# 语言提供的 `ToString` 自动实现的字符串输出，因而可以不用装箱就可以输出数据。

```csharp
int val = 30;
Console.WriteLine($"The temperature is {val} Celsius degrees now."); // SUDOKU016 raised.
```

这个例子里，请将 `val` 改成 `val.ToString()` 来避免编译器对此的侦测。