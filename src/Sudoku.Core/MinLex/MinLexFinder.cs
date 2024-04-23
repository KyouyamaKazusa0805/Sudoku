namespace Sudoku.MinLex;

/// <summary>
/// Represents a finder type.
/// </summary>
public static class MinLexFinder
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="result"></param>
	[DllImport("Sudoku.MinLex.dll", EntryPoint = "find_minlex", CallingConvention = CallingConvention.Cdecl)]
	[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
	[SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "<Pending>")]
	public static unsafe extern void Find(char* source, char* result);
}
