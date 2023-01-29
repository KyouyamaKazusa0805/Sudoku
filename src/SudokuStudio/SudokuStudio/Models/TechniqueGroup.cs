namespace SudokuStudio.Models;

/// <summary>
/// Indicates a group of technique steps.
/// </summary>
public sealed class TechniqueGroup : List<object>
{
	/// <summary>
	/// Initializes a <see cref="TechniqueGroup"/> instance via the specified list of elements.
	/// </summary>
	/// <param name="elements">The elements.</param>
	public TechniqueGroup(IEnumerable<object> elements) : base(elements)
	{
	}


	/// <summary>
	/// Indicates the key of the group.
	/// </summary>
	public object? Key { get; set; }
}
