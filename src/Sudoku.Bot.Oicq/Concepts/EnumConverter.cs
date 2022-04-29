namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines an enumeration type converter.
/// </summary>
/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
public sealed class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
	/// <inheritdoc/>
	public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
#if NETSTANDARD && !NETSTANDARD2_1_OR_GREATER || NETFRAMEWORK
		=> (TEnum)Enum.Parse(typeof(TEnum), reader.GetString()!, true);
#elif NET && NET5_0_OR_GREATER || NETCORE2_0_OR_GREATER
		=> Enum.Parse<TEnum>(reader.GetString()!, true);
#else
#error The platform is invalid. The supported platforms are '.NET', '.NET Framework' or '.NET Standard'.
#endif

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString().ToLower());
}
