namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// Defines a JSON converter that allows the conversions between <see cref="bool"/> and <see cref="int"/>.
/// </summary>
public sealed class BoolInt32Converter : JsonConverter<bool>
{
	/// <inheritdoc/>
	public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.True => true,
			JsonTokenType.False => false,
			JsonTokenType.Number => reader.TryGetInt32(out int num) && Convert.ToBoolean(num),
			_ => false,
		};

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
		=> writer.WriteNumberValue(value ? 1 : 0);
}
