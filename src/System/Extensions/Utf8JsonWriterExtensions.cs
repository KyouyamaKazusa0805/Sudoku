namespace System.Text.Json;

/// <summary>
/// Provides extension methods on <see cref="Utf8JsonWriter"/>.
/// </summary>
/// <seealso cref="Utf8JsonWriter"/>
public static class Utf8JsonWriterExtensions
{
	/// <summary>
	/// Writes a string text value specified as a <see cref="StringHandler"/> instance as an element
	/// of a JSON array.
	/// </summary>
	/// <param name="this">The <see cref="Utf8JsonWriter"/> instance.</param>
	/// <param name="handler">The string handler.</param>
	public static void WriteStringValue(
		this Utf8JsonWriter @this, [InterpolatedStringHandlerArgument] ref StringHandler handler) =>
		@this.WriteStringValue(handler.ToStringAndClear());
}
