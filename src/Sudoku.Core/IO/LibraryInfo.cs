namespace Sudoku.IO;

/// <summary>
/// Represents information about a library.
/// </summary>
public sealed class LibraryInfo
{
	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the description to the library.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the author of the library.
	/// </summary>
	public string Author { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the tags of the library.
	/// </summary>
	public string[] Tags { get; set; } = [];
}
