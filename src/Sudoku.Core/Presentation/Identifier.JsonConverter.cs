namespace Sudoku.Presentation;

partial struct Identifier
{
	/// <summary>
	/// Defines a JSON converter.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<Identifier>
	{
		/// <inheritdoc/>
		public override Identifier Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			if (!reader.Read()
				|| reader.TokenType != JsonTokenType.PropertyName
				|| reader.GetString() != nameof(UseId))
			{
				throw new JsonException();
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
			{
				throw new JsonException();
			}

			bool useId = reader.GetBoolean();
			if (useId)
			{
				if (!reader.Read()
					|| reader.TokenType != JsonTokenType.PropertyName
					|| reader.GetString() != nameof(Id))
				{
					throw new JsonException();
				}

				if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
				{
					throw new JsonException();
				}

				int id = reader.GetInt32();

				if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
				{
					throw new JsonException();
				}

				return new(id);
			}
			else
			{
				if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
				{
					throw new JsonException();
				}

				var color = JsonSerializer.Deserialize<ColorInternal>(ref reader);

				if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
				{
					throw new JsonException();
				}

				return new(color.A, color.R, color.G, color.B);
			}
		}
		
		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Identifier value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteBoolean(nameof(UseId), value.UseId);
			if (value.UseId)
			{
				writer.WriteNumber(nameof(Id), value.Id);
			}
			else
			{
				writer.WriteStartObject();
				writer.WriteNumber(nameof(A), value.A);
				writer.WriteNumber(nameof(R), value.R);
				writer.WriteNumber(nameof(G), value.G);
				writer.WriteNumber(nameof(B), value.B);
				writer.WriteEndObject();
			}
			writer.WriteEndObject();
		}
	}
}
