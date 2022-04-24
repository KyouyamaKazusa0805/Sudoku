namespace Sudoku.Runtime.Serialization;

/// <summary>
/// Defines a JSON converter.
/// </summary>
public sealed class InferenceJsonConverter : JsonConverter<Inference>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Inference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException();
		}

		return Enum.Parse<Inference>(reader.GetString()!);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Inference value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
