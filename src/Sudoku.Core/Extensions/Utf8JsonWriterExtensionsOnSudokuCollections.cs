namespace Sudoku.Collections;

/// <summary>
/// Provides extension methods on <see cref="Utf8JsonWriter"/>, and writes the collection values
/// as an array into it.
/// </summary>
internal static class Utf8JsonWriterExtensionsOnSudokuCollections
{
	/// <summary>
	/// Writes an array of <see cref="Cells"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="cells">The cells to be written.</param>
	public static void WriteCollection(this Utf8JsonWriter @this, in Cells cells)
	{
		@this.WriteStartArray();
		foreach (byte cell in cells)
		{
			@this.WriteStringValue(new Coordinate(cell).ToString());
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes an array of <see cref="Candidates"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="candidates">The candidates to be written.</param>
	public static void WriteCollection(this Utf8JsonWriter @this, in Candidates candidates)
	{
		@this.WriteStartArray();
		foreach (int candidate in candidates)
		{
			@this.WriteStringValue(new Candidates { candidate }.ToString());
		}
		@this.WriteEndArray();
	}
}
