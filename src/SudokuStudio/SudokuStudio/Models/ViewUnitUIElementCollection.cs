namespace SudokuStudio.Models;

/// <summary>
/// Represents a collection that only stores a list of <see cref="UIElement"/> instances that describes for a <see cref="ViewUnit"/> instance.
/// </summary>
/// <seealso cref="UIElement"/>
/// <seealso cref="ViewUnit"/>
public sealed class ViewUnitUIElementCollection : List<UIElement>
{
	/// <summary>
	/// Initializes a <see cref="ViewUnitUIElementCollection"/> instance.
	/// </summary>
	public ViewUnitUIElementCollection() : base()
	{
	}

	/// <summary>
	/// Initializes a <see cref="ViewUnitUIElementCollection"/> instance via the specified collection of <see cref="UIElement"/>s.
	/// </summary>
	/// <param name="controls">A list of <see cref="UIElement"/>s.</param>
	public ViewUnitUIElementCollection(IEnumerable<UIElement> controls) : base(controls)
	{
	}
}
