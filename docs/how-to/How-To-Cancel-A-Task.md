# 如何取消一个 `Task` 对象
本文将阐述如何取消一个任务。这一点可能和本项目无关，但如果你需要使用在这个项目里使用取消功能的时候，这篇文章会对你有帮助。



## 推荐的取消代码模式

如果我们确实需要在中途取消一个任务，一般的办法是通过传入一个 `CancellationToken` 结构对象到最底层的复杂方法里去。在这个最底层的方法里使用该结构对象，判别是否发出取消请求。如果是，则抛出异常来中断该方法的执行。接着，在调用方捕获该异常，以达到取消的操作。

下面我们来看一下具体的实现流程和过程。



## 取消模式

下面我们来阐述一下具体如何取消一个复杂的步骤。我们拿数独出题的功能来举例。假设出题的方法名称是 `Generate`：

```csharp
public SudokuGrid Generate()
{
    while (true)
    {
        // Try to fill with some cells, and check the vaildity of this puzzle.
        // If failed to check, again.
        // ...
        // Again and again.
    }
}
```

然后，在 UI 界面，我们必然了解到，出题的执行行为很慢。因此，我们不得不将其封装一层，改为异步方法：

```csharp
public async Task<SudokuGrid> GenerateAsync() => await Task.Run(Generate);
```

接着，我们在 UI 挂载的方法里调用该异步方法。

```csharp
private void Button_Click(object _, RoutedEventArgs e)
{
    // Disable controls to prevent any user clicking other controls,
    // which may cause the program unstable.
    DisableControls();
    
    // Wrong code!!!
    // Can't use await clause in a synchronized method.
    var grid = await GenerateAsync();
    
    // Now enable controls.
    EnableControls();
}
```

> 其中的 `RoutedEventArgs` 是 WPF 项目里控件各个事件里的默认事件参数类型。在 Winform 里，默认的类型是 `EventArgs`。

很好。现在我们发现 `await` 语句有问题，因为这个方法是同步的。所以，我们需要添加 `async` 关键字到方法上来保证程序可用异步行为：

```csharp
private async void Button_Click(object _, RoutedEventArgs e)
{
    // Disable controls to prevent any user clicking other controls,
    // which may cause the program unstable.
    DisableControls();
    
    // Wrong code!!!
    // Can't use await clause in a synchronized method.
    var grid = await GenerateAsync();
    
    // Now enable controls.
    EnableControls();
}
```

不过，我们不是很建议你这么使用。`async void` 组合是不太好的，因此我们建议你封装一层方法，即写成下面这样，才是我们推荐的代码格式：

```csharp
private async void Button_Click(object _, RoutedEventArgs e)
{
    await internalOperation();

    async Task internalOperation()
    {
        // Disable controls to prevent any user clicking other controls,
        // which may cause the program unstable.
        DisableControls();

        // Now here is correct syntax.
        var grid = await GenerateAsync();

        // Now enable controls.
        EnableControls();
    }
}
```

很好，我们已经完成了前面的铺垫的内容。下面我们来完成取消操作。

> 当然，这里是为了演示一下。`grid` 目前还作为普通变量没有赋值给别的地方用。你自己写代码的时候看着办吧！



### 第一步：为底层添加取消处理

下面我们需要为 `Generate` 方法添加取消机制。怎么添加呢？答案非常简单：

```csharp
// Here, we should add a new parameter named 'token' of type CancellationToken.
public SudokuGrid Generate(CancellationToken? token)
{
    while (true)
    {
        // Check whether user have cancelled the operation.
        if (token is { IsCancellationRequested: true })
        {
            // You should throw this exception manually.
            throw new OperationCanceledException();
        }
        
        // Try to fill with some cells, and check the vaildity of this puzzle.
        // If failed to check, again.
        // ...
        // Again and again.
    }
}
```

可以注意到，这里我们修改了两处地方。第一处是参数，我们添加了一个新的参数 `token`，它是 `CancellationToken` 类型的。

注意，这个类型是一个结构，并不是类，所以判别是否为 `null` 的时候，`token` 用的是 `HasValue` 来判定。当然，你依然可以使用 `token is not null` 或 `!(token is null)`、`token != null` 类似的判别代码来判断。不过，我们依旧建议你使用最基础也是最普通的判别办法。

> 你可能会问，那我们加这个参数，还允许它为 `null` 是为什么。有时候我们没有必要非得添加取消功能。如果我们不需要让方法支持取消，这个参数就可以传入 `null`，下面在执行和判别取消的时候，就没它什么事了。

另外，这里我们并不只是判别是否为 `null`，我们还要判断是不是用户已经发出取消的请求了。在 `CancellationToken` 结构里有一个属性，叫做 `IsCancellationRequested`，这个属性专门用来判别用户是否发出取消的请求。

接着，我们为了简单，我们就不必写成下面这样了：

```csharp
if (token.HasValue && token.Value.IsCancellationRequested)
{
    ...
}
```

这样写起来比较臃肿。C# 8 已经允许我们可以使用递归模式来判断一个对象不论是不是 `null` 都可以判断的代码模式，这样简化了代码。

那么，这一步我们就完成了。



### 第二步：在异步封装的出题方法里添加异常处理

接着，我们需要对刚才的 `GenerateAsync` 方法作出改动。我们依旧需要为方法添加 `CancellationToken` 类型的参数，然后把内部的执行代码多封装一层。

```csharp
public async Task<SudokuGrid?> GenerateAsync(CancellationToken? token)
{
    return await (
        token is { } t ? Task.Run(innerGenerate, t) : Task.Run(innerGenerate)
    );

    SudokuGrid? innerGenerate()
    {
        try
        {
            return Generate(token);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}
```

这里需要注意的是，我们引用 `Task.Run`（或者用 `Task.Factory.StartNew` 也是可以的）的时候，传入的是 Lambda 表达式，而如果签名和需要传入的 Lambda 表达式的签名一致时，是可以不写的，所以直接将方法名传入即可，它从语法上等价于 `() => innerGenerate()`。

> 注意，Lambda 表达式实际上被翻译成了一个类，并带一个执行方法。该类里会带有被捕获的参数，都以属性呈现。因此，我们只能说这是从语法上等价，但实际上并不是等价的（实际上，如果传入的是方法，则它等价传入的其实是 `new Func<SudokuGrid?>(innerGenerate)`，即一个委托的实例）。

然后需要注意这里的空递归模式 `token is { } t`。这个判别等价于 `token.HasValue`（实际上不论是可空值类型还是可空引用类型，在判别的时候，都可以采用空递归模式来判断是否为 `null`。如果不为 `null`，那么都可以写成 `obj is { } o`；相反，如果必然为 `null`，则可以写成 `!(obj is { } o)` 或 `obj is not { } o`）。

最后，`try`-`catch` 块的大括号都不可省略，它并不类似于 `if` 等控制块。

这样，我们就完成了第二步。



### 第三步：在 UI 交互的挂载函数上修改处理逻辑来捕获异常

在第二步里，我们通过本地函数的方式捕获了内部抛出的 `OperationCanceledException`。但是，为什么要捕获呢？要是我们直接在外部（比如 UI 交互的这个挂载函数 `Button_Click`）里捕获，不也是对的吗？

实际上，并不对。因为此时我们在执行本地函数 `innerGenerate` 的时候，是被 `Task.Run`（或者 `Task.Factory.StartNew`）封装了的，它传入了一个 Lambda。Lambda 最重要的特征就是创建类。换句话说，抛出异常并不是真正在 UI 上可以捕获的，而是应该在抛出异常的执行方法上才可以捕获。因此，我们无法直接捕获 `OperationCanceledException`。

这可怎么办呢？这就是为什么我们非得在内部函数返回可空类型的原因。一个 `SudokuGrid` 不管是值类型还是引用类型，我们都希望当我们取消任务执行的时候，返回 `null` 作为特殊值处理。这样的话，我们在取消的时候，只需要判断它是否为空就行了：

```csharp
private async void Button_Click(object _, RoutedEventArgs e)
{
    await internalOperation();

    async Task internalOperation()
    {
        // Disable controls to prevent any user clicking other controls,
        // which may cause the program unstable.
        DisableControls();

        // Try to generate a puzzle.
        // Returning null value means we have canceled the operation.
        // But...
        var grid = await GenerateAsync() ?? throw new OperationCanceledException();

        // Now enable controls.
        EnableControls();
    }
}
```

> 当然，SDK 里规定的 `SudokuGrid` 是一个值类型，因此我们判断 `null` 的时候用的是 `HasValue`。

注意，这里的 `operator ??` 是一个 C# 6 引入的特殊的运算符；而 `throw` 表达式可嵌入到 `??` 和 `? :` 里，则是 C# 7 里的语法。总之，上述代码等价于这个写法：

```csharp
SudokuGrid grid;
var tempGridGenerated = await GenerateAsync();
if (!tempGridGenerated.HasValue)
    throw new OperationCanceledException();
else
    grid = tempGridGenerated;
```

虽然说看起来有点绕，但是实际上代码逻辑还是很清晰的，对吧。

可是这样，我们有可能在这里抛出的异常就没有捕获了，对吧？是的，所以我们还是需要再次加上一个捕获异常的处理，来保证 UI 处理的时候可以捕获该异常。

```csharp
try
{
    return await internalOperation();
}
catch (OperationCanceledException)
{
    // User has canceled the operation.
    // Here we can do some work to redo the status for some controls.
    // Such as:
    EnableControls();
}
```

很好！我们基本上就完成了取消任务的代码了。不过……

看到我前面的注释文字里了吗，写了一个“But...”。这里还是有问题哦！出了什么问题呢？



### 第四步：添加取消令牌对象到处理里

是的，刚才的代码会报编译器错误。原因是，我们内部代码是需要传入一个参数的，而这里我们调用方并未传入参数：`await GenerateAsync()`。这怎么可能不报错呢！所以我们需要传入一个等同类型的 `CancellationToken` 对象。可是，这个对象要从哪里来呢？很简单啊，实例化一个不就可以了：

```csharp
var token = new CancellationToken();

// Or
CancellationToken token = new();

// Or
CancellationToken token = default;

// Or
var token = default(CancellationToken);
```

随你怎么写。但我并不建议使用 `default` 和 `default(CancellationToken)`；当然，我只是不建议，你要用我也管不着。

接着，我们传入到参数里。

```csharp
await GenerateAsync(token)
```

完了吗？是的，结束了。这样就搞定了整体的取消任务的代码。

当然，我们更建议你把 `token` 对象放在 `internalOperation` 这个本地函数的外部，然后当参数传进去，这样的话，我们才能保证整个调用过程下，`token` 都用的是同一个。实际上确实我们也需要保证取消的 `token` 必须是同一个才行。知道为什么吗？因为我们取消是针对于当前这个环境下的；如果你在函数内声明和实例化 `token` 的话，不是不行，但代码一来是复杂度增加，二来是你的代码会有很多奇奇怪怪的丑代码，不好维护，三来还可能会出 bug。

所以，完整的取消模式是这样写的：

```csharp
private async void Button_Click(object _, RoutedEventArgs e)
{
    var token = new CancellationToken();
    try
    {
        return await internalOperation(token);
    }
    catch (OperationCanceledException)
    {
        // User has canceled the operation.
        // Here we can do some work to redo the status for some controls.
        // Such as:
        EnableControls();
    }

    async Task internalOperation(CancellationToken token)
    {
        // Disable controls to prevent any user clicking other controls,
        // which may cause the program unstable.
        DisableControls();

        // Try to generate a puzzle.
        // Returning null value means we have canceled the operation.
        var grid = await GenerateAsync(token) ?? throw new OperationCanceledException();

        // Now enable controls.
        EnableControls();
    }
}
```



## 总结

总结一下。我们花了四个步骤来完成完成整个取消任务的代码。为了能够真正取消任务，我们必须深入到底层，使得底层能够取消。

从整个执行流程上，我们可以看出，任务是不能取消的。取消实际上是用抛出异常来中断代码执行，以达到类似取消操作的功能。而我们还知道的是，整个程序的调用顺序是 `Button_Click` -> `internalOperation`（本地函数）-> `GenerateAsync` -> `innerGenerate`（本地函数）-> `Generate`。特别是在处理 `GenerateAsync` 的时候，我们使用了 `Task.Run` 来封装本地函数，以此保证了函数可以异步调用；但实际上，这也带来了一个缺点：异常不能从外面捕获的问题，因为它是低一级的调用。



## 题外话：单词 cancel 结尾的双写咋不一样

顺带说一下。C# 里的命名在这里有一点奇妙：`CancellationToken` 里 cancel 是双写了 l 字母，而 `OperationCanceledException` 的 cancel 并未双写。感觉有点不好记？如果你要记住，实际上很简单。首先，美式英语里 l 没有双写规则（l 后缀双写是英式英语的规则），所以 canceled、cancelation 对于美式英语来说，是标准写法；而 cancelation 这个不双写的写法本身其实并不常见，大家都比较习惯了 cancellation 这个单词双写了才表示名词的表达形式，所以这个单词是双写了的。

整体来说，C# 本身是美国人发明的编程语言，所以都是遵循美式英语的取名规则来取名，当然，这里的 cancellation 看起来是个例外，实际上不是。就好像 apartment 和 flat 都是公寓一样、subway 和 underground 都是地铁一样，美式英语都喜欢用前者；但如果习惯用后者的人一多，这个固定的规则也可以被习惯的力量所打破。

