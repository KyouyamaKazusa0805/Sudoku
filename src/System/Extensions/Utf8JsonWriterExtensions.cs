namespace System.Text.Json;

/// <summary>
/// Provides extension methods on <see cref="Utf8JsonWriter"/>.
/// </summary>
/// <seealso cref="Utf8JsonWriter"/>
public static class Utf8JsonWriterExtensions
{
	/// <summary>
	/// To write an object as nested one in the JSON string stream.
	/// </summary>
	/// <typeparam name="T">The type of the instance to be serialized.</typeparam>
	/// <param name="this">The <see cref="Utf8JsonWriter"/> instance.</param>
	/// <param name="instance">The instance to be serialized.</param>
	/// <param name="options">The options.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteNestedObject<T>(this Utf8JsonWriter @this, T instance, JsonSerializerOptions? options = null)
		=> JsonSerializer.Serialize(@this, instance, options);

	/// <summary>
	/// Writes a string text value specified as a <see cref="StringHandler"/> instance as an element
	/// of a JSON array.
	/// </summary>
	/// <param name="this">The <see cref="Utf8JsonWriter"/> instance.</param>
	/// <param name="handler">The string handler.</param>
	public static void WriteStringValue(this Utf8JsonWriter @this, [InterpolatedStringHandlerArgument] ref StringHandler handler)
		=> @this.WriteStringValue(handler.ToStringAndClear());
}
