namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a special flag describing how special the current technique is.
/// </summary>
[Flags]
public enum TechniqueMetadataSpecialFlags
{
	/// <summary>
	/// Indicates no flag.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the technique is defined as a Gurth's Symmetrical Placement rule.
	/// </summary>
	/// <remarks>
	/// This technique is specially-handled - it will be executed before all possible step searchers.
	/// </remarks>
	SymmetricalPlacement = 1 << 0,
}
