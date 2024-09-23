namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Indicates a shape modifier that is used for a complex fish pattern.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum ExocetShapeKind
{
	/// <summary>
	/// Indicates the basic exocet. This field is a reserved field.
	/// </summary>
	Basic = 1 << 0,

	/// <summary>
	/// Indicates the franken exocet.
	/// </summary>
	Franken = 1 << 1,

	/// <summary>
	/// Indicates the mutant exocet.
	/// </summary>
	Mutant = 1 << 2
}
