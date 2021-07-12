# SS9010
## 基本信息

**错误编号**：`SS9010`

**错误叙述**：

* **中文**：该方法具有重载。这意味着 `dynamic` 对象传入参数后无法区分到底调用具体哪个方法。请指定强制转换。
* **英文**：This method contains overloads, which means you should specify the explicit cast from `dynamic` to the target type to avoid the compiler not recognize which method it calls.

**级别**：编译器警告

**警告级别**：1

**类型**：设计（Design）

## 描述

C# 的 `dynamic` 确实非常好用，但有时候因为它自身的特殊情况，在使用过程之中会导致有些麻烦的、无法立刻发现的错误问题。其中一个能够避免的问题就是重载方法传参的问题。

如果一个方法自身包含多个重载，且这些参数表列的对应位置上是不同的数据类型，剩下的参数都是一样的数据类型，比如这样四个方法：

* `F(int, double)`
* `F(double, double)`
* `F(decimal, double)`
* `F(char, double)`

由于传入了一个 `dynamic` 对象到第一个参数上，编译器是无法区分到底应该调用哪个方法的。因此，编译器会给出提示，希望你在这里追加一个强制转换，以指定调用的重载方法是哪一个。

```csharp
dynamic d = 30; // Even though we can see the type of 'd' is int, but compiler doesn't know.

f(d, 30.0); // Wrong here.

void f(int a, double b) { }
void f(double a, double b) { }
void f(decimal a, double b) { }
void f(char a, double b) { }
```

编译器希望你指定强制转换：

```csharp
f((double)d, 30.0);
```

这样就知道到底调用的是哪个方法了。
