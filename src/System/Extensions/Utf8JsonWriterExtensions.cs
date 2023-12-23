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
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	public static void WriteNestedObject<T>(this Utf8JsonWriter @this, T instance, JsonSerializerOptions? options = null)
		=> JsonSerializer.Serialize(@this, instance, options);

	/// <summary>
	/// To write an array of element type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The <see cref="Utf8JsonWriter"/> instance.</param>
	/// <param name="array">The array.</param>
	/// <param name="options">The options.</param>
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	public static void WriteArray<T>(this Utf8JsonWriter @this, T[] array, JsonSerializerOptions? options = null)
	{
		@this.WriteStartArray();

		foreach (var element in array)
		{
			switch (element)
			{
				case char c: { @this.WriteStringValue(c.ToString()); break; }
				case string s: { @this.WriteStringValue(s); break; }
				case var i and (sbyte or byte or short or ushort or int): { @this.WriteNumberValue((int)(object)i); break; }
				case uint u: { @this.WriteNumberValue(u); break; }
				case long l: { @this.WriteNumberValue(l); break; }
				case ulong u: { @this.WriteNumberValue(u); break; }
				case float f: { @this.WriteNumberValue(f); break; }
				case double d: { @this.WriteNumberValue(d); break; }
				case decimal d: { @this.WriteNumberValue(d); break; }
				default: { @this.WriteNestedObject(element, options); break; }
			}
		}

		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes a string text value specified as a <see cref="StringHandler"/> instance as an element
	/// of a JSON array.
	/// </summary>
	/// <param name="this">The <see cref="Utf8JsonWriter"/> instance.</param>
	/// <param name="handler">The string handler.</param>
	public static void WriteStringValue(this Utf8JsonWriter @this, [InterpolatedStringHandlerArgument] scoped ref StringHandler handler)
		=> @this.WriteStringValue(handler.ToStringAndClear());
}
