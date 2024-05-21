namespace Sudoku.Globalization;

/// <summary>
/// Represents a type that defines some easy-in-use members, making user work with types derived from <see cref="CoordinateConverter"/> easier
/// for variant culture.
/// </summary>
/// <seealso cref="CoordinateConverter"/>
/// <seealso cref="CultureInfo"/>
public static class GlobalizedConverter
{
	/// <summary>
	/// Indicates the <see cref="CoordinateConverter"/> instance for the invariant culture.
	/// </summary>
	public static CoordinateConverter InvariantCultureConverter => new RxCyConverter();


	/// <summary>
	/// Try to get a <see cref="CoordinateConverter"/> instance from the specified culture.
	/// </summary>
	/// <param name="culture">The culture.</param>
	/// <returns>The <see cref="CoordinateConverter"/> instance from the specified culture.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter GetConverter(CultureInfo culture)
		=> culture switch
		{
			{ LCID: 2052 } or { DisplayName: ['Z' or 'z', 'H' or 'h', ..] } => new K9Converter(true, CurrentCulture: culture),
			_ => new RxCyConverter(true, true, CurrentCulture: culture)
		};
}
