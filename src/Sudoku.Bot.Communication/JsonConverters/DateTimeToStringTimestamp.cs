namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// Defines a JSON converter that allows the conversion between <see cref="DateTime"/> and <c>Timestamp</c>.
/// </summary>
public sealed class DateTimeToStringTimestamp : JsonConverter<DateTime>
{
	/// <inheritdoc/>
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.Number => DateTime.MinValue.AddMilliseconds(reader.GetDouble()),
			JsonTokenType.String when (reader.GetString() ?? "0") is var timeStamp
				=> Regex.Match(timeStamp, """^\d+$""") switch
				{
					{ Success: true, Groups: [{ Value: var rawValue }, ..] } when double.Parse(rawValue) is var offsetVal
						 => timeStamp switch
						 {
							 { Length: <= 10 } => DateTime.MinValue.AddSeconds(offsetVal),
							 _ => DateTime.MinValue.AddMilliseconds(offsetVal)
						 },
					_ => DateTime.TryParse(timeStamp, out var result) ? result : DateTime.MinValue
				},
			_ => DateTime.MinValue
		};

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:sszzz"));
}
