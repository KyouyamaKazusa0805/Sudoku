using System;
using System.Runtime.CompilerServices;
using Sudoku.CodeGen;

[assembly: CLSCompliant(false)]

[assembly: InternalsVisibleTo("Sudoku.Core")]

[module: AutoDeconstructExtension(typeof(Index), nameof(Index.IsFromEnd), nameof(Index.Value))]