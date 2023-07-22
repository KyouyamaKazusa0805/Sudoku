# `CellMap` 数据结构

贯穿整个解决方案里使用到的一个比较重要的数据结构叫 `CellMap`，是一个结构：

```csharp
public struct CellMap :
    IAdditionOperators<CellMap, int, CellMap>,
    IAdditionOperators<CellMap, IEnumerable<int>, CellMap>,
    IAdditiveIdentity<CellMap, CellMap>,
    IBitwiseOperators<CellMap, CellMap, CellMap>,
    IBooleanOperators<CellMap>,
    ICollection<int>,
    IComparable,
    IComparable<CellMap>,
    IComparisonOperators<CellMap, CellMap, bool>,
    IDivisionOperators<CellMap, int, short>,
    IEnumerable,
    IEnumerable<int>,
    IEquatable<CellMap>,
    IEqualityOperators<CellMap, CellMap, bool>,
    IFormattable,
    ILogicalNotOperators<CellMap, bool>,
    ILogicalOperators<CellMap, CellMap, CellMap>,
    IMinMaxValue<CellMap>,
    IModulusOperators<CellMap, CellMap, CellMap>,
    IMultiplyOperators<CellMap, int, Candidates>,
    IParsable<CellMap>,
    IReadOnlyCollection<int>,
    IReadOnlyList<int>,
    IReadOnlySet<int>,
    ISet<int>,
    ISimpleFormattable,
    ISimpleParseable<CellMap>,
    ISpanFormattable,
    ISpanParsable<CellMap>,
    ISubtractionOperators<CellMap, int, CellMap>,
    ISubtractionOperators<CellMap, CellMap, CellMap>,
    ISubtractionOperators<CellMap, IEnumerable<int>, CellMap>
{
    public static readonly CellMap Empty;

    public readonly bool InOneHouse { get; }
    public readonly bool IsEmpty { get; }
    public readonly int CoveredHouses { get; }
    public readonly int Count { get; }
    public readonly int CoveredLine { get; }
    public readonly short ColumnMask { get; }
    public readonly int Houses { get; }
    public readonly short RowMask { get; }
    public readonly short BlockMask { get; }
    public readonly CellMap PeerIntersection { get; }
    public readonly CellMap ExpandedPeers { get; }

    public readonly int this[int index] { get; }

    public static CellMap CreateByBits(long high, long low);
    public static CellMap CreateByBits(int high, int mid, int low);
    public static CellMap Parse(string str);
    public static bool TryParse(string str, out CellMap result);
    public void Add(int offset);
    public void Add(string offset);
    public void AddRange(scoped in ReadOnlySpan<int> offsets);
    public void AddRange(IEnumerable<int> offsets);
    public readonly bool AllSetsAreInOneHouse(out int house);
    public void Clear();
    public readonly int CompareTo(scoped in CellMap other);
    public readonly bool Contains(int offset);
    public readonly void CopyTo(int* arr, int length);
    public readonly void CopyTo(scoped ref Span<int> span);
    public override readonly bool Equals([NotNullWhen(true)] object? obj);
    public readonly bool Equals(scoped in CellMap other);
    public readonly CellMap[] GetSubsets(int subsetSize);
    public readonly CellMap[] GetAllSubsets();
    public readonly CellMap[] GetAllSubsets(int limitSubsetSize);
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
    public readonly int CompareTo(scoped in CellMap other);
    public readonly OneDimensionalArrayEnumerator<int> GetEnumerator();
    public override readonly int GetHashCode();
    public void Remove(int offset);
    public readonly int[] ToArray();
    public override readonly string ToString();
    public readonly string ToString(string? format);
    public readonly string ToString(ICellMapFormatter cellMapFormatter);
    public readonly string ToString(string? format, IFormatProvider? formatProvider);

    public static bool operator !(scoped in CellMap offsets);
    public static bool operator true(scoped in CellMap offsets);
    public static bool operator false(scoped in CellMap offsets);
    public static bool operator ==(scoped in CellMap left, scoped in CellMap right);
    public static bool operator !=(scoped in CellMap left, scoped in CellMap right);
    public static bool operator >(scoped in CellMap left, scoped in CellMap right);
    public static bool operator >=(scoped in CellMap left, scoped in CellMap right);
    public static bool operator <(scoped in CellMap left, scoped in CellMap right);
    public static bool operator <=(scoped in CellMap left, scoped in CellMap right);
    public static short operator /(scoped in CellMap map, int house);
    public static CellMap operator +(scoped in CellMap collection, int offset);
    public static CellMap operator +(scoped in CellMap collection, IEnumerable<int> offsets);
    public static CellMap operator -(scoped in CellMap collection, int offset);
    public static CellMap operator -(scoped in CellMap collection, IEnumerable<int> offset);
    public static CellMap operator -(scoped in CellMap left, scoped in CellMap right);
    public static CellMap operator ~(scoped in CellMap offsets);
    public static CellMap operator %(scoped in CellMap @base, scoped in CellMap template);
    public static CellMap operator !(scoped in CellMap offsets);
    public static CellMap operator &(scoped in CellMap left, scoped in CellMap right);
    public static CellMap operator |(scoped in CellMap left, scoped in CellMap right);
    public static CellMap operator ^(scoped in CellMap left, scoped in CellMap right);
    public static Candidates operator *(in CellMap @base, int digit);
    public static CellMap operator checked +(scoped in CellMap collection, int offset);
    public static CellMap operator checked +(scoped in CellMap collection, IEnumerable<int> offsets);
    public static CellMap operator checked -(scoped in CellMap collection, int offset);
    public static CellMap operator checked -(scoped in CellMap collection, IEnumerable<int> offsets);
    public static Candidates operator checked *(in CellMap @base, int digit);
    public static short operator checked /(scoped in CellMap map, int house);

    public static implicit operator int[](scoped in CellMap offsets);
    public static implicit operator CellMap(scoped in ReadOnlySpan<int> offsets);
    public static implicit operator CellMap(scoped in Span<int> offsets);
    public static explicit operator CellMap(int[] offsets);
    public static explicit operator Span<int>(scoped in CellMap offsets);
    public static explicit operator ReadOnlySpan<int>(scoped in CellMap offsets);
    public static explicit operator Int128(scoped in CellMap offsets);
    public static explicit operator CellMap(scoped in Int128 int128);
    public static explicit operator checked CellMap(scoped in Int128 int128);
}
```

它有如上的一些成员。

## 实现原理

该数据类型使用两个 `long` 字段（一共 128 个比特）的其中 81 个比特，用来表示整个数独盘面里，选中和未选中单元格的信息。其中一个 `long` 字段只会用到 64 个比特的其中 41 个比特，而另外一个 `long` 字段则只会用到其中 40 个比特。这样一共凑够 81 个比特。

详细一些的话，`long` 字段分别被命名为 `_low` 和 `_high`，它们分别表示的是“低 41 比特”和“高 40 比特”。通过这个数据结构的设计规则，我们可以通过比特运算来获得单元格的选中或未选中的情况。

```csharp
bool Contains(int offset) => ((offset < 41 ? _low : _high) >> offset % 41 & 1) != 0;
```

比如 `Contains` 这个方法，传入的 `offset` 表示一个单元格；而整个数据类型使用的 `_low` 和 `_high` 通过给出的表达式进行位运算处理，得到当前比特是否是 1。如果是，就说明这个序列里包含这个单元格（换句话说就是选中了这个单元格）；否则就没有选中。

## 成员选讲

### 字段

#### 无参构造器和 `Empty` 静态只读字段

该数据类型提供了一个叫做 `Empty` 的字段，它用来表示一个空序列。这个字段因为是静态的只读字段，因此它在项目初始化和启动的时候被初始化一次，然后不再初始化。

我们总是建议你使用 `Empty` 属性来初始化赋值给一个 `CellMap` 类型的对象作为它的初始数值：

```csharp
var cells = CellMap.Empty;
var cells = default(CellMap);
```

从数值上讲，上述两种初始化后的结果是完全一样的，但是使用 `default` 表达式的写法 `default(CellMap)` 则和 `CellMap.Empty` 是一样的，但为了统一代码的书写规范和风格，我们总是建议你使用 `CellMap.Empty`。

另外，无参构造器 `new CellMap()` 永远不希望你使用。在项目里，无参构造器永远会引发 `NotSupportedException` 异常，因为我们永远建议你使用 `CellMap.Empty` 字段。

除了一种情况我们必须使用 `default` 表达式，就是 C# 4 的可选参数语法。值类型的无参构造器在 C# 10 里开放给用户自定义，在自定义后，值类型的无参构造器的数值结果就不再和 `default` 表达式一致。因此，早期的 `new T()` 语法在值类型里是允许作为初始化结果放在可选参数上赋值的，而如今自定义了无参构造器的值类型后，这样的语法就不再允许放在上面了，而 `CellMap.Empty` 是静态只读量而不是常量也不是字面量，因此无法放在方法的可选参数上进行参数的默认初始化表达式，在这种情况下，只能使用 `default` 表达式来初始化：

```csharp
void F(CellMap cellsList = default, ...)
{
    // ...
}

void G(CellMap cellsList = default(CellMap), ...)
{
    // ...
}
```

详情请自行参考 C# 语法“可选参数”以及“值类型无参构造器的自定义”语法规则和规范。

### 属性

#### 属性 `IsEmpty` 和 `Count`

`Count` 属性用来获取这个集合里到底有多少个已经录入（也就是说选中了）的单元格信息。

```csharp
var cells = (CellMap)new[] { 1, 3, 6, 10 };
int count = cells.Count; // 4

// Another example

var cells = new CellMap(1);
int count = cells.Count; // 21
```

另外，如果判断集合有无记录元素的话，除了可以使用 `Count == 0` 判断，还可以使用 `IsEmpty` 属性：

```csharp
var cells = new CellMap { 1, 3, 6, 10 };
bool isEmpty = cells.IsEmpty; // false
```

另外，C# 11 使用集合模式来进行模式匹配，所以，`Count == 0` 判断可以写成 `is []`：

```csharp
var cells = new CellMap { };
bool isEmpty = cells is [];
```

这样和写 `IsEmpty` 属性在语法和调用上没有任何的区别，只不过 `[]` 是模式匹配语法，用于数据判断，而具体类型下很少这么做。

#### 属性 `InOneHouse` 和方法 `AllSetsAreInOneHouse(out int)`

这两个成员都用来判断是否当前集合里所有记录了的单元格位于同一个行（或列、宫）上。换句话说，是否我能找到一个行（或列、宫），能够包含所有的这些行、列、宫。

这两个成员的计算区别在于，方法 `AllSetsAreInOneHouse(out int)` 带有一个 `out` 参数，可以返回得到这个区域的结果；而 `InOneHouse` 属性仅用来计算是否有这个行（或列、宫），即只返回一个 `bool` 结果。所以它们的区别只是有没有把这个行（或列、宫）的具体数值给反馈出来。

注意，反馈的结果数值可能有 -1 到 26 这些情况。其中 -1 表示没有找到这样的区域，而 0 到 26 分别表示的是 27 个按照数独规则划分出来的区域。其中 0 到 8 是第 1 宫到第 9 宫，9 到 17 是第 1 行到第 9 行，而 18 到 26 则是第 1 列到第 9 列，即：

* -1：当属性或方法返回 `false` 时的结果；
* 0 到 8：当属性或方法返回 `true` 时，找到的区域是第 1 宫到第 9 宫的元素；
* 9 到 17：当属性或方法返回 `true` 时，找到的区域是第 1 行到第 9 行的元素；
* 18 到 26：当属性或方法返回 `true` 时，找到的区域是第 1 列到第 9 列的元素。

因为 C# 语法的设计规则，我不能给这两个操作完全一样的不同类型的成员使用同一个名字，因此方法名显得更长一些；另外，同一种处理结果用两种不同的成员的真实原因是，`InOneHouse` 属性因为不含有反馈参数结果数值的操作，因此性能上有一定的优化。

因此，如果你要考虑性能的使用的话，如果不使用这个 `out` 参数的数值，你可以考虑使用属性代替掉方法的调用：

```csharp
var cells = (CellMap)new[] { 1, 3, 6 };
bool condition = cells.AllSetsAreInOneHouse(out _); // Here.

// We suggest you change the code to:
var cells = (CellMap)new[] { 1, 3, 6 };
bool condition = cells.InOneHouse;
```

#### 属性 `BlockMask`、`RowMask`、`ColumnMask` 和 `Houses`

这三个属性用来获取整个集合下，包含的单元格一共出现在哪些行（或列、宫）里。举个例子，第一行第一列和第一行第二列这两个单元格涉及第 1 行、第 1、2 列和第 1 宫。

这三个属性的返回结果就是表示的这些数值信息。其中 `BlockMask` 对应宫的情况，`RowMask` 对应行的情况，而 `ColumnMask` 对应列的情况。注意，这三个属性返回的结果是一个用 9 个比特表示的整数，分别就对应了 9 个行（或列、宫）的情况。比如二进制 011000100 按照次序，第 3、7、8 个比特是 1，那么这个数就对应的是第 3、7、8 行（或列、宫）是这个集合涉及的行（或列、宫）。

也就是说，按照刚才举的例子来看，`BlockMask`、`RowMask` 和 `ColumnMask` 分别返回的是 000000001、000000001 和 000000011 这三个结果：

```csharp
var cells = (CellMap)new[] { 1, 2 };
int blocks = cells.BlockMask; // 000000001 (1 in decimal)
int rows = cells.RowMask; // 000000001 (1 in decimal)
int columns = cells.ColumnMask; // 000000011 (3 in decimal)
```

最后，`Houses` 属性获取的结果是将这三个九位二进制结果进行叠加整合在一起之后得到的最终数值。也就是说，比如前面给出的 `cells` 是 { 1, 2 } 两个单元格，那么：

```csharp
var cells = (CellMap)new[] { 1, 2 };
int blocks = cells.Houses; // 000000011_000000001_000000001 (786945 in decimal)
```

#### 属性 `CoveredHouses` 和 `CoveredLine`

这两个属性是用来获取整个集合涉及的单元格都跨越了哪些行、列、宫。这两个属性的计算方式和 `InOneHouse` 以及 `AllSetsAreInOneHouse` 比较类似，也都是看是否所有单元格位于同一行（或列、宫），只不过 `CoveredHouses` 和 `CoveredLine` 属性侧重于求值，即获取这个区域的数值结果，而不是 `bool` 结果。

其中，`CoveredHouses` 属性会得到这个集合涉及的行、列、宫。比如说：

```csharp
var cells = (CellMap)new[] { 0, 1 }; // Block 1 and Row 1.
int coveredHouses = cells.CoveredHouses; // 000000000_000000001_000000001 (513 in decimal)
```

可以看到结果是 513，二进制是 000000000000000001000000001。我们将这个结果数值从右往左看作是第 0 到 26 编号的区域，然后发现比特位是 1 的区域是编号 0 和 9 的，所以按照前文给出的宫、行、列的顺序的设计规则，这个数值就对应的是第 1 宫和第 1 行是这个集合所处区域的情况。

不过，`CoveredLine` 属性会将 `CoveredHouses` 属性的结果作为基本数据进行再一次地处理，使得结果只是行或列的情况。按照数独的基本技巧的搜寻原则和存在的情况的规律性，技巧结构（例如区块）就会出现类似刚才的 { 0, 1 } 这样的单元格的情况；而这样的情况一定是属于一个宫和一个行（或列）的，因此 `CoveredLine` 属性的处理操作只会取出对应的行（或列）的准确结果。换句话说，就 { 0, 1 } 这个集合的话，`CoveredLine` 属性的结果应该是准确的结果数值 9，而不是二进制数 1000000000（二进制下的 1 后面 9 个 0）。

```csharp
var cells = (CellMap)new[] { 0, 1 }; // Block 1 and Row 1.
int coveredHouses = cells.CoveredLine; // 9
```

正是因为这个原因，`CoveredLine` 属性的属性名的 line 单词没有使用复数形式，因为它只获取这个准确结果数值，是一个实际的数字。

如果这个 `CoveredHouses` 属性的结果不同属于行（或列）的话，那么因为高比特位上都是 0，因此 `CoveredLine` 属性在处理之后，是出于找不到合适结果的一个状态，因此在这种情况下，`CoveredLine` 属性会默认返回 32。注意，返回的是 32，不是别的，不是 -1！不是 -1！不是 -1！重要的事情说三遍。至于为什么返回 32，请自行参看 .NET 库文件的源代码（位于 `BitOperations.TrailingZeroCount` 方法里）。

#### 属性 `PeerIntersection`

该属性计算的是该序列里所有元素共同可以对应到的地方。这个属性经常用于搜索删数。

#### 属性 `ExpandedPeers`

该属性用于计算该序列里的每一个元素展开为 `CellMap` 实例后的叠加结果。举个例子。如果一个集合是 r1c123 三个单元格，那么该属性的结果相当于求得 r1c1、r1c2 和 r1c3 各自的相关单元格组合起来之后的结果。

### 索引器

#### 索引器 `this[int]` 和 `this[Index]`

该数据类型提供了 `int` 和 `Index` 作为参数的索引器使用。这两个索引器获取的是第几个被记录的单元格的编号。比如说是使用 { 0, 1, 3, 6, 10 } 这几个单元格构成的 `CellMap` 集合对象的话，那么：

```csharp
var cells = (CellMap)new[] { 0, 1, 3, 6, 10 };
int first = cells[0]; // 0
int second = cells[1]; // 1
int third = cells[2]; // 3
int fourth = cells[3]; // 6
int ultimate = cells[^1]; // 10
int penultimate = cells[^2]; // 6
int antepenultimate = cells[^3]; // 3
```

C# 8 提供的索引语法可以更好地控制和获取集合的运算结果。`^1` 表示倒数第一个元素，而 `^2` 表示倒数第二个元素，`^3` 表示倒数第三个元素，以此类推。

另外，C# 11 提供的列表模式可以帮助我们对整个序列进行集合解构操作：

```csharp
if (cells is [var first, var second, .., var penultimate, _])
{
    // ...
}
```

比如语法 `[var first, var second, .., var penultimate, _]` 表示获取集合里第一个、第二个和倒数第二个元素的具体数值，然后直接自动表示为 `first`、`second` 和 `penultimate` 变量，这比起使用普通的赋值语句来说要好看一些。

### 方法

#### 方法 `Contains(int)`

该方法用来计算这个集合是否包含指定单元格。它的设计规则和前文介绍原理时定义的 `Contains` 方法是完全一样的处理过程，因此不再赘述。

#### 方法 `ToArray`

这个方法用来把集合里包含的单元格的编号直接表示为一个 `int[]` 数组类型，并返回。

#### 方法 `ToString` 和 `ToString(string?)`

这个方法用来获取这个集合的字符串表达。显示这个集合的表达模式一共有三种支持的格式化字符串：

* `t` 或 `T`：将其按表格样式呈现结果。表格的结果使用星号字符 `'*'` 表示集合包含当前单元格；不包含的话则使用的是小数点字符 `'.'`；
* `b` 或 `B`：将其按二进制数值的样式呈现结果。这个结果是一个长为 81 个字符的字符串，其中字符 0 `'0'` 表示当前集合不包含这个单元格，否则用字符 1 `'1'` 表示包含此单元格；
* `n` 或 `N`：将其表示为 RCB 坐标记号的单元格序列。比如说第一行第一个单元格记作 r1c1，而第一行第二个单元格记作 r1c2；如果第一行第一、二个单元格就可以记作 { r1c1, r1c2 } 或直接简记作 r1c12，合并其中相同的“r1”部分。

```csharp
string s;
var cells = (CellMap)new[] { 0, 1, 2, 3, 4 };

s = cells.ToString(); // r1c12345
s = cells.ToString("n"); // r1c12345
s = $"{cells:n}"; // r1c12345
```

#### 方法 `GetEnumerator`

这个方法不必关心怎么去调用和使用，你只需要知道 C# 语法允许我们使用实现良好的 `GetEnumerator` 方法允许对象使用 `foreach` 循环的语法即可：

```csharp
var cells = (CellMap)new[] { 0, 1, 3, 6, 10 };
foreach (int cell in cells)
{
    // ...
}
```

比如这样。其中的迭代变量 `cells` 分别会迭代得到 0、1、3、6、10 这些结果。

#### 方法 `Add(int)`、`AddRange(IEnumerable<int>)` 和 `AddRange(ReadOnlySpan<int>)`

这个方法才是用来手动调用以追加元素的。`Add` 方法允许传入 `int` 参数，表示添加一个编号对应的单元格到集合里去。如果重复添加的话，不会产生任何编译器错误或异常信息，但这个方法此时什么事都不做。

它和 `+` 运算符的效果类似。而 `AddRange` 方法则是添加一系列的单元格编号到集合里去。如果你需要追加一组单元格编号的话，可以用这个方法。

```csharp
int a = 1, b = 2, c = 3, d = 4;
var cells = CellMap.Empty;
cells.Add(a);
cells += b;
cells.AddRange(stackalloc[] { c });
cells.AddRange(new[] { d });
```

这些方式都可以正确往集合里追加元素。

#### 方法 `Remove(int)`

这个方法用来移除一个指定单元格。注意，参数也是 0 到 80 期间的编号。如果这个编号代表的单元格不在集合里，则这个方法什么事情都不做；否则这个单元格会被移除。

#### 方法 `Clear`

该方法用于清空列表。换句话说，从代码的含义上讲，它等价于 `this = CellMap.Empty` 的赋值。

#### 静态方法 `Parse(string)` 和 `TryParse(string, out CellMap)`

这两个方法用来将一个按照 RCB 规则书写的单元格坐标序列直接改成转换为 `CellMap` 类型的实体返回出来。其中，`Parse` 方法直接返回该转换结果，而 `TryParse` 方法返回的是 `bool` 结果，表示整个转换操作是否成功；而转换成功的话，结果将会从 `out` 参数上返回出来。

```csharp
string str = "{ r1c1, r2c12 }";
var cells = CellMap.Parse(str); // 0, 9, 10
int count = cells.Count; // 3
int coveredHouses = cells.CoveredHouses; // 0 (for block 0)
foreach (int cell in cells)
{
    // ...
}
```

#### 方法 `GetSubsets(int)`、`GetAllSubsets` 和 `GetAllSubsets(int)`

这三个方法取的是整个集合的所有元素在进行枚举的所有排列。比如说 r1c1、r1c2、r1c3 三个单元格的排列可以是 r1c1、r1c2、r1c3 三个单元格自己，也可以是 r1c12、r1c13 和 r1c23 三个每次取两个单元格的情况，也可以是整个三个单元格全算作一个整体的 r1c123，一共七种情况。`GetAllSubsets` 便是这个操作；带有参数的则表示取出它的子集里长度不超过指定参数的所有子集；`GetSubsets(int)` 则只会取出等长度的所有子集。

#### 静态方法 `CreateByBits(long, long)` 和 `CreateByBits(int, int, int)`

这两个构造器用来以比特位的形式初始化序列。其中带有 `long` 的构造器是直接按比特位序列进行初始化，初始化的模式和前文描述设计原理时的那个初始化行为完全一致。

所以请你注意，传入的参数的 `long` 一旦超过 41 比特，就会导致这个数据类型的处理操作过程不安全（因为是直接赋值，因此超出的比特位会被录入到集合里去，导致 `Count` 属性（即统计多少个记录的单元格）的数值超出预期的结果）。因此，不要传入这样的数据。

同理，带有三个 `int` 参数的构造器也是如此。这个是早期遗留下来的设计规则。早期在设计 `CellMap` 的时候，我用的是三个 `int` 字段表示的，每一个字段使用 27 个比特。不过后来改成 `long` 来避免底层多一次的拷贝了。

查看代码可以看到，它的处理过程非常简单而暴力：

```csharp
public CellMap(int high, int mid, int low) :
    this(
        (high & 0x7FFFFFFL) << 13 | (mid >> 14 & 0x1FFFL),
        (mid & 0x3FFFL) << 27 | (low & 0x7FFFFFFL)
    )
{
}
```

是直接将三个参数改造处理成两个 `long` 结果然后直接调用的 `CellMap(long, long)` 实例化的。

### 运算符

#### 位取反运算符 `~(in CellMap)`

位取反运算符用来将当前集合里记录了的单元格去掉，然后把没有记录的单元格全给加上，并返回改写后的结果。注意，这样的处理规则并不会直接改写原始对象，而是将这个对象从返回值返回，因此不会造成任何的副作用。

#### 位与运算符 `&(in CellMap, in CellMap)`

位与运算符会将两个 `CellMap` 集合里都包含的单元格取出，然后返回。比如说一个是 { 0, 1, 2 }，而另外一个是 { 1, 8, 9 }，那么结果则是只包含 { 1 } 的 `CellMap` 对象。

因此，对这个集合来说，位与运算符等价于数学概念的交集。

#### 位或运算符 `|(in CellMap, in CellMap)`

位或运算符会将两个 `CellMap` 集合里涉及的所有单元格全部取出，然后返回。比如说一个是 { 0, 1, 2 }，而另外一个是 { 1, 8, 9 }，那么结果则是包含 { 0, 1, 2, 8, 9 } 的 `CellMap` 对象。重复的元素也会被记录，但因为这个数据类型的设计原理的限制，只能记录一次。

因此，对这个集合来说，位与运算符等价于数学概念的并集。

#### 位异或运算符 `^(in CellMap, in CellMap)`

位异或运算符会将两个 `CellMap` 集合里只出现过一次的单元格全部取出，然后返回。比如说一个是 { 0, 1, 2 }，而另外一个是 { 1, 8, 9 }，那么结果则是包含 { 0, 2, 8, 9 } 的 `CellMap` 对象。重复的元素以及没有出现的元素全部不会被记录。

因此，对这个集合来说，位异或运算符等价于数学概念的对称差集。

#### 减法运算符 `-(in CellMap, in CellMap)`

减法是将符号左侧的对象包含，但右侧的对象不包含的元素全部取出，然后返回。比如说一个是 { 0, 1, 2 }，而另外一个是 { 1, 8, 9 }，那么结果则是包含 { 0, 2 } 的 `CellMap` 对象。由于编号 1 重复出现，因此会被减掉；而编号 8 和 9 在左侧集合里没有出现，所以不会被记录到结果里。

从这个角度来说，`left - right` 从代码的语义上等价于 `left & ~right`。

因此，对这个集合来说，位异或运算符等价于数学概念的差集。

#### 加减法运算符 `+(CellMap, int)` 和 `-(CellMap, int)`

这两个运算符用于追加和删除元素。不过这两个是运算符，因此不完全等价于 `Add` 和 `Remove` 方法，而等价于移除和添加元素后，将这个追加和删除了元素之后的结果返回出来；而这个操作不影响原始数据。比如：

```csharp
var cells = (CellMap)new[] { 0, 1, 2 };
var cells2 = cells - 2; // 0, 1
var cells3 = cells - 1 - 2; // 0
var cells4 = cells + 3; // 0, 1, 2, 3
var cells5 = cells + 3 - 3; // 0, 1, 2
```

#### 位与运算符 `&(in CellMap, int)`

位与运算符的第二个参数如果不是 `CellMap` 而是 `int` 的话，那么它表示的是将整个序列里的元素看成一个完整的集合，然后这个 `int` 表示的是取出其中的几个元素。那么这个运算符的结果则是通过若干单元格的集合里去指定个数的元素的所有情况。比如：

```csharp
var cells = new CellMap { 0, 1, 2, 3 };
var combinations = cells & 2;
foreach (var combination in combinations)
{
    // ...
}
```

这个 `cells & 2` 的结果是由 { 0, 1 }、{ 0, 2 }、{ 0, 3 }、{ 1, 2 }、{ 1, 3 } 和 { 2, 3 } 六个 `CellMap` 集合对象构成的数组。因此我们可以对这个结果进行 `foreach` 循环，得到每一个集合（即每一种情况）。

注意，这个运算符不考虑先后顺序，所以 { 1, 2 } 和 { 2, 1 } 是同一个东西。

#### 取模运算符 `%(in CellMap, in CellMap)`

取模运算符比较麻烦，`a % b` 可以展开为 `+(a & b) & b`。

说一下这种展开的意义。考虑数独技巧的删数规则，`a & b` 可以理解为“把 `b` 当成是模板，然后让 `a` 在这个模板上找，取得所有出现在 `b` 上的单元格”。对此结果执行 `operator +` 运算符就是在看这个结果对应的可以看到的地方，再一次使用 `& b`，可以清除掉不在模板上的对应格子。

这样理解有些复杂，我们考虑一个实际的数独技巧来举例说明。考虑[待定数组](https://www.bilibili.com/read/cv11955947)（ALS）的 ALS-XZ 技巧。我们要构造两个 ALS 部分，并且得到两个 ALS 内强链 z==x 和 x==z。两个 ALS 的 x 数字需要连起来构成弱链，即整个链为 z==x--x==z，然后删除 z 数字的共同对应的地方。

首先，我们提取出两个 ALS 必需的数值信息。比如说其中一个 ALS 是关于数字 1、2、7 的，那么将其表示为二进制 001000011 这样处理起来更快；同理另外一个也这么用二进制表达起来。然后，我们要得到 ALS 的涉及单元格。单元格我们可以用 `CellMap` 类型的对象表示起来。接着，我们知道，要想有删数，那么这个数字 z 必然来自两个 ALS 里涉及的数字。因此，我们必须使用 `mask1 & mask2` 表达式得到所有两个 ALS 都有的数字的掩码信息。接着使用 `foreach` 对其的比特位进行迭代：

```csharp
foreach (int z in mask1 & mask2)
{
    // Here 'z' is the target digit that is used for elimination.
}
```

注意，`mask1 & mask2` 是使用了整数的位与运算符，因此结果必然还是一个整数。而整数自身是不具有 `GetEnumerator` 方法的，因此无法使用上面这样的语法来迭代比特位。不过，在这个解决方案的代码里，我们提供了对比特位迭代的 `GetEnumerator` 方法，使之可以成为正确的语法和调用，你只需要引用 `System.Numerics` 命名空间即可，这个扩展是 C# 9 的扩展 `GetEnumerator` 方法的新语法特性，详情请自行参看相关的内容。

接着，假设我们用 `CandidateMaps` 表示数字 1 到 9 每一个数字在当前盘面上候选数包含这个数字的格子的列表的话，那么 `CandidateMaps[z]` 就取出了当前数字 `z` 对应的出现了的位置。假如我们找出了两个 ALS 并且列举出了用于找寻删数的两组格子（即 z==x 和 x==z 强链构造起来的两个包含 z 的单元格组）。假设它们用一个变量 `als` 表示起来的话，那么 `als & CandidateMaps[z]` 就意味着我取到的是“两个强链末端的包含 z 的格子”。此时，我们对这个结果使用 `operator +` 运算符，则就表示的是两个格子都对应的地方。不过，因为对应的单元格可能包含已经填好数字了的单元格甚至是提示数，也可能有不含这个数字 `z` 的格子，因此我们还需要再一次对这个共同对应的单元格列表作一次位与运算：`+(als & CandidateMaps[z]) & CandidateMaps[z]`，这样，我们才能正确得到关于数字 `z` 的、两个 ALS 关于数字 `z` 的删数。可以仔细对比这个表达式，它其实就是 `als % CandidateMaps[z]` 的完整展开。

所以，取模运算符对于这个数据类型的意思是“获取这个数字在一个模板上，它的相关单元格（所在行、列、宫的其余单元格）里，处于模板上的所有单元格”。这种用法多用于计算删数。

#### 比较运算符 `==(in CellMap, in CellMap)` 和 `!=(in CellMap, in CellMap)`

等号和不等号运算符用来判断两个集合是否包含完全一致的单元格。显然，从这个数据类型的设计上看，`==` 和 `!=` 可以直接被展开成 `_low` 和 `_high` 的相等比较，所以原理上等价 `a._low == b._low && a._high == b._high`，以及取反表示不等号。

这种设计的好处在于，我们直接使用掩码表示一个序列，这样就可以直接比较掩码数值来判断相等性了，节省了内存分配以及性能的不必要的损失。如果是集合和集合比较的话，那就得逐个元素比较，这样显然会慢很多。使用掩码的设计在这里就体现出了好处。
