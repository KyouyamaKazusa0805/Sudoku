namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="Cells"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// [
///   10,
///   20,
///   30
/// ]
/// </code>
/// </remarks>
/// <seealso cref="Cells"/>
public sealed class CellsJsonConverter : JsonConverter<Cells>
{
	/// <inheritdoc/>
	public override Cells Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException();
		}

		var result = Cells.Empty;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			result.Add(reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : throw new JsonException());
		}

		return result;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Cells value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (int cell in value)
		{
			writer.WriteNumberValue(cell);
		}
		writer.WriteEndArray();
	}
}
