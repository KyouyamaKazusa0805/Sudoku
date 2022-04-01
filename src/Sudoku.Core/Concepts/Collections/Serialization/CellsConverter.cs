namespace Sudoku.Concepts.Collections.Serialization;

/// <summary>
/// Defines a serialization converter to convert the <see cref="Cells"/>
/// to a JSON <see cref="string"/> or vice versa.
/// </summary>
[JsonConverter(typeof(Cells))]
public sealed class CellsConverter : JsonConverter<Cells>
{
	/// <inheritdoc/>
	public override Cells Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!CanConvert(typeToConvert))
		{
			return Cells.Empty;
		}

		var result = Cells.Empty;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.String)
			{
				result.Add(reader.GetString()!);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Cells value, JsonSerializerOptions options)
	{
		writeCollection(writer, value);


		static void writeCollection(Utf8JsonWriter @this, in Cells cells)
		{
			@this.WriteStartArray();
			foreach (byte cell in cells)
			{
				@this.WriteStringValue((Cells.Empty + cell).ToString());
			}
			@this.WriteEndArray();
		}
	}
}
