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
using Sudoku.Windows;
using static System.Console;

#if TEST_FOR_VALUE_GRID || true
var vGrid = ValueGrid.Empty;
var iGrid = Grid.Empty.Clone();
vGrid[0] = 4;
iGrid[0] = 4;
vGrid[1, 3] = true;
vGrid[3, 5] = true;
iGrid[1, 3] = true;
iGrid[3, 5] = true;
WriteLine($"{vGrid:.+:}");
WriteLine($"{iGrid:.+:}");
#endif

#if FILE_COUNTER || false
string root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif
