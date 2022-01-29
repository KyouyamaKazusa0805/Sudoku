using Sudoku.Presentation;

namespace Sudoku.Serialization.Converters;

/// <summary>
/// Defines a serialization converter to convert the <see cref="UnknownValue"/>
/// to a JSON <see cref="string"/> or vice versa.
/// </summary>
[JsonConverter(typeof(UnknownValue))]
public sealed class UnknownValueConverter : JsonConverter<UnknownValue>
{
	/// <inheritdoc/>
	/// <exception cref="JsonException">Throws when the current JSON value is mal-formed.</exception>
	public override unsafe UnknownValue Read(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options
	)
	{
		if (!CanConvert(typeToConvert))
		{
			return default;
		}

		Unsafe.SkipInit(out int cell);
		Unsafe.SkipInit(out char unknownIdentifier);
		Unsafe.SkipInit(out short digitsMask);
		while (reader.Read())
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				continue;
			}

			switch (reader.GetString())
			{
				case nameof(cell):
				{
					cell = reader.Read() ? reader.GetInt32() : throw new JsonException("The current JSON value is mal-formed.");
					break;
				}
				case nameof(unknownIdentifier):
				{
					unknownIdentifier = reader.Read() ? reader.GetString()![0] : throw new JsonException("The current JSON value is mal-formed.");
					break;
				}
				case nameof(digitsMask):
				{
					short tempMask = 0;
					foreach (int element in reader.GetInt32Array())
					{
						tempMask |= (short)(1 << element);
					}

					digitsMask = tempMask;
					break;
				}
				case var propertyName:
				{
					throw new JsonException($"The current property name '{propertyName}' is invalid to parse.");
				}
			}
		}

		return new(cell, unknownIdentifier, digitsMask);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, UnknownValue value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteNumber(Casing.ToCamelCase(nameof(UnknownValue.Cell)), value.Cell);
		writer.WriteString(Casing.ToCamelCase(nameof(UnknownValue.UnknownIdentifier)), stackalloc[] { value.UnknownIdentifier });
		writer.WriteBitCollection(Casing.ToCamelCase(nameof(UnknownValue.DigitsMask)), value.DigitsMask);
		writer.WriteEndObject();
	}
}
