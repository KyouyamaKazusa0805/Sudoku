# 使用题库进行批量解题

为了达到题库的批量完成，我做了一个 IO 处理的 API，可以读取题库的题目。

举个例子，例如我们读取一个桌面上的 txt 格式的题库，我们要完成批量解题，并且记录一些满足条件的题目，我们可以使用下面的示例代码来完成：

```csharp
using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using Sudoku.IO;
using Sudoku.Solving.Logical;
using Sudoku.Solving.Logical.Techniques;
using static System.Environment;

var desktop = GetFolderPath(SpecialFolder.Desktop);
var puzzlePath = $@"{desktop}\题目.txt";
var solver = CommonLogicalSolvers.Suitable;

var stopwatch = new Stopwatch();
stopwatch.Start();

var puzzlesCount = 0;

var lib = new GridLibrary(puzzlePath);
var total = lib.PuzzlesCount;
var traversed = 0;

await foreach (var grid in lib)
{
    Console.Clear();
    Terminal.WriteLine($"进度：{++traversed} / {total} ({(double)traversed / total:P})");

    var analysisResult = solver.Solve(grid);
    if (analysisResult is not { IsSolved: true, Steps: var steps })
    {
        continue;
    }

    var satisfiedStepsCount = 0;
    var longChaining = 0;
    foreach (var step in steps)
    {
        switch (step)
        {
            case { TechniqueTags: var tags, TechniqueCode: not (Technique.XChain or Technique.FishyCycle) }
            when tags.Flags(TechniqueTags.LongChaining) && ++longChaining >= 5:
            {
                continue;
            }
            case
            {
                TechniqueCode: Technique.Skyscraper or Technique.TwoStringKite or Technique.TurbotFish
                    or Technique.AlmostLockedPair or Technique.AlmostLockedTriple
                    or Technique.SueDeCoq or Technique.SueDeCoq3Dimension or Technique.SueDeCoqCannibalism
                    or Technique.XChain or Technique.FishyCycle
            } or { TechniqueGroup: TechniqueGroup.NormalFish or TechniqueGroup.Wing }:
            {
                satisfiedStepsCount++;
                break;
            }
        }
    }

    if (satisfiedStepsCount > 12)
    {
        await File.AppendAllTextAsync($@"{desktop}\output.txt", $"{grid}{NewLine}");
        puzzlesCount++;
    }
}

stopwatch.Stop();

Terminal.WriteLine($"满足的题目数量：{puzzlesCount}，耗时：{stopwatch.Elapsed:hh':'mm':'ss'.'fff}");
```

该做法是在解析题目，解题后计算题目里包含一些常见的非链结构技巧的数量，超过 12 个，并且链技巧不超过 5 的题目。