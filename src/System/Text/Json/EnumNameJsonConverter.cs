namespace System.Text.Json;

/// <summary>
/// Represents a type that can serialize a field from enumeration type <typeparamref name="T"/> into a JSON string,
/// whose value is equivalent to the field's name.
/// </summary>
/// <typeparam name="T">The type of the enumeration.</typeparam>
public sealed class EnumNameJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
	/// <inheritdoc/>
	public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() is { } value ? Enum.Parse<T>(value) : throw new JsonException();

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
