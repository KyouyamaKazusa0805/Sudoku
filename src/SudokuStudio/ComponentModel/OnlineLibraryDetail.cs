namespace SudokuStudio.ComponentModel;

/// <summary>
/// Represents the details to a library.
/// </summary>
public sealed class OnlineLibraryDetail
{
	/// <summary>
	/// Indicates the culture.
	/// </summary>
	[JsonPropertyName("$culture")]
	public CultureInfo? Culture { get; set; }

	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Indicates the description.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the remarks.
	/// </summary>
	public string? Remarks { get; set; }

	/// <summary>
	/// Indicates the tags.
	/// </summary>
	public string[] Tags { get; set; } = [];
}
