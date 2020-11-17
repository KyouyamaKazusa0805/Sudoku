// The attribute 'InternalVisibleToAttribute' is used for enhancing the access ability
// of the keyword 'internal'.
// If using this attribute, 'internal' members in the current assembly can be seen in other assemblies
// specified as parameters in the attribute statements.
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sudoku.Bot")]
[assembly: InternalsVisibleTo("Sudoku.CodeAnalysis")]
[assembly: InternalsVisibleTo("Sudoku.Debugging")]
[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Drawing")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[assembly: InternalsVisibleTo("Sudoku.Windows")]