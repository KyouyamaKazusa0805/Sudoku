# 探索 C# 9 的记录类型

今天我们来讲一个 C# 里的新数据类别：**记录**（Record）。

## POCO 的概念

要想知道记录是什么，我们就需要先了解一个基本概念：POCO。

POCO 的全称是 Plain Old C# Object，这个 C 除了翻译成 C# 也可以翻译成 CLR，直接翻译出来是**平凡陈旧 C#/CLR 对象**。~~典型的说了当没说系列。~~它实际上指的是一个数据类型，里面除了含有数据成员以外，别的什么都没有。

举个例子。这个 `Student` 类型就是一个简单的 POCO。

```csharp
class Student
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
}
```

因为这个数据类型里，别的啥都没有，只包含基本的 `Name`、`Age` 和 `Gender` 三个数据成员，所以满足 POCO 的基本定义。

> **数据成员**（Data Member）指的是用于存储数据的基本信息的成员类型。现如今 C# 的数据成员可以是数据类型里的字段，以及同时带有 `get` 和 `set` 的自动属性。早期 C# 只有字段可以用于存储数据，但封装机制的复杂性导致 C# 不得不简化代码，所以属性后来也可以在不声明配套字段的情况下独立存在。它们往往都是直接 `get` 和 `set` 都带有的自动属性，因此这样的自动属性也称为数据成员。

另外，这个数据类型还可以包含基本的构造器（为这三个成员赋值）、`ToString` 的重写方法用来显示输出属性的结果之类的，包含它们也不影响数据自身，所以即使有了这些成员，数据类型仍是 POCO。

不过，下面这个数据类型则不是 POCO：

```csharp
public class PersonComponent : Component
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Name { get; set; }

    public int Age { get; set; }
}
```

你可能会问，它不也只是包含数据成员？虽然是，但 `Name` 上方标记了一个特性，此特性使得这个 `Name` 属性可能在别的地方有别的用法。所以这里的 `PersonComponent` 类型不是 POCO。

POCO 专门用于记录一个数据信息，它自身没有别的用途，只存储一些基本信息，为的就是以后能够通过属性或字段的成员访问来获取它们，仅此而已。这样的数据类型是 POCO。

## 记录的基本概念

记录类型是一种特殊的类型，它为了我们能够更加容易地实现 POCO 而出现这样的类型。C# 9 里出现了这个特性。它的语法是采用和类型声明基本一致的方式，将 `class` 关键字改成 `record` 上下文关键字。

```csharp
// Early C#.
class Student
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
}

// C# 9 records.
record Student(string Name, int Age, Gender Gender);
```

是的，仅需一句话即可完美表示一个和早期写这么一大堆代码的类型。这个 `Student` 此时用 `record` 这个上下文关键字修饰后，它就被称为记录类型。

下面我们来说一下记录类型底层的实现细节，以及记录类型顺带带出来的一些新语法特性。

## 记录类型的底层

### 主构造器

记录类型在后台其实也跟前面早期声明的 `Student` 类类型是差不多的，不过只是编译器看到了 `record` 关键字之后，会自动生成这些代码，不需要你自己手写而已。因此，记录类型在底层也是一个平凡的类型。

既然是一个类型，那么它就得包含一些基本的成员。C# 9 产生的记录类型规定，一个记录类型像是上面这样的书写方式的话，在底层除了会产生这些基本的数据成员以外，还有一些别的成员：

* 一个带有这些属性对应赋值的构造器；
* `Equals` 的重写方法（签名大概是 `bool Equals(object)`）；
* `Equals` 方法，参数不是 `object` 而是这个数据类型本身（签名大概是 `bool Equals(T)`）；
* `GetHashCode` 重写方法（签名大概是 `int GetHashCode()`）；
* `ToString` 重写方法（签名大概是 `string ToString()`）；
* `Clone` 克隆方法（签名大概是 `T Clone()`）；
* `Deconstruct` 解构方法（签名大概是 `void Deconstruct(...)`，参数都是 `out` 类型的，把每一个写在小括号的数据成员全部挨个写入到这里当参数）；
* 运算符 `==` 和 `!=`，参数是这个类型自己（签名分别是 `operator ==(T, T)` 和 `operator !=(T, T)`）；
* 一个 `private` 或者 `protected` 修饰的构造器，参数是这个类型自己（签名大概是 `T(T)`）；
* 一个 `private` 或者 `protected` 修饰的 `PrintMembers` 方法（签名大概是 `bool PrintMembers(StringBuilder)`）；
* 一个 `private` 或者 `protected` 修饰的 `EqualityContract` 属性（签名大概是 `Type EqualityContract`）。

看起来有点多。我们会对这些成员挨个说明一下。我们先来说一下构造器。

根据这个内容，我们可以知道的是，记录类型会固定在底层产生两个构造器，大概是这样的：

```csharp
public Student(string Name, int Age, Gender Gender)
{
    this.Name = Name;
    this.Age = Age;
    this.Gender = Gender;
}

protected Student(Student original)
{
    this.Name = original.Name;
    this.Age = original.Age;
    this.Gender = original.Gender;
}
```

先来说第一个。第一个构造器是 `public` 修饰的。这个 `public` 是系统生成的，你是修改不了的。然后，参数表列里就是我们在最开始写在 `record Student(...)` 语句里小括号的这些信息。相当于就是顺序抄写到这里来了。然后，底层会自动产生 `Name`、`Age` 和 `Gender` 属性，然后在这里可以提供赋值。所以，有多少个属性就有多少个属性的赋值过程。

再来说第二个。第二个构造器的修饰符 `protected` 也可能是 `private`，这取决于你这个数据类型 `Student` 的 `record` 前面有没有别的修饰符。如果包含 `sealed` 修饰符的话，那么此时因为类型是 `sealed` 的，所以不可能派生出来别的类型，因此 `protected` 修饰符就不可能出现在 `sealed` 修饰符修饰的类型里，因此这种情况下，这个自动生成的构造器是 `private` 修饰的；然后在构造器里，这段代码也是固定的，挨个属性赋值。

我们把第一个构造器（挨个属性抄进来当参数）称为这个记录类型的**主构造器**（Primary Constructor）。

### `init` 属性

因为最开始我们说过，这些 `record` 后写的东西自动被底层翻译成属性，所以它们其实就是自动属性。不过，和前面手动书写的 `Student` 类不一样的地方是，这里自动生成的属性，声明格式是这样的：

```csharp
public string Name { get; init; }
public int Age { get; init; }
public Gender Gender { get; init; }
```

可以看到，属性全部的 `set` 均被替换为了一个新的关键字 `init`。`init` 的概念和 `set` 基本一致，也是跟 `set` 实现机制一致的赋值过程，但换了一个关键字后，意思不一样了：`set` 是随时随地都可以赋值，但 `init` 只能在实例化的时候，写入到初始化器里。

举个例子，我用原来的 `set` 属性的话：

```csharp
var stu = new Student();
stu.Name = "Sunnie";
stu.Age = 25;

var stu2 = new Student
{
    Name = "Sunnie",
    Age = 25
};
```

这样赋值是成功的。因为 `set` 只约束你赋值的具体过程，但什么时候赋值都行；而 `set` 改成 `init` 的话，第一种写法就不行了：

```csharp
var stu = new Student();
stu.Name = "Sunnie";
stu.Age = 25;
```

这样就不行了。可这种限制有什么意义呢？实际上，初始化器只能跟在 `new` 实例化表达式之后，而上面这个离散的 `stu.Name` 和 `stu.Age` 这些写法实际上我不一定非得跟在 `new` 之后。实际上因为它已经脱离了 `new` 表达式的语法，所以它可以在中间插入很多东西然后再来赋值。而 `init` 关键字的意义就在这里：`init` 约束属性赋值仅可以在初始化器里使用和赋值，任何其它别的地方都不行。

可这限制为什么得这样搞呢？C# 9 的记录类型认为，这样的 POCO 是在默认情况下是不可变的。不可变的意思就是，这个数据类型一旦在 `new` 声明和实例化出来后，就不得再对里面的数据进行修改；而如果假设这些属性没有 `init` 的话，所有的属性全部只有 `get`，没有了 `init` 也没有 `set` 的话，这些属性就全部只能通过构造器赋值，这样就不灵活；而 `init` 一旦有了的话，语法的约定和使用规则允许你可以在初始化器里赋值，这样限定更加合理和严谨；而另外一方面来说，`init` 你也可以不使用，即使你知道它可能可以使用初始化器来赋值，我也可以不使用。比如我上面这种赋值下，`Gender` 属性我就没有赋值。而如果使用构造器的话，你三个属性全都必须赋值，就没有必要。

你可能会问我，既然底层是一个普通类型的话，我们知道，因为上面自动产生的内容里没有无参构造器，所以 `new Student { Name = ..., Age = ... }` 的语法是不成立的，因为你没有无参构造器，就无法这么写。是的，这个时候我们需要加一个东西进去：

```csharp
record Student(string Name, int Age, Gender Gender)
{
    public Student() : this(default, default, default) { } // Add it manually.
}
```

C# 9 的记录类型也确实允许我们这么做。这样的话，上面的 `new Student { ... }` 的写法就可以被允许了。不过此时这个无参构造器是我们手写的，并不是系统生成的，这一点需要你注意。因为底层是一个类，我们早就知道一点，类里但凡包含一个非无参的构造器，那么无参构造器就必然不会自动生成。所以，这个构造器必须得自己写。

> 主构造器在记录类型里有一个基本约定：你要自己定义别的构造器，必须调用这个构造器。因此，我们这里必须书写一个 `: this(default, default, default)` 来故意调用它，但传参都使用 `default` 就好。

### `Equals` 比较方法以及 `==` 和 `!=` 运算符

为了方便使用，光只有属性和构造器的存在肯定是不够的，所以，C# 9 记录类型规定，和 `Equals` 方法相关的成员会生成如下四个：

```csharp
public override bool Equals(object obj)
{
    return obj is Student comparer && Equals(comparer);
}

public bool Equals(Student obj)
{
    return Name == obj.Name && Age == obj.Age && Gender == obj.Gender;
}

public static bool operator ==(Student left, Student right)
{
    return ReferenceEquals(left, right) || left?.Equals(right) ?? false;
}

public static bool operator !=(Student left, Student right)
{
    return !(left == right); // Simply calls the operator ==.
}
```

可以看到，系统生成的这些内容都非常好理解。这里稍微要说一下的是这个 `ReferenceEquals` 方法。它其实就是比较两个对象的引用（底层就是比较指针）是不是一样的（是不是指向同一块内存）。

如果引用不一致，那么就得比较内容。于是后面的 `left?.Equals(right) ?? false` 是一个整体。`?.` 和 `??` 运算符是我们 C# 6 的语法，`?.` 是有限判断 `?.` 左边的对象是不是 `null`。如果不是的话就执行后面的内容；否则的话直接截断，并得到 `null` 的结果。假设这个表达式里 `left` 为 `null`，那么 `Equals` 方法就不会执行，并且 `left?.Equals(right)` 就会得到 `null` 的结果，相当于把 `null` 替换掉这个表达式；与此同时，`??` 运算符表示“里面不是 `null` 的部分”——如果 `??` 左边的部分不为 `null`，那么就是它自己作为这个表达式的结果；如果是 `null` 的话，那么 `??` 后面的部分就会作为默认结果，作为整个表达式的结果。那么往前分析，`?.` 这部分如果得到的 `null` 的话，那么这个 `A?.B ?? C` 整个表达式就是 `C` 部分作为结果。

回到这个写法上。假设 `left?.Equals(right)` 表达式得到的结果是 `null`，那么整个 `?.` 和 `??` 凑在一起后，整个表达式就是 `false` 这个值，意味着对象不相等，这恰好和我们期望的比较操作是一样的，所以这个写法比较巧妙，可以作为一个定式记一下。

后面这个第 18 行代码，可能你会认为这个 `!(left == right)` 有点奇怪。实际上我们要得把这个 `==` 看成一个调用方法。因为这个类型已经重载了 `==` 运算符了，因此 `left == right` 不再是 `object` 类型里的引用比较，而是前面重载的这个行为。因为是 `!=` 运算符，所以只需要得到 `==` 运算符的结果，然后取反即可。

### `GetHashCode` 哈希码方法

这个方法我们就不多说了，它要用一些哈希码自己的知识点。你只需要知道，哈希码是用一个整数来表达对象的 ID 从而通过这个 ID 确定对象是不是一致。如果哈希码一样，那么对象就一致。这个哈希码方法就是专门计算这个数值的。

当然，既然你有如上的这些属性，所以这些属性的实例，都会挨个计算出来哈希码，然后通过一个复杂的运算整合在一起表示这个对象的哈希码。

### `ToString` 表征字符串方法和 `PrintMembers` 方法

`ToString` 方法用于显示这个对象的具体信息。所以，这个 `ToString` 也会被系统自动生成。大概的代码是这样的：

```csharp
public override string ToString()
{
    var stringBuilder = new StringBuilder("Student { ");
    if (PrintMembers(stringBuilder))
    {
        stringBuilder.Append(' ');
    }
    return stringBuilder.Append('}').ToString();
}

protected virtual bool PrintMembers(StringBuilder builder)
{
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Name = ");
    builder.Append(Name);
    builder.Append(", Age = ");
    builder.Append(Age.ToString());
    builder.Append(", Gender = ");
    builder.Append(Gender.ToString());
    return true;
}
```

能不能看懂这段代码？`ToString` 方法最终会得到一个写法大概是 `Student { Name = ..., Age = ..., Gender = ... }` 的字符串结果。然后对象的数值就会自动填到里面去。

稍微注意的是，`PrintMembers` 方法的修饰符是 `protected virtual`，返回值是 `bool`。可能你会觉得奇怪，这么执行代码返回值不一定是 `true` 吗？那么这个返回值不就没有意义？实际上，这些方法是自动生成的，也可以自己写。这个时候，可能返回值就不再必须是 `true` 了（如果失败了就会自动返回 `false` 之类）。

### `Clone` 克隆方法以及 `with` 表达式

还记得吗？这个数据类型在底层是一个类，因此只使用 `=` 赋值只能赋值引用，而 `Student` 复制构造器也只是 `private` 或者 `protected` 修饰符修饰的成员，因此我们无法使用。所以，为了避免机制冲突，我们创建了一个 `Clone` 方法。

`Clone` 方法和 `Student` 里的复制构造器的底层实现代码差不多，甚至你可以这么认为：

```csharp
public virtual Student Clone()
{
    return new(this);
}
```

即直接调用复制构造器，返回复制了每个数据成员后的这个对象即可。

而这样的 `Clone` 方法有什么用呢？还记得 C# 规定记录是不可变的吗？那么我想要改掉其中某一个或若干数据成员，又不想大量变更数据成员的话，C# 提供了一个语法，叫 `with` 表达式，而这里的 `with` 是一个新的上下文关键字。

```csharp
var stu = new Student { Name = "Sunnie", Age = 25, Gender = Gender.Boy };
var stu2 = stu with { Name = "Yui" };
```

通过 `with` 表达式，然后后面跟着一个初始化器的形式，可以产生一个新的 `stu2` 对象，并且和 `stu` 对象里只差 `Name` 属性的数值不同。而 `stu` 和 `stu2` 变量此时是不同的引用，这就是两个完全独立的个体了。而在底层，这个 `with` 方法基本上等于 `Clone` 方法产生了副本后，然后改掉了 `Name` 属性的数值。这就是这个新语法配合 `Clone` 方法的使用方式。

而请注意的是，`Clone` 方法仅可通过 `with` 表达式来隐式调用，你无法自己调用，编译器不让你调用。

### `Deconstruct` 解构方法

C# 7 里有解构函数的机制，所以允许你在左侧写值元组，右侧写对象自身，然后赋值照样成功的语法。C# 9 的记录也自带这样的方法。`Student` 有这三个属性，系统会自动根据这三个属性生成这样的解构函数：

```csharp
public void Deconstruct(out string Name, out int Age, out Gender Gender)
{
    Name = this.Name;
    Age = this.Age;
    Gender = this.Gender;
}
```

正是因为它隐式存在（你看不到它的存在，但实际上它在底层生成了），所以你可以直接调用它：

```csharp
var stu = new Student { Name = "Sunnie", Age = 25, Gender = Gender.Boy };

//...

var (name, age, gender) = stu;
```

比如这样的语法，就可以了。不想用其中的某个或某些数值的话，可以使用弃元符号 `_` 代替。

### `EqualityContract` 属性

最后说一下 `EqualityContract` 属性。这个属性是一个你无法在外面使用的属性，因为它使用的是 `private` 或者 `protected` 修饰的，它的返回值是 `Type` 类型，这个属性用来干嘛呢？用来表征这个记录类型是啥类型。

它和 `GetType` 方法执行效果是一样的，不过 `EqualityContract` 可提供给编译器生成别的代码，所以有了这么一个东西。

## 成员的合成和非合成

### 合成成员

我们说完了一个普通的记录类型的底层生成的代码，下面我们来说一下一个记录类型带来的新概念：**合成**（Synthesize）。这些成员底层会自动产生，但这不代表我们不能自己手写。因为 C# 允许我们在基本的定义后再加入一些你自己定义的东西进去，因此你也可以自己手写一些东西。当然，这也包含上面的这些内容。如果你手写了上面出现的这些内容的某个或某些的话，那么这个或这些成员此时就称为**合成成员**（Synthesized Member）。

| 自动产生的成员                                  | 可否合成                                                     |
| ----------------------------------------------- | ------------------------------------------------------------ |
| 属性对应赋值的构造器                            | 否                                                           |
| 复制构造器                                      | 否                                                           |
| 主构造器配套会生成的底层的字段和属性            | 是，但会产生编译器警告，且它们永远不会被现有的 C# 语法使用到 |
| `Equals` 重写相等性比较方法                     | 否                                                           |
| `Equals` 相等性比较方法，参数是这个数据类型本身 | 是，但必须是 `public virtual` 修饰符                         |
| `GetHashCode` 重写哈希值计算方法                | 是，但不可带有 `sealed` 修饰符                               |
| `ToString` 重写方法                             | 是，但不可带有 `sealed` 修饰符                               |
| `Clone` 克隆方法                                | 否                                                           |
| `Deconstruct` 解构方法                          | 是                                                           |
| 运算符 `==` 和 `!=`                             | 否                                                           |
| `PrintMembers` 方法                             | 是，但必须是 `private` 或 `protected virtual` 修饰符         |
| `EqualityContract` 属性                         | 是，但必须是 `private` 或 `protected virtual` 修饰符         |

我来举个例子。

```csharp
record Student(string Name, int Age, Gender Gender)
{
    public Type EqualityContract { get { return typeof(Student); } }
}
```

此时 `EqualityContract` 属性是一个合成属性。不过，因为规则要求，我们必须加上 `virtual` 修饰符，并使用 `private` 或 `protected` 修饰符。具体使用 `private` 还是 `protected` 修饰符取决于类型本身自己是不是 `sealed` 修饰过的。显然它没有，所以必须使用 `protected virtual` 组合修饰。所以 `public` 必须换为 `protected virtual` 才可以。

### 自定义成员

下面来说一下自定义的成员。自定义成员就是那些我们随便定义的，但不会和前面这些包含的成员冲突的成员。所以这些自定义成员我们也可以叫**非合成成员**（Non-Synthesized Member）。

非合成成员是可以你随便书写的，所以不受语法约束。所以，没有什么特殊的语法限制。但这里要说一下补充数据成员的问题。

由于语法的限制，记录类型的主构造器带有的这些底层生成的属性全部都是自动属性，并且包含的是 `get` 和 `init`，无法改变。不过，C# 允许我们自定义成员，所以我们可以添加数据成员到这个类型里，使得这个记录类型改为可变类型。

比如我在 `Student` 记录类型里加入可变的属性成员 `Class`：

```csharp
record Person(string Name, int Age, Gender Gender)
{
    public int Class { get; set; }
}
```

此时语法上是没有问题的，编译器也不会认为你这么写代码会有问题。不过，这变更了属性的个数，也改变了比较规则。假设我要生成 `Equals` 方法的话，因为多了一个 `Class`，那么它会不会参与比较相等性呢？

答案是，会。编译器的灵活性使得你即使自己定义了这样的数据成员，编译器照样可以识别到，因此它也会参与相等性比较。不过，一些别的方法可能就不会参与了。下面列举一下会用到这些数据成员操作的成员，并说一下，如果自定义新的数据成员后，是否会参与进去。

| 自动产生的成员                                  | 自定义新数据成员后，是否参与进去                             |
| ----------------------------------------------- | ------------------------------------------------------------ |
| 属性对应赋值的构造器                            | 不参与，因为记录类型底层代码生成只看主构造器里的成员         |
| 复制构造器                                      | 会参与                                                       |
| `Equals` 重写相等性比较方法                     | 不参与，因为这个重写方法本身不管你加不加也不会影响底层生成的代码 |
| `Equals` 相等性比较方法，参数是这个数据类型本身 | 会参与                                                       |
| `GetHashCode` 重写哈希值计算方法                | 会参与                                                       |
| `ToString` 重写方法                             | 会参与                                                       |
| `Clone` 克隆方法                                | 会参与                                                       |
| `Deconstruct` 解构方法                          | 不参与，因为记录类型底层代码生成只看主构造器里的成员         |
| 运算符 `==` 和 `!=`                             | 不参与，因为这个成员本身不管你加不加也不会影响底层生成的代码 |
| `PrintMembers` 方法                             | 会参与                                                       |
| `EqualityContract` 属性                         | 不参与，因为这个成员只是 `typeof` 表达式作为结果值，不依赖数据成员 |

你记住了吗？

## 记录类型的继承和派生机制

基本类都有继承和派生，那么记录也得有这样的机制。下面我们就来说一下。

### 记录类型的派生语法

记录类型被翻译成了类，所以必然也存在继承和派生关系。但是请注意，记录类型只能派生和继承自一个记录类型，这也就是说，你无法写一个普通的类型，然后拿给记录类型当派生类，也不能把记录类型作为基类型，派生出了一个没有 `record` 修饰的普通类型。举个例子。

```csharp
abstract record Person(string Name, int Age, Gender Gender);

sealed record Student(string Name, int Age, Gender Gender) : Person(Name, Age, Gender);
```

这么做是可以的。

```csharp
abstract record Person(string Name, int Age, Gender Gender);

sealed class Student : Person
{
    public Student(string Name, int Age, Gender Gender) { ... }

    public string Name { get; init; }
    public int Age { get; init; }
    public Gender Gender { get; init; }
}
```

即使我们知道这样我们想要表达的意思是到了，但是这么也不可以。这是 C# 语法上的一个限制：因为你派生出来了别的记录类型后，你必须得在派生类型这个地方写上构造器参数调用关系，比如前面这段代码的第 3 行的声明语句上就有 `: Person(Name, Age, Gender)` 这一部分。这个是记录的派生的固定语法。你要派生，就必须为基类型的主构造器传入对应的参数信息。

### 记录类型派生后的底层代码

那么既然派生出来的新的类型，我们就得对派生的记录类型的底层说明一下底层的代码生成的样子都有什么区别。

实际上，也没有什么特别大的区别，只是你想想看，因为它是从基类型派生下来的，所以 `virtual` 派生下来的是不是得改成 `override` 修饰符了？所以，这个就是记录类型在派生后和原本基类型唯一不同的生成的代码的不同点：`virtual` 关键字被换成了 `override`。

另外，从这个角度来说，你看看这个记录类型是可以提供派生的，所以如果我们没有编译器前文的那些限制，你自己合成方法的时候就可以不写 `virtual`，那怎么保证我派生类型是走这个方法派生的呢？这不就是出现了语法的问题和冲突了么？所以，`virtual` 在合成方法里是不可少的。

而除了这一点，我们还有 `ToString` 这些跟自身类型有关的代码生成。

```csharp
abstract record Person(string Name, int Age, Gender Gender);
sealed record Student(string Name, int Age, int Class, Gender Gender) : Person(Name, Age, Gender);
```

我们还是来看这样的两个类型，不过稍微拓展一下 `Student` 记录类型，让它看起来不那么像基类型 `Person`。倘若我有这么一个实例化语句，使用了多态：

```csharp
Person p = new Student("Sunnie", 25, 3, Gender.Boy);
```

猜猜看，我要是调用 `p.ToString` 会输出什么东西来？请选择：

* A. `Student { Name = Sunnie, Age = 25, Gender = Boy, Class = 3 }`
* B. `Person { Name = Sunnie, Age = 25, Gender = Boy }`

答案是 A。按照道理来说，`Person` 和 `Student` 类型里均包含 `ToString` 方法，不过因为继承关系，`Student` 是重写的 `Person` 类型里的方法。因此，现在这个结果是包含四个数据数值的；虽然此时 `p` 自己是 `Person` 类型，但 `ToString` 已经被重写掉，所以显示内容仍然不应该是选项 B 的结果。你答对了吗？

其它的成员在底层代码的生成里也都基本类似，就不再赘述了。不过这里说一下 `Clone` 这个方法，稍微有点特殊。

`Clone` 方法在抽象记录类型里是抽象的。这句话有点绕。换言之，编译器会对一个 `abstract record` 生成一个 `abstract` 修饰的 `Clone` 方法，此时这个类型就不再可以 `Clone` 了，也因此，这个类型也无法使用 `with` 表达式；而别的生成的成员都不受影响，生成的内容也都会按照前文给的那些个内容生成完全一样规则的内容。

### 浅谈 `record`、`sealed record` 和 `abstract record` 的异同点

我们先来说一下 `record`、`sealed record` 和 `abstract record` 的异同点。

`sealed record` 和 `record` 最为相似，不过区别在于 `sealed` 修饰的记录类型不可提供给别的类型派生。因此，`sealed` 修饰过的记录类型，里面的所有原本是 `protected virtual` 或 `protected` 的成员，全部在生成的代码里，被改成 `private` 修饰，这是它们唯一的区别。

而 `abstract record` 稍微麻烦一点。因为它是抽象的，所以不能实例化，因此 `Clone` 就是一个典型范例，它就和 `record` 和 `sealed record` 的记录类型生成下来的结果不一致：`abstract record` 是抽象的 `Clone` 方法。

而稍微注意一下。这里的 `Deconstruct` 方法最为特殊，因为这个成员没有 `virtual`、`sealed`、`override` 或者 `abstract` 之类的修饰符修饰，所以它是一个独立的个体；在派生后，哪怕参数个数一致、类型一致，也不会有别的多余的东西产生，取而代之的是标记了一个 `new` 方法修饰符到 `Deconstruct` 方法上以覆盖原始的解构函数。比如说这里 `abstract record Person` 派生了 `sealed record Student` 类型，它们全部都只包含 `string Name`、`int Age` 和 `Gender Gender` 三个属性作为主构造器的成员，那么 `Student` 派生记录类型里会这样生成代码：

```csharp
public new void Deconstruct(out string Name, out int Age, out Gender Gender)
{
    Name = base.Name;
    Age = base.Age;
    Gender = base.Gender;
}
```

即多一个 `new` 而已。

### 记录类型实现接口

记录类型除了必须从记录类型派生（如果需要自定义派生关系）的这个规则以外，还可以实现接口。它的语法和类和结构实现语法的规范规则是一样的，也是冒号后跟上实现接口的列表即可。只是要注意的是，如果此时这个记录类型有基类型主构造器调用的话，需要先写主构造器的调用，然后才是是派生接口，并且也是用的逗号隔开。比如这样：

```csharp
record Student(string Name, int Age, int Class, Gender Gender)
: Person(Name, Age, Gender), IEquatable<Student>
{
    // ...
}
```

超级简单，对吧。稍微麻烦一点的是，如果接口里包含一个和主构造器参数一样的属性名，咋办呢？举个例子。我假设有一个 `IPerson` 的接口，它里面包含了一个 `string Name` 的属性：

```csharp
public interface IPerson
{
    // ...
    
    string Name { get; init; }
    
    // ...
}
```

那么我们直接写在这个类型的后面作为实现接口：

```csharp
record Student(string Name, int Age, int Class, Gender Gender)
: Person(Name, Age, Gender), IEquatable<Student>, IPerson
{
    // ...
}
```

现在，会发生什么呢？啥也不会发生，而且编译成功。原因很简单，因为我们之前说过，主构造器的参数其实就是被翻译成了一个属性，一个带 `get` 和 `init` 的属性。而这正好匹配了 `IPerson` 的同名同返回值同访问器（即 `get` 和 `init` 也都一样）属性。因此，记录类型的接口实现其实稍微麻烦一点的地方是主构造器参数的隐式属性实现。所以这种情况下，我们直接把一个个主构造器里的参数当成一个个的实体属性，并且自带 `get` 和 `init` 访问器就行。实现接口的时候，请仔细检查它们。

但请注意，假设 `IPerson` 接口里属性是 `get` 和 `set` 而不是 `init` 的话，那么此时因为同名不同访问器，而 `set` 和 `init` 都是赋值行为，又不可同时出现，因此我们这种情况下，只能显式接口实现。

```csharp
: ..., IPerson
{
    // Explicitly implementation.
    string IPerson.Name
    {
        get => Name;
        set => throw new NotSupportedException("The operation isn't supported.") // Please note here.
    }
}
```

这样实现就可以了。不过注意这里的 `set` 赋值器。

可能你会问为什么这里直接抛异常了。你这么想。原本的 `init` 赋值范围比 `set` 要小，所以我们不可以直接赋值，所以我们这里迫不得已使用了 `with` 表达式。我们使用 `this with { ... }` 的模式来改变 `this` 对象里的 `Name` 属性数值。因为 `Name` 属性仅可在初始化的时候修改，因此只能使用 `with` 了，因此写成 `this = this with { ... }` 是正确答案。但别急。你见过运行时还 `this = ...` 的语法吗？引用类型的 `this` 引用是任何时候都不可改的，它只能老老实实分配内存，读取数据存进去，因此，这个赋值写法是不可能成立的，编译器会告诉你，“`this` 是只读的”。所以，我选择了抛异常。这个 `NotSupportedException` 异常类型经常在操作在某个条件状态下是不支持的的时候抛出。

最后，因为是自动生成，所以一个记录类型会自动生成一个 `IEquatable<T>` 接口实现语法在声明语句之后，即：

```csharp
record R(int A, double B);
```

在底层除了生成一大堆东西以外，它的声明上也会自动加上一个接口实现：

```csharp
record R(int A, double B) : IEquatable<R>;
```

这个写法和前面那个写法是等价的，因为 `IEquatable<R>` 是系统自己检测，自己实现掉了，所以不写这个接口也会被系统自动生成的代码给实现，所以它相当于是后台实现的。

## 其它无关痛痒的记录类型语法

### `partial` 修饰符修饰记录类型

如果我们要用 `partial` 修饰符来修饰记录类型，是怎么样用的呢？因为记录类型会自带参数表列构成主构造器，所以我们写 `partial` 的话，不需要每个文件都有这样的主构造器。只需要只有一个主构造器就可以了，别的全部不用写出来：

```csharp
// File 1.
partial record Person(string Name, int Age, Gender Gender);

// File 2.
partial record Person
{
    // ...
}
```

比如这样。

### 主构造器允许的参数修饰符

我们把 `record` 声明后的这一坨参数表列叫主构造器，而主构造器怎么说都得是一个构造器（虽然长得更像是方法的参数）。但不管怎么说，构造器包含参数，那么参数就一定可以包含修饰符。

不论是不是主构造器也好、方法也好、索引器也好、运算符重载也好，它们都会或多或少包含参数。C# 早期就规定了，参数一共可以有 `out`、`ref`、`in` 和 `params` 这四种修饰符修饰参数本身，而在记录类型里，修饰符 `out` 和 `ref` 是不行的，剩下那俩是可以的。比如假设我把代码改成这样：

```csharp
record Student(in string Name, in int Age, in Gender Gender);
```

这样是可以的。当然，`params` 也可以，不过这里没有需要数组传入的属性信息，所以就不举例说明了。

### 主构造器为空的记录

是的，C# 9 允许我们写一个没有主构造器的记录类型，比如长这样：

```csharp
record Student : Person;
```

可这……真的有意义吗？是的，一般来说它都没有意义，因为它没有主构造器意味着它没有参数；而且它直接在 `Person` 后面就分号结尾了，所以里面啥都没有。不过，就这个样子系统也会给你生成那些前面挨个讲过的成员。即使你知道它是没有意义的写法。

当然，主构造器为空是可以不写的，不过你也可以写成一对空括号：

```csharp
record Student() : Person;
```

也可以。但是 `Person` 上无参是不行的。因为大家都知道，`Person` 类型怎么着底层也是一个类，而且自然已经实现了一些非无参的构造器，所以无参构造器不会自动生成，所以 `Person()` 是不可以的写法；不过 `Student` 如果包含无参构造器，就可以这么写：`Student()`。

### 主构造器上使用特性

C# 甚至允许我们在参数表列上使用特性，而且可以使用特性目标来固定应用到某个成员上。

主构造器的语法的特殊性，和它绑定在一起的有参数、底层字段和后台属性三个不同的内容。

```csharp
sealed record Student(string Name, int Age, Gender Gender);
```

比如我现在有这样的代码。代码的 `Name` 是不可空的，但是初始值允许为空，只是我们不让传参的时候传入一个 `null`。这个时候我们可以使用这样的写法来标记一下：

```csharp
sealed record Student(
    [param: DisallowNull][property: DisallowNull] string Name,
    int Age,
    Gender Gender
);
```

我们使用 `[param: DisallowNull]`（这个 `param:` 特性目标可省略）和 `[property: DisallowNull]`（这个 `property:` 不可省略）分别告诉编译器在生成代码的时候，不允许参数和属性 `Name` 传入 `null`，但允许它自身初始化的时候保持 `null` 数值。

所以，主构造器的参数绑定的概念有三个，因此有效的特性目标可以是 `param`、`field` 和 `property` 这三个。

> 这里稍微注意下，C# 最开始的特性目标语法格式是必须分开的，不是说特性可以中括号里逗号分隔就可以在这里也这么写。因为这里带有特性目标，所以特性不可使用逗号写在一起，即 `[param: DisallowNull, property: DisallowNull]` 一样的语法是不正确的，必须分开写成两对中括号。

### 没有 `static record`

你想想，`record` 的诞生是为了提供更好的 POCO 实现的，一个 `static class` 是拿来干嘛的？存储工具方法的，这样的类型叫工具类，而这样的类型本身就不应该提供实例化。所以，一个静态类就根本不可能有对应的 `record` 一说，所以 `static record` 组合是不存在的。

### 暂时没有结构类型和接口类型的 `record`

`record` 被翻译成了类，所以 `record` 是不支持结构的。在 C# 10 里，`record` 会被推广到 `record struct` 上，但 C# 9 还不行，而且 C# 10 的 `record struct` 和这里的 `record` 有些细节也有不同，这个我们以后再说。

而因为 `interface` 是抽象的存在，如果把类和结构当成用户说明书的话，那么 `interface` 就是约束你说明书该写什么东西的存在。像是它自己都不能实例化的类型，我们创建 `record` 版本好像也没有什么大用吧。所以，接口没有记录类型的这个说法。
