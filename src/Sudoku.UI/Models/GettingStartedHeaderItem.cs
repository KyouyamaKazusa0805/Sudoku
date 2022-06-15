namespace Sudoku.UI.Models;

/// <summary>
/// Indicates a getting-started item.
/// </summary>
public sealed class GettingStartedItem
{
	/// <summary>
	/// Initializes a <see cref="GettingStartedItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	public GettingStartedItem() => (Content, Title, DataTemplate) = (null!, null!, null!);


	/// <summary>
	/// Indicates the content.
	/// </summary>
	public required object Content { get; set; }

	/// <summary>
	/// Indicates the title.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Indicates the data template used.
	/// </summary>
	public required DataTemplate DataTemplate { get; set; }
}
