global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text;
global using Microsoft.CSharp.RuntimeBinder;
global using Sudoku.CodeGenerating;
global using Sudoku.Data;
global using Sudoku.Data.Collections;
global using Sudoku.Drawing;
global using Sudoku.Globalization;
global using Sudoku.Models;
global using Sudoku.Resources;
global using Sudoku.Solving.Text;
global using Sudoku.Techniques;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants.Tables;
global using static Sudoku.Solving.Manual.Constants;
global using static Sudoku.Solving.Manual.FastProperties;

#if SOLUTION_WIDE_CODE_ANALYSIS || CODE_ANALYSIS
[module: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
#endif

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]