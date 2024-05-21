namespace Sudoku.Text.Serialization.Specialized;

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
public sealed class GridConverter : JsonConverter<Grid>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() is { } s ? Grid.Parse(s) : Grid.Undefined;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("#"));
}
