namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// JSON序列化JSON时将 DateTime 转换为 Timestamp<br/>
/// JSON反序列化JSON时将 Timestamp 转换为 DateTime
/// </summary>
public class DateTimeToStringTimestamp : JsonConverter<DateTime>
{
	/// <summary>
	/// 序列化JSON时 DateTime 转 Timestamp
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="value"></param>
	/// <param name="options"></param>
	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		=> writer.WriteStringValue($"{value:yyyy-MM-ddTHH:mm:sszzz}");

	/// <summary>
	/// 反序列化JSON时 Timestamp 转 DateTime
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
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
}
