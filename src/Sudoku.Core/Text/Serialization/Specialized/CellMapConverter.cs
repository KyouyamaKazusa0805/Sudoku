namespace Sudoku.Text.Serialization.Specialized;

/// <summary>
/// Indicates the JSON converter of <see cref="CellMap"/> instance.
/// </summary>
/// <seealso cref="CellMap"/>
public sealed class CellMapConverter : JsonConverter<CellMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellMap Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
