namespace Sudoku.Concepts;

public partial struct Conclusion
{
	/// <summary>
	/// Represents a JSON converter for type <see cref="Conclusion"/>.
	/// </summary>
	/// <seealso cref="Conclusion"/>
	private sealed class Converter : JsonConverter<Conclusion>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		public override Conclusion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> Parse(reader.GetString() ?? string.Empty);

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Conclusion value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString());
	}
}
