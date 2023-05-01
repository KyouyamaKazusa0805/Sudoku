namespace SudokuStudio.Interaction;

/// <summary>
/// Provides with extension methods on <see cref="SudokuFormatFlags"/>.
/// </summary>
/// <seealso cref="SudokuFormatFlags"/>
internal static class SudokuFormatFlagsExtensions
{
	/// <summary>
	/// Try to get target <see cref="IGridFormatter"/> instance.
	/// </summary>
	/// <param name="this">The flag instance.</param>
	/// <returns><see cref="IGridFormatter"/> instance.</returns>
	public static IGridFormatter GetFormatter(this SudokuFormatFlags @this)
		=> @this switch
		{
			SudokuFormatFlags.InitialFormat => SusserFormat.Default,
			SudokuFormatFlags.CurrentFormat => SusserFormat.Full,
			SudokuFormatFlags.CurrentFormatIgnoringValueKind => SusserFormatTreatingValuesAsGivens.Default,
			SudokuFormatFlags.HodokuCompatibleFormat => HodokuLibraryFormat.Default,
			SudokuFormatFlags.MultipleGridFormat => MultipleLineFormat.Default,
			SudokuFormatFlags.PencilMarkFormat => PencilMarkFormat.Default,
			SudokuFormatFlags.SukakuFormat => SukakuFormat.Default,
			SudokuFormatFlags.ExcelFormat => ExcelFormat.Default,
			SudokuFormatFlags.OpenSudokuFormat => OpenSudokuFormat.Default
		};
}
