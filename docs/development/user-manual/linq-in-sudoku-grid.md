没想到吧，我还给 SudokuGrid 实现了 LINQ。下面介绍一下基本用法。


## `select` 和 `let` 语句

首先，我们可以使用 `Select` 方法，对盘面的候选数作一个映射。

```csharp
// 基本写法：取得所有候选数。
var selection = from candidate in grid select candidate;

// 取所有候选数的单元格。
var selection = from candidate in grid select candidate / 9;

// 使用 let 语句。
var selection = from candidate in grid
                let cell = candidate / 9
                select cell;
```

它们都可以使用 `foreach` 进行迭代。

```csharp
foreach (int candidate in selection)
{
    // ...
}
```

## `where` 语句

我们可以设定一个条件，对这个条件进行筛选。

```csharp
// 基本筛选。
var selection = from candidate in grid
                where candidate % 9 == 3
                select candidate;

// 使用 let 语句。
var selection = from candidate in grid
                let cell = candidate / 9
                where RegionMaps[3].Contains(cell)
                select candidate;

// 双 let。
var selection = from candidate in grid
                let cell = candidate / 9
                let digit = candidate % 9
                where RegionMaps[3].Contains(cell) && digit == 3
                select candidate;
```

## `group-by` 语句

当然，我还写了 `group-by` 语句用来分组。

```csharp
foreach (var candidateGroup in from candidate in grid group candidate by candidate / 9)
{
    //int key = candidateGroup.Key;
    Console.WriteLine(new Candidates(candidateGroup).ToString());
}
```

请注意，在 C# 的 LINQ 里，所有的 LINQ 语句类型都会对应一种不同的迭代器，这是底层的实现。它的细节可以细到一个 `let` 和两个连着的 `let` 都使用不同的迭代器。这也是比较好实现的（虽然这看起来很复杂），也是正确的实现模式。

如果你要实现 LINQ 的迭代的话，一定要对这所有的情况都要实现迭代器，以避免组合不存在对应的迭代器而产生编译器错误。举个例子，如果 `let` 和 `where` 连续使用的迭代器没有实现的话，可能会引起 CS8016 错误，提示你是否 LINQ 实现完整了。