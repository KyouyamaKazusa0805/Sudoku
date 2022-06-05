namespace Sudoku.UI.Drawing.Serialization;

/// <summary>
/// Defines a JSON converter that can serialize or deserialize the type <see cref="Color"/>.
/// </summary>
/// <seealso cref="Color"/>
public sealed class ColorJsonConverter : JsonConverter<Color>
{
	/// <inheritdoc/>
	public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		Unsafe.SkipInit(out byte a);
		Unsafe.SkipInit(out byte r);
		Unsafe.SkipInit(out byte g);
		Unsafe.SkipInit(out byte b);
		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.EndObject:
				{
					goto Returning;
				}
				case JsonTokenType.PropertyName:
				{
					string propertyName = reader.GetString()!.ToLower();
					if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
					{
						throw new JsonException();
					}

					(
						propertyName == "a"
							? ref a
							: ref propertyName == "r"
								? ref r
								: ref propertyName == "g"
									? ref g
									: ref propertyName == "b"
										? ref b
										: ref ReferenceMarshal.RefThrow<byte, JsonException>()
					) = reader.GetByte();

					break;
				}
			}
		}

	Returning:
		return Color.FromArgb(a, r, g, b);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
		=> writer.WriteNestedObject(value, options);
}
