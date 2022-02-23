using Sudoku.Collections;

namespace Sudoku.Serialization.Converters;

/// <summary>
/// Defines a serialization converter to convert the <see cref="Grid"/>
/// to a JSON <see cref="string"/> or vice versa.
/// </summary>
[JsonConverter(typeof(Grid))]
public sealed class GridConverter : JsonConverter<Grid>
{
	/// <summary>
	/// The property name.
	/// </summary>
	private const string PropertyName = "Code";


	/// <inheritdoc/>
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!CanConvert(typeToConvert))
		{
			return Grid.Undefined;
		}

		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.PropertyName when reader.GetString() != PropertyName:
				{
					throw new InvalidOperationException("The specified property name doesn't support.");
				}
				case JsonTokenType.String when Grid.TryParse(reader.GetString()!, out var grid):
				{
					return grid;
				}
			}
		}

		return Grid.Undefined;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString(PropertyName, value.ToString("#"));
		writer.WriteEndObject();
	}
}
