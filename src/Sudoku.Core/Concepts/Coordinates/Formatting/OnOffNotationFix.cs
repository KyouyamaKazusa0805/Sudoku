namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a kind of fix that describes on/off state of a chain node.
/// </summary>
public enum OnOffNotationFix
{
	/// <summary>
	/// Indicates no fix here to describe on/off state.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the fix type is prefix.
	/// </summary>
	Prefix,

	/// <summary>
	/// Indicates the fix type is suffix.
	/// </summary>
	Suffix
}
