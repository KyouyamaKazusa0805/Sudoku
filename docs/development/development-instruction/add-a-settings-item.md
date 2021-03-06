如何添加一个窗体程序的设置项呢？我们先来了解一下，这个程序的设置项到底都放在哪里，分布是怎样的。



## 介绍一下设置窗体

![介绍](https://images.gitee.com/uploads/images/2021/0124/173740_9bbc650c_1449374.png "Intro.png")

这是一个窗体，这个窗体包含了我们需要设置的东西。它分 5 个部分：行为、解题、绘图、优先级和调色板，分别对应的是程序执行的行为、解题时候解题器需要的设置参数、绘图界面的线条粗细颜色等设置、解题器的计算调用顺序以及自定义绘图的调色板的颜色。

我们需要添加一个设置，就得找到设置到底应该分为哪一种。比如说，我接下来需要加一个“是否在显示分析结果的时候，显示技巧的简称而不是全称”一项。那么，话不多说，开始干！



## 如何添加一个设置项

### 第一步：找到对应添加的位置

前文已经说了，我们假定需要添加的是“是否在解题器分析完成后，将技巧显示为技巧名称的简称而不是全称”这么一个项目。这个项目显然不适合“绘图”、“优先级”和“调色板”这几个页面，因此我们会考虑放到“行为”和“解题”两个页面的其一里面。我个人建议你放在“行为”里，因为它并不属于某一个技巧本身解题的东西，而是属于程序执行的行为。



### 第二步：将控件加入到对应页面里

既然已经决定了我们需要添加到“行为”里，那么我们就需要在“行为”里找到对应的添加位置，然后放上控件。

```xaml
<CheckBox x:Name="_checkBoxDisplayAbbrRatherThanFullNameOfSteps"
          Content="{DynamicResource SettingsDisplayAbbrRatherThanFullNameOfSteps}"
          Grid.Row="8" VerticalAlignment="Center"
          Click="CheckBoxDisplayAbbrRatherThanFullNameOfSteps_Click"/>
```

![步骤 2-1](https://images.gitee.com/uploads/images/2021/0124/173755_3a33b291_1449374.png "Step2-1.png")

![步骤 2-2](https://images.gitee.com/uploads/images/2021/0124/173804_fdb37837_1449374.png "Step2-2.png")

需要注意的是，这个页面下的控件是放在一个大 `Grid` 布局控件里的，因此我们需要注意放在哪一行哪一列，免得错位和显示异常。前面用了 8 个控件了，所以要放到第 9 行上，不要写错了 `Grid.Row` 属性的数值（第 9 行写 8，因为第一行是 0 表示的）。



### 第三步：别忘了加翻译文字

前面第二步我们添加了控件，但发现有一处波浪线报错。它告诉我们没有这个动态资源。是的，我们还没有写翻译文字，所以需要我们现在添加。

```xaml
<!-- LangEnUs.xaml -->
<sys:String x:Key="SettingsDisplayAbbrRatherThanFullNameOfSteps">Display Abbreviation rather than full technique name in steps</sys:String>

<!-- LangZhCn.xaml -->
<sys:String x:Key="SettingsDisplayAbbrRatherThanFullNameOfSteps">在步骤信息上显示简称而不是技巧全名</sys:String>
```

![步骤 3-1](https://images.gitee.com/uploads/images/2021/0124/173814_bba774b1_1449374.png "Step3-1.png")

![步骤 3-2](https://images.gitee.com/uploads/images/2021/0124/173822_e5571b37_1449374.png "Step3-2.png")


### 第四步：给这里添加处理逻辑

接着，我们需要给这个设置项添加处理逻辑了。不过，这一步稍微复杂一点，因为它牵扯到方法本身的修改、窗体初始化的时候的修改和显示文字处的修改。

首先，我们需要知道，这个类里有一个 `_assignment` 字段。这是一个委托字段，但它并不是任何事件成员背后的字段；相反，它并不是用来处理事件用的。委托在 C# 里被设计成了一个函数列表的存在。这个字段专门存储了我们修改了哪些信息。当修改了信息后，但没有选择保存的话，`_assignment` 里的函数表将不会得到执行；相反地，如果我们确认保存配置，这个时候，`_assignment` 里的函数就会逐个被调用到，然后执行。实际上，这个字段里存储的函数都是表示一个功能的修改过程。这下你应该明白，这个委托字段是用来干什么了的吧。

所以，照着这个思路去思考，我们添加的代码就应该是形如 `_assignment += () => ...` 的语句。是的，我们确实就是这么做的。

第二，知道了 `_assignment` 字段还不够。因为我们这里是新添加的功能，所以我们还需要给底层存储这些配置的类型进行一个交互，那么，自然就需要添加一个这个对应设置项的属性数值了。

我们找到文件 `WindowsSettings.SettableProperties.cs`。这个文件存储的就是所有窗体项目里需要用到的配置项。我们找到位置，添加一个项目，名称你随意取就行（当然，我建议取名叫 `DisplayAbbrRatherThanFullNameOfSteps`，和前文的 Xaml 的键匹配）。

```csharp
/// <summary>
/// <para>
/// Indicates whether the program will display abbreviation rather than full name for a technique
/// in analysis tab page.
/// </para>
/// <para>The value is <see langword="false"/> in default case.</para>
/// </summary>
public bool DisplayAbbrRatherThanFullNameOfSteps { get; set; }
```

![步骤 4-1](https://images.gitee.com/uploads/images/2021/0124/173832_70f55758_1449374.png "Step4-1.png")

当然，属性初始化器可以不写，因为默认就是 `false`。

接着我们就可以回到原本的代码里写代码了：

```csharp
/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
private void CheckBoxDisplayAbbrRatherThanFullNameOfSteps_Click(object sender, RoutedEventArgs e)
{
    _assigments += () =>
    {
        _checkBoxDisplayAbbrRatherThanFullNameOfSteps.IsChecked =
            Settings.DisplayAbbrRatherThanFullNameOfSteps ^= true;
    }
}
```

当然，你也可以改写为 Lambda 风格（“一句话风格”）：

```csharp
/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
private void CheckBoxDisplayAbbrRatherThanFullNameOfSteps_Click(object sender, RoutedEventArgs e) =>
    _assigments += () => _checkBoxDisplayAbbrRatherThanFullNameOfSteps.IsChecked = Settings.DisplayAbbrRatherThanFullNameOfSteps ^= true;
```

Emmm……有点丑，不过不要在意。

![步骤 4-2](https://images.gitee.com/uploads/images/2021/0124/173843_72e8f2d5_1449374.png "Step4-2.png")

> 注意一下，`^= true`（或者 `^= false`）表示反转布尔量。对一个布尔量做对 `true` 或者对 `false` 的异或运算，就可以直接得到这个布尔量的取反结果。当然，你依然可以写成 `a = !a`，但这个写法不直观，也不好看（因为变量要写两次）。

在这里，我们完成了对设置项的交互和修改。但是，你觉得这样完成了吗？实际上并没有。设置项应在打开设置窗体的时候载入这些细节配置才行。如果不载入的话，程序不就出 bug 了嘛（毕竟你始终修改的是一个新的对象的设置）。那么，我们需要更改和添加对这个控件的初始化逻辑。



### 第五步：找到初始化函数，将控件初始化的代码写进去

还是这个文件。我们找到一个叫 `InitializeSettingControls` 的方法，然后添加如下的代码：

```csharp
_checkBoxDisplayAbbrRatherThanFullNameOfSteps.IsChecked = Settings.DisplayAbbrRatherThanFullNameOfSteps;
```

我当然相信你知道这一行代码放在哪里。

![步骤 5-1](https://images.gitee.com/uploads/images/2021/0124/173852_dc08ece0_1449374.png "Step5-1.png")

那么，初始化功能就完成了。窗体这边的交互我们就完成了。但是，这只是修改和交互了设置，并没有实际上生效。所以我们接下来，就是去修改代码并让其生效。这也是最难的地方。



### 第六步：让设置项生效

我们先需要思考一个问题。这个设置项到底用在哪里。前文说到，这个设置项专门表示是不是显示全称，那么我们就必须要在技巧总结表里（就是显示题目分析的那个大表格）作出修改。

我们先找到技巧表格应该修改的地方。技巧表格显示的地方在 `MainWindow.xaml.cs` 文件里。在被 `#region` 块折叠起来的第三个板块（`#region Other instance methods`）里，有一个叫 `DisplayDifficultyInfoAfterAnalyzed` 的方法（在这个板块里所有方法的最下面）。

请找到这里：

```csharp
// Gather the information.
// GridView should list the instance with each property, not fields,
// even if fields are public.
// Therefore, here may use anonymous type is okay, but using value tuples
// is bad.
var puzzleDifficultyLevel = DifficultyLevel.Unknown;
var collection = new List<DifficultyInfo>();
decimal summary = 0, summaryMax = 0;
int summaryCount = 0;
foreach (var techniqueGroup in
    from step in _analyisResult.Steps!
    orderby step.Difficulty
    group step by step.Name)
{
    string name = techniqueGroup.Key;
    int count = techniqueGroup.Count();
    decimal total = 0, minimum = decimal.MaxValue, maximum = 0;
    var minDifficultyLevel = DifficultyLevel.LastResort;
    var maxDifficultyLevel = DifficultyLevel.Unknown;
```

注意这里贴的代码的第 13 行，我们是以 `step.Name` 分组的。这里用 LINQ 分组的原因是显示呈现整个技巧表。技巧表是按照技巧名的不同来区分技巧的，所以这里我们需要修改 `step.Name`。

修改后的 LINQ 应该是这样的：

```csharp
from step in _analyisResult.Steps!
orderby step.Difficulty
group step by (
    Settings.DisplayAbbrRatherThanFullNameOfSteps
    ? step.Abbreviation ?? step.Name
    : step.Name
)
```

我们先查看配置。配置里如果需要让程序显示简称，那么就按简称分组；否则按全称分组。然后就发现，好像我们没有啥需要改的了。

有的读者可能会问我，前文的变动会影响表格，这一点确实没错。但是技巧在呈现的时候，并没有完全简写，因为在解题步骤列表里依然没有变动。是的，技巧步骤列表里确实没有变动，因为要变动是很复杂的事情。我们可能会牵扯到修改底层所有的派生 `StepInfo` 记录，并全部修改掉输出结果；另外，我们目前没有将设置项传入到 `StepInfo` 里，所以程序无法确定到底是不是需要简称。修改的幅度有点大，所以此处我们就不实现了。



### 第七步：验证效果

确实是成功了。

![步骤 7-1](https://images.gitee.com/uploads/images/2021/0124/173931_c73ada19_1449374.png "Step7-1.png")

![步骤 7-2](https://images.gitee.com/uploads/images/2021/0124/173939_dfb4eb14_1449374.png "Step7-2.png")