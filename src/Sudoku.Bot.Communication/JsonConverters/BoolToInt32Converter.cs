namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// JSON序列化时将 bool 转换为 int<br/>
/// JSON反序列化时将 int 转换为 bool
/// </summary>
public class BoolToInt32Converter : JsonConverter<bool>
{
	/// <summary>
	/// 序列化JSON时 bool 转 int
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="value"></param>
	/// <param name="options"></param>
	public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
		=> writer.WriteNumberValue(value ? 1 : 0);

	/// <summary>
	/// 反序列化JSON时 int 转 bool
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.True => true,
			JsonTokenType.False => false,
			JsonTokenType.Number => reader.TryGetInt32(out int num) && Convert.ToBoolean(num),
			_ => false,
		};
}
