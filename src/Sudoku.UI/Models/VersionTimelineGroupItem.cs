namespace Sudoku.UI.Models;

/// <summary>
/// Defines a version timeline group item.
/// </summary>
public sealed class VersionTimelineGroupItem
{
	/// <summary>
	/// Indicates the items.
	/// </summary>
	public IList<VersionTimelineItem> Items { get; set; } = new List<VersionTimelineItem>();
}
