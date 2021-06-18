本文介绍如何为窗体添加一个新的菜单选项。


## 添加一个菜单功能

为窗体添加菜单功能，需要如下的一些步骤。



### 第一步：为添加的功能的中文和英文描述添加对应的字段信息

举个例子。比如我现在想要添加一个功能，用来导出和导入绘图的基本信息，那么我们可以添加两个选项到菜单栏里。那么，这两个功能都必须取个名，然后放进菜单栏里。

我们先找到这个功能大概应该放在菜单栏的哪一个地方。显然，导入导出配置应该是文件操作，所以放在程序的第一栏（“文件……”栏）。然后，我们找到合适的位置把内容插进去：

```xaml
<!-- LangEnUs.xaml -->
<sys:String x:Key="_menuItemFileLoadDrawing">Load drawing contents from...</sys:String>
<sys:String x:Key="_menuItemFileSaveDrawing">Save drawing contents to...</sys:String>

<!-- LangZhCn.xaml -->
<sys:String x:Key="_menuItemFileLoadDrawing">读取绘图信息……</sys:String>
<sys:String x:Key="_menuItemFileSaveDrawing">保存绘图信息……</sys:String>
```

如图所示。

![步骤 1-1](https://images.gitee.com/uploads/images/2021/0124/110422_091a4fe2_1449374.png "Step1-1.png")

![步骤 1-2](https://images.gitee.com/uploads/images/2021/0124/110432_0aae6aa7_1449374.png "Step1-2.png")

> 注意，这里取名 `_menuItemFileLoadDrawing` 和 `_menuItemFileSaveDrawing` 是可以自己随意定义的，不过这里我们的约定是，**把字符串的键取名为控件名**，这样可以在引用这个信息的时候直接对应上文本，在找的时候也方便。

这样，第一步就完成了。



### 第二步：在窗体的 `xaml` 代码里添加 `MenuItem` 控件和基本的信息

然后，我们需要直接在窗体里添加处理的代码和对应的控件。

Xaml 文件里我们这么写：

```xaml
<Separator/>
<MenuItem Header="{DynamicResource _menuItemFileLoadDrawing}"
          x:Name="_menuItemFileLoadDrawing"
          Click="MenuItemFileLoadDrawing_Click"/>
<MenuItem Header="{DynamicResource _menuItemFileSaveDrawing}"
          x:Name="_menuItemFileSaveDrawing"
          Click="MenuItemFileSaveDrawing_Click"/>
```

注意到 `Click` 属性，我们给出的这个字符串，是对应了处理的方法的。如果方法没有对应上，会产生编译错误。

```csharp
private void MenuItemFileLoadDrawing_Click(object sender, RoutedEventArgs e)
{

}

private void MenuItemFileSaveDrawing_Click(object sender, RoutedEventArgs e)
{

}
```

对应上面这两个方法。输入的时候，我们可以输入 `Click=`，然后等待编译器响应。编译器一会儿会弹出菜单提示你添加一个挂载方法名。这个方法名默认是 `控件名_Click`，但由于控件名以下划线起头，所以方法名此时不满足方法取名规范。因此我们需要重新调整方法名称。调整名称在后台，使用编译器修复的功能来修改更改名称为正确的、满足规范的帕斯卡命名；而前台的 Xaml 文件会自动更改，所以不需要两边都调整，它是自动的。

![步骤 2-1](https://images.gitee.com/uploads/images/2021/0124/110446_fc07aa36_1449374.png "Step2-1.png")

![步骤 2-2](https://images.gitee.com/uploads/images/2021/0124/110454_3ef76a92_1449374.png "Step2-2.png")


### 第三步：添加处理逻辑

下面就是为这两个方法添加处理逻辑了。当然，注意确保你的代码是逻辑正确和严谨的。这里给出一个基本的实现代码，用到了别的类和信息，请从代码里查看，这里仅提供这个模块的调用实现（所有文件都抄过来就太多了）。

```csharp
/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
private void MenuItemFileLoadDrawing_Click(object sender, RoutedEventArgs e)
{
    var dialog = new OpenFileDialog
    {
        DefaultExt = "drawings",
        Filter = (string)LangSource["FilterLoadingDrawingContents"],
        Multiselect = false,
        Title = (string)LangSource["TitleLoadingDrawingContents"]
    };

    if (dialog.ShowDialog() is true)
    {
        try
        {
            _currentPainter.CustomView =
                JsonSerializer.Deserialize<MutableView>(
                File.ReadAllText(dialog.FileName), _serializerOptions
            ) ?? throw new JsonException("The custom view is currently null.");

            UpdateImageGrid();
        }
        catch
        {
            Messagings.FailedToLoadDrawingContents();
        }
    }
}

/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
private void MenuItemFileSaveDrawing_Click(object sender, RoutedEventArgs e)
{
    var customView = _currentPainter.CustomView;
    if (customView is null)
    {
        Messagings.FailedToSaveDrawingContentsDueToEmpty();
        return;
    }

    var dialog = new SaveFileDialog
    {
        AddExtension = true,
        CheckPathExists = true,
        DefaultExt = "drawings",
        Filter = (string)LangSource["FilterSavingDrawingContents"],
        Title = (string)LangSource["TitleSavingDrawingContents"]
    };

    if (dialog.ShowDialog() is true)
    {
        try
        {
            string json = JsonSerializer.Serialize(customView, _serializerOptions);
            File.WriteAllText(dialog.FileName, json);

            Messagings.SaveSuccess();
        }
        catch
        {
            Messagings.FailedToSaveDrawingContents();
        }
    }
}
```



### 第四步：添加字典信息

前文实现的时候，我们用到了四个“写死”的信息：读取和存储文件对话框的标题，和对应的文件筛选器。这些信息写进去之后不方便代码拓展和修改。所以我们依旧需要借助字典来存储它们。

找到 `LangEnUs.xaml` 和 `LangZhCn.xaml` 文件，然后添加如下的内容：

```xaml
<!-- LangEnUs.xaml -->
<sys:String x:Key="FilterLoadingDrawingContents">Text file|*.txt|Drawing contents file|*.drawings|JSON file|*.json|All files|*.*</sys:String>
<sys:String x:Key="TitleLoadingDrawingContents">Open drawing contents file from...</sys:String>
<sys:String x:Key="FilterSavingDrawingContents">Text file|*.txt|Drawing contents file|*.drawings|JSON file|*.json</sys:String>
<sys:String x:Key="TitleSavingDrawingContents">Save drawing contents file to...</sys:String>

<!-- LangZhCn.xaml -->
<sys:String x:Key="FilterLoadingDrawingContents">文本文件|*.txt|绘图信息文件|*.drawings|JSON 文件|*.json|所有文件|*.*</sys:String>
<sys:String x:Key="TitleLoadingDrawingContents">打开绘图信息文件……</sys:String>
<sys:String x:Key="FilterSavingDrawingContents">文本文件|*.txt|绘图信息文件|*.drawings|JSON 文件|*.json</sys:String>
<sys:String x:Key="TitleSavingDrawingContents">保存绘图文件……</sys:String>
```

![步骤 4-1](https://images.gitee.com/uploads/images/2021/0124/110506_a304f229_1449374.png "Step4-1.png")

![步骤 4-2](https://images.gitee.com/uploads/images/2021/0124/110515_2f348cbe_1449374.png "Step4-2.png")

然后，既然都加上了，记得替换原本代码里“写死”了的信息。用法自然就是 `(string)LangSource[键名]`。当然，你写全也没问题：`(string)Application.Current.Resources[键名]`。



### 第五步：将这两个方法移动到 `MainWindow.MenuItem.cs` 文件里

由于这两个方法是通过 `MenuItem` 控件挂载起来的，所以我们为了代码可读性和规范，将其移动到 `MainWindow.MenuItem.cs` 文件里去，找到合适的地方放上去就行，不需要修改里面的东西。当然，添加注释行是顺带的工作：

```csharp
/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
private void MenuItemFileLoadDrawing_Click(object sender, RoutedEventArgs e)
{
    // ...
}

/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
private void MenuItemFileSaveDrawing_Click(object sender, RoutedEventArgs e)
{
    // ...
}
```

因为我给所有需要挂载 `Click` 事件的方法都写了文档注释，所以这里引用一下就可以了，不需要你去改什么东西。文档注释里用到的这个 `Events` 类在 `Sudoku.DocComments` 里；如果发现没有引入的话，`using` 一下就可以了。



### 第六步：检查代码的完整性、正确性和严谨性

最后一步就是测试和调试程序了。

![步骤 6-1](https://images.gitee.com/uploads/images/2021/0124/110529_3dba969f_1449374.png "Step6-1.png")

![步骤 6-2](https://images.gitee.com/uploads/images/2021/0124/110537_fddc48bc_1449374.png "Step6-2.png")

![步骤 6-3](https://images.gitee.com/uploads/images/2021/0124/110548_fb410af3_1449374.png "Step6-3.png")

![步骤 6-4](https://images.gitee.com/uploads/images/2021/0124/110555_a2ed1a58_1449374.png "Step6-4.png")