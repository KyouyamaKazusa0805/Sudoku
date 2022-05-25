namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="Inference"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// "Strong"
/// </code>
/// </remarks>
/// <seealso cref="Inference"/>
public sealed class InferenceJsonConverter : JsonConverter<Inference>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Inference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.Read() && reader.TokenType == JsonTokenType.String && reader.GetString() is { } value
			? Enum.Parse<Inference>(value)
			: throw new JsonException();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Inference value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
