using Sudoku.Collections;

namespace Sudoku.Serialization.Converters;

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
	public override void Write(Utf8JsonWriter writer, Cells value, JsonSerializerOptions options) =>
		writer.WriteCollection(value);
}
