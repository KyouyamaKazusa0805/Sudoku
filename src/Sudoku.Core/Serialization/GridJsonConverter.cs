namespace Sudoku.Serialization;

/// <summary>
/// Defines a JSON converter that parses the string to a sudoku grid or vice versa.
/// </summary>
[JsonConverter(typeof(Grid))]
public sealed class GridJsonConverter : JsonConverter<Grid>
{
	/// <inheritdoc/>
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(Grid))
		{
			return Grid.Undefined;
		}

		if (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.String && Grid.TryParse(reader.GetString()!, out var grid))
			{
				return grid;
			}
		}

		return Grid.Undefined;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value.TextCode);
}
