namespace Sudoku.Runtime.FormattingServices;

/// <summary>
/// Represents extra options that formats a <see cref="CellMap"/> instance, or parses into a <see cref="CellMap"/> instance.
/// </summary>
/// <seealso cref="CellMap"/>
public abstract class CellMapFormatInfo : IFormatProvider
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public abstract object? GetFormat(Type? formatType);

	/// <summary>
	/// Creates a copy of the current instance.
	/// </summary>
	/// <returns>A new instance whose internal values are equal to the current instance.</returns>
	public abstract CellMapFormatInfo Clone();

	/// <summary>
	/// Try to format the current map into a valid string result.
	/// </summary>
	/// <param name="map">The map to be formatted.</param>
	/// <returns>The <see cref="string"/> representation of the argument <paramref name="map"/>.</returns>
	protected internal abstract string FormatMap(ref readonly CellMap map);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> instance into a valid <see cref="CellMap"/>.
	/// </summary>
	/// <param name="str">The string value to be parsed.</param>
	/// <returns>The <see cref="CellMap"/> as the result.</returns>
	protected internal abstract CellMap ParseMap(string str);
}