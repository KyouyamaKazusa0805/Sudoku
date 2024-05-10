namespace System.Text.Json;

/// <summary>
/// Represents a converter for <see cref="DateOnly"/> instance.
/// </summary>
/// <param name="format">Indicates the format to be initialized.</param>
/// <seealso cref="DateOnly"/>
[method: SetsRequiredMembers]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed partial class DateOnlyConverter(
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")]
	[StringSyntax(StringSyntaxAttribute.DateOnlyFormat)]
	string format
) : JsonConverter<DateOnly>
{
	/// <inheritdoc/>
	public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType != JsonTokenType.String ? throw new JsonException() : DateOnly.ParseExact(reader.GetString()!, Format);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(Format));
}
