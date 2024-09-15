namespace Sudoku.Categorization;

/// <summary>
/// Defines a feature flag.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum TechniqueFeatures
{
	/// <summary>
	/// Indicates no feature.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates it is hard to create a puzzle with this technique.
	/// </summary>
	HardToBeGenerated = 1 << 0,

	/// <summary>
	/// Indicates the current technique won't be appeared because it will be replaced with other techniques.
	/// </summary>
	WillBeReplacedByOtherTechnique = 1 << 1,

	/// <summary>
	/// Indicates the current technique can only exist in theory. In practice, this technique won't be appeared.
	/// </summary>
	OnlyExistInTheory = 1 << 2,

	/// <summary>
	/// Indicates the current technique hasn't implemented by author.
	/// </summary>
	NotImplemented = 1 << 3,

	/// <summary>
	/// Indicates the current technique only appears in direct views, i.e. candidates are not displayed.
	/// </summary>
	DirectTechniques = 1 << 4
}
