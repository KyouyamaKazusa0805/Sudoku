using System;
using System.Runtime.CompilerServices;
using Sudoku.CodeGenerating;

[assembly: InternalsVisibleTo("Sudoku.Core")]

[assembly: AutoDeconstructExtension(typeof(Index), nameof(Index.IsFromEnd), nameof(Index.Value))]