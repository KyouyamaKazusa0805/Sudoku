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
	/// Indicates full house.
	/// </summary>
	[HodokuTechniquePrefix("0000")]
	[HodokuDifficultyRating(4, HodokuDifficultyLevel.Easy)]
	[SudokuExplainerDifficultyRating(1.0)]
	[SudokuExplainerAliasedNames("Single")]
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
	[HodokuDifficultyRating(14, HodokuDifficultyLevel.Easy)]
	[SudokuExplainerDifficultyRating(1.2)]
	HiddenSingleBlock,

	/// <summary>
	/// Indicates hidden single (in row).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	[HodokuDifficultyRating(14, HodokuDifficultyLevel.Easy)]
	[SudokuExplainerDifficultyRating(1.5)]
	HiddenSingleRow,

	/// <summary>
	/// Indicates hidden single (in column).
	/// </summary>
	[HodokuTechniquePrefix("0002")]
	[HodokuDifficultyRating(14, HodokuDifficultyLevel.Easy)]
	[SudokuExplainerDifficultyRating(1.5)]
	HiddenSingleColumn,

	/// <summary>
	/// Indicates naked single.
	/// </summary>
	[HodokuTechniquePrefix("0003")]
	[HodokuDifficultyRating(4, HodokuDifficultyLevel.Easy)]
	[SudokuExplainerDifficultyRating(2.3)]
	NakedSingle,

	/// <summary>
	/// Indicates pointing.
	/// </summary>
	[HodokuTechniquePrefix("0100")]
	[HodokuDifficultyRating(50, HodokuDifficultyLevel.Medium)]
	[HodokuAliasedNames("Locked Candidates Type 1")]
	[SudokuExplainerDifficultyRating(2.6)]
	Pointing,

	/// <summary>
	/// Indicates claiming.
	/// </summary>
	[HodokuTechniquePrefix("0101")]
	[HodokuDifficultyRating(50, HodokuDifficultyLevel.Medium)]
	[HodokuAliasedNames("Locked Candidates Type 2")]
	[SudokuExplainerDifficultyRating(2.8)]
	Claiming,

	/// <summary>
	/// Indicates almost locked pair.
	/// </summary>
	[SudokuExplainerDifficultyRating(4.5, IsAdvancedDefined = true)]
	AlmostLockedPair,

	/// <summary>
	/// Indicates almost locked triple.
	/// </summary>
	[SudokuExplainerDifficultyRating(5.2, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(60, HodokuDifficultyLevel.Medium)]
	[SudokuExplainerDifficultyRating(3.0)]
	NakedPair,

	/// <summary>
	/// Indicates naked pair plus (naked pair (+)).
	/// </summary>
	NakedPairPlus,

	/// <summary>
	/// Indicates locked pair.
	/// </summary>
	[HodokuTechniquePrefix("0110")]
	[HodokuDifficultyRating(40, HodokuDifficultyLevel.Medium)]
#if false
	[SudokuExplainerDifficultyRating(2.0)]
	[SudokuExplainerAliasNames("Direct Hidden Pair")]
#endif
	LockedPair,

	/// <summary>
	/// Indicates hidden pair.
	/// </summary>
	[HodokuTechniquePrefix("0210")]
	[HodokuDifficultyRating(70, HodokuDifficultyLevel.Medium)]
	[SudokuExplainerDifficultyRating(3.4)]
	HiddenPair,

	/// <summary>
	/// Indicates naked triple.
	/// </summary>
	[HodokuTechniquePrefix("0201")]
	[HodokuDifficultyRating(80, HodokuDifficultyLevel.Medium)]
	[SudokuExplainerDifficultyRating(3.6)]
	[SudokuExplainerAliasedNames("Naked Triplet")]
	NakedTriple,

	/// <summary>
	/// Indicates naked triple plus (naked triple (+)).
	/// </summary>
	NakedTriplePlus,

	/// <summary>
	/// Indicates locked triple.
	/// </summary>
	[HodokuTechniquePrefix("0111")]
	[HodokuDifficultyRating(60, HodokuDifficultyLevel.Medium)]
#if false
	[SudokuExplainerDifficultyRating(2.5)]
	[SudokuExplainerAliasNames("Direct Hidden Triplet")]
#endif
	LockedTriple,

	/// <summary>
	/// Indicates hidden triple.
	/// </summary>
	[HodokuTechniquePrefix("0211")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Medium)]
	[SudokuExplainerDifficultyRating(4.0)]
	[SudokuExplainerAliasedNames("Hidden Triplet")]
	HiddenTriple,

	/// <summary>
	/// Indicates naked quadruple.
	/// </summary>
	[HodokuTechniquePrefix("0202")]
	[HodokuDifficultyRating(120, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(5.0)]
	[SudokuExplainerAliasedNames("Naked Quad")]
	NakedQuadruple,

	/// <summary>
	/// Indicates naked quadruple plus (naked quadruple (+)).
	/// </summary>
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates hidden quadruple.
	/// </summary>
	[HodokuTechniquePrefix("0212")]
	[HodokuDifficultyRating(150, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(5.4)]
	[SudokuExplainerAliasedNames("Hidden Quad")]
	HiddenQuadruple,

	/// <summary>
	/// Indicates X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0300")]
	[HodokuDifficultyRating(140, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(3.2)]
	XWing,

	/// <summary>
	/// Indicates finned X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0310")]
	[HodokuDifficultyRating(130, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(3.4, IsAdvancedDefined = true)]
	FinnedXWing,

	/// <summary>
	/// Indicates sashimi X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0320")]
	[HodokuDifficultyRating(150, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(3.5, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(300, HodokuDifficultyLevel.Unfair)]
	FrankenXWing,

	/// <summary>
	/// Indicates finned franken X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0340")]
	[HodokuDifficultyRating(390, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(450, HodokuDifficultyLevel.Extreme)]
	MutantXWing,

	/// <summary>
	/// Indicates finned mutant X-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0360")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(150, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(3.8)]
	Swordfish,

	/// <summary>
	/// Indicates finned swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0311")]
	[HodokuDifficultyRating(200, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(4.0, IsAdvancedDefined = true)]
	FinnedSwordfish,

	/// <summary>
	/// Indicates sashimi swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0321")]
	[HodokuDifficultyRating(240, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(4.1, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(350, HodokuDifficultyLevel.Unfair)]
	FrankenSwordfish,

	/// <summary>
	/// Indicates finned franken swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0341")]
	[HodokuDifficultyRating(410, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(450, HodokuDifficultyLevel.Extreme)]
	MutantSwordfish,

	/// <summary>
	/// Indicates finned mutant swordfish.
	/// </summary>
	[HodokuTechniquePrefix("0361")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(160, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(5.2)]
	Jellyfish,

	/// <summary>
	/// Indicates finned jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0312")]
	[HodokuDifficultyRating(250, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(5.4, IsAdvancedDefined = true)]
	FinnedJellyfish,

	/// <summary>
	/// Indicates sashimi jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0322")]
	[HodokuDifficultyRating(260, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(5.6, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(370, HodokuDifficultyLevel.Unfair)]
	FrankenJellyfish,

	/// <summary>
	/// Indicates finned franken jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0342")]
	[HodokuDifficultyRating(430, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(450, HodokuDifficultyLevel.Extreme)]
	MutantJellyfish,

	/// <summary>
	/// Indicates finned mutant jellyfish.
	/// </summary>
	[HodokuTechniquePrefix("0362")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
	Squirmbag,

	/// <summary>
	/// Indicates finned squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0313")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
	FinnedSquirmbag,

	/// <summary>
	/// Indicates sashimi squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0323")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
	FrankenSquirmbag,

	/// <summary>
	/// Indicates finned franken squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0343")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
	MutantSquirmbag,

	/// <summary>
	/// Indicates finned mutant squirmbag.
	/// </summary>
	[HodokuTechniquePrefix("0363")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
	Whale,

	/// <summary>
	/// Indicates finned whale.
	/// </summary>
	[HodokuTechniquePrefix("0314")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
	FinnedWhale,

	/// <summary>
	/// Indicates sashimi whale.
	/// </summary>
	[HodokuTechniquePrefix("0324")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
	FrankenWhale,

	/// <summary>
	/// Indicates finned franken whale.
	/// </summary>
	[HodokuTechniquePrefix("0344")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
	MutantWhale,

	/// <summary>
	/// Indicates finned mutant whale.
	/// </summary>
	[HodokuTechniquePrefix("0364")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
	Leviathan,

	/// <summary>
	/// Indicates finned leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0315")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
	FinnedLeviathan,

	/// <summary>
	/// Indicates sashimi leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0325")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
	FrankenLeviathan,

	/// <summary>
	/// Indicates finned franken leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0345")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
	MutantLeviathan,

	/// <summary>
	/// Indicates finned mutant leviathan.
	/// </summary>
	[HodokuTechniquePrefix("0365")]
	[HodokuDifficultyRating(470, HodokuDifficultyLevel.Extreme)]
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
	[HodokuDifficultyRating(160, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(4.2)]
	XyWing,

	/// <summary>
	/// Indicates XYZ-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0801")]
	[HodokuDifficultyRating(180, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(4.4)]
	XyzWing,

	/// <summary>
	/// Indicates WXYZ-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0802")]
	[SudokuExplainerDifficultyRating(4.6, IsAdvancedDefined = true)]
	WxyzWing,

	/// <summary>
	/// Indicates VWXYZ-Wing.
	/// </summary>
	[SudokuExplainerDifficultyRating(double.NaN, IsAdvancedDefined = true)]
	VwxyzWing,

	/// <summary>
	/// Indicates UVWXYZ-Wing.
	/// </summary>
	[SudokuExplainerDifficultyRating(double.NaN, IsAdvancedDefined = true)]
	UvwxyzWing,

	/// <summary>
	/// Indicates TUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainerDifficultyRating(double.NaN, IsAdvancedDefined = true)]
	TuvwxyzWing,

	/// <summary>
	/// Indicates STUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainerDifficultyRating(double.NaN, IsAdvancedDefined = true)]
	StuvwxyzWing,

	/// <summary>
	/// Indicates RSTUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainerDifficultyRating(double.NaN, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(150, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(4.4, IsAdvancedDefined = true)]
	WWing,

	/// <summary>
	/// Indicates Multi-Branch W-Wing.
	/// </summary>
	MultiBranchWWing,

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
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Uniqueness Test 1")]
	[SudokuExplainerDifficultyRating(4.5)]
	UniqueRectangleType1,

	/// <summary>
	/// Indicates unique rectangle type 2.
	/// </summary>
	[HodokuTechniquePrefix("0601")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Uniqueness Test 2")]
	[SudokuExplainerDifficultyRating(4.5)]
	[SudokuExplainerDifficultyRating(4.6, IsAdvancedDefined = true)]
	UniqueRectangleType2,

	/// <summary>
	/// Indicates unique rectangle type 3.
	/// </summary>
	[HodokuTechniquePrefix("0602")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Uniqueness Test 3")]
	[SudokuExplainerDifficultyRating(4.5, 4.8)]
	[SudokuExplainerDifficultyRating(4.6, 4.9, IsAdvancedDefined = true)]
	UniqueRectangleType3,

	/// <summary>
	/// Indicates unique rectangle type 4.
	/// </summary>
	[HodokuTechniquePrefix("0603")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Uniqueness Test 4")]
	[SudokuExplainerDifficultyRating(4.5)]
	[SudokuExplainerDifficultyRating(4.6, IsAdvancedDefined = true)]
	UniqueRectangleType4,

	/// <summary>
	/// Indicates unique rectangle type 5.
	/// </summary>
	[HodokuTechniquePrefix("0604")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Uniqueness Test 5")]
	[SudokuExplainerDifficultyRating(4.6, IsAdvancedDefined = true)]
	UniqueRectangleType5,

	/// <summary>
	/// Indicates unique rectangle type 6.
	/// </summary>
	[HodokuTechniquePrefix("0605")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Uniqueness Test 6")]
	[SudokuExplainerDifficultyRating(4.6, IsAdvancedDefined = true)]
	UniqueRectangleType6,

	/// <summary>
	/// Indicates hidden unique rectangle.
	/// </summary>
	[HodokuTechniquePrefix("0606")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Hidden Rectangle")]
	[SudokuExplainerDifficultyRating(4.8, IsAdvancedDefined = true)]
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
	/// Indicates unique rectangle external skyscraper.
	/// </summary>
	UniqueRectangleExternalSkyscraper,

	/// <summary>
	/// Indicates unique rectangle external two-string kite.
	/// </summary>
	UniqueRectangleExternalTwoStringKite,

	/// <summary>
	/// Indicates unique rectangle external turbot fish.
	/// </summary>
	UniqueRectangleExternalTurbotFish,

	/// <summary>
	/// Indicates unique rectangle external XY-Wing.
	/// </summary>
	UniqueRectangleExternalXyWing,

	/// <summary>
	/// Indicates unique rectangle external almost locked sets XZ rule.
	/// </summary>
	UniqueRectangleExternalAlmostLockedSetsXz,

	/// <summary>
	/// Indicates avoidable rectangle type 1.
	/// </summary>
	[HodokuTechniquePrefix("0607")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(4.7, IsAdvancedDefined = true)] // I think this difficulty may be a mistake.
	AvoidableRectangleType1,

	/// <summary>
	/// Indicates avoidable rectangle type 2.
	/// </summary>
	[HodokuTechniquePrefix("0608")]
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(4.5, IsAdvancedDefined = true)] // I think this difficulty may be a mistake.
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
	/// Indicates avoidable rectangle external type 1.
	/// </summary>
	AvoidableRectangleExternalType1,

	/// <summary>
	/// Indicates avoidable rectangle external type 2.
	/// </summary>
	AvoidableRectangleExternalType2,

	/// <summary>
	/// Indicates avoidable rectangle external type 3.
	/// </summary>
	AvoidableRectangleExternalType3,

	/// <summary>
	/// Indicates avoidable rectangle external type 4.
	/// </summary>
	AvoidableRectangleExternalType4,

	/// <summary>
	/// Indicates avoidable rectangle external skyscraper.
	/// </summary>
	AvoidableRectangleExternalSkyscraper,

	/// <summary>
	/// Indicates avoidable rectangle external two-string kite.
	/// </summary>
	AvoidableRectangleExternalTwoStringKite,

	/// <summary>
	/// Indicates avoidable rectangle external turbot fish.
	/// </summary>
	AvoidableRectangleExternalTurbotFish,

	/// <summary>
	/// Indicates avoidable rectangle external XY-Wing.
	/// </summary>
	AvoidableRectangleExternalXyWing,

	/// <summary>
	/// Indicates avoidable rectangle external almost locked sets XZ rule.
	/// </summary>
	AvoidableRectangleExternalAlmostLockedSetsXz,

	/// <summary>
	/// Indicates unique loop type 1.
	/// </summary>
	[SudokuExplainerDifficultyRating(4.6, 5.0)]
	UniqueLoopType1,

	/// <summary>
	/// Indicates unique loop type 2.
	/// </summary>
	[SudokuExplainerDifficultyRating(4.6, 5.0)]
	[SudokuExplainerDifficultyRating(4.7, 5.1, IsAdvancedDefined = true)]
	UniqueLoopType2,

	/// <summary>
	/// Indicates unique loop type 3.
	/// </summary>
	[SudokuExplainerDifficultyRating(4.6, 5.0)]
	[SudokuExplainerDifficultyRating(4.7, 5.1, IsAdvancedDefined = true)]
	UniqueLoopType3,

	/// <summary>
	/// Indicates unique loop type 4.
	/// </summary>
	[SudokuExplainerDifficultyRating(4.6, 5.0)]
	[SudokuExplainerDifficultyRating(4.7, 5.1, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(100, HodokuDifficultyLevel.Hard)]
	[HodokuAliasedNames("Bivalue Universal Grave + 1")]
	[SudokuExplainerDifficultyRating(5.6)]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates bi-value universal grave type 2.
	/// </summary>
	[SudokuExplainerDifficultyRating(5.7)]
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates bi-value universal grave type 3.
	/// </summary>
	[SudokuExplainerDifficultyRating(5.8, 6.1)]
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates bi-value universal grave type 4.
	/// </summary>
	[SudokuExplainerDifficultyRating(5.7)]
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
	/// Indicates RW's deadly pattern.
	/// </summary>
	RwDeadlyPattern,

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
	/// Indicates unique matrix type 1.
	/// </summary>
	UniqueMatrixType1,

	/// <summary>
	/// Indicates unique matrix type 2.
	/// </summary>
	UniqueMatrixType2,

	/// <summary>
	/// Indicates unique matrix type 3.
	/// </summary>
	UniqueMatrixType3,

	/// <summary>
	/// Indicates unique matrix type 4.
	/// </summary>
	UniqueMatrixType4,

	/// <summary>
	/// Indicates sue de coq.
	/// </summary>
	[HodokuTechniquePrefix("1101")]
	[HodokuDifficultyRating(250, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(5.0, IsAdvancedDefined = true)]
	SueDeCoq,

	/// <summary>
	/// Indicates sue de coq with isolated digit.
	/// </summary>
	[HodokuTechniquePrefix("1101")]
	[HodokuDifficultyRating(250, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(5.0, IsAdvancedDefined = true)]
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
	[HodokuDifficultyRating(130, HodokuDifficultyLevel.Hard)]
#if false
	[SudokuExplainerDifficultyRating(6.6)]
	[SudokuExplainerAliasNames("Turbot Fish")]
#endif
	Skyscraper,

	/// <summary>
	/// Indicates two-string kite.
	/// </summary>
	[HodokuTechniquePrefix("0401")]
	[HodokuDifficultyRating(150, HodokuDifficultyLevel.Hard)]
#if false
	[SudokuExplainerDifficultyRating(6.6)]
	[SudokuExplainerAliasNames("Turbot Fish")]
#endif
	TwoStringKite,

	/// <summary>
	/// Indicates turbot fish.
	/// </summary>
	[HodokuTechniquePrefix("0403")]
	[HodokuDifficultyRating(120, HodokuDifficultyLevel.Hard)]
	[SudokuExplainerDifficultyRating(6.6)]
	TurbotFish,

	/// <summary>
	/// Indicates empty rectangle.
	/// </summary>
	[HodokuTechniquePrefix("0402")]
	[HodokuDifficultyRating(120, HodokuDifficultyLevel.Hard)]
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
	[HodokuDifficultyRating(260, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(6.6, 6.9)]
	XChain,

	/// <summary>
	/// Indicates Y-Chain.
	/// </summary>
#if false
	[HodokuTechniquePrefix("0702")]
	[HodokuDifficultyRating(260, HodokuDifficultyLevel.Unfair)]
#endif
	YChain,

	/// <summary>
	/// Indicates fishy cycle (X-Cycle).
	/// </summary>
	[HodokuTechniquePrefix("0704")]
	[SudokuExplainerDifficultyRating(6.5, 6.6)]
	[SudokuExplainerAliasedNames("Bidirectional X-Cycle")]
	FishyCycle,

	/// <summary>
	/// Indicates XY-Chain.
	/// </summary>
	[HodokuTechniquePrefix("0702")]
	[HodokuDifficultyRating(260, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(110, HodokuDifficultyLevel.Hard)]
	RemotePair,

	/// <summary>
	/// Indicates purple cow.
	/// </summary>
	PurpleCow,

	/// <summary>
	/// Indicates discontinuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0707")]
	[HodokuDifficultyRating(280, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(7.0, 7.6)]
	[SudokuExplainerAliasedNames("Forcing Chain")]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates continuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0706")]
	[HodokuDifficultyRating(280, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(7.0, 7.3)]
	[SudokuExplainerAliasedNames("Bidirectional Cycle")]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates alternating inference chain.
	/// </summary>
	[HodokuTechniquePrefix("0708")]
	[HodokuDifficultyRating(280, HodokuDifficultyLevel.Unfair)]
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
	[HodokuDifficultyRating(300, HodokuDifficultyLevel.Unfair)]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates grouped continuous nice loop.
	/// </summary>
	[HodokuTechniquePrefix("0709")]
	[HodokuDifficultyRating(300, HodokuDifficultyLevel.Unfair)]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates grouped alternating inference chain.
	/// </summary>
	[HodokuTechniquePrefix("0711")]
	[HodokuDifficultyRating(300, HodokuDifficultyLevel.Unfair)]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates special case that a grouped alternating inference chain has a collision
	/// between start and end node.
	/// </summary>
	NodeCollision,

	/// <summary>
	/// Indicates nishio forcing chains.
	/// </summary>
	[SudokuExplainerDifficultyRating(7.6, 8.1)]
	NishioForcingChains,

	/// <summary>
	/// Indicates region forcing chains (i.e. house forcing chains).
	/// </summary>
	[HodokuTechniquePrefix("1301")]
	[HodokuDifficultyRating(500, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerDifficultyRating(8.2, 8.6)]
	RegionForcingChains,

	/// <summary>
	/// Indicates cell forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1301")]
	[HodokuDifficultyRating(500, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerDifficultyRating(8.2, 8.6)]
	CellForcingChains,

	/// <summary>
	/// Indicates dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	[HodokuTechniquePrefix("1303")]
	[HodokuDifficultyRating(500, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerDifficultyRating(8.6, 9.4)]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates dynamic cell forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1303")]
	[HodokuDifficultyRating(500, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerDifficultyRating(8.6, 9.4)]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates dynamic contradiction forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1304")]
	[HodokuDifficultyRating(500, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerDifficultyRating(8.8, 9.4)]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates dynamic double forcing chains.
	/// </summary>
	[HodokuTechniquePrefix("1304")]
	[HodokuDifficultyRating(500, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerDifficultyRating(8.8, 9.4)]
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
	[HodokuDifficultyRating(300, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(7.5, IsAdvancedDefined = true)]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates doubly linked ALS-XZ.
	/// </summary>
	[HodokuTechniquePrefix("0901")]
	[HodokuDifficultyRating(300, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(7.5, IsAdvancedDefined = true)]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates ALS-XY-Wing.
	/// </summary>
	[HodokuTechniquePrefix("0902")]
	[HodokuDifficultyRating(320, HodokuDifficultyLevel.Unfair)]
	[SudokuExplainerDifficultyRating(8.0, IsAdvancedDefined = true)]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates ALS-W-Wing.
	/// </summary>
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates ALS chain.
	/// </summary>
	[HodokuTechniquePrefix("0903")]
	[HodokuDifficultyRating(340, HodokuDifficultyLevel.Unfair)]
	AlmostLockedSetsChain,

	/// <summary>
	/// Indicates AHS chain.
	/// </summary>
	AlmostHiddenSetsChain,

	/// <summary>
	/// Indicates death blossom cell type.
	/// </summary>
	[HodokuTechniquePrefix("0904")]
	[HodokuDifficultyRating(360, HodokuDifficultyLevel.Unfair)]
	[HodokuAliasedNames("Death Blossom")]
	DeathBlossomCellType,

	/// <summary>
	/// Indicates death blossom house type.
	/// </summary>
	DeathBlossomHouseType,

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
	[HodokuDifficultyRating(10000, HodokuDifficultyLevel.Extreme)]
	TemplateSet,

	/// <summary>
	/// Indicates template delete.
	/// </summary>
	[HodokuTechniquePrefix("1202")]
	[HodokuDifficultyRating(10000, HodokuDifficultyLevel.Extreme)]
	TemplateDelete,

	/// <summary>
	/// Indicates bowman's bingo.
	/// </summary>
	BowmanBingo,

	/// <summary>
	/// Indicates brute force.
	/// </summary>
	[HodokuDifficultyRating(10000, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerAliasedNames("Try & Error")]
	BruteForce,
}
