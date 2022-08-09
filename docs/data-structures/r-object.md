# `R` 资源管理对象

整个项目贯穿了一个全球化的思想。不过因为项目需要使用的全球化操作和一般直接汉译英或英译汉这样的整段内容翻译不同，需要逐单词翻译并按照中英语语法的语序来调整汉化结果，因此产生了资源字典的概念。

本项目使用了资源字典，包括 `Sudoku.Core` 和 `Sudoku.UI` 所规定和定义的资源字典。虽然两种定义方式和模式不相同（`Sudoku.UI` 是采用 XAML 的 `ResourceDictionary` 标签，而 `Sudoku.Core` 直接使用的是 ResX 的文件定义），但是整个项目里实现了一个通用字典取值的数据类型，叫 `MergedResources`。

这个类型的实例化操作是无意义的，但它包含一个静态只读成员 `R`。你可以直接使用这个成员，通过索引器运算来获取资源字典里的数据，不论是否在哪里，只要在载入项目的时候关联上取值方法即可完成取值操作。

## 对 `Sudoku.UI` 以外项目的取值行为

由于实现的 UI 界面单独具有一种取值行为，因此我们需要分开说。如果不是 UI 项目的话，那么它的取值就直接将资源字典里的键（字符串）作为取值条件放进索引器里当参数即可，比如

```csharp
string? result = R["KeyYouWantToGetItsCorrespondingValue"];
```

请注意，该索引器运算永远返回可空字符串 `string?` 类型而非不可空字符串 `string` 类型，因为可能键并不对应到合适的数值信息。不过，你如果觉得该数值一定会取到正确的值的话，可以直接使用后缀 `null` 禁用运算符来避免编译器对此的提示：

```csharp
string result = R["KeyYouWantToGetItsCorrespondingValue"]!;
```

## 对 `Sudoku.UI` 项目的取值行为

该项目由于包含资源字典，所以取值行为较为复杂一些。

如果并不是使用 XAML 文件创建的资源，那么它一定存储在 `Sudoku.Core` 的资源字典之中，因此只需要使用和前文一致的办法取值即可；但如果是定义在 XAML 文件里的资源的话，需要你在 `App.xaml.cs` 文件里，往 `R` 对象添加对应的路由操作。

我们找到该文件的 `OnLaunched` 方法。在第一行添加一行路由操作即可：

```csharp
R.AddExternalResourceFetecher(
    new[] { GetType().Assembly, typeof(XamlMetaDataProvider).Assembly },
    key => Current.Resources.TryGetValue(key, out object? t) && t is string r ? r : null
);
```

其中，`AddExternalResourceFetecher` 方法是为了为别的不存储在 `Sudoku.Core` 的固有字典里的资源的额外处理过程。第一个参数传入的是需要支持的程序集对象。这里我们需要为两个 UI 相关的项目都添加路由操作，因此直接传入数组，将两个对象的对应程序集给出，并作为数组传递过去即可；第二个参数则是处理行为。我们通过键值对取值，因此该传入的 lambda 的参数 `key` 是 `string` 类型的，而它的返回值类型则是 `string?` 类型的。

> 这里的 `XamlMetaDataProvider` 是为了去获取 `Sudoku.UI.Core` 项目的程序集的“标志性”类型，它被编译器自动生成，是关于 XAML 元数据绑定和支持操作的基本类型，只要是 UI 项目就会有这样的类型自动生成。
