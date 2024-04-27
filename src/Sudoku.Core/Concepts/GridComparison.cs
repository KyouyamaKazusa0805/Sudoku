namespace Sudoku.Concepts;

/// <summary>
/// Specifies the culture, case, and sort rules to be used by certain overloads of
/// the <see cref="Grid.CompareTo(ref readonly Grid)"/> and <see cref="Grid.Equals(ref readonly Grid)"/> methods.
/// </summary>
/// <seealso cref="Grid.CompareTo(ref readonly Grid)"/>
/// <seealso cref="Grid.Equals(ref readonly Grid)"/>
public enum GridComparison
{
	/// <summary>
	/// Indicates two <see cref="Grid"/> instances only compares internal <see cref="Mask"/> values one by one.
	/// </summary>
	Default,

	/// <summary>
	/// Indicates two <see cref="Grid"/> instances compares not only internal values, but its all possible transformations.
	/// </summary>
	IncludingTransforms
}
