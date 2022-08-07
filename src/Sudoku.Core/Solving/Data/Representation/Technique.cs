namespace Sudoku.Solving.Data.Representation;

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
	[HodokuTechniquePrefix("0000")]
	FullHouse,

	/// <summary>
	/// Indicates the last digit.
	/// </summary>
	[HodokuTechniquePrefix("0001")]
	LastDigit,

	/// <summary>
	/// Indicates the hidden single (in block).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	HiddenSingleBlock,

	/// <summary>
	/// Indicates the hidden single (in row).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	HiddenSingleRow,

	/// <summary>
	/// Indicates the hidden single (in column).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	HiddenSingleColumn,

	/// <summary>
	/// Indicates the naked single.
	/// </summary>
	[HodokuTechniquePrefix("0003")]
	NakedSingle,

	/// <summary>
	/// Indicates the pointing.
	/// </summary>
	[HodokuTechniquePrefix("0100")]
	Pointing,

	/// <summary>
	/// Indicates the claiming.
	/// </summary>
	[HodokuTechniquePrefix("0101")]
	Claiming,

	/// <summary>
	/// Indicates the almost locked pair.
	/// </summary>
	AlmostLockedPair,

	/// <summary>
	/// Indicates the almost locked triple.
	/// </summary>
	AlmostLockedTriple,

	/// <summary>
	/// Indicates the almost locked quadruple.
	/// The technique may not be useful because it'll be replaced with Sue de Coq.
	/// </summary>
	AlmostLockedQuadruple,

	/// <summary>
	/// Indicates the firework pair type 1.
	/// </summary>
	FireworkPairType1,

	/// <summary>
	/// Indicates the firework pair type 2.
	/// </summary>
	FireworkPairType2,

	/// <summary>
	/// Indicates the firework pair type 3.
	/// </summary>
	FireworkPairType3,

	/// <summary>
	/// Indicates the firework triple.
	/// </summary>
	FireworkTriple,

	/// <summary>
	/// Indicates the firework quadruple.
	/// </summary>
	FireworkQuadruple,

	/// <summary>
	/// Indicates the naked pair.
	/// </summary>
	[HodokuTechniquePrefix("0200")]
	NakedPair,

	/// <summary>
	/// Indicates the naked pair plus (naked pair (+)).
	/// </summary>
	NakedPairPlus,

	/// <summary>
	/// Indicates the locked pair.
	/// </summary>
	[HodokuTechniquePrefix("0110")]
	LockedPair,

	/// <summary>
	/// Indicates the hidden pair.
	/// </summary>
	[HodokuTechniquePrefix("0210")]
	HiddenPair,

	/// <summary>
	/// Indicates the naked triple.
	/// </summary>
	[HodokuTechniquePrefix("0201")]
	NakedTriple,

	/// <summary>
	/// Indicates the naked triple plus (naked triple (+)).
	/// </summary>
	NakedTriplePlus,

	/// <summary>
	/// Indicates the locked triple.
	/// </summary>
	[HodokuTechniquePrefix("0111")]
	LockedTriple,

	/// <summary>
	/// Indicates the hidden triple.
	/// </summary>
	[HodokuTechniquePrefix("0211")]
	HiddenTriple,

	/// <summary>
	/// Indicates the naked quadruple.
	/// </summary>
	[HodokuTechniquePrefix("0202")]
	NakedQuadruple,

	/// <summary>
	/// Indicates the naked quadruple plus (naked quadruple (+)).
	/// </summary>
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates the hidden quadruple.
	/// </summary>
	[HodokuTechniquePrefix("0212")]
	HiddenQuadruple,

	/// <summary>
	/// Indicates the X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0300")]
	XWing,

	/// <summary>
	/// Indicates the finned X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0310")]
	FinnedXWing,

	/// <summary>
	/// Indicates the sashimi X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0320")]
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
	[HodokuTechniquePrefix("0330")]
	FrankenXWing,

	/// <summary>
	/// Indicates the finned franken X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0340")]
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
	[HodokuTechniquePrefix("0350")]
	MutantXWing,

	/// <summary>
	/// Indicates the finned mutant X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0360")]
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
	[HodokuTechniquePrefix("0301")]
	Swordfish,

	/// <summary>
	/// Indicates the finned swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0311")]
	FinnedSwordfish,

	/// <summary>
	/// Indicates the sashimi swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0321")]
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
	[HodokuTechniquePrefix("0331")]
	FrankenSwordfish,

	/// <summary>
	/// Indicates the finned franken swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0341")]
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
	[HodokuTechniquePrefix("0351")]
	MutantSwordfish,

	/// <summary>
	/// Indicates the finned mutant swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0361")]
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
	[HodokuTechniquePrefix("0302")]
	Jellyfish,

	/// <summary>
	/// Indicates the finned jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0312")]
	FinnedJellyfish,

	/// <summary>
	/// Indicates the sashimi jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0322")]
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
	[HodokuTechniquePrefix("0332")]
	FrankenJellyfish,

	/// <summary>
	/// Indicates the finned franken jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0342")]
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
	[HodokuTechniquePrefix("0352")]
	MutantJellyfish,

	/// <summary>
	/// Indicates the finned mutant jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0362")]
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
	[HodokuTechniquePrefix("0303")]
	Squirmbag,

	/// <summary>
	/// Indicates the finned squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0313")]
	FinnedSquirmbag,

	/// <summary>
	/// Indicates the sashimi squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0323")]
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
	[HodokuTechniquePrefix("0333")]
	FrankenSquirmbag,

	/// <summary>
	/// Indicates the finned franken squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0343")]
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
	[HodokuTechniquePrefix("0353")]
	MutantSquirmbag,

	/// <summary>
	/// Indicates the finned mutant squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0363")]
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
	[HodokuTechniquePrefix("0304")]
	Whale,

	/// <summary>
	/// Indicates the finned whale.
	/// </summary>
	[HodokuTechniquePrefix("0314")]
	FinnedWhale,

	/// <summary>
	/// Indicates the sashimi whale.
	/// </summary>
	[HodokuTechniquePrefix("0324")]
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
	[HodokuTechniquePrefix("0334")]
	FrankenWhale,

	/// <summary>
	/// Indicates the finned franken whale.
	/// </summary>
	[HodokuTechniquePrefix("0344")]
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
	[HodokuTechniquePrefix("0354")]
	MutantWhale,

	/// <summary>
	/// Indicates the finned mutant whale.
	/// </summary>
	[HodokuTechniquePrefix("0364")]
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
	[HodokuTechniquePrefix("0305")]
	Leviathan,

	/// <summary>
	/// Indicates the finned leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0315")]
	FinnedLeviathan,

	/// <summary>
	/// Indicates the sashimi leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0325")]
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
	[HodokuTechniquePrefix("0335")]
	FrankenLeviathan,

	/// <summary>
	/// Indicates the finned franken leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0345")]
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
	[HodokuTechniquePrefix("0355")]
	MutantLeviathan,

	/// <summary>
	/// Indicates the finned mutant leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0365")]
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
	[HodokuTechniquePrefix("0800")]
	XyWing,

	/// <summary>
	/// Indicates the XYZ-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0801")]
	XyzWing,

	/// <summary>
	/// Indicates the WXYZ-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0802")]
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
	[HodokuTechniquePrefix("0803")]
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
	/// Indicates the unique rectangle type 1.
	/// </summary>
	[HodokuTechniquePrefix("0600")]
	UniqueRectangleType1,

	/// <summary>
	/// Indicates the unique rectangle type 2.
	/// </summary>
	[HodokuTechniquePrefix("0601")]
	UniqueRectangleType2,

	/// <summary>
	/// Indicates the unique rectangle type 3.
	/// </summary>
	[HodokuTechniquePrefix("0602")]
	UniqueRectangleType3,

	/// <summary>
	/// Indicates the unique rectangle type 4.
	/// </summary>
	[HodokuTechniquePrefix("0603")]
	UniqueRectangleType4,

	/// <summary>
	/// Indicates the unique rectangle type 5.
	/// </summary>
	[HodokuTechniquePrefix("0604")]
	UniqueRectangleType5,

	/// <summary>
	/// Indicates the unique rectangle type 6.
	/// </summary>
	[HodokuTechniquePrefix("0605")]
	UniqueRectangleType6,

	/// <summary>
	/// Indicates the hidden unique rectangle.
	/// </summary>
	[HodokuTechniquePrefix("0606")]
	HiddenUniqueRectangle,

	/// <summary>
	/// Indicates the unique rectangle + 2D.
	/// </summary>
	UniqueRectangle2D,

	/// <summary>
	/// Indicates the unique rectangle + 2B / 1SL.
	/// </summary>
	UniqueRectangle2B1,

	/// <summary>
	/// Indicates the unique rectangle + 2D / 1SL.
	/// </summary>
	UniqueRectangle2D1,

	/// <summary>
	/// Indicates the unique rectangle + 3X.
	/// </summary>
	UniqueRectangle3X,

	/// <summary>
	/// Indicates the unique rectangle + 3x / 1SL.
	/// </summary>
	UniqueRectangle3X1L,

	/// <summary>
	/// Indicates the unique rectangle + 3X / 1SL.
	/// </summary>
	UniqueRectangle3X1U,

	/// <summary>
	/// Indicates the unique rectangle + 3X / 2SL.
	/// </summary>
	UniqueRectangle3X2,

	/// <summary>
	/// Indicates the unique rectangle + 3N / 2SL.
	/// </summary>
	UniqueRectangle3N2,

	/// <summary>
	/// Indicates the unique rectangle + 3U / 2SL.
	/// </summary>
	UniqueRectangle3U2,

	/// <summary>
	/// Indicates the unique rectangle + 3E / 2SL.
	/// </summary>
	UniqueRectangle3E2,

	/// <summary>
	/// Indicates the unique rectangle + 4x / 1SL.
	/// </summary>
	UniqueRectangle4X1L,

	/// <summary>
	/// Indicates the unique rectangle + 4X / 1SL.
	/// </summary>
	UniqueRectangle4X1U,

	/// <summary>
	/// Indicates the unique rectangle + 4x / 2SL.
	/// </summary>
	UniqueRectangle4X2L,

	/// <summary>
	/// Indicates the unique rectangle + 4X / 2SL.
	/// </summary>
	UniqueRectangle4X2U,

	/// <summary>
	/// Indicates the unique rectangle + 4X / 3SL.
	/// </summary>
	UniqueRectangle4X3,

	/// <summary>
	/// Indicates the unique rectangle + 4C / 3SL.
	/// </summary>
	UniqueRectangle4C3,

	/// <summary>
	/// Indicates the unique rectangle-XY-Wing.
	/// </summary>
	UniqueRectangleXyWing,

	/// <summary>
	/// Indicates the unique rectangle-XYZ-Wing.
	/// </summary>
	UniqueRectangleXyzWing,

	/// <summary>
	/// Indicates the unique rectangle-WXYZ-Wing.
	/// </summary>
	UniqueRectangleWxyzWing,

	/// <summary>
	/// Indicates the unique rectangle sue de coq.
	/// </summary>
	UniqueRectangleSueDeCoq,

	/// <summary>
	/// Indicates the unique rectangle unknown covering.
	/// </summary>
	UniqueRectangleUnknownCovering,

	/// <summary>
	/// Indicates the unique rectangle external type 1.
	/// </summary>
	UniqueRectangleExternalType1,

	/// <summary>
	/// Indicates the unique rectangle external type 2.
	/// </summary>
	UniqueRectangleExternalType2,

	/// <summary>
	/// Indicates the unique rectangle external type 3.
	/// </summary>
	UniqueRectangleExternalType3,

	/// <summary>
	/// Indicates the unique rectangle external type 4.
	/// </summary>
	UniqueRectangleExternalType4,

	/// <summary>
	/// Indicates the avoidable rectangle type 1.
	/// </summary>
	[HodokuTechniquePrefix("0607")]
	AvoidableRectangleType1,

	/// <summary>
	/// Indicates the avoidable rectangle type 2.
	/// </summary>
	[HodokuTechniquePrefix("0608")]
	AvoidableRectangleType2,

	/// <summary>
	/// Indicates the avoidable rectangle type 3.
	/// </summary>
	AvoidableRectangleType3,

	/// <summary>
	/// Indicates the avoidable rectangle type 5.
	/// </summary>
	AvoidableRectangleType5,

	/// <summary>
	/// Indicates the hidden avoidable rectangle.
	/// </summary>
	HiddenAvoidableRectangle,

	/// <summary>
	/// Indicates the avoidable rectangle + 2D.
	/// </summary>
	AvoidableRectangle2D,

	/// <summary>
	/// Indicates the avoidable rectangle + 3X.
	/// </summary>
	AvoidableRectangle3X,

	/// <summary>
	/// Indicates the avoidable rectangle XY-Wing.
	/// </summary>
	AvoidableRectangleXyWing,

	/// <summary>
	/// Indicates the avoidable rectangle XYZ-Wing.
	/// </summary>
	AvoidableRectangleXyzWing,

	/// <summary>
	/// Indicates the avoidable rectangle WXYZ-Wing.
	/// </summary>
	AvoidableRectangleWxyzWing,

	/// <summary>
	/// Indicates the avoidable rectangle sue de coq.
	/// </summary>
	AvoidableRectangleSueDeCoq,

	/// <summary>
	/// Indicates the avoidable rectangle guardian.
	/// </summary>
	AvoidableRectangleBrokenWing,

	/// <summary>
	/// Indicates the avoidable rectangle hidden single in block.
	/// </summary>
	AvoidableRectangleHiddenSingleBlock,

	/// <summary>
	/// Indicates the avoidable rectangle hidden single in row.
	/// </summary>
	AvoidableRectangleHiddenSingleRow,

	/// <summary>
	/// Indicates the avoidable rectangle hidden single in column.
	/// </summary>
	AvoidableRectangleHiddenSingleColumn,

	/// <summary>
	/// Indicates the unique loop type 1.
	/// </summary>
	UniqueLoopType1,

	/// <summary>
	/// Indicates the unique loop type 2.
	/// </summary>
	UniqueLoopType2,

	/// <summary>
	/// Indicates the unique loop type 3.
	/// </summary>
	UniqueLoopType3,

	/// <summary>
	/// Indicates the unique loop type 4.
	/// </summary>
	UniqueLoopType4,

	/// <summary>
	/// Indicates the extended rectangle type 1.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0620")]
#endif
	ExtendedRectangleType1,

	/// <summary>
	/// Indicates the extended rectangle type 2.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0621")]
#endif
	ExtendedRectangleType2,

	/// <summary>
	/// Indicates the extended rectangle type 3.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0622")]
#endif
	ExtendedRectangleType3,

	/// <summary>
	/// Indicates the extended rectangle type 4.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0623")]
#endif
	ExtendedRectangleType4,

	/// <summary>
	/// Indicates the bi-value universal grave type 1.
	/// </summary>
	[HodokuTechniquePrefix("0610")]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates the bi-value universal grave type 2.
	/// </summary>
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates the bi-value universal grave type 3.
	/// </summary>
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates the bi-value universal grave type 4.
	/// </summary>
	BivalueUniversalGraveType4,

	/// <summary>
	/// Indicates the bi-value universal grave + n.
	/// </summary>
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Indicates the bi-value universal grave + n with forcing chains.
	/// </summary>
	BivalueUniversalGravePlusNForcingChains,

	/// <summary>
	/// Indicates the bi-value universal grave XZ rule.
	/// </summary>
	BivalueUniversalGraveXzRule,

	/// <summary>
	/// Indicates the bi-value universal grave XY-Wing.
	/// </summary>
	BivalueUniversalGraveXyWing,

	/// <summary>
	/// Indicates the unique polygon type 1.
	/// </summary>
	UniquePolygonType1,

	/// <summary>
	/// Indicates the unique polygon type 2.
	/// </summary>
	UniquePolygonType2,

	/// <summary>
	/// Indicates the unique polygon type 3.
	/// </summary>
	UniquePolygonType3,

	/// <summary>
	/// Indicates the unique polygon type 4.
	/// </summary>
	UniquePolygonType4,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 1.
	/// </summary>
	QiuDeadlyPatternType1,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 2.
	/// </summary>
	QiuDeadlyPatternType2,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 3.
	/// </summary>
	QiuDeadlyPatternType3,

	/// <summary>
	/// Indicates the Qiu's deadly pattern type 4.
	/// </summary>
	QiuDeadlyPatternType4,

	/// <summary>
	/// Indicates the locked Qiu's deadly pattern.
	/// </summary>
	LockedQiuDeadlyPattern,

	/// <summary>
	/// Indicates the unique square type 1.
	/// </summary>
	UniqueSquareType1,

	/// <summary>
	/// Indicates the unique square type 2.
	/// </summary>
	UniqueSquareType2,

	/// <summary>
	/// Indicates the unique square type 3.
	/// </summary>
	UniqueSquareType3,

	/// <summary>
	/// Indicates the unique square type 4.
	/// </summary>
	UniqueSquareType4,

	/// <summary>
	/// Indicates the sue de coq.
	/// </summary>
	[HodokuTechniquePrefix("1101")]
	SueDeCoq,

	/// <summary>
	/// Indicates the sue de coq with isolated digit.
	/// </summary>
	[HodokuTechniquePrefix("1101")]
	SueDeCoqIsolated,

	/// <summary>
	/// Indicates the 3-dimensional sue de coq.
	/// </summary>
	SueDeCoq3Dimension,

	/// <summary>
	/// Indicates the sue de coq cannibalism.
	/// </summary>
	SueDeCoqCannibalism,

	/// <summary>
	/// Indicates the skyscraper.
	/// </summary>
	[HodokuTechniquePrefix("0400")]
	Skyscraper,

	/// <summary>
	/// Indicates the two-string kite.
	/// </summary>
	[HodokuTechniquePrefix("0401")]
	TwoStringKite,

	/// <summary>
	/// Indicates the turbot fish.
	/// </summary>
	[HodokuTechniquePrefix("0403")]
	TurbotFish,

	/// <summary>
	/// Indicates the empty rectangle.
	/// </summary>
	[HodokuTechniquePrefix("0402")]
	EmptyRectangle,

	/// <summary>
	/// Indicates the broken wing.
	/// </summary>
	[HodokuTechniquePrefix("0705")]
	BrokenWing,

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
	/// Indicates the chromatic pattern (tri-value oddagon) type 1.
	/// </summary>
	ChromaticPatternType1,

	/// <summary>
	/// Indicates the chromatic pattern (tri-value oddagon) type 2.
	/// </summary>
	ChromaticPatternType2,

	/// <summary>
	/// Indicates the chromatic pattern (tri-value oddagon) type 3.
	/// </summary>
	ChromaticPatternType3,

	/// <summary>
	/// Indicates the chromatic pattern (tri-value oddagon) type 4.
	/// </summary>
	ChromaticPatternType4,

	/// <summary>
	/// Indicates the chromatic pattern (tri-value oddagon) XZ rule.
	/// </summary>
	ChromaticPatternXzRule,

	/// <summary>
	/// Indicates the X-Chain.
	/// </summary>
	[HodokuTechniquePrefix("0701")]
	XChain,

	/// <summary>
	/// Indicates the Y-Chain.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0702")]
#endif
	YChain,

	/// <summary>
	/// Indicates the fishy cycle (X-Cycle).
	/// </summary>
	[HodokuTechniquePrefix("0704")]
	FishyCycle,

	/// <summary>
	/// Indicates the XY-Chain.
	/// </summary>
	[HodokuTechniquePrefix("0702")]
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
	/// Indicates the remote pair.
	/// </summary>
	[HodokuTechniquePrefix("0703")]
	RemotePair,

	/// <summary>
	/// Indicates the purple cow.
	/// </summary>
	PurpleCow,

	/// <summary>
	/// Indicates the discontinuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0707")]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the continuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0706")]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates the alternating inference chain.
	/// </summary>
	[HodokuTechniquePrefix("0708")]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates the grouped X-Chain.
	/// </summary>
	GroupedXChain,

	/// <summary>
	/// Indicates the grouped fishy cycle (grouped X-Cycle).
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
	[HodokuTechniquePrefix("0710")]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the grouped continuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0709")]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates the grouped alternating inference chain.
	/// </summary>
	[HodokuTechniquePrefix("0711")]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates the special case that a grouped alternating inference chain has a collision
	/// between start and end node.
	/// </summary>
	NodeCollision,

	/// <summary>
	/// Indicates the nishio forcing chains.
	/// </summary>
	NishioForcingChains,

	/// <summary>
	/// Indicates the region forcing chains (i.e. house forcing chains).
	/// </summary>
	[HodokuTechniquePrefix("1301")]
	RegionForcingChains,

	/// <summary>
	/// Indicates the cell forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1301")]
	CellForcingChains,

	/// <summary>
	/// Indicates the dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	[HodokuTechniquePrefix("1303")]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates the dynamic cell forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1303")]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates the dynamic contradiction forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1304")]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates the dynamic double forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1304")]
	DynamicDoubleForcingChains,

	/// <summary>
	/// Indicates the dynamic forcing chains.
	/// </summary>
	DynamicForcingChains,

	/// <summary>
	/// Indicates the empty rectangle intersection pair.
	/// </summary>
	EmptyRectangleIntersectionPair,

	/// <summary>
	/// Indicates the extended subset principle.
	/// </summary>
	[HodokuTechniquePrefix("1102")]
	ExtendedSubsetPrinciple,

	/// <summary>
	/// Indicates the singly linked ALS-XZ.
	/// </summary>
	[HodokuTechniquePrefix("0901")]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates the doubly linked ALS-XZ.
	/// </summary>
	[HodokuTechniquePrefix("0901")]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates the ALS-XY-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0902")]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates the ALS-W-Wing.
	/// </summary>
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates the ALS chain.
	/// </summary>
	[HodokuTechniquePrefix("0903")]
	AlmostLockedSetsChain,

	/// <summary>
	/// Indicates the AHS chain.
	/// </summary>
	AlmostHiddenSetsChain,

	/// <summary>
	/// Indicates the death blossom.
	/// </summary>
	[HodokuTechniquePrefix("0904")]
	DeathBlossom,

	/// <summary>
	/// Indicates the Gurth's symmetrical placement.
	/// </summary>
	GurthSymmetricalPlacement,

	/// <summary>
	/// Indicates the extended Gurth's symmetrical placement.
	/// </summary>
	ExtendedGurthSymmetricalPlacement,

	/// <summary>
	/// Indicates the junior exocet.
	/// </summary>
	JuniorExocet,

	/// <summary>
	/// Indicates the senior exocet.
	/// </summary>
	SeniorExocet,

	/// <summary>
	/// Indicates the complex senior exocet.
	/// </summary>
	ComplexSeniorExocet,

	/// <summary>
	/// Indicates the siamese junior exocet.
	/// </summary>
	SiameseJuniorExocet,

	/// <summary>
	/// Indicates the siamese senior exocet.
	/// </summary>
	SiameseSeniorExocet,

	/// <summary>
	/// Indicates the domino loop.
	/// </summary>
	DominoLoop,

	/// <summary>
	/// Indicates the multi-sector locked sets.
	/// </summary>
	MultisectorLockedSets,

	/// <summary>
	/// Indicates the pattern overlay method.
	/// </summary>
	PatternOverlay,

	/// <summary>
	/// Indicates the template set.
	/// </summary>
	[HodokuTechniquePrefix("1201")]
	TemplateSet,

	/// <summary>
	/// Indicates the template delete.
	/// </summary>
	[HodokuTechniquePrefix("1202")]
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
