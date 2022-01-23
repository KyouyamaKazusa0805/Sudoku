namespace Sudoku.Serialization.Converters;

/// <summary>
/// Defines a serialization converter to convert the <see cref="Crosshatch"/>
/// to a JSON <see cref="string"/> or vice versa.
/// </summary>
[JsonConverter(typeof(Crosshatch))]
public sealed class CrosshatchConverter : JsonConverter<Crosshatch>
{
	/// <inheritdoc/>
	/// <exception cref="JsonException">Throws when the JSON text is mal-formed.</exception>
	public override Crosshatch Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!CanConvert(typeToConvert))
		{
			return default;
		}

		var converter = options.GetConverter<Cells, CellsConverter>();

		Unsafe.SkipInit(out Cells startCells);
		Unsafe.SkipInit(out Cells endCells);
		while (reader.Read())
		{
			switch (reader.GetString())
			{
				case "start":
				{
					startCells = reader.ReadObject(converter, options);
					break;
				}
				case "end":
				{
					endCells = reader.ReadObject(converter, options);
					break;
				}
				case var propertyName:
				{
					throw new JsonException($"The current property name '{propertyName}' is invalid to parse.");
				}
			}
		}

		return new(startCells, endCells);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Crosshatch value, JsonSerializerOptions options)
	{
		var converter = options.GetConverter<Cells, CellsConverter>();

		writer.WriteStartObject();
		writer.WritePropertyName(Casing.ToCamelCase(nameof(Crosshatch.Start)));
		writer.WriteObject(value.Start, converter, options);
		writer.WritePropertyName(Casing.ToCamelCase(nameof(Crosshatch.End)));
		writer.WriteObject(value.End, converter, options);
		writer.WriteEndObject();
	}
}
