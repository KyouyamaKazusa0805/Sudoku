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
	public override CultureInfo? Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() switch { { } s => new(s), _ => null };

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.Name);
}
