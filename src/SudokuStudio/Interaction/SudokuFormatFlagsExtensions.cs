using Sudoku.Text.SudokuGrid;

namespace SudokuStudio.Interaction;

/// <summary>
/// Provides with extension methods on <see cref="SudokuFormatFlags"/>.
/// </summary>
/// <seealso cref="SudokuFormatFlags"/>
internal static class SudokuFormatFlagsExtensions
{
	/// <summary>
	/// Try to get target <see cref="GridConverter"/> instance.
	/// </summary>
	/// <param name="this">The flag instance.</param>
	/// <returns><see cref="GridConverter"/> instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	public static GridConverter GetConverter(this SudokuFormatFlags @this)
		=> @this switch
		{
			SudokuFormatFlags.InitialFormat => SusserConverter.Default,
			SudokuFormatFlags.CurrentFormat => SusserConverter.Full,
			SudokuFormatFlags.CurrentFormatIgnoringValueKind => SusserConverterTreatingValuesAsGivens.Default,
			SudokuFormatFlags.HodokuCompatibleFormat => HodokuLibraryConverter.Default,
			SudokuFormatFlags.MultipleGridFormat => MultipleLineConverter.Default,
			SudokuFormatFlags.PencilMarkFormat => new PencilmarkingConverter(),
			SudokuFormatFlags.SukakuFormat => SukakuConverter.Default,
			SudokuFormatFlags.ExcelFormat => new ExcelConverter(),
			SudokuFormatFlags.OpenSudokuFormat => new OpenSudokuConverter(),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
