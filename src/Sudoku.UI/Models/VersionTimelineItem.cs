namespace Sudoku.UI.Models;

/// <summary>
/// Defines a version timeline item.
/// </summary>
public sealed class VersionTimelineItem
{
	/// <summary>
	/// Initializes a <see cref="VersionTimelineItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	public VersionTimelineItem() => Description = null!;


	/// <summary>
	/// Indicates the description.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// Indicates the date that the item introduced.
	/// The value can be <see langword="null"/> if you don't want to specify it.
	/// </summary>
	public DateOnly? Date { get; set; }
}
