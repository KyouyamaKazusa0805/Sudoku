namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// JSON序列化时将 RemindType 转换为 StringNumber<br/>
/// JSON反序列化时将 StringNumber 转换为 RemindType
/// </summary>
public class RemindTypeToStringNumberConverter : JsonConverter<RemindType>
{
	/// <summary>
	/// JSON序列化时将 RemindType 转 StringNumber
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="value"></param>
	/// <param name="options"></param>
	public override void Write(Utf8JsonWriter writer, RemindType value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("D"));

	/// <summary>
	/// JSON反序列化时将 StringNumber 转 RemindType
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public override RemindType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		int numCode = reader.TokenType switch
		{
			JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? value : 0,
			JsonTokenType.Number => reader.GetInt32(),
			_ => 0
		};

		return Enum.GetNames(typeof(RemindType)).Length > numCode ? (RemindType)numCode : RemindType.Never;
	}
}
