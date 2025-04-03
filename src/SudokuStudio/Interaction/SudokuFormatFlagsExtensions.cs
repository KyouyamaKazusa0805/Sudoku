namespace SudokuStudio.Interaction;

/// <summary>
/// Provides with extension methods on <see cref="SudokuFormatFlags"/>.
/// </summary>
/// <seealso cref="SudokuFormatFlags"/>
internal static class SudokuFormatFlagsExtensions
{
	/// <summary>
	/// Try to get target <see cref="GridFormatInfo{TGrid}"/> instance of type <see cref="Grid"/>.
	/// </summary>
	/// <param name="this">The flag instance.</param>
	/// <returns>The final <see cref="GridFormatInfo{TGrid}"/> instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	public static GridFormatInfo<Grid> GetConverter(this SudokuFormatFlags @this)
		=> @this switch
		{
			SudokuFormatFlags.InitialFormat => new SusserGridFormatInfo<Grid>(),
			SudokuFormatFlags.CurrentFormat => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true },
			SudokuFormatFlags.CurrentFormatIgnoringValueKind
				=> new SusserGridFormatInfo<Grid> { WithModifiables = true, WithCandidates = true, TreatValueAsGiven = true },
#if false
			// Deprecated. This will be handled as special one.
			SudokuFormatFlags.HodokuCompatibleFormat
				=> new SusserGridFormatInfo<Grid> { WithModifiables = true, WithCandidates = true, IsCompatibleMode = true },
#endif
			SudokuFormatFlags.MultipleGridFormat => new MultipleLineGridFormatInfo { RemoveGridLines = true },
			SudokuFormatFlags.PencilMarkFormat => new PencilmarkGridFormatInfo { SubtleGridLines = true },
			SudokuFormatFlags.SukakuFormat => new SukakuGridFormatInfo(),
			SudokuFormatFlags.ExcelFormat => new CsvGridFormatInfo(),
			SudokuFormatFlags.OpenSudokuFormat => new OpenSudokuGridFormatInfo(),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
