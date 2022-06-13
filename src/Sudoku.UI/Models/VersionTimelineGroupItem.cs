namespace Sudoku.UI.Models;

/// <summary>
/// Defines a version timeline group item.
/// </summary>
public sealed class VersionTimelineGroupItem : List<VersionTimelineItem>
{
	/// <summary>
	/// Initializes a <see cref="VersionTimelineGroupItem"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public VersionTimelineGroupItem() : base(Array.Empty<VersionTimelineItem>())
	{
	}

	/// <summary>
	/// Initializes a <see cref="VersionTimelineGroupItem"/> instance via the specified items.
	/// </summary>
	/// <param name="items">The items.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public VersionTimelineGroupItem(IEnumerable<VersionTimelineItem> items) : base(items)
	{
	}


	/// <summary>
	/// Indicates the version of the timeline group used.
	/// </summary>
	public string Version { get; set; } = string.Empty;
}
