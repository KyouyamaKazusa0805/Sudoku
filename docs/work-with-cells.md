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

> The last constructor [`.ctor(int, bool)`](https://github.com/SunnieShine/Sudoku/blob/master/Sudoku.Core/Data/Cells.cs#L195) is a `private` method, which means you can only use it in the struct range. If you want to use this constructor, please use the array [`Peers`](https://github.com/SunnieShine/Sudoku/blob/master/Sudoku.Core/Constants/Processings.cs#L66) or [`PeerMaps`](https://github.com/SunnieShine/Sudoku/blob/master/Sudoku.Core/Constants/Processings.cs#L156) in the static class `Sudoku.Constants.Processings` instead.

You can read file [`Cells.cs`](https://github.com/SunnieShine/Sudoku/blob/master/Sudoku.Core/Data/Cells.cs) to learn more about other constructors.

## Fields

### `Empty`

We may use `Cells.Empty` to get an empty instance, which does not contain any cell. This instance is equivalent to `default(Cells)` or `new Cells()`. However, I still recommend you that you should use `Cells.Empty` instead of the instance created by default constructor to avoid the frequent memory allocation.

```csharp
var map = Cells.Empty;
```

> In the solution, I always name the variable `blahBlahMap` of type `Cells`, sometimes name it `blahBlahCellsMap`, in this way to tell with `int[]` and `Cells` variables. In contrast, I always name the variable `blahBlahCells` of type `int[]` if those elements are represented as cells indeed.



## Properties

### `IsEmpty`

The property `IsEmpty` is used to determine whether the current collection contain any elements. It's not calculated until it is called, rather than a just-in-time property. You can also replace this with `map.Count == 0`, where the property `Count` is a just-in-time property, which won't spend any extra time on calculating when read. For the readability, please use `IsEmpty` instead.

```csharp
var map = new Cells { 3, 5 };
Console.WriteLine(map.IsEmpty); // false
```



### `Count`

The property `Count` returns the number of elements in this collection. If the collection is empty, the return value will be 0.

```csharp
var map = new Cells { 3 };
var map2 = new Cells(3);

Console.WriteLine(map.Count); // 1
Console.WriteLine(map2.Count); // 21
```



### `InOneRegion`

The property `InOneRegion` checks whether **all** cells in this collection lies in the same row, column or block.

```csharp
var map = new Cells { 0, 1, 7 };
var map2 = new Cells { 0, 1, 9, 10 };

Console.WriteLine(map.InOneRegion); // true
Console.WriteLine(map2.InOneRegion); // false
```



### `BlockMask`、`RowMask` 和 `ColumnMask`

These three properties return `short` value indicating which rows, columns or blocks the collection covers. The return value is a 9-bit `short` integer, corresponding to block 1 to 9, row 1 to 9 or column 1 to 9. If the collection covers this region, the corresponding bit will be set 1.

```csharp
var map = new Cells { 0, 1, 9, 10 };

short blockMask = map.BlockMask; // 0b000_000_001
short rowMask = map.RowMask; // 0b000_000_011
short columnMask = map.ColumnMask; // 0b000_000_011
```

Then we should use the extension method `short.GetEnumerator` (in the project `SystemExtensions`) to iterate on each bit.

```csharp
// Using namespace.
using System.Extensions;

// Create an instance.
var map = new Cells { 0, 1, 9, 10 };

// Iterate on each set bit using short.GetEnumerator
// in the static class System.Extensions.Int16Ex.
foreach (int bit in map.RowMask)
{
    Console.Write($"r{bit + 1}");
    Console.Write(", ");
}
```

The output result will be `r1, r2, `.



### `CoveredLine`

This property get a region which the collection lies in. Different with the property `CoveredRegions` below, this property only checks for rows and columns. If found, the value will be returned directly (10 - 17 is corresponding to row 1 to 9, and 18 - 26 is corresponding to column 1 to 9). Therefore, the value will be an `int`.

```csharp
var map = new Cells { 0, 1, 7 };
var map2 = new Cells { 0, 1, 9, 10 };

Console.WriteLine(map.CoveredLine); // 9
Console.WriteLine(map2.CoveredLine); // -1
```



### `CoveredRegions`

This property will get all covered regions of the collection. If the cells spanned multiple regions, the property will return the value -1. If all cells lie in a same region, it will be recorded and returns them as an `int` value.

Different with `CoveredLine`, this property calculates for all possible regions that is satisfied the coverage condition. Although the return value is also an `int`, this property is represented by each bit.

```csharp
var map = new Cells { 0, 1 };
var map2 = new Cells { 0, 1, 7 };

int regions = map.CoveredRegions; // 0b000000000_000000001_000000001
int regions2 = map2.CoveredRegions; // 0b000000000_000000001_000000000
```

Then you should use `foreach` to iterate on each bit.

```csharp
using System.Extensions;

foreach (int region in regions)
{
    Console.Write($@"{(region / 9) switch
    {
        >= 0 and < 9 => 'b',
        >= 9 and < 18 => 'r',
        >= 18 and < 27 => 'c'
    }}{region % 9 + 1}, ");
}
```

In this way the output result will be `b1, r1, `.



### `Regions`

This property is the summary result of three properties `BlockMask`、`RowMask` 和 `ColumnMask`. It integrates all possible regions that the collection lies in. It also returns an `int` value represented as each bit.



### `PeerIntersection`

This property is difficult to explain, but it provides with an easy way to check the intersection in sudoku. This property returns a `Cells` instance that contains all cells that **all cells in this current collection** can see.

```csharp
var map = new Cells { 3, 12 };
var peerInter = map.PeerIntersection;

//      PeerMaps[3]               PeerMaps[12]               peerInter
// * * . | * * * | * * *     . . . | * * * | . . .     . . . | * * * | . . .
// * * * | . . . | . . .     * * * | . * * | * * *     * * * | . . . | . . .
// * * * | . . . | . . .     . . . | * * * | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
```

From the above example, the peer intersection returns the intersection of `PeerMaps[3]` and `PeerMaps[12]`.



## Indexers

### `int this[int]`

This indexer returns the cell at the specified position.

```csharp
var map = new Cells { 1, 20, 3, 8 };
int cell = map[2];

Console.WriteLine($"r{cell / 9 + 1}c{cell % 9 + 1}");
```

In fact `map` contains 4 cells, 1, 3, 8 and 20 in order, so the result `cell` gets the third cell, i.e. 8. Therefore the output value will be `r1c8`. Please note that the argument should be 0-based.



### `int this[Index]`

C# 8 introduces indices and ranges. If a `class` or `struct` contains a property named `Count` or `Length`, and it holds a indexer like below one, this indexer will be generated automatically. In this way we can get the value from the end.

```csharp
var map = new Cells { 1, 20, 3, 8 };
int cell = map[^1];

Console.WriteLine($"r{cell / 9 + 1}c{cell % 9 + 1}");
```

`map[^1]` is equivalent to `map[3]`, so the result is `r3c3`。



## Methods (Partial introduction)

### `readonly bool AllSetsAreInOneRegion(out int)`

This method is equivalent to `InOneRegion`. The only difference between those two is the current method will be with an `out` parameter, in order to store the region that satisfied the condition.



### `readonly short GetSubviewMask(int)`

This method gets all cells that only lie in the specified region. Due to 9 cells per region, those cells will be kept as a bit, so the return value will be a `short` value.

```csharp
using System.Extensions;

var map = new Cells { 0, 1, 7 };
short cellsMask = map.GetSubviewMask(0); // 0 means b1.

Console.WriteLine(cellsMask); // 0b000_000_011
```



### `readonly int[] ToArray`

This method gets all cells in this collection. In fact there's a `private` property `Offsets` can do this work, but it is `private`, so `ToArray` is an encapsulation.



### `readonly string ToString(string?)`

This method returns a `string` that represents for the specified format of this `Cells` collection. The argument now supports the following three values:

* `"N"`、`"n"` or `null`: Output as a normal way.
* `B` or `b`: Output them as a long series of the value 0 and 1. 1 means the collection contains that cell.
* `T` 或 `t`: Output the collection as a table.

In addition, `Cells` provides a parameterless method `ToString`. It is equivalent to this method with the argument `null`.



### `void Add(int)` 和 `void AddAnyway(int)`

These two methods both mean add a cell into this collection. The difference between them is, `Add` is used in collection initializer, and `Anyway` is called by users.

Collection initializer allows us adding extra cells into the instance initialized by `new` clause. However, this initializer also supports to pass a negative value (using the bitwise negation operator `~`) to remove a cell from the specified collection.

From the code we can learn that the collection contains a copy constructor `.ctor(in Cells)` to copy an old collection, and add or remove some cells from/into it.

```csharp
var map = new Cells { 0, 1, 7 };
var map2 = new Cells(map) { ~7 }; // Remove cell 7.
```

While the method `AddAnyway` only used for add a cell.



## Operators

### `operator &`

We use the operator `&` to create a new collection that both two `Cells` instances contain.

```csharp
var map1 = new Cells(3, false); // Or PeerMaps[3].
var map2 = new Cells(12, false); // Or PeerMaps[12].
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



### `operator |`

If we want to get all cells from two different instances, we can use the operator `|`.

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



### `operator ^`

If we want to check the cells that only contains in one instance, we should use the operator `^`.

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



### `operator -`

If we want to get the cells that are only contained in the first instance, but the second one doesn't contain, we should use the operator `-`.

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



### `operator ~`

If we want to negate a instance, just use the operator `~`.

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



### `operator +`

When we add a cell, `operator +` can help you. In fact, `operator +` is same as `new Cells(old) { newCell }`.



### `operator -`

When we remove a cell, `operator -` can help you. In fact, `operator -` is same as `new Cells(old) { ~newCellToRemove }`.



### `operator >` and `operator <`

Those two operators are only used to simplify the code. If the first collection contains "more" cells than the second one, the operator `>` will returns`true`. However, here "more" means the collections covers the another one, and last some cells. In other words, `left > right` is equivalent to the code `!(left - right).IsEmpty`. Therefore, in definition, `operator <` means `(left - right).IsEmpty`.



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

