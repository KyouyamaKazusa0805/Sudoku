global using System;
global using System.Buffers;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Sudoku.Collections;
global using Sudoku.Data;
global using Sudoku.Data.Abstractions;
global using Sudoku.Diagnostics.CodeAnalysis;
global using Sudoku.Presentation;
global using Sudoku.Resources;
global using Sudoku.Solving;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants;
global using static Sudoku.Constants.Tables;

[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[module: SkipLocalsInit] 