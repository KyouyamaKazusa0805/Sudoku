#pragma warning disable CS0618

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
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Lc,

	/// <summary>
	/// Indicates the locked candidates (LC) technique.
	/// </summary>
	LockedCandidates = Lc,

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
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Sdp,

	/// <summary>
	/// Indicates the single digit pattern (SDP) technique.
	/// </summary>
	SingleDigitPattern = Sdp,

	/// <summary>
	/// Indicates the empty rectangle intersection pair (ERIP) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Erip,

	/// <summary>
	/// Indicates the empty rectangle intersection pair (ERIP) technique.
	/// </summary>
	EmptyRectangleIntersectionPair = Erip,

	/// <summary>
	/// Indicates the almost locked candidates (ALC) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Alc,

	/// <summary>
	/// Indicates the almost locked candidates (ALC) technique.
	/// </summary>
	AlmostLockedCandidates = Alc,

	/// <summary>
	/// Indicates the alternating inference chain (AIC) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Aic,

	/// <summary>
	/// Indicates the alternating inference chain (AIC) technique.
	/// </summary>
	AlternatingInferenceChain = Aic,

	/// <summary>
	/// Indicates the forcing chains (FC) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Fc,

	/// <summary>
	/// Indicates the forcing chains (FC) technique.
	/// </summary>
	ForcingChains = Fc,

	/// <summary>
	/// Indicates the unique rectangle (UR) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Ur,

	/// <summary>
	/// Indicates the unique rectangle (UR) technique.
	/// </summary>
	UniqueRectangle = Ur,

	/// <summary>
	/// Indicates the unique rectangle plus (UR+) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus,

	/// <summary>
	/// Indicates the unique rectangle plus (UR+) technique.
	/// </summary>
	UniqueRectanglePlus = UrPlus,

	/// <summary>
	/// Indicates the unique loop (UL) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Ul,

	/// <summary>
	/// Indicates the unique loop (UL) technique.
	/// </summary>
	UniqueLoop = Ul,

	/// <summary>
	/// Indicates the extended rectangle (XR) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Xr,

	/// <summary>
	/// Indicates the extended rectangle (XR) technique.
	/// </summary>
	ExtendedRectangle = Xr,

	/// <summary>
	/// Indicates the bi-value universal grave (BUG) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Bug,

	/// <summary>
	/// Indicates the bi-value universal grave (BUG) technique.
	/// </summary>
	BivalueUniversalGrave = Bug,

	/// <summary>
	/// Indicates the reverse bi-value universal grave (Reverse BUG) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseBug,

	/// <summary>
	/// Indicates the reverse bi-value universal grave (Reverse BUG) technique.
	/// </summary>
	ReverseBivalueUniversalGrave = ReverseBug,

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
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Sdc,

	/// <summary>
	/// Indicates the sue de coq (SdC) technique.
	/// </summary>
	SueDeCoq = Sdc,

	/// <summary>
	/// Indicates the guardian technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Guardian,

	/// <summary>
	/// Indicates the broken wing technique.
	/// </summary>
	BrokenWing = Guardian,

	/// <summary>
	/// Indicates the ALS chaining-like (ALS-XZ, ALS-XY-Wing, ALS-W-Wing) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	AlsChainingLike,

	/// <summary>
	/// Indicates the ALS chaining-like (ALS-XZ, ALS-XY-Wing, ALS-W-Wing) technique.
	/// </summary>
	AlmostLockedSetsChainingLike = AlsChainingLike,

	/// <summary>
	/// Indicates the SK-Loop technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	SkLoop,

	/// <summary>
	/// Indicates the SK-Loop technique.
	/// </summary>
	DominoLoop = SkLoop,

	/// <summary>
	/// Indicates the multi-sector locked sets (MSLS) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Msls,

	/// <summary>
	/// Indicates the multi-sector locked sets (MSLS) technique.
	/// </summary>
	MultisectorLockedSets = Msls,

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
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Pom,

	/// <summary>
	/// Indicates the pattern overlay method (POM) technique.
	/// </summary>
	PatternOverlay = Pom,

	/// <summary>
	/// Indicates the templating technique.
	/// </summary>
	Templating,

	/// <summary>
	/// Indicates the brute force (BF) technique.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Bf,

	/// <summary>
	/// Indicates the brute force (BF) technique.
	/// </summary>
	BruteForce = Bf
}
