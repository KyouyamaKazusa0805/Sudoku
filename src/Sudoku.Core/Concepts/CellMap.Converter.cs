namespace Sudoku.Concepts;

public partial struct CellMap
{
	/// <summary>
	/// Indicates the JSON converter of <see cref="CellMap"/> instance.
	/// </summary>
	/// <seealso cref="CellMap"/>
	private sealed class Converter : JsonConverter<CellMap>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		public override CellMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> new(JsonSerializer.Deserialize<string[]>(ref reader, options)!);

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, CellMap value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var element in value.StringChunks)
			{
				writer.WriteStringValue(element);
			}
			writer.WriteEndArray();
		}
	}
}
