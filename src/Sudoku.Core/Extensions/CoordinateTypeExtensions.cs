namespace Sudoku.Concepts;

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
	/// <param name="arguments">The arguments to be initialized when calling the constructor.</param>
	/// <returns>
	/// A valid <see cref="CoordinateConverter"/> instance. You can use cast operators to get the instance of desired type.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter? GetConverter(this CoordinateType @this, object?[]? arguments = null)
		=> @this.GetField()?.GetGenericAttributeTypeArguments(typeof(CoordinateConverterAttribute<>)) switch
		{
			[var type] => Activator.CreateInstance(type, arguments),
			_ => null
		} as CoordinateConverter;

	/// <summary>
	/// Gets the <see cref="CoordinateParser"/> instance via the specified <see cref="CoordinateType"/> instance.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="arguments">The arguments to be initialized when calling the constructor.</param>
	/// <returns>
	/// A valid <see cref="CoordinateParser"/> instance. You can use cast operators to get the instance of desired type.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateParser? GetParser(this CoordinateType @this, object?[]? arguments = null)
		=> @this.GetField()?.GetGenericAttributeTypeArguments(typeof(CoordinateParserAttribute<>)) switch
		{
			[var type] => Activator.CreateInstance(type, arguments),
			_ => null
		} as CoordinateParser;

	/// <summary>
	/// Gets the <see cref="FieldInfo"/> instance for the specified field.
	/// </summary>
	/// <param name="this">The type of the coordinate type.</param>
	/// <returns>The final <see cref="FieldInfo"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static FieldInfo? GetField(this CoordinateType @this) => typeof(CoordinateType).GetField(@this.ToString());
}
