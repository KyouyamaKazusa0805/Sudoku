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
var grid = SudokuGrid.Parse("04000900280+91+250600060800+9+5000000+240+6902005073007506+1+90500309+70000+5000200080020+51:711 113 713 317 132 337 437 141 742 143 645 146 156 456 171 473 476 181 782 183 483 185 694");
WriteLine(grid.CandidatesCount);
WriteLine(grid.GivensCount);
WriteLine(grid.ModifiablesCount);
WriteLine(grid.EmptiesCount);
#endif

#if FILE_COUNTER || false
string root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif
