namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="Identifier"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// {
///   "useId": true,
///   "id": 3
/// }
/// </code>
/// </remarks>
/// <seealso cref="Identifier"/>
public sealed class IdentifierJsonConverter : JsonConverter<Identifier>
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
			|| reader.GetString() != nameof(Identifier.UseId))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
		{
			throw new JsonException();
		}

		return reader.GetBoolean() ? getId(ref reader) : getColor(ref reader);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int getId(ref Utf8JsonReader reader)
		{
			if (!reader.Read()
				|| reader.TokenType != JsonTokenType.PropertyName
				|| reader.GetString() != nameof(Identifier.Id))
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

			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static (byte, byte, byte, byte) getColor(ref Utf8JsonReader reader)
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

			return (color.A, color.R, color.G, color.B);
		}
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Identifier value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteBoolean(nameof(Identifier.UseId), value.UseId);
		if (value.UseId)
		{
			writer.WriteNumber(nameof(Identifier.Id), value.Id);
		}
		else
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(Identifier.A), value.A);
			writer.WriteNumber(nameof(Identifier.R), value.R);
			writer.WriteNumber(nameof(Identifier.G), value.G);
			writer.WriteNumber(nameof(Identifier.B), value.B);
			writer.WriteEndObject();
		}
		writer.WriteEndObject();
	}
}
