namespace Sudoku.Runtime.CoordinateServices;

/// <summary>
/// Provides with extension methods on <see cref="CoordinateType"/>.
/// </summary>
/// <seealso cref="CoordinateType"/>
public static class CoordinateTypeExtensions
{
	/// <summary>
	/// Gets the <see cref="CoordinateConverter"/> instance via the specified <see cref="CoordinateType"/> instance.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <returns>
	/// A valid <see cref="CoordinateConverter"/> instance. You can use cast operators to get the instance of desired type.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter? GetConverter(this CoordinateType @this)
		=> @this switch
		{
			CoordinateType.Literal => new LiteralCoordinateConverter(),
			CoordinateType.RxCy => new RxCyConverter(),
			CoordinateType.K9 => new K9Converter(),
			CoordinateType.Excel => new ExcelCoordinateConverter(),
			_ => null
		};

	/// <summary>
	/// Gets the <see cref="CoordinateParser"/> instance via the specified <see cref="CoordinateType"/> instance.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <returns>
	/// A valid <see cref="CoordinateParser"/> instance. You can use cast operators to get the instance of desired type.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateParser? GetParser(this CoordinateType @this)
		=> @this switch { CoordinateType.RxCy => new RxCyParser(), CoordinateType.K9 => new K9Parser(), _ => null };

	/// <summary>
	/// Gets the <see cref="FieldInfo"/> instance for the specified field.
	/// </summary>
	/// <param name="this">The type of the coordinate type.</param>
	/// <returns>The final <see cref="FieldInfo"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static FieldInfo? GetField(this CoordinateType @this) => typeof(CoordinateType).GetField(@this.ToString());
}
