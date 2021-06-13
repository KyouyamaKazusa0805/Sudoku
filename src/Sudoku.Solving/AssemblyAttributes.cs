using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(false)]

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]