namespace Sudoku.Presentation;

partial struct LockedTarget
{
	/// <summary>
	/// Defines a JSON converter.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<LockedTarget>
	{
		/// <inheritdoc/>
		public override LockedTarget Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			if (!reader.Read()
				|| reader.TokenType != JsonTokenType.PropertyName
				|| reader.GetString() != nameof(Digit))
			{
				throw new JsonException();
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
			{
				throw new JsonException();
			}

			int digit = reader.GetInt32();
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			var cells = Cells.Empty;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				int cell = reader.GetInt32();
				cells.Add(cell);
			}

			if (reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return new(digit, cells);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, LockedTarget value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(Digit), value.Digit);
			writer.WriteStartArray();
			foreach (int cell in value.Cells)
			{
				writer.WriteNumberValue(cell);
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
