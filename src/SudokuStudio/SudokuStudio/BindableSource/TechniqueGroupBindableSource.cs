namespace SudokuStudio.BindableSource;

/// <summary>
/// Indicates a group of technique steps.
/// </summary>
/// <param name="elements">The elements.</param>
public sealed class TechniqueGroupBindableSource(IEnumerable<object> elements) : List<object>(elements), IBindableSource
{
	/// <summary>
	/// Indicates the key of the group.
	/// </summary>
	public object? Key { get; set; }
}
