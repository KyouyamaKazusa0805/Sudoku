namespace SudokuStudio.Models;

/// <summary>
/// Defines a group that stores the data of the techniques and routing information, and its title.
/// </summary>
public sealed class TechniquePageRoutingDataGroup
{
	/// <summary>
	/// Indicates the title of the group.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Indicates the internal data collection.
	/// </summary>
	public required TechniquePageRoutingData[] Collection { get; set; }
}
