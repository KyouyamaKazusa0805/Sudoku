namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="Identifier"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// {
///   "mode": "Raw",
///   "value": {
///     "a": 255,
///     "r": 0,
///     "g": 0,
///     "b": 0
///   }
/// }
/// </code>
/// </remarks>
/// <seealso cref="Identifier"/>
public sealed class IdentifierJsonConverter : JsonConverter<Identifier>
{
	/// <inheritdoc/>
	public override Identifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName
			|| reader.GetString() != nameof(Identifier.Mode))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "Value")
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException();
		}

		return Enum.Parse<IdentifierColorMode>(reader.GetString() ?? throw new JsonException()) switch
		{
			IdentifierColorMode.Id => getId(ref reader),
			IdentifierColorMode.Raw => getColor(ref reader),
			IdentifierColorMode.Named => getNamedKind(ref reader),
			_ => throw new JsonException()
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int getId(ref Utf8JsonReader reader)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var raw = JsonSerializer.Deserialize<IdInternal>(ref reader);
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return raw.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static (byte, byte, byte, byte) getColor(ref Utf8JsonReader reader)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var raw = JsonSerializer.Deserialize<ColorInternal>(ref reader);
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return (raw.A, raw.R, raw.G, raw.B);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static DisplayColorKind getNamedKind(ref Utf8JsonReader reader)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var raw = JsonSerializer.Deserialize<NamedKindInternal>(ref reader);
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return Enum.Parse<DisplayColorKind>(raw.NamedKind);
		}
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Identifier value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString(nameof(Identifier.Mode), value.Mode.ToString());
		writer.WritePropertyName("Value");
		writer.WriteStartObject();
		switch (value.Mode)
		{
			case IdentifierColorMode.Id:
			{
				writer.WriteNumber(nameof(Identifier.Id), value.Id);
				break;
			}
			case IdentifierColorMode.Raw:
			{
				writer.WriteNumber(nameof(Identifier.A), value.A);
				writer.WriteNumber(nameof(Identifier.R), value.R);
				writer.WriteNumber(nameof(Identifier.G), value.G);
				writer.WriteNumber(nameof(Identifier.B), value.B);
				break;
			}
			case IdentifierColorMode.Named:
			{
				writer.WriteString(nameof(Identifier.NamedKind), value.NamedKind.ToString());
				break;
			}
		}
		writer.WriteEndObject();
		writer.WriteEndObject();
	}
}
