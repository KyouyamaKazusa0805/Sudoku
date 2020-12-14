# Work with Structure `Cells`

`Cells` is a struct, which means it is a value type. The instance of this type consists of 2 `long` values to describe a table of the usage of a sudoku grid. A `Cells` instance describes a cell series in a grid status.

This data structure only uses 81 of total 128 bits. Two `long` values will both use approximately only 40 bits.

You can use the code like this:

```csharp
// An instance with the third cell set.
var exemplar = new Cells { 3 };

// An instance with two cells set.
var exemplar2 = new Cells(new[] { 3, 5 });
var exemplar3 = new Cells(stackalloc[] { 3, 5 });
var examplar4 = new Cells { 3, 5 };

// An instance with 21 cells set:
// * * * | * * * | * * *
// * * * | . . . | . . .
// * * * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
var exemplar5 = new Cells(2);

// An instance with 20 cells set:
// * * . | * * * | * * *
// * * * | . . . | . . .
// * * * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
var exemplar6 = new Cells(2, false);
```

> The last constructor `.ctor(int, bool)` is a `private` method, which means you can only use it in the struct range. If you want to use this constructor, please use the array `PeerTable` in the static class `Sudoku.Constants.Processings` instead.

You can read file [`Cells.cs`](https://github.com/SunnieShine/Sudoku/blob/master/Sudoku.Core/Data/Cells.cs) to learn more about other constructors.

## `Empty` singleton

We may use `Cells.Empty` to get an empty instance, which does not contain any cell. This instance is equivalent to `default(Cells)` or `new Cells()`. However, I still recommend you that you should use `Cells.Empty` instead of the instance created by default constructor.

```csharp
var singleton = Cells.Empty;
```

## Work with operators

It will be powerful if we use the map to calculate for the relations among several tables of the cell usage.

We can use the operator `&` to get a result table of cells that two instances both contain.

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 & map2;

// * * . | * * * | * * *     . . . | * * * | . . .     . . . | * * * | . . .
// * * * | . . . | . . .     * * * | . * * | * * *     * * * | . . . | . . .
// * * * | . . . | . . .     . . . | * * * | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .  &  . . . | * . . | . . .  =  . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
```

And then you can get a new table containing the cells that cells from two cells can both "see".

If you want to get all cells from two cells, you should use the operator `|`.

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 | map2;

// * * . | * * * | * * *     . . . | * * * | . . .     * * . | * * * | * * *
// * * * | . . . | . . .     * * * | . * * | * * *     * * * | . * * | * * *
// * * * | . . . | . . .     . . . | * * * | . . .     * * * | * * * | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .  |  . . . | * . . | . . .  =  . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
```

If you want to get the cells that only one table contains, you should use the operator `^`.

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 ^ map2;

// * * . | * * * | * * *     . . . | * * * | . . .     * * . | . . . | * * *
// * * * | . . . | . . .     * * * | . * * | * * *     . . . | . * * | * * *
// * * * | . . . | . . .     . . . | * * * | . . .     * * * | * * * | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .  ^  . . . | * . . | . . .  =  . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
```

In addition, if you want to get the cells that only exist in the first map, but does not exist the second map, you should use the operator `-`.

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 - map2;

// * * . | * * * | * * *     . . . | * * * | . . .     * * . | . . . | * * *
// * * * | . . . | . . .     * * * | . * * | * * *     . . . | . . . | . . .
// * * * | . . . | . . .     . . . | * * * | . . .     * * * | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// . . * | . . . | . . .  -  . . . | * . . | . . .  =  . . * | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
```

Finally, if you want to negate the table, you should use the operator `~`.

```csharp
var map1 = new Cells(3, false);
var map = ~map1;

//    * * . | * * * | * * *     . . * | . . . | . . .
//    * * * | . . . | . . .     . . . | * * * | * * *
//    * * * | . . . | . . .     . . . | * * * | * * *
//    ------+-------+------     ------+-------+------
//    . . * | . . . | . . .     * * . | * * * | * * *
// ~  . . * | . . . | . . .  =  * * . | * * * | * * *
//    . . * | . . . | . . .     * * . | * * * | * * *
//    ------+-------+------     ------+-------+------
//    . . * | . . . | . . .     * * . | * * * | * * *
//    . . * | . . . | . . .     * * . | * * * | * * *
//    . . * | . . . | . . .     * * . | * * * | * * *
```

## Properties `CoveredRegions` and `Regions`

If you want to get the regions which all sets are in, you will use the property `CoveredRegions` to get them, where `0..9` is for block index, `9..18` is for row index and `18..27` is for column index.

```csharp
var regions1 = new Cells { 0, 1 }.CoveredRegions; // Two elements: 0 and 9.
var regions2 = new Cells { 0, 3 }.CoveredRegions; // One element: 9.
var regions3 = new Cells { 0, 1, 27, 28 }.CoveredRegions; // No element.
```

The return type is `IEnumerable<int>`, which means you can use `foreach` loop to get them.

```csharp
foreach (int region in someCells.CoveredRegions)
{
    // Code using the region.
}
```

However, if you want to get all regions that the cells cover, you should use the property `Regions`.

```csharp
var regions1 = new Cells { 0, 1 }.CoveredRegions; // Four elements: 0, 9, 18, 19.
var regions2 = new Cells { 0, 3 }.CoveredRegions; // Four elements: 0, 9, 18, 21.
var regions3 = new Cells { 0, 1, 27, 28 }.CoveredRegions; // Six elements: 0, 3, 9, 12, 18, 19.
```

## Using `foreach`

You can use `foreach` loop to iterate on each cells that the map contains.

```csharp
var map = new Cells { 0, 3, 6, 9, 12, 15, 18, 21, 24 };
foreach (int cell in map)
{
    // Code using each cell.
    int row = cell / 9 + 1, column = cell % 9 + 1;
    Console.WriteLine($"r{r}c{c}");
}
```

