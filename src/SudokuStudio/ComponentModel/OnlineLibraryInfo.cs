namespace SudokuStudio.ComponentModel;

/// <summary>
/// Represents a data type that describes for an online library.
/// </summary>
public sealed class OnlineLibraryInfo
{
	/// <summary>
	/// Indicates the schema.
	/// </summary>
	[JsonPropertyName("$schema")]
	public string? Schema { get; set; }

	/// <summary>
	/// Indicates the author.
	/// </summary>
	public string Author { get; set; } = ResourceDictionary.Get("AnonymousAuthor", App.CurrentCulture);

	/// <summary>
	/// Indicates the extra link that can visit to learn more information about the library.
	/// </summary>
	public string? Link { get; set; }

	/// <summary>
	/// Indicates the last update time.
	/// </summary>
	public DateOnly LastUpdate { get; set; }

	/// <summary>
	/// Indicates the data of library.
	/// </summary>
	public OnlineLibraryDetail[]? Data { get; set; }
}
