global using System.Buffers;
global using System.Collections;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Microsoft.VisualBasic.FileIO;
global using Sudoku.CodeGenerating;
global using static System.Math;
global using static System.Numerics.BitOperations;

[assembly: InternalsVisibleTo("Sudoku.Core")]

[assembly: AutoDeconstructExtension(typeof(Index), nameof(Index.IsFromEnd), nameof(Index.Value))]