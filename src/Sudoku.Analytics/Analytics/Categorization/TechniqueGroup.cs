namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a technique group.
/// </summary>
public enum TechniqueGroup
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
	[Abbreviation("LC")]
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
	/// Indicates the wing extension (extended subset principle) technique.
	/// </summary>
	[Abbreviation("ESP")]
	WingExtension,

	/// <summary>
	/// Indicates the empty rectangle technique.
	/// </summary>
	[Abbreviation("ER")]
	EmptyRectangle,

	/// <summary>
	/// Indicates the single digit pattern (SDP) technique.
	/// </summary>
	[Abbreviation("SDP")]
	SingleDigitPattern,

	/// <summary>
	/// Indicates the empty rectangle intersection pair (ERIP) technique.
	/// </summary>
	[Abbreviation("ERIP")]
	EmptyRectangleIntersectionPair,

	/// <summary>
	/// Indicates the almost locked candidates (ALC) technique.
	/// </summary>
	[Abbreviation("ALC")]
	AlmostLockedCandidates,

	/// <summary>
	/// Indicates the firework technique.
	/// </summary>
	Firework,

	/// <summary>
	/// Indicates the alternating inference chain (AIC) technique.
	/// </summary>
	[Abbreviation("AIC")]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates the forcing chains technique.
	/// </summary>
	ForcingChains,

	/// <summary>
	/// Indicates the blossom loop technique.
	/// </summary>
	BlossomLoop,

	/// <summary>
	/// Indicates the unique rectangle (UR) technique.
	/// </summary>
	[Abbreviation("UR")]
	UniqueRectangle,

	/// <summary>
	/// Indicates the unique rectangle plus (UR+) technique.
	/// </summary>
	[Abbreviation("UR+")]
	UniqueRectanglePlus,

	/// <summary>
	/// Indicates the avoidable rectangle (AR) technique.
	/// </summary>
	[Abbreviation("AR")]
	AvoidableRectangle,

	/// <summary>
	/// Indicates the unique loop (UL) technique.
	/// </summary>
	[Abbreviation("UL")]
	UniqueLoop,

	/// <summary>
	/// Indicates the extended rectangle (XR) technique.
	/// </summary>
	[Abbreviation("XR")]
	ExtendedRectangle,

	/// <summary>
	/// Indicates the bi-value universal grave (BUG) technique.
	/// </summary>
	[Abbreviation("BUG")]
	BivalueUniversalGrave,

	/// <summary>
	/// Indicates the Borescoper's deadly pattern technique.
	/// </summary>
	BorescoperDeadlyPattern,

	/// <summary>
	/// Indicates the Qiu's deadly pattern technique.
	/// </summary>
	QiuDeadlyPattern,

	/// <summary>
	/// Indicates the RW's deadly pattern technique.
	/// </summary>
	RwDeadlyPattern,

	/// <summary>
	/// Indicates the unique matrix technique.
	/// </summary>
	UniqueMatrix,

	/// <summary>
	/// Indicates the reverse bi-value universal grave (Reverse BUG) technique.
	/// </summary>
	ReverseBivalueUniversalGrave,

	/// <summary>
	/// Indicates the uniqueness clue cover.
	/// </summary>
	[Abbreviation("UCC")]
	UniquenessClueCover,

	/// <summary>
	/// Indicates the bi-value oddagon technique.
	/// </summary>
	BivalueOddagon,

	/// <summary>
	/// Indicates the chromatic pattern technique.
	/// </summary>
	ChromaticPattern,

	/// <summary>
	/// Indicates the sue de coq (SdC) technique.
	/// </summary>
	[Abbreviation("SdC")]
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
	/// Indicates the aligned exclusion technique.
	/// </summary>
	AlignedExclusion,

	/// <summary>
	/// Indicates the death blossom technique.
	/// </summary>
	DeathBlossom,

	/// <summary>
	/// Indicates the SK-Loop technique.
	/// </summary>
	DominoLoop,

	/// <summary>
	/// Indicates the multi-sector locked sets (MSLS) technique.
	/// </summary>
	[Abbreviation("MSLS")]
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
	[Abbreviation("POM")]
	PatternOverlay,

	/// <summary>
	/// Indicates the templating technique.
	/// </summary>
	Templating,

	/// <summary>
	/// Indicates the brute force (BF) technique.
	/// </summary>
	[Abbreviation("BF")]
	BruteForce
}
