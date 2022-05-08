namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// JSON序列化时将 Intent 转换为 StringArray<br/>
/// JSON反序列化时将 StringArray 转换为 Intent
/// </summary>
public class IntentToStringArrayConverter : JsonConverter<Intent>
{
	/// <summary>
	/// JSON序列化时将 RemindType 转 StringNumber
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="value"></param>
	/// <param name="options"></param>
	public override void Write(Utf8JsonWriter writer, Intent value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, value.ToString().Split(',').Select(f => f.Trim()), options);

	/// <summary>
	/// JSON反序列化时将 StringNumber 转 RemindType
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public override Intent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var intents = JsonSerializer.Deserialize<List<string>>(ref reader, options)?.Select(Enum.Parse<Intent>);
		return intents?.Aggregate((a, b) => a | b) ?? Intents.PublicDomain;
	}
}
