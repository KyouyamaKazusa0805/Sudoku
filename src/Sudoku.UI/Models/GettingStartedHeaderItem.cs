namespace Sudoku.UI.Models;

/// <summary>
/// Indicates a getting-started item.
/// </summary>
public sealed class GettingStartedItem
{
	/// <summary>
	/// Indicates the title.
	/// </summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the content.
	/// </summary>
	public object Content { get; set; } = null!;

	/// <summary>
	/// Indicates the data template used.
	/// </summary>
	public DataTemplate DataTemplate { get; set; } = null!;
}
