namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// Defines a JSON converter that allows the conversion between <see cref="RemindType"/> and <see cref="string"/>.
/// </summary>
public sealed class RemindTypeNumberConverter : JsonConverter<RemindType>
{
	/// <inheritdoc/>
	public override RemindType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? value : 0,
			JsonTokenType.Number => reader.GetInt32(),
			_ => 0
		} is var numCode && Enum.GetNames(typeof(RemindType)).Length > numCode ? (RemindType)numCode : RemindType.Never;

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, RemindType value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("D"));
}
