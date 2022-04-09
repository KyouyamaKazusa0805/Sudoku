# 回文数独序列搜索

搜索回文数独里的序列需要我们给出回文的基本约定：格子和格子必须是横、纵或者斜向相连的。

算法使用 DFS 来书写，因为它对初学者比较友好。

## 源代码

来看一下源码。

### `Program` 类部分

```csharp
static partial class Program
{
    private static readonly Cells[] AdjacentCells =
    {
        Cells.Empty + 1 + 9 + 10,
        Cells.Empty + 0 + 2 + 9 + 10 + 11,
        Cells.Empty + 1 + 3 + 10 + 11 + 12,
        Cells.Empty + 2 + 4 + 11 + 12 + 13,
        Cells.Empty + 3 + 5 + 12 + 13 + 14,
        Cells.Empty + 4 + 6 + 13 + 14 + 15,
        Cells.Empty + 5 + 7 + 14 + 15 + 16,
        Cells.Empty + 6 + 8 + 15 + 16 + 17,
        Cells.Empty + 7 + 16 + 17,
        Cells.Empty + 0 + 1 + 10 + 18 + 19,
        Cells.Empty + 0 + 1 + 2 + 9 + 11 + 18 + 19 + 20,
        Cells.Empty + 1 + 2 + 3 + 10 + 12 + 19 + 20 + 21,
        Cells.Empty + 2 + 3 + 4 + 11 + 13 + 20 + 21 + 22,
        Cells.Empty + 3 + 4 + 5 + 12 + 14 + 21 + 22 + 23,
        Cells.Empty + 4 + 5 + 6 + 13 + 15 + 22 + 23 + 24,
        Cells.Empty + 5 + 6 + 7 + 14 + 16 + 23 + 24 + 25,
        Cells.Empty + 6 + 7 + 8 + 15 + 17 + 24 + 25 + 26,
        Cells.Empty + 7 + 8 + 16 + 25 + 26,
        Cells.Empty + 9 + 10 + 19 + 27 + 28,
        Cells.Empty + 9 + 10 + 11 + 18 + 20 + 27 + 28 + 29,
        Cells.Empty + 10 + 11 + 12 + 19 + 21 + 28 + 29 + 30,
        Cells.Empty + 11 + 12 + 13 + 20 + 22 + 29 + 30 + 31,
        Cells.Empty + 12 + 13 + 14 + 21 + 23 + 30 + 31 + 32,
        Cells.Empty + 13 + 14 + 15 + 22 + 24 + 31 + 32 + 33,
        Cells.Empty + 14 + 15 + 16 + 23 + 25 + 32 + 33 + 34,
        Cells.Empty + 15 + 16 + 17 + 24 + 26 + 33 + 34 + 35,
        Cells.Empty + 16 + 17 + 25 + 34 + 35,
        Cells.Empty + 18 + 19 + 28 + 36 + 37,
        Cells.Empty + 18 + 19 + 20 + 27 + 29 + 36 + 37 + 38,
        Cells.Empty + 19 + 20 + 21 + 28 + 30 + 37 + 38 + 39,
        Cells.Empty + 20 + 21 + 22 + 29 + 31 + 38 + 39 + 40,
        Cells.Empty + 21 + 22 + 23 + 30 + 32 + 39 + 40 + 41,
        Cells.Empty + 22 + 23 + 24 + 31 + 33 + 40 + 41 + 42,
        Cells.Empty + 23 + 24 + 25 + 32 + 34 + 41 + 42 + 43,
        Cells.Empty + 24 + 25 + 26 + 33 + 35 + 42 + 43 + 44,
        Cells.Empty + 25 + 26 + 34 + 43 + 44,
        Cells.Empty + 27 + 28 + 37 + 45 + 46,
        Cells.Empty + 27 + 28 + 29 + 36 + 38 + 45 + 46 + 47,
        Cells.Empty + 28 + 29 + 30 + 37 + 39 + 46 + 47 + 48,
        Cells.Empty + 29 + 30 + 31 + 38 + 40 + 47 + 48 + 49,
        Cells.Empty + 30 + 31 + 32 + 39 + 41 + 48 + 49 + 50,
        Cells.Empty + 31 + 32 + 33 + 40 + 42 + 49 + 50 + 51,
        Cells.Empty + 32 + 33 + 34 + 41 + 43 + 50 + 51 + 52,
        Cells.Empty + 33 + 34 + 35 + 42 + 44 + 51 + 52 + 53,
        Cells.Empty + 34 + 35 + 43 + 52 + 53,
        Cells.Empty + 36 + 37 + 46 + 54 + 55,
        Cells.Empty + 36 + 37 + 38 + 45 + 47 + 54 + 55 + 56,
        Cells.Empty + 37 + 38 + 39 + 46 + 48 + 55 + 56 + 57,
        Cells.Empty + 38 + 39 + 40 + 47 + 49 + 56 + 57 + 58,
        Cells.Empty + 39 + 40 + 41 + 48 + 50 + 57 + 58 + 59,
        Cells.Empty + 40 + 41 + 42 + 49 + 51 + 58 + 59 + 60,
        Cells.Empty + 41 + 42 + 43 + 50 + 52 + 59 + 60 + 61,
        Cells.Empty + 42 + 43 + 44 + 51 + 53 + 60 + 61 + 62,
        Cells.Empty + 43 + 44 + 52 + 61 + 62,
        Cells.Empty + 45 + 46 + 55 + 63 + 64,
        Cells.Empty + 45 + 46 + 47 + 54 + 56 + 63 + 64 + 65,
        Cells.Empty + 46 + 47 + 48 + 55 + 57 + 64 + 65 + 66,
        Cells.Empty + 47 + 48 + 49 + 56 + 58 + 65 + 66 + 67,
        Cells.Empty + 48 + 49 + 50 + 57 + 59 + 66 + 67 + 68,
        Cells.Empty + 49 + 50 + 51 + 58 + 60 + 67 + 68 + 69,
        Cells.Empty + 50 + 51 + 52 + 59 + 61 + 68 + 69 + 70,
        Cells.Empty + 51 + 52 + 53 + 60 + 62 + 69 + 70 + 71,
        Cells.Empty + 52 + 53 + 61 + 70 + 71,
        Cells.Empty + 54 + 55 + 64 + 72 + 73,
        Cells.Empty + 54 + 55 + 56 + 63 + 65 + 72 + 73 + 74,
        Cells.Empty + 55 + 56 + 57 + 64 + 66 + 73 + 74 + 75,
        Cells.Empty + 56 + 57 + 58 + 65 + 67 + 74 + 75 + 76,
        Cells.Empty + 57 + 58 + 59 + 66 + 68 + 75 + 76 + 77,
        Cells.Empty + 58 + 59 + 60 + 67 + 69 + 76 + 77 + 78,
        Cells.Empty + 59 + 60 + 61 + 68 + 70 + 77 + 78 + 79,
        Cells.Empty + 60 + 61 + 62 + 69 + 71 + 78 + 79 + 80,
        Cells.Empty + 61 + 62 + 70 + 79 + 80,
        Cells.Empty + 63 + 64 + 73,
        Cells.Empty + 63 + 64 + 65 + 72 + 74,
        Cells.Empty + 64 + 65 + 66 + 73 + 75,
        Cells.Empty + 65 + 66 + 67 + 74 + 76,
        Cells.Empty + 66 + 67 + 68 + 75 + 77,
        Cells.Empty + 67 + 68 + 69 + 76 + 78,
        Cells.Empty + 68 + 69 + 70 + 77 + 79,
        Cells.Empty + 69 + 70 + 71 + 78 + 80,
        Cells.Empty + 70 + 71 + 79
    };


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Remove<T>(this List<T> @this) =>
        @this.RemoveAt(@this.Count - 1);
}
```

### DFS 搜索部分

```csharp
void dfs(in Grid grid, int c1, int c2, List<int> list1, List<int> list2, ref Cells cells1, ref Cells cells2)
{
    // 双层循环迭代 c1 和 c2 自己的所有周围单元格。
    foreach (int adjacentCell1 in AdjacentCells[c1])
    {
        foreach (int adjacentCell2 in AdjacentCells[c2])
        {
            // 如果互相包含单元格，说明单元格已经被记录过。
            if (cells1.Contains(adjacentCell1) || cells2.Contains(adjacentCell1)
                || cells1.Contains(adjacentCell2) || cells2.Contains(adjacentCell2))
            {
                continue;
            }

            // 如果数字不同，说明不能继续连接。
            if (grid[adjacentCell1] != grid[adjacentCell2])
            {
                continue;
            }

            if (
                // 检查是否在同一个格子。
                adjacentCell1 == adjacentCell2
            )
            {
                // 其中一条路径找到，记录进来并且退出。
                // 注意此时 adjacentCell1（或 adjacentCell12）是没有记录进来到列表里的，这里需要我们手动加进来。
                var target = list1.ToArray()
                    .Concat(new[] { adjacentCell1 })
                    .Concat(list2.ToArray().Reverse());
                if (target.Count() >= 长度) // 此处的长度可以随意调整。
                {
                    palindromes.Add(target);

#if DEBUG
                    var copied = grid;
                    Debug.WriteLine(
                        string.Join(
                            " -> ",
                            from cell in target
                            select $"{Cells.Empty + cell}({copied[cell] + 1})"));
#endif
                }

                // 注意这里必须要退出，避免继续搜寻导致的不必要的浪费时间。
                // 可以算是在递归的时候进行剪枝。
                return;
            }
            else if (
                // 检查是否其中一个格子在另外一个格子周围 8 个格子的范围之中。
                AdjacentCells[adjacentCell1].Contains(adjacentCell2)
                || AdjacentCells[adjacentCell2].Contains(adjacentCell1)
            )
            {
                // 其中一条路径找到，记录进来并且退出。
                // 注意此时 adjacentCell1（或 adjacentCell2）是没有记录进来到列表里的，这里需要我们手动加进来。
                var target = list1.ToArray()
                    .Concat(new[] { adjacentCell1, adjacentCell2 })
                    .Concat(list2.ToArray().Reverse());
                if (target.Count() >= 长度) // 此处的长度可以随意调整。
                {
                    palindromes.Add(target);

#if DEBUG
                    var copied = grid;
                    Debug.WriteLine(
                        string.Join(
                            " -> ",
                            from cell in target
                            select $"{Cells.Empty + cell}({copied[cell] + 1})"));
#endif
                }

                Console.WriteLine();

                // 注意这里必须要退出，避免继续搜寻导致的不必要的浪费时间。
                // 可以算是在递归的时候进行剪枝。
                return;
            }

            // 追加单元格信息到集合里去，准备进行递归。
            list1.Add(adjacentCell1);
            list2.Add(adjacentCell2);
            cells1.Add(adjacentCell1);
            cells2.Add(adjacentCell2);

            // 进行深度优先遍历。
            // 注意这里我们需要深度优先而不是广度优先。因为我们这里需要尽量找长的回文序列，
            // 广度优先遍历是不会退栈的，虽然效率性能什么的会比深度优先要好，但长序列不会立刻出现，
            // 毕竟一点一点搜的。
            dfs(grid, adjacentCell1, adjacentCell2, list1, list2, ref cells1, ref cells2);

            // 递归当前单元格完成，需要退栈，将记录的单元格删除。
            list1.Remove();
            list2.Remove();
            cells1.Remove(adjacentCell1);
            cells2.Remove(adjacentCell2);
        }
    }
}
```

### `Main` 方法部分

```csharp
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Concepts.Collections;
using Sudoku.Generating;

// 声明一个出题器对象。
var generator = new HardPatternPuzzleGenerator();

// 回文序列表。
var palindromes = new List<IEnumerable<int>>();

// 出题。
if (generator.Generate() is not { IsUndefined: false, Solution: var solution })
{
    return;
}

Console.WriteLine(solution.ToString("!"));

// 定义变量。
var list1 = new List<int>();
var list2 = new List<int>();
var cells1 = Cells.Empty;
var cells2 = Cells.Empty;

// 两层循环枚举迭代回文序列的开头和结尾。
for (int c1 = 0; c1 < 71; c1++)
{
    for (int c2 = c1 + 1; c2 < 72; c2++)
    {
        if (solution[c1] != solution[c2])
        {
            continue;
        }

        list1.Add(c1);
        list2.Add(c2);
        cells1.Add(c1);
        cells2.Add(c2);

        dfs(solution, c1, c2, list1, list2, ref cells1, ref cells2);

        list1.Remove();
        list2.Remove();
        cells1.Remove(c1);
        cells2.Remove(c2);
    }
}

foreach (var palindrome in palindromes)
{
    Console.Write($"Length: {palindrome.Count()} |> ");
    Console.WriteLine(string.Join(", ", palindrome));
    Console.WriteLine();
}
```

