global using System.Collections;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Text.Json.Serialization;
global using Microsoft.CSharp.RuntimeBinder;
global using Sudoku.CodeGenerating;
global using Sudoku.Data;
global using Sudoku.Data.Collections;
global using Sudoku.Drawing;
global using Sudoku.Globalization;
global using Sudoku.Models;
global using Sudoku.Resources;
global using Sudoku.Solving.BruteForces;
global using Sudoku.Solving.Checking;
global using Sudoku.Solving.Manual;
global using Sudoku.Solving.Manual.Alses;
global using Sudoku.Solving.Manual.Buffers;
global using Sudoku.Solving.Manual.Chaining;
global using Sudoku.Solving.Manual.Exocets;
global using Sudoku.Solving.Manual.Fishes;
global using Sudoku.Solving.Manual.Intersections;
global using Sudoku.Solving.Manual.LastResorts;
global using Sudoku.Solving.Manual.RankTheory;
global using Sudoku.Solving.Manual.Sdps;
global using Sudoku.Solving.Manual.Singles;
global using Sudoku.Solving.Manual.Subsets;
global using Sudoku.Solving.Manual.Symmetry;
global using Sudoku.Solving.Manual.Uniqueness;
global using Sudoku.Solving.Manual.Uniqueness.Bugs;
global using Sudoku.Solving.Manual.Uniqueness.Extended;
global using Sudoku.Solving.Manual.Uniqueness.Loops;
global using Sudoku.Solving.Manual.Uniqueness.Polygons;
global using Sudoku.Solving.Manual.Uniqueness.Qiu;
global using Sudoku.Solving.Manual.Uniqueness.Rects;
global using Sudoku.Solving.Manual.Uniqueness.Reversal;
global using Sudoku.Solving.Manual.Uniqueness.Square;
global using Sudoku.Solving.Manual.Wings.Irregular;
global using Sudoku.Solving.Manual.Wings.Regular;
global using Sudoku.Solving.Text;
global using Sudoku.Techniques;
global using Sudoku.Versioning;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants;
global using static Sudoku.Constants.Tables;
global using static Sudoku.Solving.Manual.Constants;
global using static Sudoku.Solving.Manual.FastProperties;

[assembly: AssemblyObsolete]

#if SOLUTION_WIDE_CODE_ANALYSIS || CODE_ANALYSIS
[module: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
#endif

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]