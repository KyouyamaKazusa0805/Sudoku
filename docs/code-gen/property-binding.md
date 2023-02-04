# `PropertyBindingGenerator` 源代码生成器

## 基本介绍

在 UI 项目里，我们经常使用到绑定规则。但是在 WPF 以及以上版本的绑定里，我们经常需要触发事件并往里赋值，因此字段和属性必须同时声明出来：

```csharp
private int _width;

public int Width
{
    get => _width;

    set
    {
        if (_width == value)
            return;

        _width = value;

        PropertyChanged?.Invoke(this, new(nameof(Width)));
    }
}
```

可以看到，代码里需要在属性的 `set` 块里必须要执行三个步骤的操作：

1. 判断数值的相等性。数值相同则拒绝覆盖；
2. 赋值。往字段里进行赋值；
3. 触发 `PropertyChanged` 事件，好让 WPF 程序更好地往单向绑定的数据自动触发修改 UI 其他控件里面的数据。

不过，由于这样的赋值限制，我们并不是单纯地使用 `_width = value;` 赋值语句，因此不能使用自动属性，而必须完整给出赋值的过程，并触发事件。因此，如果 UI 使用到的这样的绑定数据很多的时候，就不得不每一次都得写出字段，然后写出绑定的属性，然后重复上述的操作给出完整的赋值过程。这样的代码具有模式化的操作，但无法简单描述出来，因此在程序里提供了一个源代码生成器 `PropertyBindingGenerator` 类型，允许用户直接往字段上标记特性，这样的完整代码就会被自动生成，就不必抄写完整的写法：

```csharp
[NotifyBackingField]
private int _width;
```

如代码所示，这样的代码仅往字段上添加了一个特性，在编译期间会自动生成属性的相关代码，因此这样的代码等价于前文给出的完整版本代码。这就是这个源代码生成器的作用。

## 使用

下面介绍一下变更和微调上方完整赋值规则的一些必要用法内容。

### `NotifyBackingFieldAttribute` 特性

首先要说的是 `NotifyBackingFieldAttribute` 特性。该特性类型用于标记字段，让该标记的字段生成属性对应的完整代码，用法就跟上面介绍的那样，标记一下即可。

#### `DisableEventTrigger` 可选属性

在极少数时候，我们不需要或者不希望源代码生成器自动发出触发 `PropertyChanged` 事件的代码：

```csharp
PropertyChanged?.Invoke(this, new(nameof(Width)));
```

这个时候，我们只需要在特性上配置可选属性 `DisableEventTrigger` 为 `true` 值即可。

```csharp
[NotifyBackingField(DisableEventTrigger = true)]
private int _width;
```

这样的话，生成的属性就只包含判断相等性和赋值的语句：

```csharp
private int _width;

public int Width
{
    get => _width;

    set
    {
        if (_width == value)
            return;

        _width = value;
    }
}
```

该属性一般没有单独使用的场景，它需要搭配另外一个特性一起用。该内容稍后说明。

#### `DisableDebuggerStepThrough` 可选属性

考虑到程序执行的一些不必要控制，源代码生成器会自动对生成的属性的 `get` 和 `set` 方法标记 `DebuggerStepThroughAttribute`，表示如果调试器执行到该方法的时候，会跳过该方法里的所有代码，继续执行后面的内容。如果此时该方法里包含其他的调用，那么它们全都会跳过去。

如果你不想让源代码生成器自动标记该特性禁用跳过调试（比如用于调试过程），可设置该属性为 `true` 来避免源代码生成器生成 `DebuggerStepThroughAttribute` 的特性标记。

#### `Accessibility` 可选属性

该属性控制的是生成的属性的访问修饰符。到 C# 11 为止，C# 一共提供了 7 种访问级别：

* `public`（默认情况）
* `protected internal`
* `internal`
* `private protected`
* `protected`
* `private`
* `file`

> 其中 `file` 修饰符只用于顶级类型（非嵌套类型），它表示该类型只在文件范围内可用，类似本地函数只能在它当前声明的方法范围内可用。目前 `file` 修饰符按语法规定来看，是不能修饰属性的，但该可选属性的对应枚举类型里包含该数值是为了可能的兼容。

不过，如果你要调整生成的属性的访问级别，可以往该可选属性赋值，修改级别。如果不赋值该可选属性，那么默认生成的属性的访问级别则是 `public` 修饰的。

#### `ComparisonMode` 可选属性

有些时候我们并不希望比较器自动生成固定的比较规则，于是我们可以自定义相等性比较规则。通过配置该属性就可以控制源代码生成器对相等性比较的行为。详情请查看该属性的类型 `EqualityComparisonMode` 枚举类型。

在默认情况下，是这样的识别：

* 如果对象实现了 `==` 运算符重载，那么会调用 `==` 进行比较；
* 如果对象实现了 `Equals` 方法，那么会调用 `Equals` 方法进行比较；
* 如果对象实现了 `CompareTo` 方法，那么会调用 `CompareTo` 方法，并当且仅当该方法返回 0 的时候认为是相等的；
* 如果上述提及的成员都不包含，那么则使用默认办法：`EqualityComparer<T>.Default` 对象的 `Equals` 方法进行比较。

### `NotifyCallbackAttribute` 特性

#### 基本用法

该特性仍用于字段。它限制的是，在触发 `PropertyChanged` 事件之后，自定义的赋值行为。如果你需要往里面进行额外的赋值过程，或者别的操作，该特性就会非常有用。但请注意，它配合 `NotifyBackingFieldAttribute` 特性一起使用。并不是说你只标记该特性而不需要 `NotifyBackingFieldAttribute` 特性就可以让代码完整生成了。不是这样的。

该特性需要你传入你回调的自定义过程的方法名。该方法允许你传入一个和字段、生成的属性类型一致的参数。这种设计主要用于生成的属性的 `set` 块里的隐变量 `value`。举个例子。我在程序里要解出一道数独题，在分析结果（`LogicalSolverResult` 类型的实例）得到之后，执行替换原有的数据，刷新 UI 的表格、完成步骤列表等操作。这个时候，我如果绑定了 `AnalysisResult` 属性（或者说 `_analysisResult` 字段）的时候，我需要让别处手动更新数据，这个时候我们会使用到该自定义的回调过程，比如这样：

```csharp
SolvingPathList.ItemsSource = value?.Steps.ToList();
```

该更新过程可以直接放入属性之中，其中的 `value` 就是该属性里的那个隐变量 `value`。不过由于前文给出的那些微调规则，我们无法控制该内容，因此我们需要使用该特性。该特性标记在属性之上，传入一个方法名。这个方法名是什么呢？

我们把刚才的这个自定义赋值过程抽取为一个单独的方法，将该方法名称传过去即可：

```csharp
[NotifyBackingField]
[NotifyCallback(nameof(AnalysisResultSetterAfter))]
private LogicalSolverResult? _analysisResult;

private void AnalysisResultSetterAfter(LogicalSolverResult? v)
    => SolvingPathList.ItemsSource = v?.Steps.ToList();
```

可以看到，我们多出了第二行的代码，指定了自定义的赋值过程是下面这个方法；然后源代码生成器识别该特性后就会自动将该赋值过程添加到属性触发事件后的末尾。因此，完整版代码是这样的：

```csharp
public LogicalSolverResult? AnalysisResult
{
    get => _analysisResult;

    set
    {
        if (_analysisResult == value)
            return;

        _analysisResult = value;

        PropertyChanged?.Invoke(this, new(nameof(AnalysisResult)));

        AnalysisResultSetterAfter(value);
    }
}
```

自定义操作在最后面。

当然，有时候也不需要该 `value` 变量的传入。只要你给出的方法不带参数，那么此时的赋值过程就不会传入 `value` 了。源代码生成器也支持识别这一点。

#### 缺省构造器参数

如果回调函数的名字是“属性名”+“`SetterAfter`”构成的的话，那么构造器参数可以省略不写，源代码生成器会自动识别该名字，与其回调函数绑定起来；但如果名字不是这么取的，那么就需要你手动给出该名称。例如前文的例子里，由于绑定的回调函数名为 `AnalysisResultSetterAfter`，刚好为该字段对应属性 `AnalysisResult` 和 `SetterAfter` 拼凑的结果，所以该参数可以缺省。

```csharp
[NotifyBackingField]
[NotifyCallback]
private LogicalSolverResult? _analysisResult;

private void AnalysisResultSetterAfter(LogicalSolverResult? v)
    => SolvingPathList.ItemsSource = v?.Steps.ToList();
```

这样就可以了。

#### 注意事项

一个需要注意的地方是，我们定义的自定义回调函数的参数如果存在的话，那么类型必须要和属性和字段的类型一致；但是，除了类型一致外，可空性也需要一致，否则会产生编译器错误——C# 不允许可空性不一致导致不兼容的赋值行为：例如本来属性可空，但回调函数的参数不可空，这个时候传入的 `value` 可能为 `null`，不兼容该回调函数的类型不可空的情况，所以会产生错误。

## 备注

另外一个需要注意的地方是，该源代码生成器要求对象是可以使用 `PropertyChanged` 的事件触发的，因此对象必须要实现 `INotifyPropertyChanged` 接口。换言之，如果对象没有该接口的实现的话，那么就找不到该事件，那么就无法正确触发 `PropertyChanged` 事件，因此源代码生成器会优先去判断对象是否实现了该接口。如果没有，即使代码和特性用法正确也不会生成代码，编译期间会直接出错。

不过，如果你设置了 `DoNotEmitPropertyChangedEventTrigger` 可选属性为 `true` 的话，这也就意味着不会发出触发事件的代码，所以有没有就无所谓了。因此，如果设置了该可选属性为 `true`，那么该控件类型不论是否从 `INotifyPropertyChanged` 接口类型派生都不会影响编译。