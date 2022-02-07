# `Grid` 数据结构

`Grid` 类型是整个解决方案里表示一个数独盘面基本信息的数据结构。它的定义如下：

```csharp
public struct Grid :
    IDefaultable<Grid>,
    ISimpleFormattable,
    ISimpleParseable<Grid>,
    IEquatable<Grid>
#if FEATURE_GENERIC_MATH
    ,
    IEqualityOperators<Grid, Grid>
#if FEATURE_GENEIC_MATH_IN_ARG
    ,
    IValueEqualityOperators<Grid, Grid>
#endif
#endif
{
    public const short DefaultMask = 1023;
    public const short MaxCandidatesMask = 511;
    public const short EmptyMask = 512;
    public const short ModifiableMask = 1024;
    public const short GivenMask = 2048;
    public static readonly string EmptyString;
    public static readonly void* ValueChanged;
    public static readonly void* RefreshingCandidates;
    public static readonly Grid Undefined;
    public static readonly Grid Empty;

    public Grid();
    public Grid(short[] masks);
    public Grid(int[] gridValues, GridCreatingOption creatingOption = GridCreatingOption.None);
    public Grid(int* pGridValues, GridCreatingOption creatingOption = GridCreatingOption.None);
    public Grid(ReadOnlySpan<int> gridValues, GridCreatingOption creatingOption = GridCreatingOption.None);

    public int this[int cell] { readonly get; set; }
    public bool this[int cell, int digit] { readonly get; set; }

    public readonly Grid Solution { get; }
    public readonly Grid ResetGrid { get; }
    public readonly bool IsSolved { get; }
    public readonly bool IsValid { get; }
    public readonly bool IsUndefined { get; }
    public readonly bool IsEmpty { get; }
    public readonly bool IsDebuggerUndefined { get; }
    public readonly int CandidatesCount { get; }
    public readonly int GivensCount { get; }
    public readonly int ModifiablesCount { get; }
    public readonly int EmptiesCount { get; }
    public readonly int NullRegions { get; }
    public readonly Cells GivenCells { get; }
    public readonly Cells ModifiableCells { get; }
    public readonly Cells EmptyCells { get; }
    public readonly Cells BivalueCells { get; }
    public readonly Cells[] ValuesMap { get; }
    public readonly Cells[] DigitsMap { get; }
    public readonly Cells[] CandidatesMap { get; }

    public static bool Equals(in Grid left, in Grid right);
    public static Grid Parse(string str, GridParsingOption gridParsingOption);
    public static Grid Parse(string str, bool compatibleFirst);
    public static Grid Parse(ReadOnlySpan<char> str);
    public static Grid Parse(string str);
    public static bool TryParse(string str, GridParsingOption option, out Grid result);
    public static bool TryParse(string str, out Grid result);
    public readonly CandidateCollectionEnumerator EnumerateCandidates();
    public readonly MaskCollectionEnumerator EnumerateMasks();
    public override bool Equals([NotNullWhen(true)] object? obj);
    public bool Equals(in Grid other);
    public readonly bool? Exists(int candidate);
    public readonly bool? Exists(int cell, int digit);
    public void Fix();
    public readonly short GetCandidates(int cell);
    public readonly CandidateCollectionEnumerator GetEnumerator();
    public readonly override int GetHashCode();
    public readonly short GetMask(int offset);
    public readonly ref readonly short GetPinnableReference();
    public readonly CellStatus GetStatus(int cell);
    public void Reset();
    public void SetMask(int cell, short mask);
    public void SetStatus(int cell, CellStatus status);
    public readonly bool SimplyValidate();
    public readonly int[] ToArray();
    public readonly string ToMaskString();
    public readonly override string ToString();
    public readonly string ToString(string? format);
    public void Unfix();

    public static bool operator ==(in Grid left, in Grid right);
    public static bool operator !=(in Grid left, in Grid right);

    public ref struct MaskCollectionEnumerator
    {
        public readonly ref short Current { get; }

        public readonly MaskCollectionEnumerator GetEnumerator();
        public bool MoveNext();
        public void Reset();
    }
    public ref struct CandidateCollectionEnumerator
    {
        public readonly int Current { get; }

        public readonly CandidateCollectionEnumerator GetEnumerator();
        public bool MoveNext();
        public void Reset();
    }

    // Here we have hidden some inner types or members that is not helpful
    // for the current passage.
}
```

下面我们对这个数据结构进行介绍。

## 实现原理

这个数据结构使用的是 `short` 类型的数字构成的数组，表示的是一个完整的盘面。`short` 表示的是每一个单元格的具体信息，用的是二进制表达。每一个 `short` 数据只使用其中的 12 个比特位，其中低 9 个比特位是 1 到 9 是否在这个单元格里包含这个候选数情况，而高 3 个比特位则是表示这个单元格的具体状态。使用 `>>` 和 `&` 可以获取它们。

```csharp
short mask = _values[0]; // Gets the mask of the cell r1c1.
short candidates = mask & 511; // Gets the candidate list.
byte status = mask >> 9 & 7; // Gets the status of the cell.
```

其中，`status` 可以取的值是：

* `status` 是 1：当前单元格是空格；
* `status` 是 2：当前单元格已经填入了一个数字，不过这个数字是用户自己填的，并不是题目最开始就给了的数字（提示数）；
* `status` 是 4：当前单元格在题目最开始就有数字（提示数）。

这个数值结果也可以等价转换为 `CellStatus` 枚举类型的数值，即 `var status = (CellStatus)(mask >> 9 & 7)`，这些枚举数值均可以查看[这个链接](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/CellStatus.cs)（`CellStatus` 的相关代码）。

## 成员选讲

### 字段 `EmptyString`

这个字段提供的是 `Grid` 匹配全空的盘面信息使用的字符串表达，由 81 个零字符 `'0'` 表示的字符串。稍后我们解释 `Parse` 和 `TryParse` 方法的时候，介绍具体字符串表达的规则。

### 字段 `ValueChanged` 和 `RefreshingCandidates`

这两个字段是这个数据类型里的两个基本事件。考虑到数据类型的高性能，这两个事件都使用的是 C# 9 的函数指针语法表达的具体数据类型。详情请自行参考 C# 函数指针语法。其中：

* `ValueChanged` 对应的类型是 `delegate*<ref Grid, int, short, short, int, void>`；
* `RefreshingCandidates` 对应的类型是 `delegate*<ref Grid, void>`。

请注意，在使用的时候它们是具体类型来调用的，但为了防止外部调用，我们使用 `void*` 表示，避免用户查看该字段的类型进行非预期地调用，以及反向推导数据类型导致的类型不安全性。在这里我介绍了这两个字段的具体数据类型是为了让你能够明白这两个字段的调用原理和规则，强烈不建议用具体类型替换掉这两个字段原本的 `void*` 类型。

### 字段 `Undefined` 和 `Empty`

这两个字段封装了两个 `Grid` 类型的默认实体对象。其中 `Undefined` 和 `default(Grid)` 是一个意思，不过 `Undefined` 避免了用户创建对象产生的额外内存分配；而 `Empty` 字段则就是前文介绍的 `EmptyString` 封装得到的 `Grid` 的实体，即一个空盘。

注意 `Undefined` 和 `Empty` 的不同。`Undefined` 是数据本身都不合这个数据类型表达的规则，而 `Empty` 则满足表示规则。举个例子，`Undefined` 对于每一个 `short` 二进制数值来说，因为是默认实例化的规则，所以它们全是 0；而 `Empty` 则是空盘，即每一个单元格都是空格状态下并包含全部 1 到 9 的候选数，因此每一个 `short` 二进制数值都是 `0b001__111_111_111`，即十进制下的 1023。

可以查看前面的定义里看到，`DefaultMask` 常量的数值就是 1023。是的，这个常量专门表示的是初始化空盘数据使用的每一个 `short` 的默认数值；而 `MaxCandidatesMask` 则对应了候选数全满（1 到 9 都包含）的二进制数值 `0b111_111_111`，即十进制下的 511。

### 无参构造器 `Grid()`

无参构造器在 C# 10 里允许用户自定义，因此此时无参构造器等价于 `this = Empty`，即赋值 `Empty` 的实例化过程。请注意，这个构造器在早期因为无法自定义而等价于 `Undefined`，但在 C# 10 后，它被我们自行定义了，因此实例化的情况也发生了变化。

不过，它仍然会产生内存分配，因此建议使用 `Empty` 字段的赋值过程。

### 构造器 `Grid(short[])`

这个构造器用来直接将一个完整的 `short` 二进制掩码表赋值到这个对象本身里去，来产生对象。

可以从这个说法上看出，参数的这个数组一定得包含 81 个元素，因为数独盘面是 9 × 9 = 81 个单元格。

### 构造器 `Grid(int[], GridCreatingOption)`

这个构造器的第一个参数是由 81 个元素构成的整数序列。其中每一个元素的取值范围均在 -1 到 9 之间。严格的限制是，每一个元素要么是 -1 到 8 来分别表示空格和数字 1 到 9，或者是用 0 到 9 来分别表示空格和数字 1 到 9。

举个例子，比如第一个单元格提示数是 2，那么我们可以给 `int[]` 的第一个元素录入数字 2（或者 1），第二个单元格提示数是 6，那么我们可以给 `int[]` 的第二个元素录入数字 6（或者 5）。

第二个参数则是限定我们的映射元素和数据的规则的。`GridCreatingOptions` 是一个枚举，它包含两个情况，一个是“不转换”（`None` 枚举数值），一个则是“减去 1”（`MinusOne` 枚举数值）。如果这个参数的数值是 `MinusOne` 的话，那么第一个参数 `int[]` 的每一个元素都会被按照 1 到 9 对等数字 1 到 9 的映射关系（0 表示空格）来进行处理数值；而数值是 `None` 的话，那么第一个参数 `int[]` 的每一个元素将按照 0 到 8 对等数字 1 到 9 的映射关系（-1 表示空格）来进行处理数值。

因为在 C# 底层处理之中，索引等信息一般都使用 0 开始计算，因此在这里也是一样，0 到 8 来表示数字 1 到 9 才是更靠近底层的处理过程；但是要注意的是，因为 1 到 9 要转为 0 到 8，因此每一个数字都会比底层的数据大 1 个单位，因此这个时候我们使用的枚举数值是 `MinusOne` 作为第二个参数，这暗示了我们处理的时候要统统减 1 来匹配底层的处理过程。

### 构造器 `Grid(int*, GridCreatingOption)`

这个构造器和前面那个没啥区别，甚至可以说是一致的处理，只不过把类型从 `int[]` 换成了 `int*`。稍微注意一下的是，这个构造器的 `int*` 是指针类型，因此处理操作是不安全上下文，请一定保证调用这个构造器之前，确保这个参数指向的是一个带有 81 个元素的 `int` 数组类型。

### 构造器 `Grid(ReadOnlySpan<int>, GridCreatingOption)`

这个构造器也是一样，只不过换成了 `ReadOnlySpan<int>` 代替了原始的 `int[]` 和 `int*`。

### 属性 `ResetGrid`

`ResetGrid` 属性获取的是这个盘面的初盘信息，即去掉这个盘面由用户填入的期间的数值（如果有的话），只保留提示数和空格的基本信息。

### 属性 `Solution`、`IsValid` 和 `IsSolved`

这个属性包含的是这个题目的解，即答案。请注意，如果题目是多解题和无解题目的话，这个属性的数值是不正常的——多解题或无解题的该属性数值是 `Undefined`。

检测题目是否有效（唯一解），可以使用 `IsValid` 属性来判断；而判断题目是否已经是不剩空格全部完成了的状态的话，可以使用 `IsSolved` 属性来获取。

### 属性 `IsUndefined` 和 `IsEmpty`

这两个属性用来判断对象是否和 `Empty` 以及 `Undefined` 的数值完全相等，即每一个 `short` 数值是否都等于 `Undefined` 或是 `Empty` 的 `short` 数值（即前文介绍的 0 或 1023）。

### 属性 `IsDebuggerUndefined`

这个属性用来判断当前盘面是不是除了第一个单元格的所有单元格的 `short` 对应二进制数值都是 0。

在 Visual Studio 环境调试下，因为缓冲区（Fixed-Sized Buffer）特性无法正常显示数值，因此除了第一个元素外，所有剩余的元素均会使用 0 表示。在调试和显示结果的时候，这个属性可能会被调用到，然后呈现出对应的数值结果，避免出现调试器的崩溃问题。

### 属性 `GivensCount`、`ModifiablesCount`、`EmptiesCount` 和 `CandidatesCount`

这几个属性用来获取盘面当前的提示数、用户填入的数字、空格以及候选数的总个数。

### 属性 `NullRegions`

这个属性用来获取整个盘面里，有哪些区域（行、列、宫）下是全空格的。比如第一行每一个格子都是空格的话，这样的区域就称为 null region（空区域）。

这个属性的结果是一个由 27 个比特位凑成的一个 `int` 掩码数值，低 27 个比特位就表示第 1 宫到第 9 宫、第 1 行到第 9 行、第 1 列到第 9 列里有多少是这样的空区域。

### 属性 `GivenCells`、`ModifiableCells` 和 `EmptyCells`

这三个属性用来表示和获取这个盘面里所有的提示数、用户自行填入的数字、以及空格的位置。这三个属性均返回 `Cells` 类型的对象，获取的是具体位置，而并非个数。

你可以说，`GivensCount` 就等于是 `GiventCells.Count` 的结果，剩下两个属性也以此类推。

### 属性 `BivalueCells`

这个属性获取的是这个盘面的所有双值格的位置。所谓的双值格（Bi-value Cell），就指的是单元格是空格，且只包含两个候选数情况的单元格。这个属性获取的是全盘里所有双值格的具体位置，返回的也是 `Cells` 类型的对象。

### 属性 `ValuesMap`、`DigitsMap` 和 `CandidatesMap`

这三个属性均返回 `Cells` 类型的数组，全部的数组都是由 9 个元素构成，分别对应的是数字 1 到 9 的具体信息。具体来说：

* `ValuesMap`：对应每一种数字在盘面里出现提示数或自行填入的数字的具体位置。比如全盘已经有 8 个数字 1 是确定值状态（确定值就是提示数和自行填入的数字的总称），那么 `ValuesMap[0]` 对应的 `Cells` 就包含这 8 个单元格信息；
* `DigitsMap`：对应每一种数字在盘面里无视单元格状态下，候选数包含此数字的所有单元格。请注意这个属性会忽略单元格状态，不论是空格状态下包含数字 1 的候选数情况，还是单元格直接是提示数 1 的状态，在这个属性下均视为“包含候选数 1”，然后将这些包含的情况取出构成一个 `Cells`。9 个数字对应的就是 9 个元素了；
* `CandidatesMap`：对应每一种数字在盘面空格里的候选数包含它们的单元格具体位置。这个和 `DigitsMap` 不同点在于，`DigitsMap` 不看单元格状态，但 `CandidatesMap` 只看空格。

### 索引器 `this[int]`

这个索引器表示获取指定单元格的填数是什么。这个索引器直接获取填数信息，只要它是确定值就行（不论是提示数还是自行填入的），返回值就是这个结果数值，返回结果范围是 -1 到 8，-1 表示当前单元格是空格状态导致无法取出合理的数值；0 到 8 则对应的是数字 1 到 9。

另外，这个索引器还可以允许赋值，即你可以随便给一个数值赋值到对应的单元格上去，这样就可以使得这个数字自动填入到盘面里，并引发候选数的重新计算。

### 索引器 `this[int, int]`

这个索引器用来获取某个单元格的数字是否存在。注意，它和 `Exists` 方法功能类似，但区别是返回值。这个索引器返回 `bool` 类型，只看包含与否；而 `Exists` 方法将会判断单元格的状态，它返回的是 `bool?` 类型的结果，包含三种情况。详情请参看稍后介绍的 `Exists` 方法。

### 实例方法 `EnumerateCandidates` 和 `EnumerateMasks`

这两个实例方法用来产生枚举器对象，并直接枚举盘面的候选数序列和掩码表序列。其中，`EnumerateCandidates` 枚举的是候选数，而 `EnumerateMasks` 枚举的是掩码。

可以查看它们的返回值，它们各自都对应了一个嵌套的枚举器类型，其中：

* `MaskCollectionEnumerator`：对应 `EnumerateMasks` 的返回值，迭代的每一个元素都是 `ref short` 类型的；
* `CandidateCollectionEnumerator`：对应 `EnumerateCandidates` 的返回值，迭代的每一个元素是 `int` 类型的。

用法如下：

```csharp
// Calls method 'EnumerateMasks'.
foreach (ref short mask in grid.EnumerateMasks())
{
    // Here you can insert any code you want to modify or operate the mask.
    // Please note that the iteration variable is by reference, so you can use
    // the keyword 'ref' or 'ref readonly' as the modifier to limit the usage
    // on the iteration variable.
    // If 'ref short mask', the variable can be modified,
    // If 'ref readonly short mask' or just 'short mask', the variable cannot be modified.
}

// Calls method 'EnumerateCandidates'.
foreach (int candidate in grid.EnumerateCandidates())
{
    // Here you can insert any code you want.
    // For example:
    int cell = candidate / 9, digit = candidate % 9;
    int row = cell / 9, column = cell % 9;
    // ...
}
```

### 实例方法 `Equals(object?)`、`Equals(in Grid)`

这两个方法都用来判断对象的数值是否一致。不过这两个方法是实例的方法，它们在底层来说没有任何实质的区别。但是由于静态方法和运算符更好用，所以我们更建议你使用 `Equals(in Grid, in Grid)` 静态方法，或者是直接使用运算符 `==` 和 `!=` 来判断。

### 实例方法 `Exists(int)` 和 `Exists(int, int)`

这两个方法用来获取当前单元格是否包含特定的候选数信息，只不过这两个方法的返回值是 `bool?` 类型，因为它包含三种可能情况：

* `true`：当前单元格包含此候选数；
* `false`：当前单元格不含有此候选数，但是当前单元格是空格；
* `null`：当前单元格不是空格。

如果说这个单元格是空格，并且有这个候选数的情况的话，`Exists` 方法才会返回 `true` 数值。如果要确定结果是否是 `true`，因为 `bool?` 类型的特殊性，你必须追加 `== true` 或 `is true` 来判别。

### 实例方法 `Fix` 和 `Unfix`

这两个实例方法将当前盘面里包含的自行填入的数字的状态进行改变。其中：

* `Fix`：将自行填入的数字直接固定下来，改成提示数；
* `Unfix`：将盘面所有的确定值都变为空格。

### 实例方法 `GetCandidates(int)`

这个实例方法将会计算和获取指定单元格的候选数掩码表。不过这个计算操作我们在前文的设计原理上已经说了，所以你可以参考前面给出的数据类型的设计原理得到这个方法的底层计算表达式。

其中的参数必须是 0 到 80 之间的数字。虽然参数的类型是 `int` 允许超过 0 到 80，但是数独盘面的定义约束下，单元格的编号只有 81 个是有效的数值。

### 实例方法 `GetEnumerator`

这个实例方法允许我们直接对 `Grid` 类型的实例进行 `foreach` 的循环迭代过程。不过它等价于迭代遍历候选数，因此相当于是简化了 `EnumerateCandidates` 方法的调用。

### 实例方法 `GetHashCode`

这个方式是从 `ValueType` 基类型上重写掉的，它表示计算这个数据类型的哈希码。

不过请注意，我们不建议你使用这个方法来判断对象是否一致；请判断一致的时候永远使用 `Equals` 方法或 `==` 来比较。

### 实例方法 `GetMask(int)`

这个方法也是获取掩码的，不过它获取的是这个单元格的完整掩码信息，就是整个 `short` 数值，而不是去掉了单元格状态信息的掩码数值。

### 实例方法 `GetPinnableReference`

这个方法允许我们定义 `fixed` 操作，固定 `Grid` 类型的变量。请注意，固定下来的结果就是整个掩码表的起始地址：

```csharp
fixed (short* p = grid)
{
    short firstCellMask = p[0];
}
```

比如这样。

### 实例方法 `GetStatus(int)`

这个方法就是我们前文在讲解类型设计的底层原理的时候说到的那个获取 `CellStatus` 枚举结果的封装方法了。是的，它就等价于 `>> 9 & 7` 的结果，然后进行了一次强制转换。

### 实例方法 `Reset`

`Reset` 方法用来清空自行填入的数字，使得题目直接恢复初盘的样子。请注意，这个方法会重新计算候选数信息，因此掩码表不一定仅仅是单元格状态的变化。

### 实例方法 `SetMask(int, short)` 和 `SetStatus(int, CellStatus)`

不必多说了吧，对应前面的 `GetMask` 和 `GetStatus` 的相反操作：变更掩码信息和变更状态信息。

### 实例方法 `SimplyValidate`

这个方法用来简单检测是否有行列宫上有相同的数字。仅此而已。

这个方法仅用来验证题目最基本的不可重复性的规则。如果题目含有更深层次的错误导致的多解或无解情况，这个方法是验证不出的。

### 实例方法 `ToArray`

这个方法用来将掩码信息重新转换回一个 `int[]` 结果，其中的每一个元素都是用 -1 到 8 表示，其中 -1 表示空格，而 0 到 8 则对应的是 1 到 9 的数字信息。

### 实例方法 `ToString` 和 `ToMaskString`

这两个方法用来获取这个数独盘面的字符串表达。其中，`ToString` 获取的是基本的、由 81 个字符构成的字符串，其中直接用 0 表示空格，而 1 到 9 则表示的是提示数 1 到 9 的信息；而 `ToMaskString` 则是得到一组 81 个 `short` 掩码数值构成的序列，然后将其直接使用逗号分隔写出来的一个完整的字符串结果。

早期的数据结构的设计下，`ToMaskString` 被用来验证和调试显示结果到 Visual Studio 的调试器上，结果因为无法识别缓冲区字段的关系，导致这个方法不再被使用。不过该方法并未被删除。

### 实例方法 `ToString(string?)`

这个实例方法控制整个数独盘面以什么格式显示和呈现出字符串形式的结果来。参数是这个输出的格式化字符串信息，它支持如下的一些情况：

* 点 `"."`：只显示提示数和空格信息，其中空格会被显示为小数点字符 `'.'`，而自行填入的数据会被视为空格显示为小数点字符 `'.'`；
* 加号 `"+"`：表示展示出自行填入的数字信息。自行填入的数字信息会在数字前面追加加号字符 `'+'` 来区分它和普通的提示数信息。比如 `"+3"` 会被视为自行填入的 3，而不是提示数 3；提示数 3 则必须去掉加号字符，即 `"3"`；
* 零 `"0"`：和 `"."` 类似，只不过小数点字符用的是零字符 `'0'` 作为替换；
* 冒号 `":"`：表示展示出当前盘面里被额外删掉的候选数的序列信息。如果带有这个格式化字符串，那么结果里会在盘面的字符串信息之后紧跟上一个冒号字符 `':'`，然后使用一组若干个三位数构成的数字序列，表示删去的数字序列。比如 `:123 234`，表示基于这个盘面下，第 2 行第 3 列的 1，以及第 3 行第 4 列的 2 也被去掉了；
* 感叹号 `"!"`：表示将自行填入的数据按照提示数展示，即省去里面的加号字符 `'+'`；
* 星号 `"*"`：表示将前文得到的字符串表达规则进一步简化，省去连续的若干零字符 `'0'` 或小数点字符 `'.'` 以缩短字符串长度；
* 原义字符符号 `"@"`：表示将整个盘面按照多行的表达形式、包含候选数状态的完整信息呈现出来；
* 波浪符号 `"~"`：表示将盘面按照 Sukaku（候选数数独）的字符串表示规则进行表达；
* 百分号 `"%"`：表示将盘面按照 Excel 能够读取的字符串格式（`*.csv` 格式）进行表达；
* 脱字号 `"^"`：表示将盘面按照 Open Sudoku 软件能够读取的字符串格式（`*.opensudoku` 格式）进行表达。

它们之间是可以组合使用的，比如格式化字符串 `".+:"` 表示显示出所有的提示数、自行填入的数字，以及候选数删去的情况：

```csharp
string result = grid.ToString(".+:");
string result = $"{grid:.+:}"; // Same as the above statement.
```

当然，也有一些不能混用，比如 `"."` 和 `"0"` 做的是相同而对立的行为，所以它们俩不能混用。

不过，因为详细讲解的内容非常多，所以我把他们移动到别单独的一篇文章里去了。详情请参考那篇文章。

### 静态方法 `Parse` 以及 `TryParse` 系列方法

名为 `Parse` 的一系列方法用来将前文 `ToString` 方法产生的字符串信息反向解析为 `Grid` 类型。所有前文给定的输出的结果字符串均是支持反向解析成 `Grid` 类型的。

`TryParse` 系列方法则是 `Parse` 方法的另外一种表示方式，它们的区别在于，`TryParse` 方法不会产生异常；如果匹配失败或是内部产生了异常，`TryParse` 将使用返回值 `false` 来表达这一情况；相反地，`true` 返回值表示的是解析操作成功，`TryParse` 的 `out Grid` 类型的参数则表示的就是整个解析结果了；而对于 `Parse` 方法来说，它不会吃掉异常，而返回值结果也不是 `bool` 类型，而直接是 `Grid` 类型。

### 运算符 `==(in Grid, in Grid)` 和 `!=(in Grid, in Grid)`

这两个运算符前文已经说过了，就是 `Equals` 系列方法的替代品，也是建议使用的比较方式。
