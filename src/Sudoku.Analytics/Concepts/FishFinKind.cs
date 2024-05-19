namespace Sudoku.Analytics.Steps;

/// <summary>
/// Indicates a fin modifier that is used for a complex fish pattern.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum FishFinKind
{
	/// <summary>
	/// Indicates the normal fish (i.e. no fins).
	/// </summary>
	Normal = 1 << 0,

	/// <summary>
	/// Indicates the finned fish
	/// (i.e. contains fins, but the fish may be regular when the fins are removed).
	/// </summary>
	Finned = 1 << 1,

	/// <summary>
	/// Indicates the sashimi fish
	/// (i.e. contains fins, and the fish may be degenerated to hidden singles when the fins are removed).
	/// </summary>
	Sashimi = 1 << 2,

	/// <summary>
	/// Indicates the Siamese fish (i.e. two fish share same base sets, with different cover sets).
	/// </summary>
	Siamese = 1 << 3
}
