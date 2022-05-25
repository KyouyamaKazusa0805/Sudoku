namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization
/// on type <see cref="SoleCandidateNode"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// {
///   "cell": 10,
///   "digit": 2
/// }
/// </code>
/// </remarks>
/// <seealso cref="SoleCandidateNode"/>
public sealed class SoleCandidateNodeJsonConverter : JsonConverter<SoleCandidateNode>
{
	/// <inheritdoc/>
	public override SoleCandidateNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(SoleCandidateNode.Cell))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(SoleCandidateNode.Digit))
		{
			throw new JsonException();
		}

		if (!byte.TryParse(reader.GetString()!, out byte cell))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
		{
			throw new JsonException();
		}

		byte digit = reader.GetByte();
		if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
		{
			throw new JsonException();
		}

		return new(cell, (byte)(digit - 1));
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, SoleCandidateNode value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString(nameof(SoleCandidateNode.Cell), RxCyNotation.ToCellString(value.Cell));
		writer.WriteNumber(nameof(SoleCandidateNode.Digit), value.Digit + 1);
		writer.WriteEndObject();
	}
}
