namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a base type for formatting information on a type.
/// </summary>
/// <typeparam name="T">The type of an object to be formatted.</typeparam>
public abstract class FormatInfo<T> : IFormatProvider where T : allows ref struct
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public abstract object? GetFormat(Type? formatType);

	/// <summary>
	/// Creates a copy of the current instance.
	/// </summary>
	/// <returns>A new instance whose internal values are equal to the current instance.</returns>
	public abstract FormatInfo<T> Clone();

	/// <summary>
	/// Try to format the current object into a valid string result.
	/// </summary>
	/// <param name="obj">An object to be formatted.</param>
	/// <returns>The <see cref="string"/> representation of the argument <paramref name="obj"/>.</returns>
	protected internal abstract string FormatCore(ref readonly T obj);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> instance into a valid instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="str">The string value to be parsed.</param>
	/// <returns>An instance of type <typeparamref name="T"/>.</returns>
	protected internal abstract T ParseCore(string str);
}
