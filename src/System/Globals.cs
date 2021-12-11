global using System;
global using System.Buffers;
global using System.Collections;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using Microsoft.CodeAnalysis;
global using Microsoft.VisualBasic.FileIO;
global using Sudoku.Diagnostics.CodeGen;
global using static System.Math;
global using static System.Numerics.BitOperations;

[assembly: InternalsVisibleTo("Sudoku.Core")]

#if WINDOWS_APP
[assembly: InternalsVisibleTo("Sudoku.UI")]
#endif

[assembly: AutoDeconstructExtension<Index>(nameof(Index.IsFromEnd), nameof(Index.Value))] 