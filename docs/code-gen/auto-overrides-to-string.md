# `AutoOverridesToStringGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成 `ToString` 方法的重写代码。

**类型名**：[`AutoOverridesToStringGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoOverridesToStringGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

该源代码生成器依赖于特性 `AutoOverridesToStringAttribute`。`ToString` 方法生成的代码模板，默认是和记录类型和记录结构类型对应的 `PrintMembers` 方法相同的执行过程，即模式为 `"类型名称 { 成员名 = 值, ... }"` 的字符串。

如果你需要重写掉它，按照这个形式的话，可以使用此特性标记到类型上去即可。需要传入的参数是字段、属性或无参非 `void` 返回值类型的方法名称。

```csharp
[AutoOverridesToString(nameof(Name), nameof(Age), nameof(_isBoy))]
partial class Student
{
}
```

### `Pattern` 属性

如果你不想使用这样的默认输出格式的话，可以使用该属性来自定义显示的字符串的格式。该属性需要传入一个字符串，该字符串满足 C# 的基本语法的代码片段。另外，也提供如下的调用简化记号：

* 星号 `*`：表示 `ToString` 方法的调用；
* 中括号索引运算 `[索引]`：表示在特性实例化传参的整个成员名序列里的第几个成员。
* 大括号 `{表达式}`：表示该表达式在输出显示字符串期间，是内插的部分。

比如说，`{((char)[0]).*}` 表达式就表示，将实例化该特性的参数序列里的第一个成员取出，然后强制转换为 `char` 类型后，调用 `ToString` 方法，比如第一个参数是一个 `byte` 类型的对象（假设叫 `_char`），那么该表达式等价于内插字符串的表达式 `$"{((char)_char).ToString()}"`，即 `"((char)_char).ToString()"`。

## 注意事项

不必多说。至少你的中括号索引运算不能超过成员总数量吧。对吧。
