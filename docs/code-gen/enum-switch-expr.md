# `EnumSwitchExpressionGenerator` 源代码生成器

## 基本介绍

**功能**：生成枚举类型各个字段路由操作的扩展方法。

**类型名**：[`EnumSwitchExpressionGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/EnumSwitchExpressionGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

对于值类型而言，有些时候我们会需要使用 `switch` 表达式来进行处理和反馈各个字段的对应数值。例如该项目里的对称类型：

```csharp
[Flags]
public enum SymmetryType : byte
{
	None = 0,
	Central = 1,
	Diagonal = 1 << 1,
	AntiDiagonal = 1 << 2,
	XAxis = 1 << 3,
	YAxis = 1 << 4,
	AxisBoth = 1 << 5,
	DiagonalBoth = 1 << 6,
	All = 1 << 7
}
```

该类型提供的是一种对称规则。如果我们想要按照各个字段显示对应的字符串表达形式的话，我们可以使用 `EnumSwitchExpressionRootAttribute` 和 `EnumSwitchExpressionArmAttribute` 特性来完成。首先我们需要给类型标记 `EnumSwitchExpressionRootAttribute` 特性，用来表示生成的这个返回的 `switch` 表达式应该用什么方法名字。源代码生成器会生成这样的代码，是以扩展方法的方式呈现的，因此会使用扩展方法来包裹生成的 `switch` 表达式的代码，而这个方法的名字就是我这里所说的需要写进去的名字。接着，我们需要给每一个枚举类型的字段写上对应的数值，用来返回。

该特性在同一个枚举字段里是可以重复使用的，因此我们还需要给出对应的对应方法名称。有些时候我们不一定只对某个枚举类型生成一个这样的方法，因此我们允许该特性多次使用，只要给出的生成的方法名称不一样就 OK。

```csharp
[Flags]
[EnumSwitchExpressionRoot("GetName")]
public enum SymmetryType : byte
{
	[EnumSwitchExpressionArm("GetName", "No symmetry")]
	None = 0,
	[EnumSwitchExpressionArm("GetName", "Central symmetry type")]
	Central = 1,
	[EnumSwitchExpressionArm("GetName", "Diagonal symmetry type")]
	Diagonal = 1 << 1,
	[EnumSwitchExpressionArm("GetName", "Anti-diagonal symmetry type")]
	AntiDiagonal = 1 << 2,
	[EnumSwitchExpressionArm("GetName", "X-axis symmetry type")]
	XAxis = 1 << 3,
	[EnumSwitchExpressionArm("GetName", "Y-axis symmetry type")]
	YAxis = 1 << 4,
	[EnumSwitchExpressionArm("GetName", "Both X-axis and Y-axis")]
	AxisBoth = 1 << 5,
	[EnumSwitchExpressionArm("GetName", "Both diagonal and anti-diagonal")]
	DiagonalBoth = 1 << 6,
	[EnumSwitchExpressionArm("GetName", "All symmetry type")]
	All = 1 << 7
}
```

这样使用。然后，源代码生成器会自动对该类型生成对应的方法：

```csharp
[CompilerGenerated]
[GeneratedCode("Sudoku.Diagnostics.CodeGen.Generators.EnumSwitchExpressionGenerator", "0.11")]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static string GetName(this SymmetryType @this)
    => @this switch
    {
        SymmetryType.None => "No symmetry",
        SymmetryType.Central => "Central symmetry type",
        SymmetryType.Diagonal => "Diagonal symmetry type",
        SymmetryType.AntiDiagonal => "Anti-diagonal symmetry type",
        SymmetryType.XAxis => "X-axis symmetry type",
        SymmetryType.YAxis => "Y-axis symmetry type",
        SymmetryType.AxisBoth => "Both X-axis and Y-axis",
        SymmetryType.DiagonalBoth => "Both diagonal and anti-diagonal",
        SymmetryType.All => "All symmetry type",
        _ => throw new ArgumentOutOfRangeException(nameof(@this))
    };
```

### `MethodDescription` 属性

该属性为可选属性。赋值表达的是该生成的方法的方法介绍文字。该文字会被注入到方法的文档注释之中。

### `ThisParameterDescription` 属性

该属性表达的是该生成方法里的 `@this` 参数的介绍文字。该文字也会被注入到方法的文档注释里。

### `ReturnValueDescription` 属性

该属性表达的是方法的返回值的介绍文字。该文字也会被注入到方法的文档注释里。

### `NotDefinedBehavior` 属性

该属性是枚举类型的数值，表示的是对于如果没有标记 `EnumSwitchExpressionArmAttribute` 的字段，生成了方法会自动路由到默认情况上（即 `switch` 表达式里的弃元符号 `_` 对应的返回表达式）到底返回什么。该属性包含两种可能：

1. 默认返回对象的对应整数数值的字符串表示；
2. 抛 `ArgumentOutOfRangeException` 异常。（默认值）

如果不设置该属性的话，默认会抛出异常，就像前文的代码里一样；如果设置为前者的话，那么生成的代码会变为 `@this.ToString()`，而不是 `throw 异常` 的表达式了。
