namespace SudokuStudio.BindableSource;

/// <summary>
/// Defines a group that stores the data of the techniques and routing information, and its title.
/// </summary>
public sealed class TechniquePageRoutingBindableSourceGroup
{
	/// <summary>
	/// Indicates the title of the group.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Indicates the internal data collection.
	/// </summary>
	public required TechniquePageRoutingBindableSource[] Collection { get; set; }
}
