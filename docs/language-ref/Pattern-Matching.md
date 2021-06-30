# 模式匹配
## 模式匹配是啥？

**模式匹配**（Pattern Matching），在编程里指的是，把一个不知道具体数据信息的玩意儿，通过一些固定的语法格式来确定模式数据的具体内容的过程。

C# 的模式匹配非常美丽，因为语法格式非常有趣、简单、且更具有可读性。下面我们来看看 C# 的模式匹配。

## C# 各种各样的模式

C# 里有很多种类的、用来控制和判断数据的**模式**（Pattern），它们语法不同，判断的东西、语义也不一样。下面我们列举它们。

### 声明模式（Declaration Pattern）

声明模式用于简单判断一个模糊的数据类型是否是某个具体的数据类型，并尝试将其转换过去。

```csharp
object greeting = "Hello, World!";
if (greeting is string message)
{
    Console.WriteLine(message.ToLower());
}
```

注意语法 `greeting is string message` 的写法。C# 最开始允许 `is` 的写法是 `obj is T`，而 `T` 之后写的变量指的是“如果 `obj` 确实是 `T` 类型的实例的话，那么 `message` 就可以使用了”。换句话说，这段代码等价于下面这样的代码：

```csharp
object greeting = "Hello, World!";
if (greeting is string)
{
    string message = (string)greeting;
    Console.WriteLine(message.ToLower());
}
```

即在大括号里等效进行类型转换。

### `var` 模式（`var` Pattern）

#### 语法

有一些时候，我们可以内联模式匹配和变量声明。

```csharp
static bool IsAcceptable(int length, int absLimit)
{
    return SimulateDataFetch(length) is var results
        && results.Min() >= -absLimit
        && results.Max() <= absLimit;
}

static int[] SimulateDataFetch(int length)
{
    var rand = new Random();
    int[] result = new int[length];
    for (int i = 0; i < length; i++)
    {
        result[i] = rand.Next(-100, 100);
    }

    return result;
}
```

我们来看下这个例子。`SimulateDataFetch` 方法获取指定长度的数组，数组的每个元素都是 -100 到 100 之间的随机数。`IsAcceptable` 方法则是验证序列是不是在某个数字的范围内。可以从例子里看出，我们直接将方法调用的结果直接内联到 `return` 语句里，我们写的是 `is var results`。这个语句和下面这段代码相当：

```csharp
int[] results = SimulateDataFetch(length);
return results.Min() >= -absLimit && results.Max() <= absLimit;
```

#### 声明模式和 `var` 模式的区别

请稍微注意一下。`var` 模式和声明模式的书写格式完全一样，唯一的区别是，一个写的是类型的具体名称，一个则是写的固定的关键字 `var`。声明模式下，写的数据具体类型会作为数据的判断类型进行判断；而 `var` 仅等价于变量声明，它并不具有任何的数据类型的判断。

虽然我们可以看到，`var` 模式的例子里，由于 `SimulateDataFetch` 方法返回的 `int[]` 类型是固定的，因此我们完全可以写成 `SimulateDataFetch(length) is int[] results`。但是，由于这个写法写的是具体类型，因此 C# 会做一次没有意义的数据判断：判断结果是不是 `int[]` 类型的。但显然这个判断是没有意义的，因此我们建议在这种“变量内联到条件里”的情况，使用 `var` 模式而不是声明模式。

### 常量模式（Constant Pattern）

#### 语法

常量模式针对于一个可能为 `null` 的数据类型的实例，判断是否等于某个具体的常量。

```csharp
object o = 3;
if (o is 30)
    Console.WriteLine("The condition is true.");
else
    Console.WriteLine("The condition is false.");
```

在这段代码下，我们直接在 `is` 后紧跟一个数值，这表示将 `object` 进行类型和数值的双重判断。等价的代码是这样的：

```csharp
object o = 3;
if (o is int i && i == 30)
    Console.WriteLine("The condition is true.");
```

那么，为什么非得是 `int` 呢？因为这里的 3 这个字面量，默认是 `int` 类型的，因此这里在等价代码里用的是 `int` 作为类型的判断。

#### 可空值类型的常量模式匹配是不必要的

另外，我们也可以对一个可空值类型使用这样的常量模式判断。

```csharp
int? i = null, j = 30;
if (i is 3 && j is 3)
    Console.WriteLine("The condition is true.");
else
    Console.WriteLine("The condition is false.");
```

比如这个格式。这个格式很明显等价于 `i is int p && p == 3 && j is int q && q == 3`。正是因为写起来很长，所以我们才会使用这样的模式匹配来简化代码。不过，这个模式匹配是不必要的。早在可空值类型出现的时候，它们自身的等号和不等号的逻辑就已经可具有这样的判断了。换句话说，你写成这样，和模式匹配的格式将是一样的等价代码。

```csharp
int? i = null, j = 30;
if (i == 3 && j == 3)
    Console.WriteLine("The condition is true.");
else
    Console.WriteLine("The condition is false.");
```

这里，`i` 就算是 `null`，也不会影响判断 `i == 3` 的结果。大不了结果为 `false` 就是了。

### 对位模式（Positional Pattern）

对位模式是将一个数据成员通过解构方法来产生解构，来判断属性数据的过程。假设我们拥有这样一个数据类型：

```csharp
public readonly struct Point
{
    public int X;
    public int Y;
}
```

显然这里的 `X` 和 `Y` 是 `Point` 里仅存的两个数据成员。如果我们在某个时刻判断数据信息的具体数值的时候，我们可能会使用如下的写法：

```csharp
if (point.X == 30 && point.Y == 30)
    // ...
```

在 C# 里，我们只要写上一个自定义的解构函数，就可以对对象进行解构操作。我们写一个 `Deconstruct` 方法，然后带有两个参数：`x` 和 `y`，它们都是 `out int` 类型的。

```csharp
public void Deconstruct(out int x, out int y)
{
    x = X;
    y = Y;
}
```

通过这样的赋值后，我们就可以通过这个解构来完成数据的解构了：

```csharp
var point = new Point { X = 10, Y = 60 };

// Deconstruct.
var (x, y) = point;
```

当然，在模式匹配的时候，我们可以使用这样的代码对上面的写法进行简化：

```csharp
if (x == 30 && y == 30)
    // ...
```

不过，这样还不是很好看。因此 C# 还有这样的对位模式匹配：

```csharp
if (point is (x: 30, y: 30))
    // ...
```

通过一对小括号，我们可以实现对这个数据成员具体数值的检测。至于这里为什么是小写字母 `x` 和 `y`，是因为解构函数的参数分别是 `x` 和 `y`，这是一一对应的。正是因为如此，这个模式才叫做对位模式。其中，因为它借助了解构函数，而解构后的对象分配使用的是一对小括号（`var (x, y) = point;` 这个语句），因此为了配合这个写法，也用的是小括号。只是这里需要写出解构参数的名字。

稍微注意一点的是，C# 允许在同一个数据类型下定义多个解构函数，所以这正是为什么必须给出解构参数名称的原因。除非这个类型就这一个解构函数，那么我们都必须写出解构参数名。

### 解构模式（Deconstruct Pattern）

#### 语法

因为前文我们拥有了解构函数，也拥有了 `var` 模式，因此 C# 灵活的语法提供了 `var` 模式的解构版本：

```csharp
if (point is var (x, y) && x == 30 && y == 30)
    // ...
```

稍微注意一下这里的语法是写成 `var (x, y)`。当然，你也可以内联 `var` 关键字。和值元组的语法一致，你依然可以用 `(var x, var y)` 的语法。

```csharp
if (point is (var x, var y) && x == 30 && y == 30)
    // ...
```

这样是可以的。

#### 可空值类型解构模式的别样意义

在 C# 里，可空值类型一直是一种方便也不方便的数据类型。它的声明和使用都比较方便，但问题就出在它可能是 `null` 数值。假设前文的 `Point` 我们用的是可空类型的话：

```csharp
Point? nullable = new Point { X = 30, Y = 30 };
```

此时，我们在后续的代码里，无从根据代码直接确定 `nullable` 是否为 `null`（除非看取了 `nullable` 的值才行）。因此，一旦我们对这个类型进行解构：

```csharp
if (nullable is var (x, y))
    // ...
```

这就不单纯和 `var` 模式一样。它牵扯到数据是不是 `null` 才可解构的问题。如果数据都是 `null` 了，我们就无法解构。因此，可空值类型的解构模式会先判断对象是不是不为 `null`，然后才是解构。

```csharp
if (nullable != null && nullable.Value is (x: var x, y: var y))
    // ...
```

> `nullable != null` 和 `nullable.HasValue` 是等效的，所以写 `nullable.HasValue` 也没问题。

#### 弃元模式

一旦解构后，我们就有办法只判断其中的一个数据。假设前文的解构函数存在的话，那么我们必然会解构成两个数据（`x` 和 `y`）。但是，如果我们仅判断 `x` 的数据，而不关心 `y` 是多少的话，我们可以使用一个下划线 `_` 来表示“`y` 我们不用判断”，或者说“`y` 的模式匹配总是成立的”。

```csharp
if (nullable is (x: 30, y: _))
    // ...
```

或者

```csharp
if (nullable is var (x, _) && x == 30)
    // ...
```

这么写都是可以的。

### 类型模式（Type Pattern）

#### 语法

与其单独讲类型模式，还不如让你先明白，声明模式的那个类型，就是类型模式。

```csharp
if (list is int[] arr)
    // ...
```

所以，你干脆理解成这样：“声明模式 = 类型模式（就是这个类型）+ 变量定义”。但是，单独提出来说，是有原因的。

#### 声明模式弃元

在 C# 里，`switch` 语句可以专门对一个不知道是什么类型的东西作模式匹配：

```csharp
switch (obj)
{
    case int[] arr: // ...
    case IEnumerable<int> enumerable: // ...
    case List<int> list: // ...
    default: // ...
}
```

这里，C# 也是允许的。可问题在于，`arr`、`enumerable` 等变量如果不用，我们无法去掉：C# 9 之前，这个变量是不可省去的：即使不用，你也得写弃元符号：`_`。

```csharp
switch (obj)
{
    case int[] _: // ...
    case IEnumerable<int> _: // ...
    case List<int> _: // ...
    default: // ...
}
```

不过，从 C# 9 开始，弃元符号就可以不写了。于是乎，模式匹配就可以简写成真正的类型模式了：

```csharp
switch (obj)
{
    case int[]: // ...
    case IEnumerable<int>: // ...
    case List<int>: // ...
    default: // ...
}
```

### 属性模式（Property Pattern）

#### 语法

属性模式是用于专门体现对象的属性信息的匹配模式。我们使用一对大括号来表达参数是否必须满足这个数值信息。

假如，我们现在的 `Point` 类型的 `X` 和 `Y` 不再使用字段表达，而是用属性来表达：

```csharp
public readonly struct Point
{
    public int X { get; }
    public int Y { get; }
}
```

那么，我们即使不给出解构函数，也可以使用属性的方式来对每一个成员信息进行判断：

```csharp
if (point is { X: 30, Y: 30 })
    // ...
```

属性模式专门给属性提供数据判断的服务，因此这种模式叫属性模式。

#### 属性模式的弃元

一般来说，属性模式下，由于不需要依赖于解构函数，因此属性是可以写出来判断的；反过来说，如果属性不判断的话，那么写出来就没意义了。不过 C# 的语法允许我们使用弃元来默认通过某个属性的判定：

```csharp
if (point is { X: 30, Y: _ })
    // ...
```

这样的话，`Y` 属性是永真式，即不用判断了。说白了，这里的 `Y: _` 是可以不写的。只是 C# 允许这种语法存在，体现出了语法的灵活性。

#### 空属性模式及变量声明内联

如果属性模式里的成员为空，那么它表示什么呢？

```csharp
if (nullable is { } point)
    // ...
```

是的，对于可空类型（不管是值类型也好，还是引用类型也好），都表示“不为 `null`”。比如 `nullable` 是一个可空的 `Point` 类型，那么 `is { }` 就表示 `nullable.HasValue`。当满足条件后，我们用 `point` 表示这个 `Point` 类型的数据。

从这个例子里，我们可以得到的若干信息是这些：

1. `is { }` 表示“不为 `null`”，适用于任何可为空的类型；
2. 大括号后可继续内联一个变量，和 `is T variable` 写法格式（声明模式）一致，但是，注意内联的这个变了和原始变量的类型和可空语义的不同：被匹配的变量（原始变量）是可空的，但是内联的后者这个变量是一定不空的。

> C# 是允许变量声明的内联作为模式匹配的一部分的。这里仅用空属性模式介绍了内联变量的写法，但你要知道的是，内联变量可用在任何情况下的属性模式。

#### 递归的模式匹配

C# 强大的地方在于，语法很灵活，这样我们写代码可以不用唯一的一条道路去实现。比如前面的解构模式。`(x: var x, y: var y)` 里又是一个 `var` 模式的变量声明。所以，正是因为这样，我们学 C# 就不必学得那么痛苦。

C# 的属性模式是 C# 一大秀儿语法。它允许递归使用属性模式进行判断。假设我有这么一个对象：

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public Person? Father { get; set; }
    public Person? Mother { get; set; }
}
```

这个对象是表示一个人的基本数据信息，比如名字啊、年龄啥的，当然也存储了 ta 的父母的实例的引用。

> 其中，我们假设 `Gender` 类型是个暂时只包含 `Male` 和 `Female` 俩字段的枚举类型。
>
> `Person?` 语法表示 `Person` 这个引用类型具有和值类型类似的语法：这个属性信息可为 `null`。反之，如果没有 `?` 标记的类型，这个成员的数值就不能为 `null`。这个语法是 C# 8 里的，这里为了体现出判断用法，故意写上了 `?` 来表达为 `null`、更显眼一点；另外，这里故意取可为 `null` 的写法，还有一个目的，是为了体现一会儿模式匹配的语义，所以请不要和现实世界进行对比或者对号入座。

假如，我们要判断是否某个人的姓名是“张三”、年龄 24，他爸叫“张二”、而他的妈妈则叫“李四”。如果要判断这个对象的具体信息，我们可以这么写代码：

```csharp
if (
    zhangSan is
    {
        Name: "Zhang San",
        Age: 24,
        Father: { Name: "Zhang 'er" },
        Mother: { Name: "Li si" }
    }
)
{
    Console.WriteLine("Zhang san does satisfy that condition.");
}
```

注意这里的模式匹配写法。前面模式匹配就用的是大括号，因此我们可以对对象的内部信息继续作判断。比如 `Father` 和 `Mother` 属性又是一个 `Person` 类型的对象，因此我们还可以接续一个大括号对 `Father` 和 `Mother` 的值的具体内容继续进行判断。

一定要注意。`Father` 和 `Mother` 属性是可能为 `null` 的。当 `Father` 属性的数值本身就是 `null` 的时候，那么显然就不存在 `Name: "Zhang 'er"` 的判断行为了：因为 `null` 值本身就无法继续判断内部数据了。因此，在 `Father` 为 `null` 的时候，模式匹配结果一定是 `false`。当且仅当整个判断的逻辑全都匹配，`if` 条件才成立。

顺带给大家看下，C# 的模式匹配到底多有魅力：给大家展示一个我之前写过的一段代码，用到了这里的模式匹配。

```csharp
if (
    node is
    {
        Expression: MemberAccessExpressionSyntax
        {
            RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
            Expression: MemberAccessExpressionSyntax
            {
                RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
                Expression: IdentifierNameSyntax
                {
                    Identifier: { ValueText: "TextResources" }
                },
                Name: IdentifierNameSyntax
                {
                    Identifier: { ValueText: "Current" }
                }
            },
            Name: IdentifierNameSyntax
            {
                Identifier: { ValueText: var methodName }
            } nameNode
        },
        ArgumentList: var argList
    }
)
{
    // ...
}
```

这里，这么一大坨都是递归的模式匹配。正好这体现出了模式匹配的魅力。

### 关系模式（Relational Pattern）

#### 语法

前面的模式可以解决一大部分的问题了，但是有些时候，数据判断和取值无法对一个范围来判断，因此还不够灵活。C# 里还有关系模式，来对数据的范围来判断。

```csharp
if (obj is > 30)
    // ...
```

即使 `obj` 不是 `int` 类型，我们依旧可以这么写。这个代码等价于 `obj is int i && i > 30`。

C# 允许 `>`、`>=`、`<` 和 `<=` 四个运算符，写在 `is` 后，来表达范围判断。稍微注意一下的地方是，`is > 30` 的 30 必须是常量才行。

给大家看一个例子：

```csharp
Console.WriteLine(Classify(13)); // output: Too high
Console.WriteLine(Classify(double.NaN)); // output: Unknown
Console.WriteLine(Classify(2.4)); // output: Acceptable

static string Classify(double measurement) => measurement switch
{
    < -4.0 => "Too low",
    > 10.0 => "Too high",
    double.NaN => "Unknown",
    _ => "Acceptable",
};
```

不过怎么理解，就靠你自己了。

#### 不推断类型的时候，不要用模式匹配

正是因为出了这个模式，下面两句话就变成等价的了：

```csharp
int v = 30;

bool condition1 = v > 30;
bool condition2 = v is > 30;
```

显然，要不要 `is`，语句都可以理解。但是，有 `is` 需要模式匹配，因此显然复杂一点。因此，我们建议在数据类型不用判断的时候，不要使用 `is`。当然，这里说的结论指的是这里这种情况。

### 逻辑模式（Logical Pattern）

因为模式匹配里的每个模式并不是一个“数据信息”，因此我们无法直接对模式用 `&&`、`||` 等符号来进行拼接组合。C# 为了解决这个问题，多了三个关键字：`and`、`or` 和 `not` 来拼接模式。

#### 合取模式

合取模式用 `and` 拼接模式，来表达这些模式都必须成立。

```csharp
static bool IsLowerLetter(char c) => c is >= 'a' and <= 'z';
```

比如这里，`>= 'a' and <= 'z'` 整个表达式用来表达，`>= 'a'` 和 `<= 'z'` 两个条件必须都满足。如果要写分开，就必须写成 `c is >= 'a' && c is <= 'z'`。

#### 析取模式

析取模式用 `or` 拼接。

```csharp
static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
```

注意，`or` 拼接了前面 `>= 'a' and <= 'z'` 和后面 `>= 'A' and <= 'Z'` 两个模式。`or` 表示两个模式有一个模式能够匹配成功就可以。

#### 取反模式

取反模式用 `not`。

```csharp
if (input is not null)
    // ...
```

最常见的就是这里。我们如果判断对象是不是不为 `null`，那么我们最常用的就是写成 `is not null`。`is null` 属于前面的常量模式，判断对象是不是 `null`。它和 `==` 运算符的区别是，`==` 运算符可重载，重载会影响 `==` 的判断和使用逻辑；而 `is` 是永远不变的判断模式。

#### 混用析取、合取和取反

当然，你也可以混用到 `and` 和 `or` 关键字拼接起来的模式里。

```csharp
if (ch is >= '0' and <= '9' or '.')
    // ...
```

这表示 `ch` 是不是字符 0 到 9，或者是小数点。将其取反：

```csharp
if (ch is not (>= '0' and <= '9' or '.'))
    // ...
```

这就表示取反。当然了，你可以使用类似数学知识，将 `not` 套到小括号里：

```csharp
if (ch is not (>= '0' and <= '9' or '.')) ;
if (ch is not ((>= '0' and <= '9') or '.')) ;
if (ch is not (>= '0' and <= '9') and not '.') ;
if (ch is (not >= '0' or not <= '9') and not '.') ;
```

这 4 行内容可以帮助你理解和拼接模式的具体内容。

#### 合取式、析取式和取反式的优先级

稍微注意一下。合取式 `and` 和数学上是一样的，比 `or` 更优先推理，因此无需对 `and` 和 `or` 模式一起的复杂模式匹配添加括号：

```csharp
static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
```

比如这样，`(>= 'a' and <= 'z')` 和 `(>= 'A' and <= 'Z')` 的小括号可以不要。

取反式的话，因为它只和一个模式结合使用，不像是 `and` 和 `or` 需要两个模式结合，因此 `not` 的优先级比 `and` 和 `not` 都要高。所以，上面的例子里，这个写法你应该是知道哪些地方省略了小括号。

```csharp
if (ch is (not >= '0' or not <= '9') and not '.') ;
```

### 扩展属性模式（Extended Property Pattern）

因为属性模式本身有些地方很鸡肋，因此出现了扩展属性模式。扩展属性模式允许将层级的大括号缩减成 `A.B.C` 的形式。

```csharp
if (
    zhangSan is
    {
        Name: "Zhang San",
        Age: 24,
        Father: { Name: "Zhang 'er" },
        Mother: { Name: "Li si" }
    }
)
{
    Console.WriteLine("Zhang san does satisfy that condition.");
}
```

这是之前的属性递归介绍的代码。这个写法里，`Father` 里再次包含一层大括号。扩展属性模式允许将这个代码简写为 `Father.Name`：

```csharp
if (
    zhangSan is
    {
        Name: "Zhang San",
        Age: 24,
        Father.Name: "Zhang 'er",
        Mother.Name: "Li si"
    }
)
{
    Console.WriteLine("Zhang san does satisfy that condition.");
}
```

即少一个大括号的层级级别。

### 长度模式（Length Pattern）

#### 语法

C# 拥有各种各样的集合类型，但是它们不统一的地方是，有些用 `Count` 表示长度，有些则是 `Length`。C# 为了避免这个差异，长度模式匹配的时候，可以使用中括号来匹配它们：

```csharp
object o = new int[5] { 1, 3, 5, 7, 9 };

if (o is int[] [10])
    // ...
```

注意这里的 `int[] [10]` 写法。`int[]` 是判断 `o` 是不是 `int[]` 类型的。如果是，则判断模式 `[10]`。是的，这是一个模式，叫做长度模式，判断这个集合是不是长度为 10。在 `int[]` 里等价于 `is int[] { Length: 10 }`；不过对于一些别的集合，可能 `Length` 就得换成 `Count` 了。在此啰嗦一下，如果一个集合既有 `Length` 又有 `Count` 属性的话，实际判断的时候用的是前者，即 `Length`。

#### 模式的适用范围

虽然这个写法统一了集合的判别模型，但请注意，对于一个普通变量来说，要想使用此模式匹配的话，如果不写类型的时候，那么最大的判别范围是 `IEnumerable<T>`。换句话说，如果不写上类型模式（即那个类型名称的话），它等价于 `is IEnumerable<T> [length]` 这个写法。

你可能会问我，`IEnumerable<T>` 里哪有 `Length` 或 `Count` 属性啊？是的，这是 C# 团队指定的特殊例子。只有这个集合下，不需要有这俩属性也可以使用此判断模式。但需要注意的是，一般来说，我们都必须写上集合的类型，然后再写长度。如果是 `IEnumerable<T>` 的话，对模式匹配来说会有性能上的影响。

### 列表模式（List Pattern）

#### 语法

为了将集合的元素提取出来判断，C# 拥有了列表模式。因为有时候需要使用索引器来取元素，但索引器不包含模式匹配的语法，因此还不足以灵活语法，因此列表模式就诞生了。

列表模式是将一个不知道是不是集合的对象，用列表的格式列举出来，对其中的元素挨个进行判断的模式。

我们使用一对中括号进行判断。使用范围记号 `..` 来表达“这是一个范围”。举个例子：`[1, .., 3]` 表示判断一个序列的第一个元素是不是 1，而最后一个元素是不是 3。所以，自然这个写法就等价于下面这个格式了：

```csharp
if (arr is [10] [1, .., 3])
    // ...
```

它等价于

```csharp
if (arr.Length == 10 && arr[0] == 1 && arr[^1] == 3)
    // ...
```

这里的 `^1` 是 C# 8 里的表达式，表示倒数第一个元素。`^n` 就是倒数第 n 个元素。可以从这个写法里看出，`..` 是灵活的：它不是固定长度，是随着整个模式匹配的序列来确定 `..` 的长度的。这么写是为了简化代码的书写格式。

当然，假设我们判断倒数第二个元素而不是倒数第一个的话，那么我们可以尝试在倒数第一个元素的判断信息上添加弃元记号 `_` 来表达占位：

```csharp
if (arr is [10] [1, .., 3, _])
    // ...
```

弃元记号在这里起到了很重要的作用。一个弃元记号占一个位置，这恰好表达和判断了 `arr[^2]` 的数据，而不是 `arr[^1]`。

#### 预防性长度判断

和前文一致，要用这个模式的话，这个数据类型除了拥有 `Length` 或 `Count` 属性外，索引器成员是必不可少的。另外，如果你不写上范围记号 `..` 的话，就成了判断恰好这些数据了。

```csharp
if (arr is [10] [1, 2, 4]) ;
if (arr is [10] [1, 2, 4, ..]) ;
if (arr is [1, 2, 4]) ;
```

这三个写法的区别是，第一个和第二个是一样的判断：因为 `[1, 2, 4]` 按照顺序，判断的都是前三个数据的数值，因此长度给出后，判断的自然是前三个数据了，而后续的数据不用管，写上 `..` 和不写 `..` 都是没有关系的。但是第三个则不一样了。第三个因为长度模式不存在的关系，模式匹配的长度模式会依赖于 `[1, 2, 4]` 这个列表模式。这个模式只给出了三个元素，因此不写长度模式的话，编译器会认为这个写法下，长度模式是 `[3]`；相反，如果你加上了 `..` 的话，编译器就不再去确认后面的元素信息了。

但是，为了避免抛出异常，C# 会贴心地做一个“预防性判断”。如果 `arr` 没有这么长呢？假设 `arr` 就俩元素，那么判断 `[1, 2, 4, ..]` 就可能产生一个异常。因此，C# 会自动生成一条预判长度语句：`arr.Length >= 3`。

因此，如下四种写法的等价格式是这样的：

| 语法                          | 等价判断                                                     |
| ----------------------------- | ------------------------------------------------------------ |
| `arr is [10] [1, 2, 4]`       | `arr.Length == 10 && arr[0] == 1 && arr[1] == 2 && arr[2] == 4` |
| `arr is [10] [1, 2, 4, ..]`   | `arr.Length == 10 && arr[0] == 1 && arr[1] == 2 && arr[2] == 4` |
| `arr is [1, 2, 4]`          | `arr.Length == 3 && arr[0] == 1 && arr[1] == 2 && arr[2] == 4` |
| `arr is [1, 2, 4, ..]`      | `arr.Length >= 3 && arr[0] == 1 && arr[1] == 2 && arr[2] == 4` |

“默认生成预防性长度判断”这一点比较隐晦，因此你一定要记住。

#### 无条件成立的列表模式

当然，既然可以允许判断列表模式，那么自然就有 `[..]` 这种写法。这个写法的意思是，集合的元素无条件成立。但是，这个写法还不如不写，对吧。

### 分片模式（Slice Pattern）

C# 允许对集合类型的分片。举个例子。

```csharp
if (arr is [10] [_, .. var slice, _])
    // ...
```

这段代码表示，我们将中间的 8 个元素提取出来，变成一个列表。它等价于这个写法：

```csharp
if (arr.Length == 10 && arr[1..^1] is var slice)
    // ...
```

一定要注意，`1..^1` 的语义是“取` [1]` 到 `[^2]` 的元素，而绝对不是取到 `^1`。因为 C# 的范围表达式是取前不取后的（前闭后开的半开区间）。

> 顺带一说，集合模式最大范围能够支持到 `IEnumerable<T>` 这个泛型接口类型上。目前来说，带有 `Slice` 方法的具体类型都可以使用分片模式进行分片。

## 总结

下面列举前文所有模式匹配，每一种模式在 C# 里出现和允许使用的版本号。

| 模式类型名称 | 什么版本起可用 |
| ------------ | -------------- |
| 声明模式     | C# 7           |
| `var` 模式   | C# 7           |
| 常量模式     | C# 7           |
| 解构模式     | C# 7           |
| 类型模式     | C# 7           |
| 属性模式     | C# 8           |
| 关系模式     | C# 9           |
| 逻辑模式     | C# 9           |
| 扩展属性模式 | C# 10          |
| 长度模式     | C# 10          |
| 列表模式     | C# 10          |
| 分片模式     | C# 10          |

