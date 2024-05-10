namespace System.Text.Json;

/// <summary>
/// Represents a JSON convetrer that serializes and deserializes a <see cref="CultureInfo"/> object.
/// </summary>
/// <seealso cref="CultureInfo"/>
public sealed class CultureInfoConverter : JsonConverter<CultureInfo>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.Number => new(reader.GetInt32()),
			JsonTokenType.Null => null,
			JsonTokenType.String when reader.GetString() is { } result => new(result),
			_ => throw new JsonException()
		};

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.Name);
}
