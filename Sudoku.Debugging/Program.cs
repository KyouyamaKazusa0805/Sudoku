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
var grid = SudokuGrid.Parse(":0000:x:10+340+9080040+582300000+30100003000+450820+58+30140+8+149+5020+3+30000+8000006093000070605030:612 619 719 929 631 733 933 637 737 638 744 645 745 748 973 775 589 993 999::");
WriteLine(grid.CandidatesCount);
WriteLine(grid.GivensCount);
WriteLine(grid.ModifiablesCount);
WriteLine(grid.EmptiesCount);
#endif

#if FILE_COUNTER || false
string root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif
