global using System.Buffers;
global using System.ComponentModel;
global using System.Globalization;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using System.Runtime.Intrinsics;
global using System.Runtime.Serialization;
global using Sudoku.Collections;
global using Sudoku.Presentation;
global using Sudoku.Resources;
global using Sudoku.Solving.BruteForces;
global using Sudoku.Solving.Collections;
global using Sudoku.Solving.Manual;
global using Sudoku.Solving.Manual.Buffer;
global using Sudoku.Solving.Manual.Checkers;
global using Sudoku.Solving.Manual.Searchers;
global using Sudoku.Solving.Manual.Steps;
global using Sudoku.Solving.Manual.Text;
global using Sudoku.Techniques;
global using static System.Algorithm.Combinatorics;
global using static System.Algorithm.Sequences;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants.Tables;
global using static Sudoku.Solving.Manual.Buffer.FastProperties;
global using static Sudoku.Solving.Manual.Constants;

[module: SkipLocalsInit]