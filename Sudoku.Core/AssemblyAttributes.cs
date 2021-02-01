#if !WIN_UI && !WPF_OR_CONSOLE
// The project should be used in WinUI, WPF project or a console program.
// If both 'WIN_UI' and 'WPF_OR_CONSOLE' is disabled, the project will disallow you compile it.
#error The solution must contain the compilation either symbol "WIN_UI" or "WPF_OR_CONSOLE".
#elif WIN_UI && WPF_OR_CONSOLE
// You can't define both 'WIN_UI' and 'WPF_OR_CONSOLE' two symbols in a same project.
#error Those two symbols are exclusive. You shouldn't define both two symbols.
#endif

using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("Sudoku.Bot")]
[assembly: InternalsVisibleTo("Sudoku.CodeAnalysis")]
[assembly: InternalsVisibleTo("Sudoku.Debugging")]
[assembly: InternalsVisibleTo("Sudoku.Diagnostics")]
[assembly: InternalsVisibleTo("Sudoku.Drawing")]
[assembly: InternalsVisibleTo("Sudoku.Painting")]
[assembly: InternalsVisibleTo("Sudoku.Solving")]
[assembly: InternalsVisibleTo("Sudoku.Windows")]
[assembly: InternalsVisibleTo("Sudoku.UI")]

[module: SkipLocalsInit]