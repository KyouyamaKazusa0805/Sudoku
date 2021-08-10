using System.Runtime.CompilerServices;

#if SOLUTION_WIDE_CODE_ANALYSIS || CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

#if SOLUTION_WIDE_CODE_ANALYSIS || CODE_ANALYSIS
[module: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
#endif

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]