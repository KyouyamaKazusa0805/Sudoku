namespace System;

/// <summary>
/// Defines a type that supports <c>ToString(<see langword="string"/>?)</c>.
/// </summary>
/// <seealso cref="ToString(string?)"/>
public interface ISimpleFormattable : IFormattable
{
	/// <summary>
	/// Formats the value of the current instance using the specified format.
	/// </summary>
	/// <param name="format">The format to use, or <see langword="null"/> to use the default format.</param>
	/// <returns>The value of the current instance in the specified format.</returns>
	/// <exception cref="FormatException">Throws when the format is invalid.</exception>
	public abstract string ToString(string? format);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(format);
}
