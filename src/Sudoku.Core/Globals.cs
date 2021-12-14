global using System;
global using System.Buffers;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Sudoku.Data;
global using Sudoku.Data.Parsers;
global using Sudoku.Diagnostics.CodeGen;
global using Sudoku.Globalization;
global using Sudoku.Resources;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants;
global using static Sudoku.Constants.Tables;

#if !WINDOWS_APP
global using System.Reflection;
#endif

[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Drawing.Old")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[assembly: InternalsVisibleTo("Sudoku.Windows.Old")]
[assembly: InternalsVisibleTo("Sudoku.UI")]

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]
