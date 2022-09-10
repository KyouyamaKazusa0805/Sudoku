namespace Sudoku.Text.Serialization.Specialized;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="CellMap"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// [
///   "r1c1",
///   "r1c2",
///   "r1c3"
/// ]
/// </code>
/// </remarks>
/// <seealso cref="CellMap"/>
public sealed class CellMapJsonConverter : JsonConverter<CellMap>
{
	/// <inheritdoc/>
	public override CellMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException();
		}

		var result = CellMap.Empty;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			result.Add(
				reader.TokenType == JsonTokenType.String && RxCyNotation.TryParseCell(reader.GetString()!, out var c)
					? c
					: throw new JsonException()
			);
		}

		return result;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, CellMap value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var cell in value)
		{
			writer.WriteStringValue(RxCyNotation.ToCellString(cell));
		}
		writer.WriteEndArray();
	}
}
