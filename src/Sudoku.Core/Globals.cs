global using System.Buffers;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Sudoku.CodeGenerating;
global using Sudoku.Data;
global using Sudoku.Data.Parsers;
global using Sudoku.Globalization;
global using Sudoku.Resources;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants;
global using static Sudoku.Constants.Tables;

[assembly: InternalsVisibleTo("Sudoku.Bot")]
[assembly: InternalsVisibleTo("Sudoku.CodeAnalysis")]
[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Drawing")]
[assembly: InternalsVisibleTo("Sudoku.Painting")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[assembly: InternalsVisibleTo("Sudoku.Windows")]
[assembly: InternalsVisibleTo("Sudoku.UI")]

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]