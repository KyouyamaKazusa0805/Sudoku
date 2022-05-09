namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// Defines a JSON converter that allows the conversion between <see cref="Color"/> and <see cref="uint"/>.
/// </summary>
public sealed class ColorUInt32Converter : JsonConverter<Color>
{
	/// <inheritdoc/>
	public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.Number => Color.FromArgb((int)reader.GetUInt32()),
			_ => Color.Black,
		};

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
		=> writer.WriteNumberValue((uint)value.ToArgb());
}
