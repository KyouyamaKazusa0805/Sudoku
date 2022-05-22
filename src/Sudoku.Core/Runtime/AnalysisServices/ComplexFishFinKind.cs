namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Indicates the fin modifiers.
/// </summary>
[Flags]
public enum ComplexFishFinKind
{
	/// <summary>
	/// Indicates the normal fish (i.e. no fins).
	/// </summary>
	Normal = 1,

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
	/// Indicates the siamese fish (i.e. two fish share same base sets, with different cover sets).
	/// </summary>
	Siamese = 1 << 3
}
