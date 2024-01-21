namespace Sudoku.Text.Serialization.Specialized;

/// <summary>
/// Indicates the JSON converter of <see cref="CandidateMap"/> instance.
/// </summary>
/// <seealso cref="CandidateMap"/>
public sealed class CandidateMapConverter : JsonConverter<CandidateMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateMap Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> new(JsonSerializer.Deserialize<string[]>(ref reader, options)!);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, CandidateMap value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var element in value.StringChunks)
		{
			writer.WriteStringValue(element);
		}
		writer.WriteEndArray();
	}
}
