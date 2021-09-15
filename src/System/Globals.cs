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
global using System.Runtime.Versioning;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using Microsoft.VisualBasic.FileIO;
global using Sudoku.CodeGenerating;
global using static System.Math;
global using static System.Numerics.BitOperations;

[assembly: InternalsVisibleTo("Sudoku.Core")]

#if WIN_UI_PROJECT
[assembly: InternalsVisibleTo("Sudoku.UI")]
#endif

[assembly: AutoDeconstructExtension(typeof(Index), nameof(Index.IsFromEnd), nameof(Index.Value))]