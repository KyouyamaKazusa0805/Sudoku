namespace Sudoku.Concepts;

/// <summary>
/// Defines a chute.
/// </summary>
/// <param name="Index">Index of the chute. The value is between 0 and 6.</param>
/// <param name="Cells">The cells used.</param>
/// <param name="IsRow">Indicates whether the chute is in a mega-row.</param>
/// <param name="HousesMask">Indicates the houses used.</param>
public readonly record struct Chute(int Index, ref readonly CellMap Cells, bool IsRow, HouseMask HousesMask) :
	IFormattable,
	IParsable<Chute>
{
	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
		=> (
			formatProvider switch
			{
				CultureInfo c => CoordinateConverter.GetConverter(c),
				CoordinateConverter c => c,
				_ => CoordinateConverter.InvariantCultureConverter
			}
		).ChuteConverter(this);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string str, out Chute result) => TryParse(str, null, out result);

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Chute result)
	{
		try
		{
			if (s is null)
			{
				throw new FormatException();
			}

			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Chute Parse(string s) => Parse(s, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Chute Parse(string s, IFormatProvider? provider)
	{
		var parser = provider switch
		{
			CultureInfo c => CoordinateParser.GetParser(c),
			CoordinateParser c => c,
			_ => CoordinateParser.InvariantCultureParser
		};
		return parser.ChuteParser(s) is [var result]
			? result
			: throw new FormatException(ResourceDictionary.ExceptionMessage("MultipleChuteValuesFound"));
	}
}
