# 代码反射

代码反射实际上就是使用这个解决方案提供的 API，来检测代码本身的一些相关信息，比如代码行数、代码字符数、代码文件总数等。你需要使用的是 `Sudoku.Test.FileCounting` 类型提供的 API。

```csharp
using Sudoku.Test;

// Uses the static method to get the reflection information result.
var result = FileCounting.CountUp();

// Output the result.
Console.WriteLine(result.ToString());
```

我们通过使用 `FileCounting.CountUp` 方法来获取基本的数据信息，并得到结果后，返回该结果。这个方法返回的是 `FileCounterResult` 类型的实例，因此要想显示结果的话，请使用 `ToString` 方法，或者直接写进 `Console.WriteLine` 里不写 `ToString` 的调用部分也行。

另外，你仍然可以使用这个类型的底层封装起来的代码。底层使用到的类型叫 `FileCounter`，它位于 `Sudoku.Diagnostic` 命名空间。因此要想使用代码的话，需要引用该命名空间。

```csharp
using Sudoku.Diagnostics;

// Uses the type 'FileCounter'.
var result = new FileCounter(
    root: Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName,
    extension: "cs",
    withBinOrObjDirectory: false
).CountUp();

// Output the result.
Console.WriteLine(result.ToString());
```

这样也可以。注意构造器的第一个参数——`root` 参数，这个参数传入的地址是我们要检测的整个解决方案的 sln 文件的所在路径，当然，这里是让你传入文件夹，并不是 sln 文件的文件路径，而是 sln 文件所在的文件夹的文件夹路径。

另外，因为它是递归检测里面的所有文件，所以你可以把 `root` 参数的路径再往前进一步：进入到 src 文件夹里，也是可以的。

最后，`FileCounter` 类型自身在处理过程之中会自动去掉空行，所以无需担心空行会占用数据结果的一部分。