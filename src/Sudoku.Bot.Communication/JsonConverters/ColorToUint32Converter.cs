namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// JSON序列化时将 Color 转换为 UInt32<br/>
/// JSON反序列化时将 UInt32 转换为 Color
/// </summary>
public class ColorToUint32Converter : JsonConverter<Color>
{
	/// <summary>
	/// 序列化JSON时 Color 转 int
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="value"></param>
	/// <param name="options"></param>
	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
		=> writer.WriteNumberValue((uint)value.ToArgb());

	/// <summary>
	/// 反序列化JSON时 int 转 Color
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.Number => Color.FromArgb((int)reader.GetUInt32()),
			_ => Color.Black,
		};
}
