namespace Sudoku.UI.Models;

/// <summary>
/// Defines a version timeline group item.
/// </summary>
public sealed class VersionTimelineGroupItem
{
	/// <summary>
	/// Initializes a <see cref="VersionTimelineGroupItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	public VersionTimelineGroupItem() => Version = null!;


	/// <summary>
	/// Indicates the version of the timeline group used.
	/// </summary>
	public required string Version { get; set; }

	/// <summary>
	/// Indicates the items.
	/// </summary>
	public IList<VersionTimelineItem> Items { get; set; } = new List<VersionTimelineItem>();
}
