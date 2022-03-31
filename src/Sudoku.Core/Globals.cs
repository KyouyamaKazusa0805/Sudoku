global using System.CommonComparers.Equality;
global using System.ComponentModel;
global using System.Runtime.InteropServices;
global using System.Runtime.Intrinsics;
global using Sudoku.Collections;
global using Sudoku.Concepts;
global using Sudoku.Concepts.Collections;
global using Sudoku.Concepts.Formatting;
global using Sudoku.Concepts.Parsing;
global using Sudoku.DataHandling;
global using Sudoku.Solving;
global using static System.Algorithm.Sequences;
global using static System.Algorithm.Sorting;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants;
global using static Sudoku.Constants.Tables;

[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[module: SkipLocalsInit]