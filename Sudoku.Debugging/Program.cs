#pragma warning disable IDE0079 // Unused suppressions.
#pragma warning disable IDE0005
#pragma warning disable IDE1006

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Diagnostics;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Windows;
using static System.Console;

#if SUDOKU_GRID_TESTER || true
var grid = SudokuGrid.Empty;
WriteLine(grid.ToMaskString());
#endif

#if FILE_COUNTER || false
string root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif
