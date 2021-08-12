global using System.Runtime.CompilerServices;
global using Sudoku.CodeGenerating;

[assembly: InternalsVisibleTo("Sudoku.Core")]

[assembly: AutoDeconstructExtension(typeof(Index), nameof(Index.IsFromEnd), nameof(Index.Value))]