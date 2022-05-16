# `AutoImplementsDefaultableGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成实现 `IDefaultable<>` 接口的操作。

**类型名**：[`AutoImplementsDefaultableGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoImplementsDefaultableGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

该源代码生成器绑定了特性 `AutoImplementsDefaultableAttribute`，用于生成 `IDefaultable<>` 接口相关的操作。实现的代码目标是暴露一个 `public` 的字段，并按显式接口实现掉 `IDefaultable<>` 给的静态属性 `Default` 以及实例属性 `IsDefault`。

这样的设计是为了故意使得一些不属于数字（即不能派生自 `INumber<>` 接口）的数据类型实现默认化处理，这样可以在将来实现一些操作的时候得到一定程度的实现。

只需要标记该特性上去，并设置一个参数表示生成的、暴露出来的 `public static readonly` 字段的名字。该名字一般可以叫 `Undefined`、`Empty`、`Default`，不过别的名字也可以。

### `DefaultFieldDescription` 属性

该属性表示的是该生成的、暴露出来的字段的介绍文字。该字符串信息会被注入到文档注释里。

### `Pattern` 属性

该属性用于控制的是，这个公有的字段的初始化表达式，它相当于 `public static readonly T 字段 = 数值` 的 `= 数值` 部分。该属性可以不设置数值，默认的话是不初始化，或者相当于 `default(T)`。

### `IsDefaultExpression` 属性

该属性设置和表示 `IsDefault` 属性的实现表达式。该属性可以不设置数值，默认和 `this == 公有暴露出来的字段` 是等价的，这要求你必须实现 `operator ==` 和 `operator !=` 才可使用；如果你没有这么做的话，也可以自定义比较规则，使用别的办法，比如 `CompareTo` 方法，或者是转换为数字后和 0 进行比较，等等。

## 注意事项

该源代码生成器对属性 `Default` 以及 `IsDefault` 均为显式接口实现，是为了避免冲突。该设置是无法修改的。
