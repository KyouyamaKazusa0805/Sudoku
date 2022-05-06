# `AutoBePinnableGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成固定的固定对象方法 `GetPinnableReference` 的正确代码。

**类型名**：[`AutoBePinnableGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoBePinnableGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

假如我有一个数据类型包含一个定长的缓冲区字段：

```csharp
class ExampleType
{
    private fixed short _maskList[100];
}
```

我们想让该类型的实例使用 `fixed` 固定语句，早期的语法是做不到的。到 C# 7 开始拥有自定义的语义规则，才开始允许。方法是对该类型添加一个无参、返回引用的 `GetPinnableReference` 方法：

```csharp
class ExampleType
{
    private fixed short _maskList[100];

    public ref readonly short GetPinnableReference() => ref _maskList[0];
}
```

这样就可以了：

```csharp
var instance = new ExampleType();
fixed (short* ptr = instance)
{
    // ...
}
```

现在，我们仅需追加一个特性即可完成该任务。做法是在类型上添加 `AutoBePinnableAttribute` 特性，填上 `fixed` 变量的类型，以及固定返回的表达式，最后追加 `partial` 关键字到类型上，就可以了：

```csharp
[AutoBePinnable(typeof(short), "_maskList[0]")]
class ExampleType
{
    private fixed short _maskList[100];
}
```

### `ReturnsReadOnlyReference` 属性

该属性控制返回值类型是 `ref readonly` 还是 `ref`。如果设置为 `true`，那么源代码生成器会自动生成 `ref readonly` 修饰的返回值类型；否则只会带有 `ref` 修饰符。该属性的默认数值为 `true`。
