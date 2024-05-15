namespace System.Text.Json;

/// <summary>
/// Represents a converter for <see cref="DateOnly"/> instance.
/// </summary>
/// <seealso cref="DateOnly"/>
public sealed partial class DateOnlyConverter : JsonConverter<DateOnly>
{
	/// <summary>
	/// Indicates the format text to be used.
	/// </summary>
	[StringSyntax(StringSyntaxAttribute.DateOnlyFormat)]
	public required string Format { get; init; }


	/// <inheritdoc/>
	public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType != JsonTokenType.String ? throw new JsonException() : DateOnly.ParseExact(reader.GetString()!, Format);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(Format));
}
