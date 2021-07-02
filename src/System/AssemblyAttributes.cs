using System;
using System.Runtime.CompilerServices;
using Sudoku.CodeGenerating;

[assembly: CLSCompliant(false)]

[assembly: InternalsVisibleTo("Sudoku.Core")]

[assembly: AutoDeconstructExtension(typeof(Index), nameof(Index.IsFromEnd), nameof(Index.Value))]