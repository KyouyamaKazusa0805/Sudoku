namespace Sudoku.Techniques;

/// <summary>
/// Indicates a technique group.
/// </summary>
/// <remarks>
/// Different with <see cref="TechniqueTags"/>, this enumeration type contains
/// the real technique group that the technique belongs to. In addition, the value
/// of this type may effect the displaying of the analysis result.
/// </remarks>
public enum TechniqueGroup : byte
{
	/// <summary>
	/// Indicates the technique doesn't belong to any group.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the singles technique.
	/// </summary>
	Single,

	/// <summary>
	/// Indicates the locked candidates (LC) technique.
	/// </summary>
	LockedCandidates,

	/// <summary>
	/// Indicates the subset technique.
	/// </summary>
	Subset,

	/// <summary>
	/// Indicates the normal fish technique.
	/// </summary>
	NormalFish,

	/// <summary>
	/// Indicates the complex fish technique.
	/// </summary>
	ComplexFish,

	/// <summary>
	/// Indicates the wing technique.
	/// </summary>
	Wing,

	/// <summary>
	/// Indicates the empty rectangle technique.
	/// </summary>
	EmptyRectangle,

	/// <summary>
	/// Indicates the single digit pattern (SDP) technique.
	/// </summary>
	SingleDigitPattern,

	/// <summary>
	/// Indicates the empty rectangle intersection pair (ERIP) technique.
	/// </summary>
	EmptyRectangleIntersectionPair,

	/// <summary>
	/// Indicates the almost locked candidates (ALC) technique.
	/// </summary>
	AlmostLockedCandidates,

	/// <summary>
	/// Indicates the alternating inference chain (AIC) technique.
	/// </summary>
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates the forcing chains (FC) technique.
	/// </summary>
	ForcingChains,

	/// <summary>
	/// Indicates the unique rectangle (UR) technique.
	/// </summary>
	UniqueRectangle,

	/// <summary>
	/// Indicates the unique rectangle plus (UR+) technique.
	/// </summary>
	UniqueRectanglePlus,

	/// <summary>
	/// Indicates the unique loop (UL) technique.
	/// </summary>
	UniqueLoop,

	/// <summary>
	/// Indicates the extended rectangle (XR) technique.
	/// </summary>
	ExtendedRectangle,

	/// <summary>
	/// Indicates the bi-value universal grave (BUG) technique.
	/// </summary>
	BivalueUniversalGrave,

	/// <summary>
	/// Indicates the reverse bi-value universal grave (Reverse BUG) technique.
	/// </summary>
	ReverseBivalueUniversalGrave,

	/// <summary>
	/// Indicates the deadly pattern technique.
	/// </summary>
	DeadlyPattern,

	/// <summary>
	/// Indicates the bi-value oddagon technique.
	/// </summary>
	BivalueOddagon,

	/// <summary>
	/// Indicates the sue de coq (SdC) technique.
	/// </summary>
	SueDeCoq,

	/// <summary>
	/// Indicates the broken wing technique.
	/// </summary>
	BrokenWing,

	/// <summary>
	/// Indicates the ALS chaining-like (ALS-XZ, ALS-XY-Wing, ALS-W-Wing) technique.
	/// </summary>
	AlmostLockedSetsChainingLike,

	/// <summary>
	/// Indicates the SK-Loop technique.
	/// </summary>
	DominoLoop,

	/// <summary>
	/// Indicates the multi-sector locked sets (MSLS) technique.
	/// </summary>
	MultisectorLockedSets,

	/// <summary>
	/// Indicates the exocet technique.
	/// </summary>
	Exocet,

	/// <summary>
	/// Indicates the symmetry technique.
	/// </summary>
	Symmetry,

	/// <summary>
	/// Indicates the technique checked and searched relies on the rank theory.
	/// </summary>
	RankTheory,

	/// <summary>
	/// Indicates the bowman bingo technique.
	/// </summary>
	BowmanBingo,

	/// <summary>
	/// Indicates the pattern overlay method (POM) technique.
	/// </summary>
	PatternOverlay,

	/// <summary>
	/// Indicates the templating technique.
	/// </summary>
	Templating,

	/// <summary>
	/// Indicates the brute force (BF) technique.
	/// </summary>
	BruteForce
}
