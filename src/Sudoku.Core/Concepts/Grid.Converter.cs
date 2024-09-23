namespace Sudoku.Concepts;

public partial struct Grid
{
	/// <summary>
	/// Indicates the JSON converter of the current type.
	/// </summary>
	private sealed class Converter : JsonConverter<Grid>
	{
		/// <inheritdoc/>
		public override bool HandleNull => true;


		/// <inheritdoc/>
		public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> reader.GetString() is { } s ? Parse(s) : Undefined;

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString("#"));
	}
}
