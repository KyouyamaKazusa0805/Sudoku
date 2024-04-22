namespace Sudoku.Text.Serialization.Specialized;

/// <summary>
/// Represents a JSON converter for type <see cref="Conclusion"/>.
/// </summary>
/// <seealso cref="Conclusion"/>
public sealed class ConclusionConverter : JsonConverter<Conclusion>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	public override Conclusion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Conclusion.ParseExact(reader.GetString() ?? string.Empty, new RxCyParser());

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Conclusion value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(GlobalizedConverter.InvariantCultureConverter));
}
