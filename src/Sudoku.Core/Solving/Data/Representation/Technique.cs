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
	/// Indicates full house.
	/// </summary>
	[HodokuTechniquePrefix("0000")]
	FullHouse,

	/// <summary>
	/// Indicates last digit.
	/// </summary>
	[HodokuTechniquePrefix("0001")]
	LastDigit,

	/// <summary>
	/// Indicates hidden single (in block).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	HiddenSingleBlock,

	/// <summary>
	/// Indicates hidden single (in row).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	HiddenSingleRow,

	/// <summary>
	/// Indicates hidden single (in column).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	HiddenSingleColumn,

	/// <summary>
	/// Indicates naked single.
	/// </summary>
	[HodokuTechniquePrefix("0003")]
	NakedSingle,

	/// <summary>
	/// Indicates pointing.
	/// </summary>
	[HodokuTechniquePrefix("0100")]
	Pointing,

	/// <summary>
	/// Indicates claiming.
	/// </summary>
	[HodokuTechniquePrefix("0101")]
	Claiming,

	/// <summary>
	/// Indicates almost locked pair.
	/// </summary>
	AlmostLockedPair,

	/// <summary>
	/// Indicates almost locked triple.
	/// </summary>
	AlmostLockedTriple,

	/// <summary>
	/// Indicates almost locked quadruple.
	/// The technique may not be useful because it'll be replaced with Sue de Coq.
	/// </summary>
	AlmostLockedQuadruple,

	/// <summary>
	/// Indicates firework pair type 1.
	/// </summary>
	FireworkPairType1,

	/// <summary>
	/// Indicates firework pair type 2.
	/// </summary>
	FireworkPairType2,

	/// <summary>
	/// Indicates firework pair type 3.
	/// </summary>
	FireworkPairType3,

	/// <summary>
	/// Indicates firework triple.
	/// </summary>
	FireworkTriple,

	/// <summary>
	/// Indicates firework quadruple.
	/// </summary>
	FireworkQuadruple,

	/// <summary>
	/// Indicates naked pair.
	/// </summary>
	[HodokuTechniquePrefix("0200")]
	NakedPair,

	/// <summary>
	/// Indicates naked pair plus (naked pair (+)).
	/// </summary>
	NakedPairPlus,

	/// <summary>
	/// Indicates locked pair.
	/// </summary>
	[HodokuTechniquePrefix("0110")]
	LockedPair,

	/// <summary>
	/// Indicates hidden pair.
	/// </summary>
	[HodokuTechniquePrefix("0210")]
	HiddenPair,

	/// <summary>
	/// Indicates naked triple.
	/// </summary>
	[HodokuTechniquePrefix("0201")]
	NakedTriple,

	/// <summary>
	/// Indicates naked triple plus (naked triple (+)).
	/// </summary>
	NakedTriplePlus,

	/// <summary>
	/// Indicates locked triple.
	/// </summary>
	[HodokuTechniquePrefix("0111")]
	LockedTriple,

	/// <summary>
	/// Indicates hidden triple.
	/// </summary>
	[HodokuTechniquePrefix("0211")]
	HiddenTriple,

	/// <summary>
	/// Indicates naked quadruple.
	/// </summary>
	[HodokuTechniquePrefix("0202")]
	NakedQuadruple,

	/// <summary>
	/// Indicates naked quadruple plus (naked quadruple (+)).
	/// </summary>
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates hidden quadruple.
	/// </summary>
	[HodokuTechniquePrefix("0212")]
	HiddenQuadruple,

	/// <summary>
	/// Indicates X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0300")]
	XWing,

	/// <summary>
	/// Indicates finned X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0310")]
	FinnedXWing,

	/// <summary>
	/// Indicates sashimi X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0320")]
	SashimiXWing,

	/// <summary>
	/// Indicates siamese finned X-Wing.
	/// </summary>
	SiameseFinnedXWing,

	/// <summary>
	/// Indicates siamese sashimi X-Wing.
	/// </summary>
	SiameseSashimiXWing,

	/// <summary>
	/// Indicates franken X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0330")]
	FrankenXWing,

	/// <summary>
	/// Indicates finned franken X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0340")]
	FinnedFrankenXWing,

	/// <summary>
	/// Indicates sashimi franken X-Wing.
	/// </summary>
	SashimiFrankenXWing,

	/// <summary>
	/// Indicates siamese finned franken X-Wing.
	/// </summary>
	SiameseFinnedFrankenXWing,

	/// <summary>
	/// Indicates siamese sashimi franken X-Wing.
	/// </summary>
	SiameseSashimiFrankenXWing,

	/// <summary>
	/// Indicates mutant X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0350")]
	MutantXWing,

	/// <summary>
	/// Indicates finned mutant X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0360")]
	FinnedMutantXWing,

	/// <summary>
	/// Indicates sashimi mutant X-Wing.
	/// </summary>
	SashimiMutantXWing,

	/// <summary>
	/// Indicates siamese finned mutant X-Wing.
	/// </summary>
	SiameseFinnedMutantXWing,

	/// <summary>
	/// Indicates siamese sashimi mutant X-Wing.
	/// </summary>
	SiameseSashimiMutantXWing,

	/// <summary>
	/// Indicates swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0301")]
	Swordfish,

	/// <summary>
	/// Indicates finned swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0311")]
	FinnedSwordfish,

	/// <summary>
	/// Indicates sashimi swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0321")]
	SashimiSwordfish,

	/// <summary>
	/// Indicates siamese finned swordfish.
	/// </summary>
	SiameseFinnedSwordfish,

	/// <summary>
	/// Indicates siamese sashimi swordfish.
	/// </summary>
	SiameseSashimiSwordfish,

	/// <summary>
	/// Indicates swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0331")]
	FrankenSwordfish,

	/// <summary>
	/// Indicates finned franken swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0341")]
	FinnedFrankenSwordfish,

	/// <summary>
	/// Indicates sashimi franken swordfish.
	/// </summary>
	SashimiFrankenSwordfish,

	/// <summary>
	/// Indicates siamese finned franken swordfish.
	/// </summary>
	SiameseFinnedFrankenSwordfish,

	/// <summary>
	/// Indicates siamese sashimi franken swordfish.
	/// </summary>
	SiameseSashimiFrankenSwordfish,

	/// <summary>
	/// Indicates mutant swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0351")]
	MutantSwordfish,

	/// <summary>
	/// Indicates finned mutant swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0361")]
	FinnedMutantSwordfish,

	/// <summary>
	/// Indicates sashimi mutant swordfish.
	/// </summary>
	SashimiMutantSwordfish,

	/// <summary>
	/// Indicates siamese finned mutant swordfish.
	/// </summary>
	SiameseFinnedMutantSwordfish,

	/// <summary>
	/// Indicates siamese sashimi mutant swordfish.
	/// </summary>
	SiameseSashimiMutantSwordfish,

	/// <summary>
	/// Indicates jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0302")]
	Jellyfish,

	/// <summary>
	/// Indicates finned jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0312")]
	FinnedJellyfish,

	/// <summary>
	/// Indicates sashimi jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0322")]
	SashimiJellyfish,

	/// <summary>
	/// Indicates siamese finned jellyfish.
	/// </summary>
	SiameseFinnedJellyfish,

	/// <summary>
	/// Indicates siamese sashimi jellyfish.
	/// </summary>
	SiameseSashimiJellyfish,

	/// <summary>
	/// Indicates franken jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0332")]
	FrankenJellyfish,

	/// <summary>
	/// Indicates finned franken jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0342")]
	FinnedFrankenJellyfish,

	/// <summary>
	/// Indicates sashimi franken jellyfish.
	/// </summary>
	SashimiFrankenJellyfish,

	/// <summary>
	/// Indicates siamese finned franken jellyfish.
	/// </summary>
	SiameseFinnedFrankenJellyfish,

	/// <summary>
	/// Indicates siamese sashimi franken jellyfish.
	/// </summary>
	SiameseSashimiFrankenJellyfish,

	/// <summary>
	/// Indicates mutant jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0352")]
	MutantJellyfish,

	/// <summary>
	/// Indicates finned mutant jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0362")]
	FinnedMutantJellyfish,

	/// <summary>
	/// Indicates sashimi mutant jellyfish.
	/// </summary>
	SashimiMutantJellyfish,

	/// <summary>
	/// Indicates siamese finned mutant jellyfish.
	/// </summary>
	SiameseFinnedMutantJellyfish,

	/// <summary>
	/// Indicates siamese sashimi mutant jellyfish.
	/// </summary>
	SiameseSashimiMutantJellyfish,

	/// <summary>
	/// Indicates squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0303")]
	Squirmbag,

	/// <summary>
	/// Indicates finned squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0313")]
	FinnedSquirmbag,

	/// <summary>
	/// Indicates sashimi squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0323")]
	SashimiSquirmbag,

	/// <summary>
	/// Indicates siamese finned squirmbag.
	/// </summary>
	SiameseFinnedSquirmbag,

	/// <summary>
	/// Indicates siamese sashimi squirmbag.
	/// </summary>
	SiameseSashimiSquirmbag,

	/// <summary>
	/// Indicates franken squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0333")]
	FrankenSquirmbag,

	/// <summary>
	/// Indicates finned franken squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0343")]
	FinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates sashimi franken squirmbag.
	/// </summary>
	SashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates siamese finned franken squirmbag.
	/// </summary>
	SiameseFinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates siamese sashimi franken squirmbag.
	/// </summary>
	SiameseSashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates mutant squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0353")]
	MutantSquirmbag,

	/// <summary>
	/// Indicates finned mutant squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0363")]
	FinnedMutantSquirmbag,

	/// <summary>
	/// Indicates sashimi mutant squirmbag.
	/// </summary>
	SashimiMutantSquirmbag,

	/// <summary>
	/// Indicates siamese finned mutant squirmbag.
	/// </summary>
	SiameseFinnedMutantSquirmbag,

	/// <summary>
	/// Indicates siamese sashimi mutant squirmbag.
	/// </summary>
	SiameseSashimiMutantSquirmbag,

	/// <summary>
	/// Indicates whale.
	/// </summary>
	[HodokuTechniquePrefix("0304")]
	Whale,

	/// <summary>
	/// Indicates finned whale.
	/// </summary>
	[HodokuTechniquePrefix("0314")]
	FinnedWhale,

	/// <summary>
	/// Indicates sashimi whale.
	/// </summary>
	[HodokuTechniquePrefix("0324")]
	SashimiWhale,

	/// <summary>
	/// Indicates siamese finned whale.
	/// </summary>
	SiameseFinnedWhale,

	/// <summary>
	/// Indicates siamese sashimi whale.
	/// </summary>
	SiameseSashimiWhale,

	/// <summary>
	/// Indicates franken whale.
	/// </summary>
	[HodokuTechniquePrefix("0334")]
	FrankenWhale,

	/// <summary>
	/// Indicates finned franken whale.
	/// </summary>
	[HodokuTechniquePrefix("0344")]
	FinnedFrankenWhale,

	/// <summary>
	/// Indicates sashimi franken whale.
	/// </summary>
	SashimiFrankenWhale,

	/// <summary>
	/// Indicates siamese finned franken whale.
	/// </summary>
	SiameseFinnedFrankenWhale,

	/// <summary>
	/// Indicates siamese sashimi franken whale.
	/// </summary>
	SiameseSashimiFrankenWhale,

	/// <summary>
	/// Indicates mutant whale.
	/// </summary>
	[HodokuTechniquePrefix("0354")]
	MutantWhale,

	/// <summary>
	/// Indicates finned mutant whale.
	/// </summary>
	[HodokuTechniquePrefix("0364")]
	FinnedMutantWhale,

	/// <summary>
	/// Indicates sashimi mutant whale.
	/// </summary>
	SashimiMutantWhale,

	/// <summary>
	/// Indicates siamese finned mutant whale.
	/// </summary>
	SiameseFinnedMutantWhale,

	/// <summary>
	/// Indicates siamese sashimi mutant whale.
	/// </summary>
	SiameseSashimiMutantWhale,

	/// <summary>
	/// Indicates leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0305")]
	Leviathan,

	/// <summary>
	/// Indicates finned leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0315")]
	FinnedLeviathan,

	/// <summary>
	/// Indicates sashimi leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0325")]
	SashimiLeviathan,

	/// <summary>
	/// Indicates siamese finned leviathan.
	/// </summary>
	SiameseFinnedLeviathan,

	/// <summary>
	/// Indicates siamese sashimi leviathan.
	/// </summary>
	SiameseSashimiLeviathan,

	/// <summary>
	/// Indicates franken leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0335")]
	FrankenLeviathan,

	/// <summary>
	/// Indicates finned franken leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0345")]
	FinnedFrankenLeviathan,

	/// <summary>
	/// Indicates sashimi franken leviathan.
	/// </summary>
	SashimiFrankenLeviathan,

	/// <summary>
	/// Indicates siamese finned franken leviathan.
	/// </summary>
	SiameseFinnedFrankenLeviathan,

	/// <summary>
	/// Indicates siamese sashimi franken leviathan.
	/// </summary>
	SiameseSashimiFrankenLeviathan,

	/// <summary>
	/// Indicates mutant leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0355")]
	MutantLeviathan,

	/// <summary>
	/// Indicates finned mutant leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0365")]
	FinnedMutantLeviathan,

	/// <summary>
	/// Indicates sashimi mutant leviathan.
	/// </summary>
	SashimiMutantLeviathan,

	/// <summary>
	/// Indicates siamese finned mutant leviathan.
	/// </summary>
	SiameseFinnedMutantLeviathan,

	/// <summary>
	/// Indicates siamese sashimi mutant leviathan.
	/// </summary>
	SiameseSashimiMutantLeviathan,

	/// <summary>
	/// Indicates XY-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0800")]
	XyWing,

	/// <summary>
	/// Indicates XYZ-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0801")]
	XyzWing,

	/// <summary>
	/// Indicates WXYZ-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0802")]
	WxyzWing,

	/// <summary>
	/// Indicates VWXYZ-Wing.
	/// </summary>
	VwxyzWing,

	/// <summary>
	/// Indicates UVWXYZ-Wing.
	/// </summary>
	UvwxyzWing,

	/// <summary>
	/// Indicates TUVWXYZ-Wing.
	/// </summary>
	TuvwxyzWing,

	/// <summary>
	/// Indicates STUVWXYZ-Wing.
	/// </summary>
	StuvwxyzWing,

	/// <summary>
	/// Indicates RSTUVWXYZ-Wing.
	/// </summary>
	RstuvwxyzWing,

	/// <summary>
	/// Indicates incomplete WXYZ-Wing.
	/// </summary>
	IncompleteWxyzWing,

	/// <summary>
	/// Indicates incomplete VWXYZ-Wing.
	/// </summary>
	IncompleteVwxyzWing,

	/// <summary>
	/// Indicates incomplete UVWXYZ-Wing.
	/// </summary>
	IncompleteUvwxyzWing,

	/// <summary>
	/// Indicates incomplete TUVWXYZ-Wing.
	/// </summary>
	IncompleteTuvwxyzWing,

	/// <summary>
	/// Indicates incomplete STUVWXYZ-Wing.
	/// </summary>
	IncompleteStuvwxyzWing,

	/// <summary>
	/// Indicates incomplete RSTUVWXYZ-Wing.
	/// </summary>
	IncompleteRstuvwxyzWing,

	/// <summary>
	/// Indicates W-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0803")]
	WWing,

	/// <summary>
	/// Indicates M-Wing.
	/// </summary>
	MWing,

	/// <summary>
	/// Indicates local wing.
	/// </summary>
	LocalWing,

	/// <summary>
	/// Indicates split wing.
	/// </summary>
	SplitWing,

	/// <summary>
	/// Indicates hybrid wing.
	/// </summary>
	HybridWing,

	/// <summary>
	/// Indicates grouped XY-Wing.
	/// </summary>
	GroupedXyWing,

	/// <summary>
	/// Indicates grouped W-Wing.
	/// </summary>
	GroupedWWing,

	/// <summary>
	/// Indicates grouped M-Wing.
	/// </summary>
	GroupedMWing,

	/// <summary>
	/// Indicates grouped local wing.
	/// </summary>
	GroupedLocalWing,

	/// <summary>
	/// Indicates grouped split wing.
	/// </summary>
	GroupedSplitWing,

	/// <summary>
	/// Indicates grouped hybrid wing.
	/// </summary>
	GroupedHybridWing,

	/// <summary>
	/// Indicates unique rectangle type 1.
	/// </summary>
	[HodokuTechniquePrefix("0600")]
	UniqueRectangleType1,

	/// <summary>
	/// Indicates unique rectangle type 2.
	/// </summary>
	[HodokuTechniquePrefix("0601")]
	UniqueRectangleType2,

	/// <summary>
	/// Indicates unique rectangle type 3.
	/// </summary>
	[HodokuTechniquePrefix("0602")]
	UniqueRectangleType3,

	/// <summary>
	/// Indicates unique rectangle type 4.
	/// </summary>
	[HodokuTechniquePrefix("0603")]
	UniqueRectangleType4,

	/// <summary>
	/// Indicates unique rectangle type 5.
	/// </summary>
	[HodokuTechniquePrefix("0604")]
	UniqueRectangleType5,

	/// <summary>
	/// Indicates unique rectangle type 6.
	/// </summary>
	[HodokuTechniquePrefix("0605")]
	UniqueRectangleType6,

	/// <summary>
	/// Indicates hidden unique rectangle.
	/// </summary>
	[HodokuTechniquePrefix("0606")]
	HiddenUniqueRectangle,

	/// <summary>
	/// Indicates unique rectangle + 2D.
	/// </summary>
	UniqueRectangle2D,

	/// <summary>
	/// Indicates unique rectangle + 2B / 1SL.
	/// </summary>
	UniqueRectangle2B1,

	/// <summary>
	/// Indicates unique rectangle + 2D / 1SL.
	/// </summary>
	UniqueRectangle2D1,

	/// <summary>
	/// Indicates unique rectangle + 3X.
	/// </summary>
	UniqueRectangle3X,

	/// <summary>
	/// Indicates unique rectangle + 3x / 1SL.
	/// </summary>
	UniqueRectangle3X1L,

	/// <summary>
	/// Indicates unique rectangle + 3X / 1SL.
	/// </summary>
	UniqueRectangle3X1U,

	/// <summary>
	/// Indicates unique rectangle + 3X / 2SL.
	/// </summary>
	UniqueRectangle3X2,

	/// <summary>
	/// Indicates unique rectangle + 3N / 2SL.
	/// </summary>
	UniqueRectangle3N2,

	/// <summary>
	/// Indicates unique rectangle + 3U / 2SL.
	/// </summary>
	UniqueRectangle3U2,

	/// <summary>
	/// Indicates unique rectangle + 3E / 2SL.
	/// </summary>
	UniqueRectangle3E2,

	/// <summary>
	/// Indicates unique rectangle + 4x / 1SL.
	/// </summary>
	UniqueRectangle4X1L,

	/// <summary>
	/// Indicates unique rectangle + 4X / 1SL.
	/// </summary>
	UniqueRectangle4X1U,

	/// <summary>
	/// Indicates unique rectangle + 4x / 2SL.
	/// </summary>
	UniqueRectangle4X2L,

	/// <summary>
	/// Indicates unique rectangle + 4X / 2SL.
	/// </summary>
	UniqueRectangle4X2U,

	/// <summary>
	/// Indicates unique rectangle + 4X / 3SL.
	/// </summary>
	UniqueRectangle4X3,

	/// <summary>
	/// Indicates unique rectangle + 4C / 3SL.
	/// </summary>
	UniqueRectangle4C3,

	/// <summary>
	/// Indicates unique rectangle-XY-Wing.
	/// </summary>
	UniqueRectangleXyWing,

	/// <summary>
	/// Indicates unique rectangle-XYZ-Wing.
	/// </summary>
	UniqueRectangleXyzWing,

	/// <summary>
	/// Indicates unique rectangle-WXYZ-Wing.
	/// </summary>
	UniqueRectangleWxyzWing,

	/// <summary>
	/// Indicates unique rectangle sue de coq.
	/// </summary>
	UniqueRectangleSueDeCoq,

	/// <summary>
	/// Indicates unique rectangle unknown covering.
	/// </summary>
	UniqueRectangleUnknownCovering,

	/// <summary>
	/// Indicates unique rectangle external type 1.
	/// </summary>
	UniqueRectangleExternalType1,

	/// <summary>
	/// Indicates unique rectangle external type 2.
	/// </summary>
	UniqueRectangleExternalType2,

	/// <summary>
	/// Indicates unique rectangle external type 3.
	/// </summary>
	UniqueRectangleExternalType3,

	/// <summary>
	/// Indicates unique rectangle external type 4.
	/// </summary>
	UniqueRectangleExternalType4,

	/// <summary>
	/// Indicates avoidable rectangle type 1.
	/// </summary>
	[HodokuTechniquePrefix("0607")]
	AvoidableRectangleType1,

	/// <summary>
	/// Indicates avoidable rectangle type 2.
	/// </summary>
	[HodokuTechniquePrefix("0608")]
	AvoidableRectangleType2,

	/// <summary>
	/// Indicates avoidable rectangle type 3.
	/// </summary>
	AvoidableRectangleType3,

	/// <summary>
	/// Indicates avoidable rectangle type 5.
	/// </summary>
	AvoidableRectangleType5,

	/// <summary>
	/// Indicates hidden avoidable rectangle.
	/// </summary>
	HiddenAvoidableRectangle,

	/// <summary>
	/// Indicates avoidable rectangle + 2D.
	/// </summary>
	AvoidableRectangle2D,

	/// <summary>
	/// Indicates avoidable rectangle + 3X.
	/// </summary>
	AvoidableRectangle3X,

	/// <summary>
	/// Indicates avoidable rectangle XY-Wing.
	/// </summary>
	AvoidableRectangleXyWing,

	/// <summary>
	/// Indicates avoidable rectangle XYZ-Wing.
	/// </summary>
	AvoidableRectangleXyzWing,

	/// <summary>
	/// Indicates avoidable rectangle WXYZ-Wing.
	/// </summary>
	AvoidableRectangleWxyzWing,

	/// <summary>
	/// Indicates avoidable rectangle sue de coq.
	/// </summary>
	AvoidableRectangleSueDeCoq,

	/// <summary>
	/// Indicates avoidable rectangle guardian.
	/// </summary>
	AvoidableRectangleBrokenWing,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in block.
	/// </summary>
	AvoidableRectangleHiddenSingleBlock,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in row.
	/// </summary>
	AvoidableRectangleHiddenSingleRow,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in column.
	/// </summary>
	AvoidableRectangleHiddenSingleColumn,

	/// <summary>
	/// Indicates unique loop type 1.
	/// </summary>
	UniqueLoopType1,

	/// <summary>
	/// Indicates unique loop type 2.
	/// </summary>
	UniqueLoopType2,

	/// <summary>
	/// Indicates unique loop type 3.
	/// </summary>
	UniqueLoopType3,

	/// <summary>
	/// Indicates unique loop type 4.
	/// </summary>
	UniqueLoopType4,

	/// <summary>
	/// Indicates extended rectangle type 1.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0620")]
#endif
	ExtendedRectangleType1,

	/// <summary>
	/// Indicates extended rectangle type 2.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0621")]
#endif
	ExtendedRectangleType2,

	/// <summary>
	/// Indicates extended rectangle type 3.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0622")]
#endif
	ExtendedRectangleType3,

	/// <summary>
	/// Indicates extended rectangle type 4.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0623")]
#endif
	ExtendedRectangleType4,

	/// <summary>
	/// Indicates bi-value universal grave type 1.
	/// </summary>
	[HodokuTechniquePrefix("0610")]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates bi-value universal grave type 2.
	/// </summary>
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates bi-value universal grave type 3.
	/// </summary>
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates bi-value universal grave type 4.
	/// </summary>
	BivalueUniversalGraveType4,

	/// <summary>
	/// Indicates bi-value universal grave + n.
	/// </summary>
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Indicates bi-value universal grave false candidate type.
	/// </summary>
	BivalueUniversalGraveFalseCandidateType,

	/// <summary>
	/// Indicates bi-value universal grave + n with forcing chains.
	/// </summary>
	BivalueUniversalGravePlusNForcingChains,

	/// <summary>
	/// Indicates bi-value universal grave XZ rule.
	/// </summary>
	BivalueUniversalGraveXzRule,

	/// <summary>
	/// Indicates bi-value universal grave XY-Wing.
	/// </summary>
	BivalueUniversalGraveXyWing,

	/// <summary>
	/// Indicates reverse unique rectangle type 1.
	/// </summary>
	ReverseUniqueRectangleType1,

	/// <summary>
	/// Indicates reverse unique rectangle type 2.
	/// </summary>
	ReverseUniqueRectangleType2,

	/// <summary>
	/// Indicates reverse unique rectangle type 3.
	/// </summary>
	ReverseUniqueRectangleType3,

	/// <summary>
	/// Indicates reverse unique rectangle type 4.
	/// </summary>
	ReverseUniqueRectangleType4,

	/// <summary>
	/// Indicates reverse unique loop type 1.
	/// </summary>
	ReverseUniqueLoopType1,

	/// <summary>
	/// Indicates reverse unique loop type 2.
	/// </summary>
	ReverseUniqueLoopType2,

	/// <summary>
	/// Indicates reverse unique loop type 3.
	/// </summary>
	ReverseUniqueLoopType3,

	/// <summary>
	/// Indicates reverse unique loop type 4.
	/// </summary>
	ReverseUniqueLoopType4,

	/// <summary>
	/// Indicates uniqueness clue cover type 2.
	/// </summary>
	UniquenessClueCoverType2,

	/// <summary>
	/// Indicates uniqueness clue cover RW's type.
	/// </summary>
	UniquenessClueCoverRwType,

	/// <summary>
	/// Indicates unique polygon type 1.
	/// </summary>
	UniquePolygonType1,

	/// <summary>
	/// Indicates unique polygon type 2.
	/// </summary>
	UniquePolygonType2,

	/// <summary>
	/// Indicates unique polygon type 3.
	/// </summary>
	UniquePolygonType3,

	/// <summary>
	/// Indicates unique polygon type 4.
	/// </summary>
	UniquePolygonType4,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 1.
	/// </summary>
	QiuDeadlyPatternType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 2.
	/// </summary>
	QiuDeadlyPatternType2,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 3.
	/// </summary>
	QiuDeadlyPatternType3,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 4.
	/// </summary>
	QiuDeadlyPatternType4,

	/// <summary>
	/// Indicates locked Qiu's deadly pattern.
	/// </summary>
	LockedQiuDeadlyPattern,

	/// <summary>
	/// Indicates unique square type 1.
	/// </summary>
	UniqueSquareType1,

	/// <summary>
	/// Indicates unique square type 2.
	/// </summary>
	UniqueSquareType2,

	/// <summary>
	/// Indicates unique square type 3.
	/// </summary>
	UniqueSquareType3,

	/// <summary>
	/// Indicates unique square type 4.
	/// </summary>
	UniqueSquareType4,

	/// <summary>
	/// Indicates sue de coq.
	/// </summary>
	[HodokuTechniquePrefix("1101")]
	SueDeCoq,

	/// <summary>
	/// Indicates sue de coq with isolated digit.
	/// </summary>
	[HodokuTechniquePrefix("1101")]
	SueDeCoqIsolated,

	/// <summary>
	/// Indicates 3-dimensional sue de coq.
	/// </summary>
	SueDeCoq3Dimension,

	/// <summary>
	/// Indicates sue de coq cannibalism.
	/// </summary>
	SueDeCoqCannibalism,

	/// <summary>
	/// Indicates skyscraper.
	/// </summary>
	[HodokuTechniquePrefix("0400")]
	Skyscraper,

	/// <summary>
	/// Indicates two-string kite.
	/// </summary>
	[HodokuTechniquePrefix("0401")]
	TwoStringKite,

	/// <summary>
	/// Indicates turbot fish.
	/// </summary>
	[HodokuTechniquePrefix("0403")]
	TurbotFish,

	/// <summary>
	/// Indicates empty rectangle.
	/// </summary>
	[HodokuTechniquePrefix("0402")]
	EmptyRectangle,

	/// <summary>
	/// Indicates broken wing.
	/// </summary>
	[HodokuTechniquePrefix("0705")]
	BrokenWing,

	/// <summary>
	/// Indicates bi-value oddagon type 1.
	/// </summary>
	BivalueOddagonType1,

	/// <summary>
	/// Indicates bi-value oddagon type 2.
	/// </summary>
	BivalueOddagonType2,

	/// <summary>
	/// Indicates bi-value oddagon type 3.
	/// </summary>
	BivalueOddagonType3,

	/// <summary>
	/// Indicates grouped bi-value oddagon.
	/// </summary>
	GroupedBivalueOddagon,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 1.
	/// </summary>
	ChromaticPatternType1,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 2.
	/// </summary>
	ChromaticPatternType2,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 3.
	/// </summary>
	ChromaticPatternType3,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 4.
	/// </summary>
	ChromaticPatternType4,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) XZ rule.
	/// </summary>
	ChromaticPatternXzRule,

	/// <summary>
	/// Indicates X-Chain.
	/// </summary>
	[HodokuTechniquePrefix("0701")]
	XChain,

	/// <summary>
	/// Indicates Y-Chain.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0702")]
#endif
	YChain,

	/// <summary>
	/// Indicates fishy cycle (X-Cycle).
	/// </summary>
	[HodokuTechniquePrefix("0704")]
	FishyCycle,

	/// <summary>
	/// Indicates XY-Chain.
	/// </summary>
	[HodokuTechniquePrefix("0702")]
	XyChain,

	/// <summary>
	/// Indicates XY-Cycle.
	/// </summary>
	XyCycle,

	/// <summary>
	/// Indicates XY-X-Chain.
	/// </summary>
	XyXChain,

	/// <summary>
	/// Indicates remote pair.
	/// </summary>
	[HodokuTechniquePrefix("0703")]
	RemotePair,

	/// <summary>
	/// Indicates purple cow.
	/// </summary>
	PurpleCow,

	/// <summary>
	/// Indicates discontinuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0707")]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates continuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0706")]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates alternating inference chain.
	/// </summary>
	[HodokuTechniquePrefix("0708")]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates grouped X-Chain.
	/// </summary>
	GroupedXChain,

	/// <summary>
	/// Indicates grouped fishy cycle (grouped X-Cycle).
	/// </summary>
	GroupedFishyCycle,

	/// <summary>
	/// Indicates grouped XY-Chain.
	/// </summary>
	GroupedXyChain,

	/// <summary>
	/// Indicates grouped XY-Cycle.
	/// </summary>
	GroupedXyCycle,

	/// <summary>
	/// Indicates grouped XY-X-Chain.
	/// </summary>
	GroupedXyXChain,

	/// <summary>
	/// Indicates grouped purple cow.
	/// </summary>
	GroupedPurpleCow,

	/// <summary>
	/// Indicates grouped discontinuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0710")]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates grouped continuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0709")]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates grouped alternating inference chain.
	/// </summary>
	[HodokuTechniquePrefix("0711")]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates special case that a grouped alternating inference chain has a collision
	/// between start and end node.
	/// </summary>
	NodeCollision,

	/// <summary>
	/// Indicates nishio forcing chains.
	/// </summary>
	NishioForcingChains,

	/// <summary>
	/// Indicates region forcing chains (i.e. house forcing chains).
	/// </summary>
	[HodokuTechniquePrefix("1301")]
	RegionForcingChains,

	/// <summary>
	/// Indicates cell forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1301")]
	CellForcingChains,

	/// <summary>
	/// Indicates dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	[HodokuTechniquePrefix("1303")]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates dynamic cell forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1303")]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates dynamic contradiction forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1304")]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates dynamic double forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1304")]
	DynamicDoubleForcingChains,

	/// <summary>
	/// Indicates dynamic forcing chains.
	/// </summary>
	DynamicForcingChains,

	/// <summary>
	/// Indicates empty rectangle intersection pair.
	/// </summary>
	EmptyRectangleIntersectionPair,

	/// <summary>
	/// Indicates extended subset principle.
	/// </summary>
	[HodokuTechniquePrefix("1102")]
	ExtendedSubsetPrinciple,

	/// <summary>
	/// Indicates singly linked ALS-XZ.
	/// </summary>
	[HodokuTechniquePrefix("0901")]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates doubly linked ALS-XZ.
	/// </summary>
	[HodokuTechniquePrefix("0901")]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates ALS-XY-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0902")]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates ALS-W-Wing.
	/// </summary>
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates ALS chain.
	/// </summary>
	[HodokuTechniquePrefix("0903")]
	AlmostLockedSetsChain,

	/// <summary>
	/// Indicates AHS chain.
	/// </summary>
	AlmostHiddenSetsChain,

	/// <summary>
	/// Indicates death blossom.
	/// </summary>
	[HodokuTechniquePrefix("0904")]
	DeathBlossom,

	/// <summary>
	/// Indicates Gurth's symmetrical placement.
	/// </summary>
	GurthSymmetricalPlacement,

	/// <summary>
	/// Indicates extended Gurth's symmetrical placement.
	/// </summary>
	ExtendedGurthSymmetricalPlacement,

	/// <summary>
	/// Indicates junior exocet.
	/// </summary>
	JuniorExocet,

	/// <summary>
	/// Indicates senior exocet.
	/// </summary>
	SeniorExocet,

	/// <summary>
	/// Indicates complex senior exocet.
	/// </summary>
	ComplexSeniorExocet,

	/// <summary>
	/// Indicates siamese junior exocet.
	/// </summary>
	SiameseJuniorExocet,

	/// <summary>
	/// Indicates siamese senior exocet.
	/// </summary>
	SiameseSeniorExocet,

	/// <summary>
	/// Indicates domino loop.
	/// </summary>
	DominoLoop,

	/// <summary>
	/// Indicates multi-sector locked sets.
	/// </summary>
	MultisectorLockedSets,

	/// <summary>
	/// Indicates pattern overlay method.
	/// </summary>
	PatternOverlay,

	/// <summary>
	/// Indicates template set.
	/// </summary>
	[HodokuTechniquePrefix("1201")]
	TemplateSet,

	/// <summary>
	/// Indicates template delete.
	/// </summary>
	[HodokuTechniquePrefix("1202")]
	TemplateDelete,

	/// <summary>
	/// Indicates bowman's bingo.
	/// </summary>
	BowmanBingo,

	/// <summary>
	/// Indicates brute force.
	/// </summary>
	BruteForce,
}
