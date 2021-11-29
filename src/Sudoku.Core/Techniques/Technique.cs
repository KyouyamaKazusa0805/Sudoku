#pragma warning disable CS0618

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
	FullHouse,

	/// <summary>
	/// Indicates the last digit.
	/// </summary>
	LastDigit,

	/// <summary>
	/// Indicates the hidden single (in block).
	/// </summary>
	HiddenSingleBlock,

	/// <summary>
	/// Indicates the hidden single (in row).
	/// </summary>
	HiddenSingleRow,

	/// <summary>
	/// Indicates the hidden single (in column).
	/// </summary>
	HiddenSingleColumn,

	/// <summary>
	/// Indicates the naked single.
	/// </summary>
	NakedSingle,

	/// <summary>
	/// Indicates the pointing.
	/// </summary>
	Pointing,

	/// <summary>
	/// Indicates the claiming.
	/// </summary>
	Claiming,

	/// <summary>
	/// Indicates the ALP.
	/// </summary>
	AlmostLockedPair,

	/// <summary>
	/// Indicates the ALT.
	/// </summary>
	AlmostLockedTriple,

	/// <summary>
	/// Indicates the ALQ.
	/// </summary>
	AlmostLockedQuadruple,

	/// <summary>
	/// Indicates the naked pair.
	/// </summary>
	NakedPair,

	/// <summary>
	/// Indicates the naked pair plus (naked pair (+)).
	/// </summary>
	NakedPairPlus,

	/// <summary>
	/// Indicates the locked pair.
	/// </summary>
	LockedPair,

	/// <summary>
	/// Indicates the hidden pair.
	/// </summary>
	HiddenPair,

	/// <summary>
	/// Indicates the naked triple.
	/// </summary>
	NakedTriple,

	/// <summary>
	/// Indicates the naked triple plus (naked triple (+)).
	/// </summary>
	NakedTriplePlus,

	/// <summary>
	/// Indicates the locked triple.
	/// </summary>
	LockedTriple,

	/// <summary>
	/// Indicates the hidden triple.
	/// </summary>
	HiddenTriple,

	/// <summary>
	/// Indicates the naked quadruple.
	/// </summary>
	NakedQuadruple,

	/// <summary>
	/// Indicates the naked quadruple plus (naked quadruple (+)).
	/// </summary>
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates the hidden quadruple.
	/// </summary>
	HiddenQuadruple,

	/// <summary>
	/// Indicates the X-Wing.
	/// </summary>
	XWing,

	/// <summary>
	/// Indicates the finned X-Wing.
	/// </summary>
	FinnedXWing,

	/// <summary>
	/// Indicates the sashimi X-Wing.
	/// </summary>
	SashimiXWing,

	/// <summary>
	/// Indicates the siamese finned X-Wing.
	/// </summary>
	SiameseFinnedXWing,

	/// <summary>
	/// Indicates the siamese sashimi X-Wing.
	/// </summary>
	SiameseSashimiXWing,

	/// <summary>
	/// Indicates the franken X-Wing.
	/// </summary>
	FrankenXWing,

	/// <summary>
	/// Indicates the finned franken X-Wing.
	/// </summary>
	FinnedFrankenXWing,

	/// <summary>
	/// Indicates the sashimi franken X-Wing.
	/// </summary>
	SashimiFrankenXWing,

	/// <summary>
	/// Indicates the siamese finned franken X-Wing.
	/// </summary>
	SiameseFinnedFrankenXWing,

	/// <summary>
	/// Indicates the siamese sashimi franken X-Wing.
	/// </summary>
	SiameseSashimiFrankenXWing,

	/// <summary>
	/// Indicates the mutant X-Wing.
	/// </summary>
	MutantXWing,

	/// <summary>
	/// Indicates the finned mutant X-Wing.
	/// </summary>
	FinnedMutantXWing,

	/// <summary>
	/// Indicates the sashimi mutant X-Wing.
	/// </summary>
	SashimiMutantXWing,

	/// <summary>
	/// Indicates the siamese finned mutant X-Wing.
	/// </summary>
	SiameseFinnedMutantXWing,

	/// <summary>
	/// Indicates the siamese sashimi mutant X-Wing.
	/// </summary>
	SiameseSashimiMutantXWing,

	/// <summary>
	/// Indicates the swordfish.
	/// </summary>
	Swordfish,

	/// <summary>
	/// Indicates the finned swordfish.
	/// </summary>
	FinnedSwordfish,

	/// <summary>
	/// Indicates the sashimi swordfish.
	/// </summary>
	SashimiSwordfish,

	/// <summary>
	/// Indicates the siamese finned swordfish.
	/// </summary>
	SiameseFinnedSwordfish,

	/// <summary>
	/// Indicates the siamese sashimi swordfish.
	/// </summary>
	SiameseSashimiSwordfish,

	/// <summary>
	/// Indicates the swordfish.
	/// </summary>
	FrankenSwordfish,

	/// <summary>
	/// Indicates the finned franken swordfish.
	/// </summary>
	FinnedFrankenSwordfish,

	/// <summary>
	/// Indicates the sashimi franken swordfish.
	/// </summary>
	SashimiFrankenSwordfish,

	/// <summary>
	/// Indicates the siamese finned franken swordfish.
	/// </summary>
	SiameseFinnedFrankenSwordfish,

	/// <summary>
	/// Indicates the siamese sashimi franken swordfish.
	/// </summary>
	SiameseSashimiFrankenSwordfish,

	/// <summary>
	/// Indicates the mutant swordfish.
	/// </summary>
	MutantSwordfish,

	/// <summary>
	/// Indicates the finned mutant swordfish.
	/// </summary>
	FinnedMutantSwordfish,

	/// <summary>
	/// Indicates the sashimi mutant swordfish.
	/// </summary>
	SashimiMutantSwordfish,

	/// <summary>
	/// Indicates the siamese finned mutant swordfish.
	/// </summary>
	SiameseFinnedMutantSwordfish,

	/// <summary>
	/// Indicates the siamese sashimi mutant swordfish.
	/// </summary>
	SiameseSashimiMutantSwordfish,

	/// <summary>
	/// Indicates the jellyfish.
	/// </summary>
	Jellyfish,

	/// <summary>
	/// Indicates the finned jellyfish.
	/// </summary>
	FinnedJellyfish,

	/// <summary>
	/// Indicates the sashimi jellyfish.
	/// </summary>
	SashimiJellyfish,

	/// <summary>
	/// Indicates the siamese finned jellyfish.
	/// </summary>
	SiameseFinnedJellyfish,

	/// <summary>
	/// Indicates the siamese sashimi jellyfish.
	/// </summary>
	SiameseSashimiJellyfish,

	/// <summary>
	/// Indicates the franken jellyfish.
	/// </summary>
	FrankenJellyfish,

	/// <summary>
	/// Indicates the finned franken jellyfish.
	/// </summary>
	FinnedFrankenJellyfish,

	/// <summary>
	/// Indicates the sashimi franken jellyfish.
	/// </summary>
	SashimiFrankenJellyfish,

	/// <summary>
	/// Indicates the siamese finned franken jellyfish.
	/// </summary>
	SiameseFinnedFrankenJellyfish,

	/// <summary>
	/// Indicates the siamese sashimi franken jellyfish.
	/// </summary>
	SiameseSashimiFrankenJellyfish,

	/// <summary>
	/// Indicates the mutant jellyfish.
	/// </summary>
	MutantJellyfish,

	/// <summary>
	/// Indicates the finned mutant jellyfish.
	/// </summary>
	FinnedMutantJellyfish,

	/// <summary>
	/// Indicates the sashimi mutant jellyfish.
	/// </summary>
	SashimiMutantJellyfish,

	/// <summary>
	/// Indicates the siamese finned mutant jellyfish.
	/// </summary>
	SiameseFinnedMutantJellyfish,

	/// <summary>
	/// Indicates the siamese sashimi mutant jellyfish.
	/// </summary>
	SiameseSashimiMutantJellyfish,

	/// <summary>
	/// Indicates the squirmbag.
	/// </summary>
	Squirmbag,

	/// <summary>
	/// Indicates the finned squirmbag.
	/// </summary>
	FinnedSquirmbag,

	/// <summary>
	/// Indicates the sashimi squirmbag.
	/// </summary>
	SashimiSquirmbag,

	/// <summary>
	/// Indicates the siamese finned squirmbag.
	/// </summary>
	SiameseFinnedSquirmbag,

	/// <summary>
	/// Indicates the siamese sashimi squirmbag.
	/// </summary>
	SiameseSashimiSquirmbag,

	/// <summary>
	/// Indicates the franken squirmbag.
	/// </summary>
	FrankenSquirmbag,

	/// <summary>
	/// Indicates the finned franken squirmbag.
	/// </summary>
	FinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates the sashimi franken squirmbag.
	/// </summary>
	SashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates the siamese finned franken squirmbag.
	/// </summary>
	SiameseFinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates the siamese sashimi franken squirmbag.
	/// </summary>
	SiameseSashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates the mutant squirmbag.
	/// </summary>
	MutantSquirmbag,

	/// <summary>
	/// Indicates the finned mutant squirmbag.
	/// </summary>
	FinnedMutantSquirmbag,

	/// <summary>
	/// Indicates the sashimi mutant squirmbag.
	/// </summary>
	SashimiMutantSquirmbag,

	/// <summary>
	/// Indicates the siamese finned mutant squirmbag.
	/// </summary>
	SiameseFinnedMutantSquirmbag,

	/// <summary>
	/// Indicates the siamese sashimi mutant squirmbag.
	/// </summary>
	SiameseSashimiMutantSquirmbag,

	/// <summary>
	/// Indicates the whale.
	/// </summary>
	Whale,

	/// <summary>
	/// Indicates the finned whale.
	/// </summary>
	FinnedWhale,

	/// <summary>
	/// Indicates the sashimi whale.
	/// </summary>
	SashimiWhale,

	/// <summary>
	/// Indicates the siamese finned whale.
	/// </summary>
	SiameseFinnedWhale,

	/// <summary>
	/// Indicates the siamese sashimi whale.
	/// </summary>
	SiameseSashimiWhale,

	/// <summary>
	/// Indicates the franken whale.
	/// </summary>
	FrankenWhale,

	/// <summary>
	/// Indicates the finned franken whale.
	/// </summary>
	FinnedFrankenWhale,

	/// <summary>
	/// Indicates the sashimi franken whale.
	/// </summary>
	SashimiFrankenWhale,

	/// <summary>
	/// Indicates the siamese finned franken whale.
	/// </summary>
	SiameseFinnedFrankenWhale,

	/// <summary>
	/// Indicates the siamese sashimi franken whale.
	/// </summary>
	SiameseSashimiFrankenWhale,

	/// <summary>
	/// Indicates the mutant whale.
	/// </summary>
	MutantWhale,

	/// <summary>
	/// Indicates the finned mutant whale.
	/// </summary>
	FinnedMutantWhale,

	/// <summary>
	/// Indicates the sashimi mutant whale.
	/// </summary>
	SashimiMutantWhale,

	/// <summary>
	/// Indicates the siamese finned mutant whale.
	/// </summary>
	SiameseFinnedMutantWhale,

	/// <summary>
	/// Indicates the siamese sashimi mutant whale.
	/// </summary>
	SiameseSashimiMutantWhale,

	/// <summary>
	/// Indicates the leviathan.
	/// </summary>
	Leviathan,

	/// <summary>
	/// Indicates the finned leviathan.
	/// </summary>
	FinnedLeviathan,

	/// <summary>
	/// Indicates the sashimi leviathan.
	/// </summary>
	SashimiLeviathan,

	/// <summary>
	/// Indicates the siamese finned leviathan.
	/// </summary>
	SiameseFinnedLeviathan,

	/// <summary>
	/// Indicates the siamese sashimi leviathan.
	/// </summary>
	SiameseSashimiLeviathan,

	/// <summary>
	/// Indicates the franken leviathan.
	/// </summary>
	FrankenLeviathan,

	/// <summary>
	/// Indicates the finned franken leviathan.
	/// </summary>
	FinnedFrankenLeviathan,

	/// <summary>
	/// Indicates the sashimi franken leviathan.
	/// </summary>
	SashimiFrankenLeviathan,

	/// <summary>
	/// Indicates the siamese finned franken leviathan.
	/// </summary>
	SiameseFinnedFrankenLeviathan,

	/// <summary>
	/// Indicates the siamese sashimi franken leviathan.
	/// </summary>
	SiameseSashimiFrankenLeviathan,

	/// <summary>
	/// Indicates the mutant leviathan.
	/// </summary>
	MutantLeviathan,

	/// <summary>
	/// Indicates the finned mutant leviathan.
	/// </summary>
	FinnedMutantLeviathan,

	/// <summary>
	/// Indicates the sashimi mutant leviathan.
	/// </summary>
	SashimiMutantLeviathan,

	/// <summary>
	/// Indicates the siamese finned mutant leviathan.
	/// </summary>
	SiameseFinnedMutantLeviathan,

	/// <summary>
	/// Indicates the siamese sashimi mutant leviathan.
	/// </summary>
	SiameseSashimiMutantLeviathan,

	/// <summary>
	/// Indicates the XY-Wing.
	/// </summary>
	XyWing,

	/// <summary>
	/// Indicates the XYZ-Wing.
	/// </summary>
	XyzWing,

	/// <summary>
	/// Indicates the WXYZ-Wing.
	/// </summary>
	WxyzWing,

	/// <summary>
	/// Indicates the VWXYZ-Wing.
	/// </summary>
	VwxyzWing,

	/// <summary>
	/// Indicates the UVWXYZ-Wing.
	/// </summary>
	UvwxyzWing,

	/// <summary>
	/// Indicates the TUVWXYZ-Wing.
	/// </summary>
	TuvwxyzWing,

	/// <summary>
	/// Indicates the STUVWXYZ-Wing.
	/// </summary>
	StuvwxyzWing,

	/// <summary>
	/// Indicates the RSTUVWXYZ-Wing.
	/// </summary>
	RstuvwxyzWing,

	/// <summary>
	/// Indicates the incomplete WXYZ-Wing.
	/// </summary>
	IncompleteWxyzWing,

	/// <summary>
	/// Indicates the incomplete VWXYZ-Wing.
	/// </summary>
	IncompleteVwxyzWing,

	/// <summary>
	/// Indicates the incomplete UVWXYZ-Wing.
	/// </summary>
	IncompleteUvwxyzWing,

	/// <summary>
	/// Indicates the incomplete TUVWXYZ-Wing.
	/// </summary>
	IncompleteTuvwxyzWing,

	/// <summary>
	/// Indicates the incomplete STUVWXYZ-Wing.
	/// </summary>
	IncompleteStuvwxyzWing,

	/// <summary>
	/// Indicates the incomplete RSTUVWXYZ-Wing.
	/// </summary>
	IncompleteRstuvwxyzWing,

	/// <summary>
	/// Indicates the W-Wing.
	/// </summary>
	WWing,

	/// <summary>
	/// Indicates the M-Wing.
	/// </summary>
	MWing,

	/// <summary>
	/// Indicates the local wing.
	/// </summary>
	LocalWing,

	/// <summary>
	/// Indicates the split wing.
	/// </summary>
	SplitWing,

	/// <summary>
	/// Indicates the hybrid wing.
	/// </summary>
	HybridWing,

	/// <summary>
	/// Indicates the grouped XY-Wing.
	/// </summary>
	GroupedXyWing,

	/// <summary>
	/// Indicates the grouped W-Wing.
	/// </summary>
	GroupedWWing,

	/// <summary>
	/// Indicates the grouped M-Wing.
	/// </summary>
	GroupedMWing,

	/// <summary>
	/// Indicates the grouped local wing.
	/// </summary>
	GroupedLocalWing,

	/// <summary>
	/// Indicates the grouped split wing.
	/// </summary>
	GroupedSplitWing,

	/// <summary>
	/// Indicates the grouped hybrid wing.
	/// </summary>
	GroupedHybridWing,

	/// <summary>
	/// Indicates the UR type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrType1,

	/// <summary>
	/// Indicates the unique rectangle type 1.
	/// </summary>
	UniqueRectangleType1 = UrType1,

	/// <summary>
	/// Indicates the UR type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrType2,

	/// <summary>
	/// Indicates the unique rectangle type 2.
	/// </summary>
	UniqueRectangleType2 = UrType2,

	/// <summary>
	/// Indicates the UR type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrType3,

	/// <summary>
	/// Indicates the unique rectangle type 3.
	/// </summary>
	UniqueRectangleType3 = UrType3,

	/// <summary>
	/// Indicates the UR type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrType4,

	/// <summary>
	/// Indicates the unique rectangle type 4.
	/// </summary>
	UniqueRectangleType4 = UrType4,

	/// <summary>
	/// Indicates the UR type 5.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrType5,

	/// <summary>
	/// Indicates the unique rectangle type 5.
	/// </summary>
	UniqueRectangleType5 = UrType5,

	/// <summary>
	/// Indicates the UR type 6.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrType6,

	/// <summary>
	/// Indicates the unique rectangle type 6.
	/// </summary>
	UniqueRectangleType6 = UrType6,

	/// <summary>
	/// Indicates the hidden UR.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	HiddenUr,

	/// <summary>
	/// Indicates the hidden unique rectangle.
	/// </summary>
	HiddenUniqueRectangle = HiddenUr,

	/// <summary>
	/// Indicates the UR + 2D.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus2D,

	/// <summary>
	/// Indicates the UR + 2D.
	/// </summary>
	UniqueRectangle2D = UrPlus2D,

	/// <summary>
	/// Indicates the UR + 2B / 1SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus2B1SL,

	/// <summary>
	/// Indicates the UR + 2B / 1SL.
	/// </summary>
	UniqueRectangle2B1 = UrPlus2B1SL,

	/// <summary>
	/// Indicates the UR + 2D / 1SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus2D1SL,

	/// <summary>
	/// Indicates the UR + 2D / 1SL.
	/// </summary>
	UniqueRectangle2D1 = UrPlus2D1SL,

	/// <summary>
	/// Indicates the UR + 3X.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3X,

	/// <summary>
	/// Indicates the UR + 3X.
	/// </summary>
	UniqueRectangle3X = UrPlus3X,

	/// <summary>
	/// Indicates the UR + 3x / 1SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3x1SL_Lower,

	/// <summary>
	/// Indicates the UR + 3x / 1SL.
	/// </summary>
	UniqueRectangle3X1L = UrPlus3x1SL_Lower,

	/// <summary>
	/// Indicates the UR + 3X / 1SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3X1SL_Upper,

	/// <summary>
	/// Indicates the UR + 3X / 1SL.
	/// </summary>
	UniqueRectangle3X1U = UrPlus3X1SL_Upper,

	/// <summary>
	/// Indicates the UR + 3X / 2SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3X2SL,

	/// <summary>
	/// Indicates the UR + 3X / 2SL.
	/// </summary>
	UniqueRectangle3X2 = UrPlus3X2SL,

	/// <summary>
	/// Indicates the UR + 3N / 2SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3N2SL,

	/// <summary>
	/// Indicates the UR + 3N / 2SL.
	/// </summary>
	UniqueRectangle3N2 = UrPlus3N2SL,

	/// <summary>
	/// Indicates the UR + 3U / 2SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3U2SL,

	/// <summary>
	/// Indicates the UR + 3U / 2SL.
	/// </summary>
	UniqueRectangle3U2 = UrPlus3U2SL,

	/// <summary>
	/// Indicates the UR + 3E / 2SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus3E2SL,

	/// <summary>
	/// Indicates the UR + 3E / 2SL.
	/// </summary>
	UniqueRectangle3E2 = UrPlus3E2SL,

	/// <summary>
	/// Indicates the UR + 4x / 1SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus4x1SL_Lower,

	/// <summary>
	/// Indicates the UR + 4x / 1SL.
	/// </summary>
	UniqueRectangle4X1L = UrPlus4x1SL_Lower,

	/// <summary>
	/// Indicates the UR + 4X / 1SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus4X1SL_Upper,

	/// <summary>
	/// Indicates the UR + 4X / 1SL.
	/// </summary>
	UniqueRectangle4X1U = UrPlus4X1SL_Upper,

	/// <summary>
	/// Indicates the UR + 4x / 2SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus4x2SL_Lower,

	/// <summary>
	/// Indicates the UR + 4x / 2SL.
	/// </summary>
	UniqueRectangle4X2L = UrPlus4x2SL_Lower,

	/// <summary>
	/// Indicates the UR + 4X / 2SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus4X2SL_Upper,

	/// <summary>
	/// Indicates the UR + 4X / 2SL.
	/// </summary>
	UniqueRectangle4X2U = UrPlus4X2SL_Upper,

	/// <summary>
	/// Indicates the UR + 4X / 3SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus4X3SL,

	/// <summary>
	/// Indicates the UR + 4X / 3SL.
	/// </summary>
	UniqueRectangle4X3 = UrPlus4X3SL,

	/// <summary>
	/// Indicates the UR + 4C / 3SL.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrPlus4C3SL,

	/// <summary>
	/// Indicates the UR + 4C / 3SL.
	/// </summary>
	UniqueRectangle4C3 = UrPlus4C3SL,

	/// <summary>
	/// Indicates the UR-XY-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrXyWing,

	/// <summary>
	/// Indicates the UR-XY-Wing.
	/// </summary>
	UniqueRectangleXyWing = UrXyWing,

	/// <summary>
	/// Indicates the UR-XYZ-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrXyzWing,

	/// <summary>
	/// Indicates the UR-XYZ-Wing.
	/// </summary>
	UniqueRectangleXyzWing = UrXyzWing,

	/// <summary>
	/// Indicates the UR-WXYZ-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrWxyzWing,

	/// <summary>
	/// Indicates the UR-WXYZ-Wing.
	/// </summary>
	UniqueRectangleWxyzWing = UrWxyzWing,

	/// <summary>
	/// Indicates the UR sue de coq.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrSdc,

	/// <summary>
	/// Indicates the UR sue de coq.
	/// </summary>
	UniqueRectangleSueDeCoq = UrSdc,

	/// <summary>
	/// Indicates the UR unknown covering.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrUnknownCovering,

	/// <summary>
	/// Indicates the UR unknown covering.
	/// </summary>
	UniqueRectangleUnknownCovering = UrUnknownCovering,

	/// <summary>
	/// Indicates the UR guardian.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UrGuardian,

	/// <summary>
	/// Indicates the UR guardian.
	/// </summary>
	UniqueRectangleBrokenWing = UrGuardian,

	/// <summary>
	/// Indicates the AR type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArType1,

	/// <summary>
	/// Indicates the AR type 1.
	/// </summary>
	AvoidableRectangleType1 = ArType1,

	/// <summary>
	/// Indicates the AR type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArType2,

	/// <summary>
	/// Indicates the AR type 2.
	/// </summary>
	AvoidableRectangleType2 = ArType2,

	/// <summary>
	/// Indicates the AR type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArType3,

	/// <summary>
	/// Indicates the AR type 3.
	/// </summary>
	AvoidableRectangleType3 = ArType3,

	/// <summary>
	/// Indicates the AR type 5.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArType5,

	/// <summary>
	/// Indicates the AR type 5.
	/// </summary>
	AvoidableRectangleType5 = ArType5,

	/// <summary>
	/// Indicates the hidden AR.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	HiddenAr,

	/// <summary>
	/// Indicates the hidden AR.
	/// </summary>
	HiddenAvoidableRectangle = HiddenAr,

	/// <summary>
	/// Indicates the AR + 2D.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArPlus2D,

	/// <summary>
	/// Indicates the AR + 2D.
	/// </summary>
	AvoidableRectangle2D = ArPlus2D,

	/// <summary>
	/// Indicates the AR + 3X.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArPlus3X,

	/// <summary>
	/// Indicates the AR + 3X.
	/// </summary>
	AvoidableRectangle3X = ArPlus3X,

	/// <summary>
	/// Indicates the AR-XY-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArXyWing,

	/// <summary>
	/// Indicates the AR-XY-Wing.
	/// </summary>
	AvoidableRectangleXyWing = ArXyWing,

	/// <summary>
	/// Indicates the AR-XYZ-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArXyzWing,

	/// <summary>
	/// Indicates the AR-XYZ-Wing.
	/// </summary>
	AvoidableRectangleXyzWing = ArXyzWing,

	/// <summary>
	/// Indicates the AR-WXYZ-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArWxyzWing,

	/// <summary>
	/// Indicates the AR-WXYZ-Wing.
	/// </summary>
	AvoidableRectangleWxyzWing = ArWxyzWing,

	/// <summary>
	/// Indicates the AR sue de coq.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArSdc,

	/// <summary>
	/// Indicates the AR sue de coq.
	/// </summary>
	AvoidableRectangleSueDeCoq = ArSdc,

	/// <summary>
	/// Indicates the AR guardian.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArGuardian,

	/// <summary>
	/// Indicates the AR guardian.
	/// </summary>
	AvoidableRectangleBrokenWing = ArGuardian,

	/// <summary>
	/// Indicates the AR hidden single in block.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArHiddenSingleBlock,

	/// <summary>
	/// Indicates the AR hidden single in block.
	/// </summary>
	AvoidableRectangleHiddenSingleBlock = ArHiddenSingleBlock,

	/// <summary>
	/// Indicates the AR hidden single in row.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArHiddenSingleRow,

	/// <summary>
	/// Indicates the AR hidden single in row.
	/// </summary>
	AvoidableRectangleHiddenSingleRow = ArHiddenSingleRow,

	/// <summary>
	/// Indicates the AR hidden single in column.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ArHiddenSingleColumn,

	/// <summary>
	/// Indicates the AR hidden single in column.
	/// </summary>
	AvoidableRectangleHiddenSingleColumn = ArHiddenSingleColumn,

	/// <summary>
	/// Indicates the UL type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UlType1,

	/// <summary>
	/// Indicates the UL type 1.
	/// </summary>
	UniqueLoopType1 = UlType1,

	/// <summary>
	/// Indicates the UL type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UlType2,

	/// <summary>
	/// Indicates the UL type 2.
	/// </summary>
	UniqueLoopType2 = UlType2,

	/// <summary>
	/// Indicates the UL type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UlType3,

	/// <summary>
	/// Indicates the UL type 3.
	/// </summary>
	UniqueLoopType3 = UlType3,

	/// <summary>
	/// Indicates the UL type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UlType4,

	/// <summary>
	/// Indicates the UL type 4.
	/// </summary>
	UniqueLoopType4 = UlType4,

	/// <summary>
	/// Indicates the XR type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	XrType1,

	/// <summary>
	/// Indicates the XR type 1.
	/// </summary>
	ExtendedRectangleType1 = XrType1,

	/// <summary>
	/// Indicates the XR type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	XrType2,

	/// <summary>
	/// Indicates the XR type 2.
	/// </summary>
	ExtendedRectangleType2 = XrType2,

	/// <summary>
	/// Indicates the XR type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	XrType3,

	/// <summary>
	/// Indicates the XR type 3.
	/// </summary>
	ExtendedRectangleType3 = XrType3,

	/// <summary>
	/// Indicates the XR type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	XrType4,

	/// <summary>
	/// Indicates the XR type 4.
	/// </summary>
	ExtendedRectangleType4 = XrType4,

	/// <summary>
	/// Indicates the BUG type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugType1,

	/// <summary>
	/// Indicates the BUG type 1.
	/// </summary>
	BivalueUniversalGraveType1 = BugType1,

	/// <summary>
	/// Indicates the BUG type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugType2,

	/// <summary>
	/// Indicates the BUG type 2.
	/// </summary>
	BivalueUniversalGraveType2 = BugType2,

	/// <summary>
	/// Indicates the BUG type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugType3,

	/// <summary>
	/// Indicates the BUG type 3.
	/// </summary>
	BivalueUniversalGraveType3 = BugType3,

	/// <summary>
	/// Indicates the BUG type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugType4,

	/// <summary>
	/// Indicates the BUG type 4.
	/// </summary>
	BivalueUniversalGraveType4 = BugType4,

	/// <summary>
	/// Indicates the BUG + n.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugMultiple,

	/// <summary>
	/// Indicates the BUG + n.
	/// </summary>
	BivalueUniversalGravePlusN = BugMultiple,

	/// <summary>
	/// Indicates the BUG + n with forcing chains.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugMultipleFc,

	/// <summary>
	/// Indicates the BUG + n with forcing chains.
	/// </summary>
	BivalueUniversalGravePlusNForcingChains = BugMultipleFc,

	/// <summary>
	/// Indicates the BUG-XZ.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugXz,

	/// <summary>
	/// Indicates the BUG-XZ.
	/// </summary>
	BivalueUniversalGraveXzRule = BugXz,

	/// <summary>
	/// Indicates the BUG-XY-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BugXyWing,

	/// <summary>
	/// Indicates the BUG-XY-Wing.
	/// </summary>
	BivalueUniversalGraveXyWing = BugXyWing,

	/// <summary>
	/// Indicates the BDP type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BdpType1,

	/// <summary>
	/// Indicates the unique polygon type 1.
	/// </summary>
	UniquePolygonType1 = BdpType1,

	/// <summary>
	/// Indicates the BDP type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BdpType2,

	/// <summary>
	/// Indicates the unique polygon type 2.
	/// </summary>
	UniquePolygonType2 = BdpType2,

	/// <summary>
	/// Indicates the BDP type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BdpType3,

	/// <summary>
	/// Indicates the unique polygon type 3.
	/// </summary>
	UniquePolygonType3 = BdpType3,

	/// <summary>
	/// Indicates the BDP type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	BdpType4,

	/// <summary>
	/// Indicates the unique polygon type 4.
	/// </summary>
	UniquePolygonType4 = BdpType4,

	/// <summary>
	/// Indicates the QDP type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	QdpType1,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 1.
	/// </summary>
	QiuDeadlyPatternType1 = QdpType1,

	/// <summary>
	/// Indicates the QDP type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	QdpType2,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 2.
	/// </summary>
	QiuDeadlyPatternType2 = QdpType2,

	/// <summary>
	/// Indicates the QDP type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	QdpType3,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 3.
	/// </summary>
	QiuDeadlyPatternType3 = QdpType3,

	/// <summary>
	/// Indicates the QDP type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	QdpType4,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 4.
	/// </summary>
	QiuDeadlyPatternType4 = QdpType4,

	/// <summary>
	/// Indicates the locked QDP.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	LockedQdp,

	/// <summary>
	/// Indicates the locked Qiu's deadly pattern.
	/// </summary>
	LockedQiuDeadlyPattern = LockedQdp,

	/// <summary>
	/// Indicates the US type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UsType1,

	/// <summary>
	/// Indicates the unique square type 1.
	/// </summary>
	UniqueSquareType1 = UsType1,

	/// <summary>
	/// Indicates the US type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UsType2,

	/// <summary>
	/// Indicates the unique square type 2.
	/// </summary>
	UniqueSquareType2 = UsType2,

	/// <summary>
	/// Indicates the US type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UsType3,

	/// <summary>
	/// Indicates the unique square type 3.
	/// </summary>
	UniqueSquareType3 = UsType3,

	/// <summary>
	/// Indicates the US type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	UsType4,

	/// <summary>
	/// Indicates the unique square type 4.
	/// </summary>
	UniqueSquareType4 = UsType4,

	/// <summary>
	/// Indicates the reverse UR type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUrType1,

	/// <summary>
	/// Indicates the reverse unique rectangle type 1.
	/// </summary>
	ReverseUniqueRectangleType1 = ReverseUrType1,

	/// <summary>
	/// Indicates the reverse UR type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUrType2,

	/// <summary>
	/// Indicates the reverse unique rectangle type 2.
	/// </summary>
	ReverseUniqueRectangleType2 = ReverseUrType2,

	/// <summary>
	/// Indicates the reverse UR type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUrType3,

	/// <summary>
	/// Indicates the reverse unique rectangle type 3.
	/// </summary>
	ReverseUniqueRectangleType3 = ReverseUrType3,

	/// <summary>
	/// Indicates the reverse UR type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUrType4,

	/// <summary>
	/// Indicates the reverse unique rectangle type 4.
	/// </summary>
	ReverseUniqueRectangleType4 = ReverseUrType4,

	/// <summary>
	/// Indicates the reverse UL type 1.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUlType1,

	/// <summary>
	/// Indicates the reverse unique loop type 1.
	/// </summary>
	ReverseUniqueLoopType1 = ReverseUlType1,

	/// <summary>
	/// Indicates the reverse UL type 2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUlType2,

	/// <summary>
	/// Indicates the reverse unique loop type 2.
	/// </summary>
	ReverseUniqueLoopType2 = ReverseUlType2,

	/// <summary>
	/// Indicates the reverse UL type 3.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUlType3,

	/// <summary>
	/// Indicates the reverse unique loop type 3.
	/// </summary>
	ReverseUniqueLoopType3 = ReverseUlType3,

	/// <summary>
	/// Indicates the reverse UL type 4.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ReverseUlType4,

	/// <summary>
	/// Indicates the reverse unique loop type 4.
	/// </summary>
	ReverseUniqueLoopType4 = ReverseUlType4,

	/// <summary>
	/// Indicates the SdC.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Sdc,

	/// <summary>
	/// Indicates the sue de coq.
	/// </summary>
	SueDeCoq = Sdc,

	/// <summary>
	/// Indicates the sue de coq with isolated digit.
	/// </summary>
	SueDeCoqIsolated,

	/// <summary>
	/// Indicates the 3-dimension SdC.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Sdc3d,

	/// <summary>
	/// Indicates the 3-dimensional sue de coq.
	/// </summary>
	SueDeCoq3Dimension = Sdc3d,

	/// <summary>
	/// Indicates the cannibalized SdC.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	CannibalizedSdc,

	/// <summary>
	/// Indicates the sue de coq cannibalism.
	/// </summary>
	SueDeCoqCannibalism = CannibalizedSdc,

	/// <summary>
	/// Indicates the skyscraper.
	/// </summary>
	Skyscraper,

	/// <summary>
	/// Indicates the two-string kite.
	/// </summary>
	TwoStringKite,

	/// <summary>
	/// Indicates the turbot fish.
	/// </summary>
	TurbotFish,

	/// <summary>
	/// Indicates the empty rectangle.
	/// </summary>
	EmptyRectangle,

	/// <summary>
	/// Indicates the guardian.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Guardian,

	/// <summary>
	/// Indicates the broken wing.
	/// </summary>
	BrokenWing = Guardian,

	/// <summary>
	/// Indicates the bi-value oddagon type 1.
	/// </summary>
	BivalueOddagonType1,

	/// <summary>
	/// Indicates the bi-value oddagon type 2.
	/// </summary>
	BivalueOddagonType2,

	/// <summary>
	/// Indicates the bi-value oddagon type 3.
	/// </summary>
	BivalueOddagonType3,

	/// <summary>
	/// Indicates the grouped bi-value oddagon.
	/// </summary>
	GroupedBivalueOddagon,

	/// <summary>
	/// Indicates the X-Chain.
	/// </summary>
	XChain,

	/// <summary>
	/// Indicates the Y-Chain.
	/// </summary>
	YChain,

	/// <summary>
	/// Indicates the fishy cycle.
	/// </summary>
	FishyCycle,

	/// <summary>
	/// Indicates the XY-Chain.
	/// </summary>
	XyChain,

	/// <summary>
	/// Indicates the XY-Cycle.
	/// </summary>
	XyCycle,

	/// <summary>
	/// Indicates the XY-X-Chain.
	/// </summary>
	XyXChain,

	/// <summary>
	/// Indicates the purple cow.
	/// </summary>
	PurpleCow,

	/// <summary>
	/// Indicates the discontinuous nice loop.
	/// </summary>
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the continuous nice loop.
	/// </summary>
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates the AIC.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Aic,

	/// <summary>
	/// Indicates the alternating inference chain.
	/// </summary>
	AlternatingInferenceChain = Aic,

	/// <summary>
	/// Indicates the grouped X-Chain.
	/// </summary>
	GroupedXChain,

	/// <summary>
	/// Indicates the grouped fishy cycle.
	/// </summary>
	GroupedFishyCycle,

	/// <summary>
	/// Indicates the grouped XY-Chain.
	/// </summary>
	GroupedXyChain,

	/// <summary>
	/// Indicates the grouped XY-Cycle.
	/// </summary>
	GroupedXyCycle,

	/// <summary>
	/// Indicates the grouped XY-X-Chain.
	/// </summary>
	GroupedXyXChain,

	/// <summary>
	/// Indicates the grouped purple cow.
	/// </summary>
	GroupedPurpleCow,

	/// <summary>
	/// Indicates the grouped discontinuous nice loop.
	/// </summary>
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the grouped continuous nice loop.
	/// </summary>
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates the grouped AIC.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	GroupedAic,

	/// <summary>
	/// Indicates the grouped alternating inference chain.
	/// </summary>
	GroupedAlternatingInferenceChain = GroupedAic,

	/// <summary>
	/// Indicates the nishio FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	NishioFc,

	/// <summary>
	/// Indicates the nishio forcing chains.
	/// </summary>
	NishioForcingChains = NishioFc,

	/// <summary>
	/// Indicates the region FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	RegionFc,

	/// <summary>
	/// Indicates the region forcing chains.
	/// </summary>
	RegionForcingChains = RegionFc,

	/// <summary>
	/// Indicates the cell FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	CellFc,

	/// <summary>
	/// Indicates the cell forcing chains.
	/// </summary>
	CellForcingChains = CellFc,

	/// <summary>
	/// Indicates the dynamic region FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	DynamicRegionFc,

	/// <summary>
	/// Indicates the dynamic region forcing chains.
	/// </summary>
	DynamicRegionForcingChains = DynamicRegionFc,

	/// <summary>
	/// Indicates the dynamic cell FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	DynamicCellFc,

	/// <summary>
	/// Indicates the dynamic cell forcing chains.
	/// </summary>
	DynamicCellForcingChains = DynamicCellFc,

	/// <summary>
	/// Indicates the dynamic contradiction FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	DynamicContradictionFc,

	/// <summary>
	/// Indicates the dynamic contradiction forcing chains.
	/// </summary>
	DynamicContradictionForcingChains = DynamicContradictionFc,

	/// <summary>
	/// Indicates the dynamic double FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	DynamicDoubleFc,

	/// <summary>
	/// Indicates the dynamic double forcing chains.
	/// </summary>
	DynamicDoubleForcingChains = DynamicDoubleFc,

	/// <summary>
	/// Indicates the dynamic FCs.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	DynamicFc,

	/// <summary>
	/// Indicates the dynamic forcing chains.
	/// </summary>
	DynamicForcingChains = DynamicFc,

	/// <summary>
	/// Indicates the ERIP.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Erip,

	/// <summary>
	/// Indicates the empty rectangle intersection pair.
	/// </summary>
	EmptyRectangleIntersectionPair = Erip,

	/// <summary>
	/// Indicates the ESP.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Esp,

	/// <summary>
	/// Indicates the extended subset principle.
	/// </summary>
	ExtendedSubsetPrinciple = Esp,

	/// <summary>
	/// Indicates the singly linked ALS-XZ.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	SinglyLinkedAlsXz,

	/// <summary>
	/// Indicates the singly linked ALS-XZ.
	/// </summary>
	SinglyLinkedAlmostLockedSetsXzRule = SinglyLinkedAlsXz,

	/// <summary>
	/// Indicates the doubly linked ALS-XZ.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	DoublyLinkedAlsXz,

	/// <summary>
	/// Indicates the doubly linked ALS-XZ.
	/// </summary>
	DoublyLinkedAlmostLockedSetsXzRule = DoublyLinkedAlsXz,

	/// <summary>
	/// Indicates the ALS-XY-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	AlsXyWing,

	/// <summary>
	/// Indicates the ALS-XY-Wing.
	/// </summary>
	AlmostLockedSetsXyWing = AlsXyWing,

	/// <summary>
	/// Indicates the ALS-W-Wing.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	AlsWWing,

	/// <summary>
	/// Indicates the ALS-W-Wing.
	/// </summary>
	AlmostLockedSetsWWing = AlsWWing,

	/// <summary>
	/// Indicates the death blossom.
	/// </summary>
	DeathBlossom,

	/// <summary>
	/// Indicates the GSP.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Gsp,

	/// <summary>
	/// Indicates the Gurth's symmetrical placement.
	/// </summary>
	GurthSymmetricalPlacement = Gsp,

	/// <summary>
	/// Indicates the GSP2.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Gsp2,

	/// <summary>
	/// Indicates the extended Gurth's symmetrical placement.
	/// </summary>
	ExtendedGurthSymmetricalPlacement = Gsp2,

	/// <summary>
	/// Indicates the JE.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Je,

	/// <summary>
	/// Indicates the junior exocet.
	/// </summary>
	JuniorExocet = Je,

	/// <summary>
	/// Indicates the SE.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Se,

	/// <summary>
	/// Indicates the senior exocet.
	/// </summary>
	SeniorExocet = Se,

	/// <summary>
	/// Indicates the complex SE.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	ComplexSe,

	/// <summary>
	/// Indicates the complex senior exocet.
	/// </summary>
	ComplexSeniorExocet = ComplexSe,

	/// <summary>
	/// Indicates the siamese JE. 
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	SiameseJe,

	/// <summary>
	/// Indicates the siamese junior exocet.
	/// </summary>
	SiameseJuniorExocet = SiameseJe,

	/// <summary>
	/// Indicates the siamese SE.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	SiameseSe,

	/// <summary>
	/// Indicates the siamese senior exocet.
	/// </summary>
	SiameseSeniorExocet = SiameseSe,

	/// <summary>
	/// Indicates the SK-Loop.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	SkLoop,

	/// <summary>
	/// Indicates the domino loop.
	/// </summary>
	DominoLoop = SkLoop,

	/// <summary>
	/// Indicates the multi-sector locked sets.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Msls,

	/// <summary>
	/// Indicates the multi-sector locked sets.
	/// </summary>
	MultisectorLockedSets = Msls,

	/// <summary>
	/// Indicates the POM.
	/// </summary>
	[Obsolete("In the future, we don't use any abbreviations in this solution.", false)]
	Pom,

	/// <summary>
	/// Indicates the pattern overlay method.
	/// </summary>
	PatternOverlay = Pom,

	/// <summary>
	/// Indicates the template set.
	/// </summary>
	TemplateSet,

	/// <summary>
	/// Indicates the template delete.
	/// </summary>
	TemplateDelete,

	/// <summary>
	/// Indicates the bowman's bingo.
	/// </summary>
	BowmanBingo,

	/// <summary>
	/// Indicates the brute force.
	/// </summary>
	BruteForce,
}
