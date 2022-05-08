namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// Defines a JSON converter that allows the conversion between <see cref="PrivacyType"/> and <see cref="string"/>.
/// </summary>
public sealed class PrivacyTypeToStringNumberConverter : JsonConverter<PrivacyType>
{
	/// <inheritdoc/>
	public override PrivacyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (PrivacyType)(
			reader.TokenType switch
			{
				JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? value : 0,
				JsonTokenType.Number => reader.GetInt32(),
				_ => 0
			}
		);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, PrivacyType value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("D"));
}
