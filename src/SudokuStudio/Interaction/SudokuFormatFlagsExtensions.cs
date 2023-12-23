namespace SudokuStudio.Interaction;

/// <summary>
/// Provides with extension methods on <see cref="SudokuFormatFlags"/>.
/// </summary>
/// <seealso cref="SudokuFormatFlags"/>
internal static class SudokuFormatFlagsExtensions
{
	/// <summary>
	/// Try to get target <see cref="IConceptConverter{T}"/> instance of type <see cref="Grid"/>.
	/// </summary>
	/// <param name="this">The flag instance.</param>
	/// <returns>The final <see cref="IConceptConverter{T}"/> instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	public static IConceptConverter<Grid> GetConverter(this SudokuFormatFlags @this)
		=> @this switch
		{
			SudokuFormatFlags.InitialFormat => SusserGridConverter.Default,
			SudokuFormatFlags.CurrentFormat => SusserGridConverter.Full,
			SudokuFormatFlags.CurrentFormatIgnoringValueKind => SusserGridConverterTreatingValuesAsGivens.Default,
			SudokuFormatFlags.HodokuCompatibleFormat => HodokuLibraryGridConverter.Default,
			SudokuFormatFlags.MultipleGridFormat => MultipleLineGridConverter.Default,
			SudokuFormatFlags.PencilMarkFormat => new PencilmarkingGridConverter(),
			SudokuFormatFlags.SukakuFormat => SukakuGridConverter.Default,
			SudokuFormatFlags.ExcelFormat => new ExcelGridConverter(),
			SudokuFormatFlags.OpenSudokuFormat => new OpenSudokuGridConverter(),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
