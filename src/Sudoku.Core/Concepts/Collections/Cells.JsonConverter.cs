namespace Sudoku.Concepts.Collections;

partial struct Cells
{
	/// <summary>
	/// Defines a JSON converter.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<Cells>
	{
		/// <inheritdoc/>
		public override Cells Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			if (!reader.Read()
				|| reader.TokenType != JsonTokenType.PropertyName
				|| reader.GetString() != nameof(Cells))
			{
				throw new JsonException();
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			var result = Empty;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.Number:
					{
						result.Add(reader.GetInt32());
						break;
					}
					default:
					{
						throw new JsonException();
					}
				}
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return result;
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Cells value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(nameof(Cells));
			writer.WriteStartArray();
			foreach (int cell in value)
			{
				writer.WriteNumberValue(cell);
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
