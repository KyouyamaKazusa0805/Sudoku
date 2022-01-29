global using System.Buffers;
global using System.ComponentModel;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using Sudoku.Collections;
global using Sudoku.Data;
global using Sudoku.Diagnostics.CodeAnalysis;
global using Sudoku.Presentation;
global using Sudoku.Solving;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants;
global using static Sudoku.Constants.Tables;

[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[module: SkipLocalsInit] 