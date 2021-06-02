#if DEBUG && (NET5_0 || NETSTANDARD2_0)

using System.IO;
using Sudoku.Diagnostics.CodeAnalysis.IO;
using static System.Environment;

DiagnosticResultOutput.Output(
	"SupportedDiagnosticIds.txt",
	Path.Combine(GetFolderPath(SpecialFolder.Desktop), "Result.csv")
);

#endif