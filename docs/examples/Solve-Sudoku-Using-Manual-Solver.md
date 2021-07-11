# 使用人工解题器解题

如果我们要想完成对人工解题的操作，我们除了需要学习 `SudokuGrid` 这个基本数据结构（位于 `Sudoku.Data` 命名空间）的使用外，还需要知道如何使用 `ManualSolver` 类型。

`ManualSolver` 类型是一个解题器类型，它里面自带一个 `Solve` 方法，提供用于解题。在对 `ManualSolver` 类型实例化（直接无参实例化）后，使用 `Solve` 方法即可完成解题功能。`ManualSolver` 位于 `Sudoku.Solving.Manual` 命名空间下。

如代码所示。

```csharp
using System;
using Sudoku.Data;
using Sudoku.Solving.Manual;

// Parse a puzzle from the string text.
var grid = SudokuGrid.Parse("........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........");

// Declare a manual solver that uses techniques used by humans to solve a puzzle.
var solver = new ManualSolver();

// To solve a puzzle synchonously.
var analysisResult = solver.Solve(grid);

// Output the analysis result.
Console.WriteLine(analysisResult.ToString());
```

和另一篇文章介绍的数据类型差不多，代码反射在计算后会返回 `FileCounterResult` 类型；这个也是类似的：在完成 `Solve` 方法的调用后，返回的结果类型是 `AnalysisResult` 类型的实例，这个实例包含众多可使用的属性，比如：

* `IsSolved` 属性：表示题目是否完成解题。因为早期 `ManualSolver` 类型无法使用暴力破解填入数字，所以早期可能会导致此属性结果为 `false`；现在使用 `ManualSolver` 完成解题已经可以支持暴力破解，因此该属性总是为 `true`（除非题目不是唯一解）；
* `ElapsedTime` 属性：表示题目在解题过程期间的用时，一般在正常计算过程后，都是正确的用时；除非题目不是唯一解的时候可能会使得该属性为默认数值，即为值 `default(TimeSpan)`、`new TimeSpan()` 或者 `TimeSpan.FromMilliseconds(0)`；
* `Additional` 属性：表示题目在解题失败（由于多解或无解等抛异常行为导致的失败）后，异常等错误信息存储的属性。换句话说，这个属性存储的其实是解题失败的时候的错误信息、为什么错误（相关错误数据）等等。一般这个属性的结果可以是 `Exception` 类型或 `string` 类型；
* `Solution` 属性：题目的解。当完成解题后，该属性一定包含正确的解的数值。如果题目无法正常完成，该属性为 `null`；
* `StepGrids` 属性：题目在人工解题过程之中，每一个步骤（出数或删数后）得到的结果盘面，在 UI 界面里会使用这个属性来显示呈现每个步骤的对应盘面。如果解题失败，该属性为 `null`；
* `Steps` 属性：题目在人工解题过程之中，得到的每一个步骤的具体信息。注意它存储的是步骤的对应技巧信息，而不是步骤的对应盘面。如果解题失败，该属性为 `null`；
* `MaxDifficulty` 属性：`Steps` 属性存储的所有步骤的难度系数的最大值。如果 `Steps` 为 `null`，该属性会使用默认结果 20 作为结果表示出来；
* `TotalDifficulty` 属性：`Steps` 属性的所有步骤的难度系数总和。如果 `Steps` 为 `null`，该属性返回 0；
* `PearlDifficulty` 属性：`Steps` 属性里第一个步骤的难度系数。如果 `Steps` 为 `null`，该属性返回 0；
* `DiamondDifficulty` 属性：`Steps` 属性里第一个出数技巧之前的所有技巧的最大难度系数。如果 `Steps` 为 `null` 或者里面一个元素都没有的话，该属性返回 20；
* `SolvingStepsCount` 属性：表示 `Steps` 有多少个元素，即整个解题过程后，用到多少步骤。`Steps` 为 `null` 则返回 0；
* `DifficultyLevel` 属性：表示本题目使用到的最难的数独技巧（`Steps` 里难度系数最大的第一个技巧）的难度级别。如果 `Steps` 为 `null`，则返回 `DifficultyLevel.Unknown`；
* `Bottleneck` 属性：返回本题目的瓶颈步骤（即第一个出数技巧步骤的信息）。

其它的 `AnalysisResult` 相关的成员请自行参考源代码。

