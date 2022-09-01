namespace Sudoku.Text.Serialization.Specialized;

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
	/// <summary>
	/// Indicates the property name <c>"Value"</c>.
	/// </summary>
	private const string ValuePropertyName = "Value";


	/// <inheritdoc/>
	public override Identifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName
			|| reader.GetString() != ConvertName(nameof(Identifier.Mode), options))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName
			|| reader.GetString() != ConvertName(ValuePropertyName, options))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException();
		}

		var mode = Enum.Parse<IdentifierColorMode>(reader.GetString() ?? throw new JsonException(), true);
		return mode switch
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

			var raw = (IdInternal)Deserialize(ref reader, typeof(IdInternal), IdInternalSerializerContext.Default)!;
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

			var raw = (ColorInternal)Deserialize(ref reader, typeof(ColorInternal), ColorInternalSerializerContext.Default)!;
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

			var raw = (NamedKindInternal)Deserialize(ref reader, typeof(NamedKindInternal), NamedKindInternalJsonSerializerContext.Default)!;
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return Enum.Parse<DisplayColorKind>(raw.NamedKind, true);
		}
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Identifier value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString(ConvertName(nameof(value.Mode), options), value.Mode.ToString());
		writer.WritePropertyName(ConvertName(ValuePropertyName, options));
		writer.WriteStartObject();
		switch (value.Mode)
		{
			case IdentifierColorMode.Id:
			{
				writer.WriteNumber(nameof(value.Id), value.Id);
				break;
			}
			case IdentifierColorMode.Raw:
			{
				writer.WriteNumber(ConvertName(nameof(value.A), options), value.A);
				writer.WriteNumber(ConvertName(nameof(value.R), options), value.R);
				writer.WriteNumber(ConvertName(nameof(value.G), options), value.G);
				writer.WriteNumber(ConvertName(nameof(value.B), options), value.B);
				break;
			}
			case IdentifierColorMode.Named:
			{
				writer.WriteString(ConvertName(nameof(value.NamedKind), options), value.NamedKind.ToString());
				break;
			}
		}
		writer.WriteEndObject();
		writer.WriteEndObject();
	}


	/// <summary>
	/// Try to convert the specified string value as the specified naming policy
	/// specified in argument <paramref name="options"/>.
	/// </summary>
	/// <param name="base">The string to convert.</param>
	/// <param name="options">The options.</param>
	/// <returns>The value converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string ConvertName(string @base, JsonSerializerOptions options)
		=> options.PropertyNamingPolicy?.ConvertName(@base) ?? @base;
}
