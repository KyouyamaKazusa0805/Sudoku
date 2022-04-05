global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.CommonComparers.Equality;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Intrinsics;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Sudoku.Concepts;
global using Sudoku.Concepts.Collections;
global using Sudoku.Concepts.Collections.Handlers;
global using Sudoku.Concepts.Formatting;
global using Sudoku.Concepts.Notations;
global using Sudoku.Concepts.Parsing;
global using Sudoku.Concepts.Solving;
global using Sudoku.Concepts.Solving.ChainNodes;
global using Sudoku.Presentation.Nodes;
global using Sudoku.Runtime.AnalysisServices;
global using Sudoku.Runtime.Reflection;
global using Sudoku.Solving;
global using static System.Algorithm.Sequences;
global using static System.Algorithm.Sorting;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Runtime.AnalysisServices.CommonReadOnlies;

[assembly: InternalsVisibleTo("Sudoku.Solving")]
[module: SkipLocalsInit]