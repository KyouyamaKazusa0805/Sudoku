# 代码风格

本文介绍一下整个项目使用的代码的书写风格。



## 基本规则

### 类型设计以及引用

只要是变量、字段类型定义后要有等号赋值的时候：

1. 如果它是内置类型（有关键字的那种），总是用关键字书写类型，避免用别的符号，也不使用 `var`；
2. 如果是同行定义多个变量的话，只能使用显式类型名称的话，就用显式类型名；
3. 如果是 `foreach` 或 `await foreach` 迭代变量类型转换的时候，除非类型必须要强制转换的时候，那就必须要用显式类型名，否则任何时候都使用关键字写法（内置类型）或者 `var`；
4. 如果是 `using`、`await using` 的自动释放变量的话，永远用 `var`；
5. 如果是模式匹配里的类型模式，只要类型模式的类型和 `is` 左边的表达式结果类型一致，则永远用 `var` 而不是显式类型名称；
6. 如果是 LINQ 查询表达式的话，`from` 后的迭代变量如果需要类型转换的，就必须写出显式类型名称；
7. 其它所有情况全部使用 `var`。

如果是同行上的变量名的话，由于类型已经给定，因此在实例化语句 `new` 里，永远使用隐式 `new`（即 `new()`）而不是 `new T()`。

迭代器类型一般不必提供 `Dispose` 方法，因为它们不存在需要内存释放的地方。如果必须考虑内存释放，请实现良构的内存释放规则。内容请参照微软提供的 `IDisposable` 接口的释放规则。

原则上尽量不给基本数据类型（`int`、`double` 等）提供扩展方法，但你可以试着写成工具类型（静态类），然后来引用。


### 成员

尽量全部避免使用 `this.` 来引用成员。

类型的不同成员类型之间会使用双空行来分隔不同的成员，而同成员类型的成员之间会使用一行空行来分隔。

类型的成员顺序依次是：

1. 字段（字段内部排序顺序依次是：常量字段、静态只读字段、静态字段、只读实例字段、实例字段）；
2. 构造器（构造器内部排序顺序依次是：实例构造器、静态构造器、析构器，不过析构器本项目可能用不着）；
3. 属性（先静态属性，然后是实例属性）；
4. 索引器；
5. 事件（先静态事件，然后是实例事件）；
6. 方法（先实例方法，然后是静态方法）；
7. 运算符重载；
8. 类型转换器（先显式类型转换，然后是隐式类型转换）。

相同类型的成员之间，有如下的排序关系：

1. 访问修饰符级别从高到低顺序依次是 `public`、`internal`、`protected internal`、`protected`、`private protected` 和 `private`；
2. 如果访问修饰符相同，则按 `void`、`object`、内置类型、结构类型、枚举类型、类类型、委托类型、接口类型的顺序排序返回值；
3. 如果返回值类型相同，则按标识符名称的字母表顺序排序（这一点也可以不遵守）；
4. 如果标识符名相同，则按参数，按照第 2 点的类型顺序对其排序参数序列。如果第一个参数类型相同，就继续比较第二个参数，以此类推；
5. 如果是非只读结构的话，则优先列举所有的 `readonly` 修饰过的成员，然后才是没有 `readonly` 修饰的成员（注意，这个地方说的 `readonly` 是成员的修饰符，不是返回值修饰符）。

`nameof` 表达式：

1. 总是用 `nameof` 表达式来代替成员引用的名称、参数和临时变量名称，除非它不可引用；
2. 如果是泛型类型的成员名称引用的话，也使用 `nameof` 表达式，不过因为语法不支持的关系，我们默认可对泛型参数插入 `object` 补充，比如 `A<T1, T2>` 类型下有一个字段 `Name`，要引用它的话可以使用 `nameof(A<object, object>.Name)` 来引用；
3. 如果是复杂一点的类型名称引用的话，按照 `nameof` 表达式的表现原理，我们可以逐层表达出来，比如 `A.B.C` 可以用 `$"{nameof(A)}.{nameof(A.B)}.{nameof(A.B.C)}"` 的方式表示 `"A.B.C"`；
4. `typeof(T).Name` 当且仅当不可代替的时候不使用 `nameof`，否则均代替为 `nameof(T)` 代替 `typeof(T).Name`。


### 命名

成员名称按照如下的命名法来命名：
1. 字段：
    1. 如果是 `static readonly` 组合修饰或是 `const` 修饰，用 `PascalCase`；
    2. 如果是 `public` 修饰，用 `PascalCase`；
    3. 其它所有情况的字段都用 `_camelCase`。
2. 属性、方法、字段风格的事件以及完整事件标识符：`PascalCase`；
3. 类型名：
    1. 接口：`IPascalCase`；
    2. 事件相关的委托：`PascalCaseEventHandler`；
    3. 其它所有类型名：`PascalCase`。
4. 参数：`camelCase`；
5. 泛型参数：
    1. 如果泛型参数没有泛型约束，则一般就叫 `T` 即可；
    2. 如果泛型参数是奇异递归模板模式，则命名为 `TSelf`；
    3. 如果泛型参数有泛型约束，则一般要用 `TPascalCase` 的方式带处约束规则，比如 `TStruct` 表示的是带有 `where TStruct : struct` 约束的泛型参数，接口的 `I` 要省略掉；
    4. 如果泛型参数带有多个不同的泛型约束规则，则一般按主要的泛型约束作为名称带出，比如 `TStep` 可以表示带 `where TStep : class, IStep` 约束的泛型参数，因为 `IStep` 接口是主要继承内容，所以它需要在命名规则里体现；
    5. 详细命名规范：
        1. 没有泛型约束：`T` 或 `T名字`，具体情况看需不需要给其命名；
        2. 奇异递归模板模式：`TSelf`；
        3. `where T : struct`：`TStruct`；
        4. `where T : class`：`TClass`；
        5. `where T : unmanaged`：`TUnmanaged`；
        6. `where T : notnull`：`TNotNull`；
        7. `where T : Delegate`：`TDelegate`；
        8. `where T : Enum`：`TEnum`；
        9. `where T : 类`：`T类`；
        10. `where T : 类?`：`TNullable类`；
        11. `where T : I接口`：`T接口`；
        12. `where T : I接口?`：`TNullable接口`；
        13. `where T : 类, I接口1, I接口2, ...`：找出主要接口名为命名对象；
        14. `where T : I接口1, I接口2, ...`：找出主要接口名为命名对象；
        15. `where T : unmanaged, Enum`：`TEnum`；
        16. `where T : struct, I接口, ...`：找出主要接口名为命名对象；
        17. `where T : class, I接口, ...`：找出主要接口名为命名对象；
        18. `where T : notnull, 类, I接口, ...`：`T类`；
        19. 其它情况具体情况具体分析。
6. 临时变量和临时常量：`camelCase`；
7. 本地函数：`camelCase`；
8. 值元组元素：`PascalCase`；
9. 扩展方法的第一个参数名：`@this`；
10. 解构函数的各个参数：`camelCase`；
11. 带有扩展方法的静态类名：`类型名Extensions`。


### 修饰符

任何时候我们都会写出成员的访问修饰符，即使它是默认的情况，除非这个成员不支持使用访问修饰符（比如静态构造器等等）。

尽量按照最小的访问修饰符级别对所有成员规划访问级别，比如只用在当前程序集的字段就不要用 `public` 修饰了，一定用 `internal`。

如果是顶级类型的话，如果不是只用来给当前项目用的类型，就永远都请使用 `public` 修饰，哪怕外部是 0 次引用，否则才是 `internal`。

如果是嵌套类型的构造器的话，尽量使用 `internal` 防止外部调用此构造器实例化嵌套类型对象。

基本不使用虚方法。除非方法必须要用来派生重写，才会带上 `virtual` 修饰符。

如果类型可继续往下派生，但成员不需要继续派生重写了，并且成员是虚方法和未被密封掉的，请使用 `sealed override` 组合修饰防止对象继续派生重写。

修饰符顺序按照 Visual Studio 默认提供的排序顺序对修饰符排序：

1. 访问修饰符：`public`、`internal`、`protected`、`protected internal`、`private protected`、`private`；
2. 静态修饰符：`static`；
3. 外部修饰符：`extern`；
4. 继承控制修饰符：`new`、`virtual`、`abstract`、`sealed`、`override`；
5. 只读修饰符：`readonly`；
6. 不安全代码修饰符：`unsafe`；
7. 跨线程修饰符：`volatile`；
8. 异步修饰符：`async`。


### 流程控制

尽量不使用单行的 `if`、`while` 等控制流程语句，而总是写出大括号然后包裹执行代码，即使只有一行。

`switch` 语句（注意是语句，不是 `switch` 表达式）均使用大括号来标记每一个 `case` 标签的范围。

`switch` 标签内如果要使用复杂的处理，需要创建方法的，多创建在 `switch` 语句整个范围的末尾，作为本地函数存储。

如果用 `goto` 语句和标签的话，标签往往会比正常代码少一层缩进，除非它已经顶级了。

循环语句尽量用 `for` 以及 `while` 循环。在极少数时候也可以用 `do-while` 循环。

对于集合类型，虽然 `foreach` 循环有时候会比普通的循环或者是指针操作要慢（考虑性能的时候），但项目仍使用 `foreach` 循环来提升保证可读性。

对于自己实现的集合类型，如果要实现迭代器，尽量不使用 `yield return` 和 `yield break` 反馈数据，而是自己实现迭代器类型。

如果类型的成员只有一行执行代码（比如不返回任何数值的方法调用，或者是 `return` 语句），总是建议使用 Lambda 风格的成员表述语法来表达，比如 `public int Method() => 42;` 而不是使用大括号写法 `public int Method() { return 42; }`。

Lambda 风格的成员表述语法下，`=>` 符号总是放在和成员声明签名部分一致的同一行的末尾处，而不换行；相反，将唯一执行的返回结果/表达式/过程换行书写，除非它很短，可以写在同一行上。


### 弃元

如果在模式匹配里需要弃元的，只要可以不写，就不写出弃元符号 `_`，例如：

* 属性模式：`obj is { Prop: _ }`
* 属性模式后定义的变量：`obj is { } _`

等等。但请注意，少数时候弃元符号不可省略，例如 C# 8 提供的 `switch` 表达式的最后一个默认分支、非执行语句的表达式的返回值等。那么它们必须给出弃元符号。

对于方法执行来说和兼容早期调用规则规范，如果不使用方法的返回值，一般不写弃元符号，除非该方法包含 `out` 参数并且使用了该参数，而放弃了返回值的时候，才给出弃元符号，比如 `_ = Method(out var a)`，虽然这样也可以不写弃元符号的赋值过程，但这样表述可以更加清晰地告知用户，返回值是故意舍弃的，并且我只用的是该方法的 `out` 参数的数值。

对于 C# 7 的 `out` 内联变量语法，我们总是使用 `out _` 的写法来代替 `out var _` 表示弃元。

下面展示一个复杂的语句，以展示如何在项目里使用弃元的基本规范：

```csharp
if (
    expr is
    {
        Prop:
        {
            NestedProp1: [_, .., _] _,
            NestedProp2: T (x: _, y: _, z: _) { } _
        } _
    } _
)
{
    // ...
}
```

如果 `expr` 是值类型的话，那么上述代码按照项目的规范可简化为：

```csharp
if (expr.Prop: { NestedProp1: [_, .., _], NestedProp2: T (x: _, y: _, z: _) })
{
    // ...
}
```


### `using` 指令

永远都是使用 `global using` 和 `global using static` 指令（C# 10 起可用）来引用命名空间，而在每一个代码文件都不出现任何的 `using` 和 `using static` 指令，除非在文件定义临时的长类型名的 `using` 类型别名。

`global using` 和 `global using static` 指令必须是按字典序排序。


### 文档注释

所有类型和成员（包括非 `public` 的）一般情况下全部都会带上文档注释，除非极少数没必要写文档注释的成员，或者是不能书写文档注释的地方（比如本地函数）。

文档注释规则默认按照基本的 Visual Studio 提供的文档注释规则进行书写文档注释。

`summary` 块总是换行，而不写在同一行。详细写法可参照文末给出的代码范例。

如果方法带有参数，要么总是全部参数都给出注释文字，要么全部参数都不给出注释文字。

如果 `list` 的 `type` 属性为 `table` 的时候，`listheader` 块可有可无，但 `item` 必须同时提供 `term` 和 `description` 块。

如果 `list` 的 `type` 属性不是 `table` 的时候，一般不提供 `listheader` 块，并且 `item` 里默认省略不写 `term` 块，直接按照 `item` 内包含注释文字的模式来书写，而不是 `item` 里包含唯一的 `term` 块，然后再在 `term` 里写注释文字，这样层级会更多，导致文档注释在代码里的可读性降低。

`list` 块里的文字原则上不缩进，虽然缩进之后可能看着可读性比不缩进要高，但是文档注释的嵌套规则一般是若干 `item` 块平行书写，因此定界很方便。


### 缩进、空格、换行等排版规范

整个解决方案均使用的是 [Allman 缩进方式](https://en.wikipedia.org/wiki/Indentation_style#Allman_style)，即大括号上下对应，而不是将开大括号放在代码末尾不换行。

缩进是用的 Tab 而不是四个空格。

避免任何排版之前忘记的插入空格，比如说 `if` 和 `(` 之间的空格、`a + b` 表达式里 `a` 和 `+` 符号之间的空格以及 `+` 和 `b` 之间的空格等等。

所有带有类型的文件（除了 `Globals` 文件只存储 `global` 引用指令以及程序集级别特性），默认在文件末尾都会带有一行空行。

如果是非 ASCII 字符作为代码的一部分的话（即使是放在字符串里），如果它比较常见，比如 `` `，可以直接写字面量，否则总是使用 `\u数字` 的方式引用该字符。

尽量保证代码在同一行上的字符总数不超过 Visual Studio 代码编辑器的宽度。如果超出了的话，你需要移动横向的滚动条来查看后面的代码，这样影响写代码和阅读代码的体验。如果不超过界面的话，可以放在同一行上；如果会超出界面宽度，请换行书写。

代码换行总是采用 Allman 缩进，但它只针对于大括号有效；开小括号 `(`（例如方法调用的参数表列的那个括号）仍然跟在代码之后，不换行；而开中括号 `[` 可以随意，一般建议和开小括号 `(` 的规范一致。

如果类型带有泛型参数，那么类型名称会很长的时候，它是特殊情况，可以不换行。但我更建议对于这种类型单独写一行 `using` 指令代替掉长类型名。

在模式匹配里，大括号 `{}` 书写要遵循 Allman 缩进方式，而小括号 `()` 则不遵循；同样地，中括号 `[]` 看你自己，可以换行也可以不换行，但我建议和小括号一致，不换行。

如果 `if` 条件里的代码较长需要换行的时候，需要遵循如下的规则：

1. 如果所有的内容只包含布尔条件之间的 `&&`、`||`、`&`、`|`、`^` 以及 `!` 这些基本运算，而不包括小括号的时候，可以将每一个条件单独写在一行里，第一个条件直接跟在 `if(` 的开小括号 `(` 之后书写，而后面的条件则直接换行到下一行；后面的条件同理；
2. 如果条件包含组合情况（小括号），则考虑将小括号单独列成一行书写，而小括号里的内容单独写在下面的行里，并增加一层缩进；
3. 如果条件包含模式匹配等复杂运算的，建议按照这些复杂运算过程的规则排版之后，将 `if(...)` 条件的 `if(` 和 `)` 两个部分单独列成一行，而条件部分不再跟着 `(` 后直接书写，而不是跟着 `(` 之后。

如果 `for` 循环的初始化部分、条件部分或增量部分包含复杂代码，一律按照 `if` 如此的排版规则将其单独成行放置，而 `for` 循环每一个部分末尾的分号 `;` 跟着当前部分的最后一部分代码后直接书写，而不单独换行；

如果 `while` 条件和 `do-while` 条件部分过长，也按前面介绍的方式排版。

如果 `foreach` 循环的集合变量表达式过长，则单独给集合变量换行书写。

如果要赋值例如 LINQ 查询表达式一样的语句，我们尽量建议将 `变量 = 表达式` 的等号 `=` 后直接换行，然后在下一行开始表达式的书写，而不是紧跟着书写，否则这样变量的长度会影响表达式的书写，导致 tab 和空格的混用，非常不好。

所有运算符如果期间要换行，都请考虑在上一层排版的基础上多一层缩进，比如 `a = c ? t : f` 的 `t` 和 `f` 换行的时候，需要给它们都多加一层缩进。

如果是条件运算符 `?:` 表达式换行的话，建议将符号 `?` 和 `:` 也带着换行，而不是写在上一行的末尾。

`?.` 和 `!.` 运算需要换行的时候，建议将 `?` 和 `!` 以及成员访问运算符 `.` 拆开写：`?` 和 `!` 写在上一行末尾跟着前面的代码，而成员访问运算符 `.` 放在下一行去，并且给成员访问运算符 `.` 这行代码增加一层缩进。

如果是派生继承关系的 `:` 后跟的类型过多，我们要保留冒号和前面类型声明在同一行，然后将所有的实现接口类型以及基类型全部换行书写，然后增加一层缩进。

如果是调用基类构造器 `: base()` 和当前类型的别的构造器 `: this()` 的话，也将冒号跟在前面的代码，然后将调用的内容换行并增加一层缩进。


### Editorconfig 文件

`.editorconfig` 文件归纳基本所有目前 Visual Studio 提供的代码分析配置项，请主要使用它来约束代码，尽量少使用 `#pragma warning disable` 和 `#nullable disable` 等预处理指令约束文件级分析，除非是分析器的 bug。



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
    public Conclusion(ConclusionType type, int candidate) :
        this(((int)type << 10) + candidate)
    {
    }

    /// <summary>
    /// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
    /// </summary>
    /// <param name="type">The conclusion type.</param>
    /// <param name="cell">The cell.</param>
    /// <param name="digit">The digit.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Conclusion(ConclusionType type, int cell, int digit) :
        this(((int)type << 10) + cell * 9 + digit)
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
