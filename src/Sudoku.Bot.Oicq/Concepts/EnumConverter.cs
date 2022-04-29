namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines an enumeration type converter.
/// </summary>
/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
public sealed class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
	/// <inheritdoc/>
	public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Enum.Parse<TEnum>(reader.GetString()!, true);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString().ToLower());
}
