namespace SudokuStudio.Interaction;

/// <summary>
/// Provides with extension methods on <see cref="SudokuFormatFlags"/>.
/// </summary>
/// <seealso cref="SudokuFormatFlags"/>
internal static class SudokuFormatFlagsExtensions
{
	/// <summary>
	/// Try to get target <see cref="GridFormatInfo"/> instance of type <see cref="Grid"/>.
	/// </summary>
	/// <param name="this">The flag instance.</param>
	/// <returns>The final <see cref="GridFormatInfo"/> instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	public static GridFormatInfo GetConverter(this SudokuFormatFlags @this)
		=> @this switch
		{
			SudokuFormatFlags.InitialFormat => new SusserGridFormatInfo(),
			SudokuFormatFlags.CurrentFormat => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true },
			SudokuFormatFlags.CurrentFormatIgnoringValueKind
				=> new SusserGridFormatInfo { WithModifiables = true, WithCandidates = true, TreatValueAsGiven = true },
#if false
			// Deprecated. This will be handled as special one.
			SudokuFormatFlags.HodokuCompatibleFormat
				=> new SusserGridFormatInfo { WithModifiables = true, WithCandidates = true, IsCompatibleMode = true },
#endif
			SudokuFormatFlags.MultipleGridFormat => new MultipleLineGridFormatInfo { RemoveGridLines = true },
			SudokuFormatFlags.PencilMarkFormat => new PencilmarkGridFormatInfo { SubtleGridLines = true },
			SudokuFormatFlags.SukakuFormat => new SukakuGridFormatInfo(),
			SudokuFormatFlags.ExcelFormat => new CsvGridFormatInfo(),
			SudokuFormatFlags.OpenSudokuFormat => new OpenSudokuGridFormatInfo(),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
