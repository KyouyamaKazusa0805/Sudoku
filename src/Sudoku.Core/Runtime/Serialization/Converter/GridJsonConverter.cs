namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="Grid"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// "4+80002+306+31000+9004000+4030109+38+24+675+1+75+1+39+8+46+2+2640708+3902090000010000002080+36+20007:713 723 533 633 537 575 176 576 577 579 583 784 586 587 496 997"
/// </code>
/// </remarks>
/// <seealso cref="Grid"/>
public sealed class GridJsonConverter : JsonConverter<Grid>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.Read() && reader.TokenType == JsonTokenType.String && reader.GetString() is { } value
			? Grid.Parse(value)
			: throw new JsonException();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("#"));
}
