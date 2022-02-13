namespace Sudoku.Techniques;

/// <summary>
/// Represents a technique instance, which is used for comparison.
/// </summary>
public enum Technique : short
{
	/// <summary>
	/// The placeholder of this enumeration type.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the full house.
	/// </summary>
	[TechniqueName("Full House")]
	FullHouse,

	/// <summary>
	/// Indicates the last digit.
	/// </summary>
	[TechniqueName("Last Digit")]
	LastDigit,

	/// <summary>
	/// Indicates the hidden single (in block).
	/// </summary>
	[TechniqueName("Hidden Single in Block")]
	HiddenSingleBlock,

	/// <summary>
	/// Indicates the hidden single (in row).
	/// </summary>
	[TechniqueName("Hidden Single in Row")]
	HiddenSingleRow,

	/// <summary>
	/// Indicates the hidden single (in column).
	/// </summary>
	[TechniqueName("Hidden Single in Column")]
	HiddenSingleColumn,

	/// <summary>
	/// Indicates the naked single.
	/// </summary>
	[TechniqueName("Naked Single")]
	NakedSingle,

	/// <summary>
	/// Indicates the pointing.
	/// </summary>
	[TechniqueName("Pointing")]
	Pointing,

	/// <summary>
	/// Indicates the claiming.
	/// </summary>
	[TechniqueName("Claiming")]
	Claiming,

	/// <summary>
	/// Indicates the almost locked pair.
	/// </summary>
	[TechniqueName("Almost Locked Pair")]
	AlmostLockedPair,

	/// <summary>
	/// Indicates the almost locked triple.
	/// </summary>
	[TechniqueName("Almost Locked Triple")]
	AlmostLockedTriple,

	/// <summary>
	/// Indicates the almost locked quadruple.
	/// The technique may not be useful because it'll be replaced with Sue de Coq.
	/// </summary>
	[TechniqueName("Almost Locked Quadruple")]
	AlmostLockedQuadruple,

	/// <summary>
	/// Indicates the naked pair.
	/// </summary>
	[TechniqueName("Naked Pair")]
	NakedPair,

	/// <summary>
	/// Indicates the naked pair plus (naked pair (+)).
	/// </summary>
	[TechniqueName("Naked Pair (+)")]
	NakedPairPlus,

	/// <summary>
	/// Indicates the locked pair.
	/// </summary>
	[TechniqueName("Locked Pair")]
	LockedPair,

	/// <summary>
	/// Indicates the hidden pair.
	/// </summary>
	[TechniqueName("Hidden Pair")]
	HiddenPair,

	/// <summary>
	/// Indicates the naked triple.
	/// </summary>
	[TechniqueName("Naked Triple")]
	NakedTriple,

	/// <summary>
	/// Indicates the naked triple plus (naked triple (+)).
	/// </summary>
	[TechniqueName("Naked Triple (+)")]
	NakedTriplePlus,

	/// <summary>
	/// Indicates the locked triple.
	/// </summary>
	[TechniqueName("Locked Triple")]
	LockedTriple,

	/// <summary>
	/// Indicates the hidden triple.
	/// </summary>
	[TechniqueName("Hidden Triple")]
	HiddenTriple,

	/// <summary>
	/// Indicates the naked quadruple.
	/// </summary>
	[TechniqueName("Naked Quadruple")]
	NakedQuadruple,

	/// <summary>
	/// Indicates the naked quadruple plus (naked quadruple (+)).
	/// </summary>
	[TechniqueName("Naked Quadruple (+)")]
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates the hidden quadruple.
	/// </summary>
	[TechniqueName("Hidden Quadruple")]
	HiddenQuadruple,

	/// <summary>
	/// Indicates the X-Wing.
	/// </summary>
	[TechniqueName("X-Wing")]
	XWing,

	/// <summary>
	/// Indicates the finned X-Wing.
	/// </summary>
	[TechniqueName("Finned X-Wing")]
	FinnedXWing,

	/// <summary>
	/// Indicates the sashimi X-Wing.
	/// </summary>
	[TechniqueName("Sashimi X-Wing")]
	SashimiXWing,

	/// <summary>
	/// Indicates the siamese finned X-Wing.
	/// </summary>
	[TechniqueName("Siamese Finned X-Wing")]
	SiameseFinnedXWing,

	/// <summary>
	/// Indicates the siamese sashimi X-Wing.
	/// </summary>
	[TechniqueName("Siamese Sashimi X-Wing")]
	SiameseSashimiXWing,

	/// <summary>
	/// Indicates the franken X-Wing.
	/// </summary>
	[TechniqueName("Franken X-Wing")]
	FrankenXWing,

	/// <summary>
	/// Indicates the finned franken X-Wing.
	/// </summary>
	[TechniqueName("Finned Franken X-Wing")]
	FinnedFrankenXWing,

	/// <summary>
	/// Indicates the sashimi franken X-Wing.
	/// </summary>
	[TechniqueName("Sashimi Franken X-Wing")]
	SashimiFrankenXWing,

	/// <summary>
	/// Indicates the siamese finned franken X-Wing.
	/// </summary>
	[TechniqueName("Siamese Finned Franken X-Wing")]
	SiameseFinnedFrankenXWing,

	/// <summary>
	/// Indicates the siamese sashimi franken X-Wing.
	/// </summary>
	[TechniqueName("Siamese Sashimi Franken X-Wing")]
	SiameseSashimiFrankenXWing,

	/// <summary>
	/// Indicates the mutant X-Wing.
	/// </summary>
	[TechniqueName("Mutant X-Wing")]
	MutantXWing,

	/// <summary>
	/// Indicates the finned mutant X-Wing.
	/// </summary>
	[TechniqueName("Finned Mutant X-Wing")]
	FinnedMutantXWing,

	/// <summary>
	/// Indicates the sashimi mutant X-Wing.
	/// </summary>
	[TechniqueName("Sashimi Mutant X-Wing")]
	SashimiMutantXWing,

	/// <summary>
	/// Indicates the siamese finned mutant X-Wing.
	/// </summary>
	[TechniqueName("Siamese Finned Mutant X-Wing")]
	SiameseFinnedMutantXWing,

	/// <summary>
	/// Indicates the siamese sashimi mutant X-Wing.
	/// </summary>
	[TechniqueName("Siamese Sashimi Mutant X-Wing")]
	SiameseSashimiMutantXWing,

	/// <summary>
	/// Indicates the swordfish.
	/// </summary>
	[TechniqueName("Swordfish")]
	Swordfish,

	/// <summary>
	/// Indicates the finned swordfish.
	/// </summary>
	[TechniqueName("Finned Swordfish")]
	FinnedSwordfish,

	/// <summary>
	/// Indicates the sashimi swordfish.
	/// </summary>
	[TechniqueName("Sashimi Swordfish")]
	SashimiSwordfish,

	/// <summary>
	/// Indicates the siamese finned swordfish.
	/// </summary>
	[TechniqueName("Siamese Finned Swordfish")]
	SiameseFinnedSwordfish,

	/// <summary>
	/// Indicates the siamese sashimi swordfish.
	/// </summary>
	[TechniqueName("Siamese Sashimi Swordfish")]
	SiameseSashimiSwordfish,

	/// <summary>
	/// Indicates the swordfish.
	/// </summary>
	[TechniqueName("Franken Swordfish")]
	FrankenSwordfish,

	/// <summary>
	/// Indicates the finned franken swordfish.
	/// </summary>
	[TechniqueName("Finned Franken Swordfish")]
	FinnedFrankenSwordfish,

	/// <summary>
	/// Indicates the sashimi franken swordfish.
	/// </summary>
	[TechniqueName("Sashimi Franken Swordfish")]
	SashimiFrankenSwordfish,

	/// <summary>
	/// Indicates the siamese finned franken swordfish.
	/// </summary>
	[TechniqueName("Siamese Finned Franken Swordfish")]
	SiameseFinnedFrankenSwordfish,

	/// <summary>
	/// Indicates the siamese sashimi franken swordfish.
	/// </summary>
	[TechniqueName("Siamese Sashimi Franken Swordfish")]
	SiameseSashimiFrankenSwordfish,

	/// <summary>
	/// Indicates the mutant swordfish.
	/// </summary>
	[TechniqueName("Mutant Swordfish")]
	MutantSwordfish,

	/// <summary>
	/// Indicates the finned mutant swordfish.
	/// </summary>
	[TechniqueName("Finned Mutant Swordfish")]
	FinnedMutantSwordfish,

	/// <summary>
	/// Indicates the sashimi mutant swordfish.
	/// </summary>
	[TechniqueName("Sashimi Mutant Swordfish")]
	SashimiMutantSwordfish,

	/// <summary>
	/// Indicates the siamese finned mutant swordfish.
	/// </summary>
	[TechniqueName("Siamese Finned Mutant Swordfish")]
	SiameseFinnedMutantSwordfish,

	/// <summary>
	/// Indicates the siamese sashimi mutant swordfish.
	/// </summary>
	[TechniqueName("Siamese Sashimi Mutant Swordfish")]
	SiameseSashimiMutantSwordfish,

	/// <summary>
	/// Indicates the jellyfish.
	/// </summary>
	[TechniqueName("Jellyfish")]
	Jellyfish,

	/// <summary>
	/// Indicates the finned jellyfish.
	/// </summary>
	[TechniqueName("Finned Jellyfish")]
	FinnedJellyfish,

	/// <summary>
	/// Indicates the sashimi jellyfish.
	/// </summary>
	[TechniqueName("Sashimi Jellyfish")]
	SashimiJellyfish,

	/// <summary>
	/// Indicates the siamese finned jellyfish.
	/// </summary>
	[TechniqueName("Siamese Finned Jellyfish")]
	SiameseFinnedJellyfish,

	/// <summary>
	/// Indicates the siamese sashimi jellyfish.
	/// </summary>
	[TechniqueName("Siamese Sashimi Jellyfish")]
	SiameseSashimiJellyfish,

	/// <summary>
	/// Indicates the franken jellyfish.
	/// </summary>
	[TechniqueName("Franken Jellyfish")]
	FrankenJellyfish,

	/// <summary>
	/// Indicates the finned franken jellyfish.
	/// </summary>
	[TechniqueName("Finned Franken Jellyfish")]
	FinnedFrankenJellyfish,

	/// <summary>
	/// Indicates the sashimi franken jellyfish.
	/// </summary>
	[TechniqueName("Sashimi Franken Jellyfish")]
	SashimiFrankenJellyfish,

	/// <summary>
	/// Indicates the siamese finned franken jellyfish.
	/// </summary>
	[TechniqueName("Siamese Finned Franken Jellyfish")]
	SiameseFinnedFrankenJellyfish,

	/// <summary>
	/// Indicates the siamese sashimi franken jellyfish.
	/// </summary>
	[TechniqueName("Siamese Sashimi Franken Jellyfish")]
	SiameseSashimiFrankenJellyfish,

	/// <summary>
	/// Indicates the mutant jellyfish.
	/// </summary>
	[TechniqueName("Mutant Jellyfish")]
	MutantJellyfish,

	/// <summary>
	/// Indicates the finned mutant jellyfish.
	/// </summary>
	[TechniqueName("Finned Mutant Jellyfish")]
	FinnedMutantJellyfish,

	/// <summary>
	/// Indicates the sashimi mutant jellyfish.
	/// </summary>
	[TechniqueName("Sashimi Mutant Jellyfish")]
	SashimiMutantJellyfish,

	/// <summary>
	/// Indicates the siamese finned mutant jellyfish.
	/// </summary>
	[TechniqueName("Siamese Finned Mutant Jellyfish")]
	SiameseFinnedMutantJellyfish,

	/// <summary>
	/// Indicates the siamese sashimi mutant jellyfish.
	/// </summary>
	[TechniqueName("Siamese Sashimi Mutant Jellyfish")]
	SiameseSashimiMutantJellyfish,

	/// <summary>
	/// Indicates the squirmbag.
	/// </summary>
	[TechniqueName("Squirmbag")]
	Squirmbag,

	/// <summary>
	/// Indicates the finned squirmbag.
	/// </summary>
	[TechniqueName("Finned Squirmbag")]
	FinnedSquirmbag,

	/// <summary>
	/// Indicates the sashimi squirmbag.
	/// </summary>
	[TechniqueName("Sashimi Squirmbag")]
	SashimiSquirmbag,

	/// <summary>
	/// Indicates the siamese finned squirmbag.
	/// </summary>
	[TechniqueName("Siamese Finned Squirmbag")]
	SiameseFinnedSquirmbag,

	/// <summary>
	/// Indicates the siamese sashimi squirmbag.
	/// </summary>
	[TechniqueName("Siamese Sashimi Squirmbag")]
	SiameseSashimiSquirmbag,

	/// <summary>
	/// Indicates the franken squirmbag.
	/// </summary>
	[TechniqueName("Franken Squirmbag")]
	FrankenSquirmbag,

	/// <summary>
	/// Indicates the finned franken squirmbag.
	/// </summary>
	[TechniqueName("Finned Franken Squirmbag")]
	FinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates the sashimi franken squirmbag.
	/// </summary>
	[TechniqueName("Sashimi Franken Squirmbag")]
	SashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates the siamese finned franken squirmbag.
	/// </summary>
	[TechniqueName("Siamese Finned Franken Squirmbag")]
	SiameseFinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates the siamese sashimi franken squirmbag.
	/// </summary>
	[TechniqueName("Siamese Sashimi Franken Squirmbag")]
	SiameseSashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates the mutant squirmbag.
	/// </summary>
	[TechniqueName("Mutant Squirmbag")]
	MutantSquirmbag,

	/// <summary>
	/// Indicates the finned mutant squirmbag.
	/// </summary>
	[TechniqueName("Finned Mutant Squirmbag")]
	FinnedMutantSquirmbag,

	/// <summary>
	/// Indicates the sashimi mutant squirmbag.
	/// </summary>
	[TechniqueName("Sashimi Mutant Squirmbag")]
	SashimiMutantSquirmbag,

	/// <summary>
	/// Indicates the siamese finned mutant squirmbag.
	/// </summary>
	[TechniqueName("Siamese Finned Mutant Squirmbag")]
	SiameseFinnedMutantSquirmbag,

	/// <summary>
	/// Indicates the siamese sashimi mutant squirmbag.
	/// </summary>
	[TechniqueName("Siamese Sashimi Mutant Squirmbag")]
	SiameseSashimiMutantSquirmbag,

	/// <summary>
	/// Indicates the whale.
	/// </summary>
	[TechniqueName("Whale")]
	Whale,

	/// <summary>
	/// Indicates the finned whale.
	/// </summary>
	[TechniqueName("Finned Whale")]
	FinnedWhale,

	/// <summary>
	/// Indicates the sashimi whale.
	/// </summary>
	[TechniqueName("Sashimi Whale")]
	SashimiWhale,

	/// <summary>
	/// Indicates the siamese finned whale.
	/// </summary>
	[TechniqueName("Siamese Finned Whale")]
	SiameseFinnedWhale,

	/// <summary>
	/// Indicates the siamese sashimi whale.
	/// </summary>
	[TechniqueName("Siamese Sashimi Whale")]
	SiameseSashimiWhale,

	/// <summary>
	/// Indicates the franken whale.
	/// </summary>
	[TechniqueName("Franken Whale")]
	FrankenWhale,

	/// <summary>
	/// Indicates the finned franken whale.
	/// </summary>
	[TechniqueName("Finned Franken Whale")]
	FinnedFrankenWhale,

	/// <summary>
	/// Indicates the sashimi franken whale.
	/// </summary>
	[TechniqueName("Sashimi Franken Whale")]
	SashimiFrankenWhale,

	/// <summary>
	/// Indicates the siamese finned franken whale.
	/// </summary>
	[TechniqueName("Siamese Finned Franken Whale")]
	SiameseFinnedFrankenWhale,

	/// <summary>
	/// Indicates the siamese sashimi franken whale.
	/// </summary>
	[TechniqueName("Siamese Sashimi Franken Whale")]
	SiameseSashimiFrankenWhale,

	/// <summary>
	/// Indicates the mutant whale.
	/// </summary>
	[TechniqueName("Mutant Whale")]
	MutantWhale,

	/// <summary>
	/// Indicates the finned mutant whale.
	/// </summary>
	[TechniqueName("Finned Mutant Whale")]
	FinnedMutantWhale,

	/// <summary>
	/// Indicates the sashimi mutant whale.
	/// </summary>
	[TechniqueName("Sashimi Mutant Whale")]
	SashimiMutantWhale,

	/// <summary>
	/// Indicates the siamese finned mutant whale.
	/// </summary>
	[TechniqueName("Siamese Finned Mutant Whale")]
	SiameseFinnedMutantWhale,

	/// <summary>
	/// Indicates the siamese sashimi mutant whale.
	/// </summary>
	[TechniqueName("Siamese Sashimi Mutant Whale")]
	SiameseSashimiMutantWhale,

	/// <summary>
	/// Indicates the leviathan.
	/// </summary>
	[TechniqueName("Leviathan")]
	Leviathan,

	/// <summary>
	/// Indicates the finned leviathan.
	/// </summary>
	[TechniqueName("Finned Leviathan")]
	FinnedLeviathan,

	/// <summary>
	/// Indicates the sashimi leviathan.
	/// </summary>
	[TechniqueName("Sashimi Leviathan")]
	SashimiLeviathan,

	/// <summary>
	/// Indicates the siamese finned leviathan.
	/// </summary>
	[TechniqueName("Siamese Finned Leviathan")]
	SiameseFinnedLeviathan,

	/// <summary>
	/// Indicates the siamese sashimi leviathan.
	/// </summary>
	[TechniqueName("Siamese Sashimi Leviathan")]
	SiameseSashimiLeviathan,

	/// <summary>
	/// Indicates the franken leviathan.
	/// </summary>
	[TechniqueName("Franken Leviathan")]
	FrankenLeviathan,

	/// <summary>
	/// Indicates the finned franken leviathan.
	/// </summary>
	[TechniqueName("Finned Franken Leviathan")]
	FinnedFrankenLeviathan,

	/// <summary>
	/// Indicates the sashimi franken leviathan.
	/// </summary>
	[TechniqueName("Sashimi Franken Leviathan")]
	SashimiFrankenLeviathan,

	/// <summary>
	/// Indicates the siamese finned franken leviathan.
	/// </summary>
	[TechniqueName("Siamese Finned Franken Leviathan")]
	SiameseFinnedFrankenLeviathan,

	/// <summary>
	/// Indicates the siamese sashimi franken leviathan.
	/// </summary>
	[TechniqueName("Siamese Sashimi Franken Leviathan")]
	SiameseSashimiFrankenLeviathan,

	/// <summary>
	/// Indicates the mutant leviathan.
	/// </summary>
	[TechniqueName("Mutant Leviathan")]
	MutantLeviathan,

	/// <summary>
	/// Indicates the finned mutant leviathan.
	/// </summary>
	[TechniqueName("Finned Mutant Leviathan")]
	FinnedMutantLeviathan,

	/// <summary>
	/// Indicates the sashimi mutant leviathan.
	/// </summary>
	[TechniqueName("Sashimi Mutant Leviathan")]
	SashimiMutantLeviathan,

	/// <summary>
	/// Indicates the siamese finned mutant leviathan.
	/// </summary>
	[TechniqueName("Siamese Finned Mutant Leviathan")]
	SiameseFinnedMutantLeviathan,

	/// <summary>
	/// Indicates the siamese sashimi mutant leviathan.
	/// </summary>
	[TechniqueName("Siamese Sashimi Mutant Leviathan")]
	SiameseSashimiMutantLeviathan,

	/// <summary>
	/// Indicates the XY-Wing.
	/// </summary>
	[TechniqueName("XY-Wing")]
	XyWing,

	/// <summary>
	/// Indicates the XYZ-Wing.
	/// </summary>
	[TechniqueName("XYZ-Wing")]
	XyzWing,

	/// <summary>
	/// Indicates the WXYZ-Wing.
	/// </summary>
	[TechniqueName("WXYZ-Wing")]
	WxyzWing,

	/// <summary>
	/// Indicates the VWXYZ-Wing.
	/// </summary>
	[TechniqueName("VWXYZ-Wing")]
	VwxyzWing,

	/// <summary>
	/// Indicates the UVWXYZ-Wing.
	/// </summary>
	[TechniqueName("UVWXYZ-Wing")]
	UvwxyzWing,

	/// <summary>
	/// Indicates the TUVWXYZ-Wing.
	/// </summary>
	[TechniqueName("TUVWXYZ-Wing")]
	TuvwxyzWing,

	/// <summary>
	/// Indicates the STUVWXYZ-Wing.
	/// </summary>
	[TechniqueName("STUVWXYZ-Wing")]
	StuvwxyzWing,

	/// <summary>
	/// Indicates the RSTUVWXYZ-Wing.
	/// </summary>
	[TechniqueName("RSTUVWXYZ-Wing")]
	RstuvwxyzWing,

	/// <summary>
	/// Indicates the incomplete WXYZ-Wing.
	/// </summary>
	[TechniqueName("Incomplete WXYZ-Wing")]
	IncompleteWxyzWing,

	/// <summary>
	/// Indicates the incomplete VWXYZ-Wing.
	/// </summary>
	[TechniqueName("Incomplete VWXYZ-Wing")]
	IncompleteVwxyzWing,

	/// <summary>
	/// Indicates the incomplete UVWXYZ-Wing.
	/// </summary>
	[TechniqueName("Incomplete UVWXYZ-Wing")]
	IncompleteUvwxyzWing,

	/// <summary>
	/// Indicates the incomplete TUVWXYZ-Wing.
	/// </summary>
	[TechniqueName("Incomplete TUVWXYZ-Wing")]
	IncompleteTuvwxyzWing,

	/// <summary>
	/// Indicates the incomplete STUVWXYZ-Wing.
	/// </summary>
	[TechniqueName("Incomplete STUVWXYZ-Wing")]
	IncompleteStuvwxyzWing,

	/// <summary>
	/// Indicates the incomplete RSTUVWXYZ-Wing.
	/// </summary>
	[TechniqueName("Incomplete RSTUVWXYZ-Wing")]
	IncompleteRstuvwxyzWing,

	/// <summary>
	/// Indicates the W-Wing.
	/// </summary>
	[TechniqueName("W-Wing")]
	WWing,

	/// <summary>
	/// Indicates the M-Wing.
	/// </summary>
	[TechniqueName("M-Wing")]
	MWing,

	/// <summary>
	/// Indicates the local wing.
	/// </summary>
	[TechniqueName("Local Wing")]
	LocalWing,

	/// <summary>
	/// Indicates the split wing.
	/// </summary>
	[TechniqueName("Split Wing")]
	SplitWing,

	/// <summary>
	/// Indicates the hybrid wing.
	/// </summary>
	[TechniqueName("Hybrid Wing")]
	HybridWing,

	/// <summary>
	/// Indicates the grouped XY-Wing.
	/// </summary>
	[TechniqueName("Grouped XY-Wing")]
	GroupedXyWing,

	/// <summary>
	/// Indicates the grouped W-Wing.
	/// </summary>
	[TechniqueName("Grouped W-Wing")]
	GroupedWWing,

	/// <summary>
	/// Indicates the grouped M-Wing.
	/// </summary>
	[TechniqueName("Grouped M-Wing")]
	GroupedMWing,

	/// <summary>
	/// Indicates the grouped local wing.
	/// </summary>
	[TechniqueName("Grouped Local Wing")]
	GroupedLocalWing,

	/// <summary>
	/// Indicates the grouped split wing.
	/// </summary>
	[TechniqueName("Grouped Split Wing")]
	GroupedSplitWing,

	/// <summary>
	/// Indicates the grouped hybrid wing.
	/// </summary>
	[TechniqueName("Grouped Hybrid Wing")]
	GroupedHybridWing,

	/// <summary>
	/// Indicates the unique rectangle type 1.
	/// </summary>
	[TechniqueName("Unique Rectangle Type 1")]
	UniqueRectangleType1,

	/// <summary>
	/// Indicates the unique rectangle type 2.
	/// </summary>
	[TechniqueName("Unique Rectangle Type 2")]
	UniqueRectangleType2,

	/// <summary>
	/// Indicates the unique rectangle type 3.
	/// </summary>
	[TechniqueName("Unique Rectangle Type 3")]
	UniqueRectangleType3,

	/// <summary>
	/// Indicates the unique rectangle type 4.
	/// </summary>
	[TechniqueName("Unique Rectangle Type 4")]
	UniqueRectangleType4,

	/// <summary>
	/// Indicates the unique rectangle type 5.
	/// </summary>
	[TechniqueName("Unique Rectangle Type 5")]
	UniqueRectangleType5,

	/// <summary>
	/// Indicates the unique rectangle type 6.
	/// </summary>
	[TechniqueName("Unique Rectangle Type 6")]
	UniqueRectangleType6,

	/// <summary>
	/// Indicates the hidden unique rectangle.
	/// </summary>
	[TechniqueName("Hidden Unique Rectangle")]
	HiddenUniqueRectangle,

	/// <summary>
	/// Indicates the unique rectangle + 2D.
	/// </summary>
	[TechniqueName("Unique Rectangle + 2D")]
	UniqueRectangle2D,

	/// <summary>
	/// Indicates the unique rectangle + 2B / 1SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 2B/1SL")]
	UniqueRectangle2B1,

	/// <summary>
	/// Indicates the unique rectangle + 2D / 1SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 2D/1SL")]
	UniqueRectangle2D1,

	/// <summary>
	/// Indicates the unique rectangle + 3X.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3X")]
	UniqueRectangle3X,

	/// <summary>
	/// Indicates the unique rectangle + 3x / 1SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3x/1SL")]
	UniqueRectangle3X1L,

	/// <summary>
	/// Indicates the unique rectangle + 3X / 1SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3X/1SL")]
	UniqueRectangle3X1U,

	/// <summary>
	/// Indicates the unique rectangle + 3X / 2SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3X/2SL")]
	UniqueRectangle3X2,

	/// <summary>
	/// Indicates the unique rectangle + 3N / 2SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3N/2SL")]
	UniqueRectangle3N2,

	/// <summary>
	/// Indicates the unique rectangle + 3U / 2SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3U/2SL")]
	UniqueRectangle3U2,

	/// <summary>
	/// Indicates the unique rectangle + 3E / 2SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 3E/2SL")]
	UniqueRectangle3E2,

	/// <summary>
	/// Indicates the unique rectangle + 4x / 1SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 4x/1SL")]
	UniqueRectangle4X1L,

	/// <summary>
	/// Indicates the unique rectangle + 4X / 1SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 4X/1SL")]
	UniqueRectangle4X1U,

	/// <summary>
	/// Indicates the unique rectangle + 4x / 2SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 4x/2SL")]
	UniqueRectangle4X2L,

	/// <summary>
	/// Indicates the unique rectangle + 4X / 2SL.
	/// </summary>
	[TechniqueName("Unique Rectangle + 4X/2SL")]
	UniqueRectangle4X2U,

	/// <summary>
	/// Indicates the unique rectangle + 4X / 3SL.
	/// </summary>
	[TechniqueName("Unique Rectangle 4X/3SL")]
	UniqueRectangle4X3,

	/// <summary>
	/// Indicates the unique rectangle + 4C / 3SL.
	/// </summary>
	[TechniqueName("Unique Rectangle 4C/3SL")]
	UniqueRectangle4C3,

	/// <summary>
	/// Indicates the unique rectangle-XY-Wing.
	/// </summary>
	[TechniqueName("Unique Rectangle XY-Wing")]
	UniqueRectangleXyWing,

	/// <summary>
	/// Indicates the unique rectangle-XYZ-Wing.
	/// </summary>
	[TechniqueName("Unique Rectangle XYZ-Wing")]
	UniqueRectangleXyzWing,

	/// <summary>
	/// Indicates the unique rectangle-WXYZ-Wing.
	/// </summary>
	[TechniqueName("Unique Rectangle WXYZ-Wing")]
	UniqueRectangleWxyzWing,

	/// <summary>
	/// Indicates the unique rectangle sue de coq.
	/// </summary>
	[TechniqueName("Unique Rectangle Sue de Coq")]
	UniqueRectangleSueDeCoq,

	/// <summary>
	/// Indicates the unique rectangle unknown covering.
	/// </summary>
	[TechniqueName("Unique Rectangle Unknown Covering")]
	UniqueRectangleUnknownCovering,

	/// <summary>
	/// Indicates the unique rectangle guardian.
	/// </summary>
	[TechniqueName("Unique Rectangle Guardian")]
	UniqueRectangleBrokenWing,

	/// <summary>
	/// Indicates the avoidable rectangle type 1.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Type 1")]
	AvoidableRectangleType1,

	/// <summary>
	/// Indicates the avoidable rectangle type 2.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Type 2")]
	AvoidableRectangleType2,

	/// <summary>
	/// Indicates the avoidable rectangle type 3.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Type 3")]
	AvoidableRectangleType3,

	/// <summary>
	/// Indicates the avoidable rectangle type 5.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Type 5")]
	AvoidableRectangleType5,

	/// <summary>
	/// Indicates the hidden avoidable rectangle.
	/// </summary>
	[TechniqueName("Hidden Avoidable Rectangle")]
	HiddenAvoidableRectangle,

	/// <summary>
	/// Indicates the avoidable rectangle + 2D.
	/// </summary>
	[TechniqueName("Avoidable Rectangle + 2D")]
	AvoidableRectangle2D,

	/// <summary>
	/// Indicates the avoidable rectangle + 3X.
	/// </summary>
	[TechniqueName("Avoidable Rectangle + 3X")]
	AvoidableRectangle3X,

	/// <summary>
	/// Indicates the avoidable rectangle XY-Wing.
	/// </summary>
	[TechniqueName("Avoidable Rectangle XY-Wing")]
	AvoidableRectangleXyWing,

	/// <summary>
	/// Indicates the avoidable rectangle XYZ-Wing.
	/// </summary>
	[TechniqueName("Avoidable Rectangle XYZ-Wing")]
	AvoidableRectangleXyzWing,

	/// <summary>
	/// Indicates the avoidable rectangle WXYZ-Wing.
	/// </summary>
	[TechniqueName("Avoidable Rectangle WXYZ-Wing")]
	AvoidableRectangleWxyzWing,

	/// <summary>
	/// Indicates the avoidable rectangle sue de coq.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Sue de Coq")]
	AvoidableRectangleSueDeCoq,

	/// <summary>
	/// Indicates the avoidable rectangle guardian.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Guardian")]
	AvoidableRectangleBrokenWing,

	/// <summary>
	/// Indicates the avoidable rectangle hidden single in block.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Hidden Single in Block")]
	AvoidableRectangleHiddenSingleBlock,

	/// <summary>
	/// Indicates the avoidable rectangle hidden single in row.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Hidden Single in Row")]
	AvoidableRectangleHiddenSingleRow,

	/// <summary>
	/// Indicates the avoidable rectangle hidden single in column.
	/// </summary>
	[TechniqueName("Avoidable Rectangle Hidden Single in Column")]
	AvoidableRectangleHiddenSingleColumn,

	/// <summary>
	/// Indicates the unique loop type 1.
	/// </summary>
	[TechniqueName("Unique Loop Type 1")]
	UniqueLoopType1,

	/// <summary>
	/// Indicates the unique loop type 2.
	/// </summary>
	[TechniqueName("Unique Loop Type 2")]
	UniqueLoopType2,

	/// <summary>
	/// Indicates the unique loop type 3.
	/// </summary>
	[TechniqueName("Unique Loop Type 3")]
	UniqueLoopType3,

	/// <summary>
	/// Indicates the unique loop type 4.
	/// </summary>
	[TechniqueName("Unique Loop Type 4")]
	UniqueLoopType4,

	/// <summary>
	/// Indicates the extended rectangle type 1.
	/// </summary>
	[TechniqueName("Extended Rectangle Type 1")]
	ExtendedRectangleType1,

	/// <summary>
	/// Indicates the extended rectangle type 2.
	/// </summary>
	[TechniqueName("Extended Rectangle Type 2")]
	ExtendedRectangleType2,

	/// <summary>
	/// Indicates the extended rectangle type 3.
	/// </summary>
	[TechniqueName("Extended Rectangle Type 3")]
	ExtendedRectangleType3,

	/// <summary>
	/// Indicates the extended rectangle type 4.
	/// </summary>
	[TechniqueName("Extended Rectangle Type 4")]
	ExtendedRectangleType4,

	/// <summary>
	/// Indicates the bi-value universal grave type 1.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave Type 1")]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates the bi-value universal grave type 2.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave Type 2")]
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates the bi-value universal grave type 3.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave Type 3")]
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates the bi-value universal grave type 4.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave Type 4")]
	BivalueUniversalGraveType4,

	/// <summary>
	/// Indicates the bi-value universal grave + n.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave + n")]
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Indicates the bi-value universal grave + n with forcing chains.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave + n Forcing Chains")]
	BivalueUniversalGravePlusNForcingChains,

	/// <summary>
	/// Indicates the bi-value universal grave XZ rule.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave XZ Rule")]
	BivalueUniversalGraveXzRule,

	/// <summary>
	/// Indicates the bi-value universal grave XY-Wing.
	/// </summary>
	[TechniqueName("Bi-value Universal Grave XY-Wing")]
	BivalueUniversalGraveXyWing,

	/// <summary>
	/// Indicates the unique polygon type 1.
	/// </summary>
	[TechniqueName("Unique Polygon Type 1")]
	UniquePolygonType1,

	/// <summary>
	/// Indicates the unique polygon type 2.
	/// </summary>
	[TechniqueName("Unique Polygon Type 2")]
	UniquePolygonType2,

	/// <summary>
	/// Indicates the unique polygon type 3.
	/// </summary>
	[TechniqueName("Unique Polygon Type 3")]
	UniquePolygonType3,

	/// <summary>
	/// Indicates the unique polygon type 4.
	/// </summary>
	[TechniqueName("Unique Polygon Type 4")]
	UniquePolygonType4,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 1.
	/// </summary>
	[TechniqueName("Qiu's Deadly Pattern Type 1")]
	QiuDeadlyPatternType1,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 2.
	/// </summary>
	[TechniqueName("Qiu's Deadly Pattern Type 2")]
	QiuDeadlyPatternType2,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 3.
	/// </summary>
	[TechniqueName("Qiu's Deadly Pattern Type 3")]
	QiuDeadlyPatternType3,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 4.
	/// </summary>
	[TechniqueName("Qiu's Deadly Pattern Type 4")]
	QiuDeadlyPatternType4,

	/// <summary>
	/// Indicates the locked Qiu's deadly pattern.
	/// </summary>
	[TechniqueName("Locked Qiu's Deadly Pattern")]
	LockedQiuDeadlyPattern,

	/// <summary>
	/// Indicates the unique square type 1.
	/// </summary>
	[TechniqueName("Unique Square Type 1")]
	UniqueSquareType1,

	/// <summary>
	/// Indicates the unique square type 2.
	/// </summary>
	[TechniqueName("Unique Square Type 2")]
	UniqueSquareType2,

	/// <summary>
	/// Indicates the unique square type 3.
	/// </summary>
	[TechniqueName("Unique Square Type 3")]
	UniqueSquareType3,

	/// <summary>
	/// Indicates the unique square type 4.
	/// </summary>
	[TechniqueName("Unique Square Type 4")]
	UniqueSquareType4,

	/// <summary>
	/// Indicates the sue de coq.
	/// </summary>
	[TechniqueName("Sue de Coq")]
	SueDeCoq,

	/// <summary>
	/// Indicates the sue de coq with isolated digit.
	/// </summary>
	[TechniqueName("Sue de Coq Isolated Digit")]
	SueDeCoqIsolated,

	/// <summary>
	/// Indicates the 3-dimensional sue de coq.
	/// </summary>
	[TechniqueName("Sue de Coq 3 Dimension")]
	SueDeCoq3Dimension,

	/// <summary>
	/// Indicates the sue de coq cannibalism.
	/// </summary>
	[TechniqueName("Sue de Coq Cannibalism")]
	SueDeCoqCannibalism,

	/// <summary>
	/// Indicates the skyscraper.
	/// </summary>
	[TechniqueName("Skyscraper")]
	Skyscraper,

	/// <summary>
	/// Indicates the two-string kite.
	/// </summary>
	[TechniqueName("Two-String Kite")]
	TwoStringKite,

	/// <summary>
	/// Indicates the turbot fish.
	/// </summary>
	[TechniqueName("Turbot Fish")]
	TurbotFish,

	/// <summary>
	/// Indicates the empty rectangle.
	/// </summary>
	[TechniqueName("Empty Rectangle")]
	EmptyRectangle,

	/// <summary>
	/// Indicates the broken wing.
	/// </summary>
	[TechniqueName("Guardian")]
	BrokenWing,

	/// <summary>
	/// Indicates the bi-value oddagon type 1.
	/// </summary>
	[TechniqueName("Bi-value Oddagon Type 1")]
	BivalueOddagonType1,

	/// <summary>
	/// Indicates the bi-value oddagon type 2.
	/// </summary>
	[TechniqueName("Bi-value Oddagon Type 2")]
	BivalueOddagonType2,

	/// <summary>
	/// Indicates the bi-value oddagon type 3.
	/// </summary>
	[TechniqueName("Bi-value Oddagon Type 3")]
	BivalueOddagonType3,

	/// <summary>
	/// Indicates the grouped bi-value oddagon.
	/// </summary>
	[TechniqueName("Grouped Bi-value Oddagon")]
	GroupedBivalueOddagon,

	/// <summary>
	/// Indicates the X-Chain.
	/// </summary>
	[TechniqueName("X-Chain")]
	XChain,

	/// <summary>
	/// Indicates the Y-Chain.
	/// </summary>
	[TechniqueName("Y-Chain")]
	YChain,

	/// <summary>
	/// Indicates the fishy cycle.
	/// </summary>
	[TechniqueName("Fishy Cycle")]
	FishyCycle,

	/// <summary>
	/// Indicates the XY-Chain.
	/// </summary>
	[TechniqueName("XY-Chain")]
	XyChain,

	/// <summary>
	/// Indicates the XY-Cycle.
	/// </summary>
	[TechniqueName("XY-Cycle")]
	XyCycle,

	/// <summary>
	/// Indicates the XY-X-Chain.
	/// </summary>
	[TechniqueName("XY-X-Chain")]
	XyXChain,

	/// <summary>
	/// Indicates the purple cow.
	/// </summary>
	[TechniqueName("Purple Cow")]
	PurpleCow,

	/// <summary>
	/// Indicates the discontinuous nice loop.
	/// </summary>
	[TechniqueName("Discontinuous Nice Loop")]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the continuous nice loop.
	/// </summary>
	[TechniqueName("Continuous Nice Loop")]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates the alternating inference chain.
	/// </summary>
	[TechniqueName("Alternating Inference Chain")]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates the grouped X-Chain.
	/// </summary>
	[TechniqueName("Grouped X-Chain")]
	GroupedXChain,

	/// <summary>
	/// Indicates the grouped fishy cycle.
	/// </summary>
	[TechniqueName("Grouped Fishy Cycle")]
	GroupedFishyCycle,

	/// <summary>
	/// Indicates the grouped XY-Chain.
	/// </summary>
	[TechniqueName("Grouped XY-Chain")]
	GroupedXyChain,

	/// <summary>
	/// Indicates the grouped XY-Cycle.
	/// </summary>
	[TechniqueName("Grouped XY-Cycle")]
	GroupedXyCycle,

	/// <summary>
	/// Indicates the grouped XY-X-Chain.
	/// </summary>
	[TechniqueName("Grouped XY-X-Chain")]
	GroupedXyXChain,

	/// <summary>
	/// Indicates the grouped purple cow.
	/// </summary>
	[TechniqueName("Grouped Purple Cow")]
	GroupedPurpleCow,

	/// <summary>
	/// Indicates the grouped discontinuous nice loop.
	/// </summary>
	[TechniqueName("Grouped Discontinuous Nice Loop")]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the grouped continuous nice loop.
	/// </summary>
	[TechniqueName("Grouped Continuous Nice Loop")]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates the grouped alternating inference chain.
	/// </summary>
	[TechniqueName("Grouped Alternating Inference Chain")]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates the nishio forcing chains.
	/// </summary>
	[TechniqueName("Nishio Forcing Chains")]
	NishioForcingChains,

	/// <summary>
	/// Indicates the region forcing chains.
	/// </summary>
	[TechniqueName("Region Forcing Chains")]
	RegionForcingChains,

	/// <summary>
	/// Indicates the cell forcing chains.
	/// </summary>
	[TechniqueName("Cell Forcing Chains")]
	CellForcingChains,

	/// <summary>
	/// Indicates the dynamic region forcing chains.
	/// </summary>
	[TechniqueName("Dynamic Region Forcing Chains")]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates the dynamic cell forcing chains.
	/// </summary>
	[TechniqueName("Dynamic Cell Forcing Chains")]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates the dynamic contradiction forcing chains.
	/// </summary>
	[TechniqueName("Dynamic Contradiction Forcing Chains")]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates the dynamic double forcing chains.
	/// </summary>
	[TechniqueName("Dynamic Double Forcing Chains")]
	DynamicDoubleForcingChains,

	/// <summary>
	/// Indicates the dynamic forcing chains.
	/// </summary>
	[TechniqueName("Dynamic Forcing Chains")]
	DynamicForcingChains,

	/// <summary>
	/// Indicates the empty rectangle intersection pair.
	/// </summary>
	[TechniqueName("Empty Rectangle Intersection Pair")]
	EmptyRectangleIntersectionPair,

	/// <summary>
	/// Indicates the extended subset principle.
	/// </summary>
	[TechniqueName("Extended Subset Principle")]
	ExtendedSubsetPrinciple,

	/// <summary>
	/// Indicates the singly linked ALS-XZ.
	/// </summary>
	[TechniqueName("Singly-Linked Almost Locked Sets XZ Rule")]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates the doubly linked ALS-XZ.
	/// </summary>
	[TechniqueName("Doubly-Linked Almost Locked Sets XZ Rule")]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates the ALS-XY-Wing.
	/// </summary>
	[TechniqueName("Almost Locked Sets XY-Wing")]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates the ALS-W-Wing.
	/// </summary>
	[TechniqueName("Almost Locked Sets W-Wing")]
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates the death blossom.
	/// </summary>
	[TechniqueName("Death Blossom")]
	DeathBlossom,

	/// <summary>
	/// Indicates the Gurth's symmetrical placement.
	/// </summary>
	[TechniqueName("Gurth's Symmetrical Placement")]
	GurthSymmetricalPlacement,

	/// <summary>
	/// Indicates the extended Gurth's symmetrical placement.
	/// </summary>
	[TechniqueName("Extended Gurth's Symmetrical Placement")]
	ExtendedGurthSymmetricalPlacement,

	/// <summary>
	/// Indicates the junior exocet.
	/// </summary>
	[TechniqueName("Junior Exocet")]
	JuniorExocet,

	/// <summary>
	/// Indicates the senior exocet.
	/// </summary>
	[TechniqueName("Senior Exocet")]
	SeniorExocet,

	/// <summary>
	/// Indicates the complex senior exocet.
	/// </summary>
	[TechniqueName("Complex Senior Exocet")]
	ComplexSeniorExocet,

	/// <summary>
	/// Indicates the siamese junior exocet.
	/// </summary>
	[TechniqueName("Siamese Junior Exocet")]
	SiameseJuniorExocet,

	/// <summary>
	/// Indicates the siamese senior exocet.
	/// </summary>
	[TechniqueName("Siamese Senior Exocet")]
	SiameseSeniorExocet,

	/// <summary>
	/// Indicates the domino loop.
	/// </summary>
	[TechniqueName("Domino Loop")]
	DominoLoop,

	/// <summary>
	/// Indicates the multi-sector locked sets.
	/// </summary>
	[TechniqueName("Multi-sector Locked Sets")]
	MultisectorLockedSets,

	/// <summary>
	/// Indicates the pattern overlay method.
	/// </summary>
	[TechniqueName("Pattern Overlay")]
	PatternOverlay,

	/// <summary>
	/// Indicates the template set.
	/// </summary>
	[TechniqueName("Template Set")]
	TemplateSet,

	/// <summary>
	/// Indicates the template delete.
	/// </summary>
	[TechniqueName("Template Delete")]
	TemplateDelete,

	/// <summary>
	/// Indicates the bowman's bingo.
	/// </summary>
	[TechniqueName("Bowman Bingo")]
	BowmanBingo,

	/// <summary>
	/// Indicates the brute force.
	/// </summary>
	[TechniqueName("Brute Force")]
	BruteForce,
}
