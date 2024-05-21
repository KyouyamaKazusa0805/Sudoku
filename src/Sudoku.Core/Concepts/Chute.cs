namespace Sudoku.Concepts;

/// <summary>
/// Defines a chute.
/// </summary>
/// <param name="Index">Index of the chute. The value is between 0 and 6.</param>
/// <param name="Cells">The cells used.</param>
/// <param name="IsRow">Indicates whether the chute is in a mega-row.</param>
/// <param name="HousesMask">Indicates the houses used.</param>
public readonly record struct Chute(int Index, ref readonly CellMap Cells, bool IsRow, HouseMask HousesMask) : ISudokuConcept<Chute>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
		=> ToString(GlobalizedConverter.GetConverter(formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => converter.ChuteConverter(this);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, System.IFormatProvider? formatProvider) => ToString(formatProvider);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string str, out Chute result) => TryParse(str, new RxCyParser(), out result);

	/// <inheritdoc/>
	public static bool TryParse<T>(string str, T parser, out Chute result) where T : CoordinateParser
	{
		try
		{
			result = parser.ChuteParser(str)[0];
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Chute Parse(string str) => Parse(str, new RxCyParser());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Chute Parse<T>(string str, T parser) where T : CoordinateParser
		=> parser.ChuteParser(str) is [var result]
			? result
			: throw new FormatException(ResourceDictionary.ExceptionMessage("MultipleChuteValuesFound"));
}
