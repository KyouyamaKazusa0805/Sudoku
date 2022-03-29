# 代码风格

本文介绍一下整个项目使用的代码的书写风格。

## 基本规则

1. 整个解决方案均使用的是 [Allman 缩进方式](https://en.wikipedia.org/wiki/Indentation_style#Allman_style)，即大括号上下对应，而不是将开大括号放在代码末尾不换行；
1. 缩进是用的 Tab 而不是四个空格；
1. 尽量全部避免使用 `this.` 来引用成员；
1. 任何时候我们都会写出成员的访问修饰符，即使它是默认的情况，除非这个成员不支持使用访问修饰符（比如静态构造器等等）；
1. 永远都是使用 `global using` 和 `global using static` 指令（C# 10 起可用）来引用命名空间，而在每一个代码文件都不出现任何的 `using` 和 `using static` 指令，除非在文件定义临时的长类型名的 `using` 类型别名；
1. `global using` 和 `global using static` 指令必须是按字典序排序；
1. 类型的不同成员类型之间会使用双空行来分隔不同的成员，而同成员类型的成员之间会使用一行空行来分隔；
1. 类型的成员顺序依次是：
      1. 字段（字段内部排序顺序依次是：常量字段、静态只读字段、静态字段、只读实例字段、实例字段）；
      1. 构造器（构造器内部排序顺序依次是：实例构造器、静态构造器、析构器，不过析构器本项目可能用不着）；
      1. 属性（先静态属性，然后是实例属性）；
      1. 索引器；
      1. 方法（先实例方法，然后是静态方法）；
      1. 运算符重载；
      1. 类型转换器（先显式类型转换，然后是隐式类型转换）。

1. 相同类型的成员之间，有如下的排序关系：
      1. 访问修饰符级别从高到低顺序依次是 `public`、`internal`、`protected internal`、`protected`、`private protected` 和 `private`；
      1. 如果访问修饰符相同，则按 `void`、`object`、内置类型、结构类型、枚举类型、类类型、委托类型、接口类型的顺序排序返回值；
      1. 如果返回值类型相同，则按标识符名称的字母表顺序排序（这一点也可以不遵守）；
      1. 如果标识符名相同，则按参数，按照第 2 点的类型顺序对其排序参数序列。如果第一个参数类型相同，就继续比较第二个参数，以此类推。
1. 如果是非只读结构的话，则优先列举所有的 `readonly` 修饰过的成员，然后才是没有 `readonly` 修饰的成员（注意，这个地方说的 `readonly` 是成员的修饰符，不是返回值修饰符）；
1. 修饰符顺序按照 Visual Studio 默认提供的排序顺序对修饰符排序：
      1. 访问修饰符：`public`、`internal`、`protected`、`protected internal`、`private protected`、`private`；
      1. 静态修饰符：`static`；
      1. 外部修饰符：`extern`；
      1. 继承控制修饰符：`new`、`virtual`、`abstract`、`sealed`、`override`；
      1. 只读修饰符：`readonly`；
      1. 不安全代码修饰符：`unsafe`；
      1. 跨线程修饰符：`volatile`；
      1. 异步修饰符：`async`。

1. 避免任何排版之前忘记的插入空格，比如说 `if` 和 `(` 之间的空格、`a + b` 表达式里 `a` 和 `+` 符号之间的空格以及 `+` 和 `b` 之间的空格等等；
1. 只要是变量、字段类型定义后要有等号赋值的时候：
      1. 如果它是内置类型（有关键字的那种），总是用关键字书写类型，避免用别的符号，也不使用 `var`；
      1. 如果是同行定义多个变量的话，只能使用显式类型名称的话，就用显式类型名；
      1. 如果是 `foreach` 或 `await foreach` 迭代变量类型转换的时候，除非类型必须要强制转换的时候，那就必须要用显式类型名，否则任何时候都使用关键字写法（内置类型）或者 `var`；
      1. 如果是 `using`、`await using` 的自动释放变量的话，永远用 `var`；
      1. 如果是模式匹配里的类型模式，只要类型模式的类型和 `is` 左边的表达式结果类型一致，则永远用 `var` 而不是显式类型名称；
      1. 如果是 LINQ 查询表达式的话，`from` 后的迭代变量如果需要类型转换的，就必须写出显式类型名称；
      1. 其它所有情况全部使用 `var`。

1. 如果是同行上的变量名的话，由于类型已经给定，因此在实例化语句 `new` 里，永远使用隐式 `new`（即 `new()`）而不是 `new T()`；
1. 成员名称按照如下的命名法来命名：
      1. 字段：
            1. 如果是 `static readonly` 组合修饰或是 `const` 修饰，用 `PascalCase`；
            1. 其它所有情况的字段都用 `_camelCase`。

      1. 属性、方法、字段风格的事件以及完整事件标识符：`PascalCase`；
      1. 类型名：
            1. 接口：`IPascalCase`；
            1. 和事件相关的委托：`PascalCaseEventHandler`；
            1. 其它所有类型名：`PascalCase`。

      1. 参数：`camelCase`；
      1. 泛型参数：
            1. 如果泛型参数没有泛型约束，则一般就叫 `T` 即可；
            1. 如果泛型参数是奇异递归模板模式，则命名为 `TSelf`；
            1. 如果泛型参数有泛型约束，则一般要用 `TPascalCase` 的方式带处约束规则，比如 `TStruct` 表示的是带有 `where TStruct : struct` 约束的泛型参数，接口的 `I` 要省略掉；
            1. 如果泛型参数带有多个不同的泛型约束规则，则一般按主要的泛型约束作为名称带出，比如 `TStep` 可以表示带 `where TStep : class, IStep` 约束的泛型参数，因为 `IStep` 接口是主要继承内容，所以它需要在命名规则里体现；
            1. 详细命名规范：
                  1. 没有泛型约束：`T` 或 `T名字`，具体情况看需不需要给其命名；
                  1. 奇异递归模板模式：`TSelf`；
                  1. `where T : struct`：`TStruct`；
                  1. `where T : class`：`TClass`；
                  1. `where T : unmanaged`：`TUnmanaged`；
                  1. `where T : notnull`：`TNotNull`；
                  1. `where T : Delegate`：`TDelegate`；
                  1. `where T : Enum`：`TEnum`；
                  1. `where T : 类`：`T类`；
                  1. `where T : 类?`：`TNullable类`；
                  1. `where T : I接口`：`T接口`；
                  1. `where T : I接口?`：`TNullable接口`；
                  1. `where T : 类, I接口1, I接口2, ...`：找出主要接口名为命名对象；
                  1. `where T : I接口1, I接口2, ...`：找出主要接口名为命名对象；
                  1. `where T : unmanaged, Enum`：`TEnum`；
                  1. `where T : struct, I接口, ...`：找出主要接口名为命名对象；
                  1. `where T : class, I接口, ...`：找出主要接口名为命名对象；
                  1. `where T : notnull, 类, I接口, ...`：`T类`；
                  1. 其它情况具体情况具体分析。

      1. 临时变量和临时常量：`camelCase`；
      1. 本地函数：`camelCase`；
      1. 值元组元素：`PascalCase`；
      1. 扩展方法的第一个参数名：`@this`；
      1. 解构函数的各个参数：`camelCase`。

1. `nameof` 表达式：
      1. 总是用 `nameof` 表达式来代替成员引用的名称、参数和临时变量名称，除非它不可引用；
      1. 如果是泛型类型的成员名称引用的话，也使用 `nameof` 表达式，不过因为语法不支持的关系，我们默认可对泛型参数插入 `object` 补充，比如 `A<T1, T2>` 类型下有一个字段 `Name`，要引用它的话可以使用 `nameof(A<object, object>.Name)` 来引用；
      1. 如果是复杂一点的类型名称引用的话，按照 `nameof` 表达式的表现原理，我们可以逐层表达出来，比如 `A.B.C` 可以用 `$"{nameof(A)}.{nameof(A.B)}.{nameof(A.B.C)}"` 的方式表示 `"A.B.C"`；
      1. `typeof(T).Name` 当且仅当不可代替的时候不使用 `nameof`，否则均代替为 `nameof(T)` 代替 `typeof(T).Name`。

1. `switch` 语句（注意是语句，不是 `switch` 表达式）均使用大括号来标记每一个 `case` 标签的范围；
1. `switch` 标签内如果要使用复杂的处理，需要创建方法的，多创建在 `switch` 语句整个范围的末尾，作为本地函数存储；
1. 如果是非 ASCII 字符作为代码的一部分的话（即使是放在字符串里），如果它比较常见，比如 `` `，可以直接写字面量，否则总是使用 `\u数字` 的方式引用该字符；
1. 如果用 `goto` 语句和标签的话，标签往往会比正常代码少一层缩进，除非它已经顶级了；
1. 尽量不使用单行的 `if`、`while` 等控制流程语句，而总是写出大括号然后包裹执行代码，即使只有一行；
1. 尽量按照最小的访问修饰符级别对所有成员规划访问级别，比如只用在当前程序集的字段就不要用 `public` 修饰了，一定用 `internal`；
1. 如果是顶级类型的话，如果不是只用来给当前项目用的类型，就永远都请使用 `public` 修饰，哪怕外部是 0 次引用，否则才是 `internal`；
1. 如果是嵌套类型的构造器的话，尽量使用 `internal` 防止外部调用此构造器实例化嵌套类型对象；
1. 基本不使用虚方法。除非方法必须要用来派生重写，才会带上 `virtual` 修饰符；
1. 如果类型可继续往下派生，但成员不需要继续派生重写了，并且成员是虚方法和未被密封掉的，请使用 `sealed override` 组合修饰防止对象继续派生重写；
1. 所有类型和成员（包括非 `public` 的）一般情况下全部都会带上文档注释，除非极少数没必要写文档注释的成员，或者是不能书写文档注释的地方（比如本地函数），文档注释规则默认按照基本的 Visual Studio 提供的文档注释规则进行书写文档注释；
1. 所有带有类型的文件（除了 `Globals` 文件只存储 `global` 引用指令以及程序集级别特性），默认在文件末尾都会带有一行空行；
1. `.editorconfig` 文件归纳基本所有目前 Visual Studio 提供的代码分析配置项，请主要使用它来约束代码，尽量少使用 `#pragma warning disable` 和 `#nullable disable` 等预处理指令约束文件级分析，除非是分析器的 bug。



## 代码文件范例

下面列举一个范本文件。

```csharp
namespace Sudoku;

/// <summary>
/// Encapsulates a conclusion representation while solving in logic.
/// </summary>
/// <param name="Mask">
/// Indicates the mask that holds the information for the cell, digit and the conclusion type.
/// The bits distribution is like:
/// <code><![CDATA[
/// 16       8       0
///  |-------|-------|
///  |     |---------|
/// 16    10         0
///        |   used  |
/// ]]></code>
/// </param>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="ConclusionType.Elimination"/> as the type), the instance
/// will be greater; if those two hold same conclusion type, but one of those two holds
/// the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <seealso cref="ConclusionType.Elimination"/>
public readonly record struct Conclusion(int Mask) :
	IComparable<Conclusion>,
	IDefaultable<Conclusion>,
	IEquatable<Conclusion>
#if FEATURE_GENERIC_MATH
	,
	IComparisonOperators<Conclusion, Conclusion>,
	IEqualityOperators<Conclusion, Conclusion>
#endif
{
	/// <summary>
	/// <inheritdoc cref="IDefaultable{T}.Default"/>
	/// </summary>
	public static readonly Conclusion Default = default;


	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int candidate) : this(((int)type << 10) + candidate)
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int cell, int digit) : this(((int)type << 10) + cell * 9 + digit)
	{
	}


	/// <summary>
	/// Indicates the cell.
	/// </summary>
	public int Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Mask & ((1 << 10) - 1);
	}

	/// <summary>
	/// The conclusion type to control the action of applying.
	/// If the type is <see cref="ConclusionType.Assignment"/>,
	/// this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(Mask >> 10 & 1);
	}

	/// <inheritdoc/>
	bool IDefaultable<Conclusion>.IsDefault
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == default;
	}

	/// <inheritdoc/>
	static Conclusion IDefaultable<Conclusion>.Default
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Default;
	}


	/// <summary>
	/// Deconstruct the instance into multiple values.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out int candidate)
	{
		conclusionType = (ConclusionType)(Mask >> 10 & 1);
		candidate = Mask & ((1 << 10) - 1);
	}

	/// <summary>
	/// Deconstruct the instance into multiple values.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out int cell, out int digit)
	{
		conclusionType = (ConclusionType)(Mask >> 10 & 1);
		cell = Candidate / 9;
		digit = Candidate % 9;
	}

	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTo(ref Grid grid)
	{
		switch (ConclusionType)
		{
			case ConclusionType.Assignment:
			{
				grid[Cell] = Digit;
				break;
			}
			case ConclusionType.Elimination:
			{
				grid[Cell, Digit] = false;
				break;
			}
		}
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => Mask == other.Mask;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => Mask - other.Mask;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{Cells.Empty + Cell}{ConclusionType.GetNotation()}{Digit + 1}";

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo(object? obj) =>
		obj is Conclusion comparer
			? CompareTo(comparer)
			: throw new ArgumentException($"The argument must be of type '{nameof(Conclusion)}'", nameof(obj));
#endif


	/// <inheritdoc/>
	public static bool operator <(Conclusion left, Conclusion right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	public static bool operator <=(Conclusion left, Conclusion right) => left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	public static bool operator >(Conclusion left, Conclusion right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	public static bool operator >=(Conclusion left, Conclusion right) => left.CompareTo(right) >= 0;
}

```