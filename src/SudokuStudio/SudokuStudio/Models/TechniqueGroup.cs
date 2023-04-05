namespace SudokuStudio.Models;

/// <summary>
/// Indicates a group of technique steps.
/// </summary>
/// <param name="elements">The elements.</param>
public sealed class TechniqueGroup(IEnumerable<object> elements) : List<object>(elements)
{
	/// <summary>
	/// Indicates the key of the group.
	/// </summary>
	public object? Key { get; set; }
}
