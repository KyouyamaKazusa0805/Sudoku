using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Represents a technique instance, which is used for comparison.
	/// </summary>
	/*closed*/
	public enum TechniqueCode : short
	{
		/// <summary>
		/// The placeholder of this enumeration type.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the full house.
		/// </summary>
		[TechniqueDisplay("Full House")]
		FullHouse,

		/// <summary>
		/// Indicates the last digit.
		/// </summary>
		[TechniqueDisplay("Last Digit")]
		LastDigit,

		/// <summary>
		/// Indicates the hidden single.
		/// </summary>
		[TechniqueDisplay("Hidden Single")]
		HiddenSingleRow,

		/// <summary>
		/// Indicates the hidden single.
		/// </summary>
		[TechniqueDisplay("Hidden Single")]
		HiddenSingleColumn,

		/// <summary>
		/// Indicates the hidden single.
		/// </summary>
		[TechniqueDisplay("Hidden Single")]
		HiddenSingleBlock,

		/// <summary>
		/// Indicates the naked single.
		/// </summary>
		[TechniqueDisplay("Naked Single")]
		NakedSingle,

		/// <summary>
		/// Indicates the pointing.
		/// </summary>
		[TechniqueDisplay("Pointing")]
		Pointing,

		/// <summary>
		/// Indicates the claiming.
		/// </summary>
		[TechniqueDisplay("Claiming")]
		Claiming,

		/// <summary>
		/// Indicates the ALP.
		/// </summary>
		[TechniqueDisplay("Almost Locked Pair")]
		AlmostLockedPair,

		/// <summary>
		/// Indicates the ALT.
		/// </summary>
		[TechniqueDisplay("Almost Locked Triple")]
		AlmostLockedTriple,

		/// <summary>
		/// Indicates the ALQ.
		/// </summary>
		[TechniqueDisplay("Almost Locked Quadruple")]
		AlmostLockedQuadruple,

		/// <summary>
		/// Indicates the naked pair.
		/// </summary>
		[TechniqueDisplay("Naked Pair")]
		NakedPair,

		/// <summary>
		/// Indicates the naked pair plus (naked pair (+)).
		/// </summary>
		[TechniqueDisplay("Naked Pair (+)")]
		NakedPairPlus,

		/// <summary>
		/// Indicates the locked pair.
		/// </summary>
		[TechniqueDisplay("Locked Pair")]
		LockedPair,

		/// <summary>
		/// Indicates the hidden pair.
		/// </summary>
		[TechniqueDisplay("Hidden Pair")]
		HiddenPair,

		/// <summary>
		/// Indicates the naked triple.
		/// </summary>
		[TechniqueDisplay("Naked Triple")]
		NakedTriple,

		/// <summary>
		/// Indicates the naked triple plus (naked triple (+)).
		/// </summary>
		[TechniqueDisplay("Naked Triple (+)")]
		NakedTriplePlus,

		/// <summary>
		/// Indicates the locked triple.
		/// </summary>
		[TechniqueDisplay("Locked Triple")]
		LockedTriple,

		/// <summary>
		/// Indicates the hidden triple.
		/// </summary>
		[TechniqueDisplay("Hidden Triple")]
		HiddenTriple,

		/// <summary>
		/// Indicates the naked quadruple.
		/// </summary>
		[TechniqueDisplay("Naked Quadruple")]
		NakedQuadruple,

		/// <summary>
		/// Indicates the naked quadruple plus (naked quadruple (+)).
		/// </summary>
		[TechniqueDisplay("Naked Quadruple (+)")]
		NakedQuadruplePlus,

		/// <summary>
		/// Indicates the hidden quadruple.
		/// </summary>
		[TechniqueDisplay("Hidden Quadruple")]
		HiddenQuadruple,

		/// <summary>
		/// Indicates the X-Wing.
		/// </summary>
		[TechniqueDisplay("X-Wing")]
		XWing,

		/// <summary>
		/// Indicates the finned X-Wing.
		/// </summary>
		[TechniqueDisplay("Finned X-Wing")]
		FinnedXWing,

		/// <summary>
		/// Indicates the sashimi X-Wing.
		/// </summary>
		[TechniqueDisplay("Sashimi X-Wing")]
		SashimiXWing,

		/// <summary>
		/// Indicates the siamese finned X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned X-Wing")]
		SiameseFinnedXWing,

		/// <summary>
		/// Indicates the siamese sashimi X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi X-Wing")]
		SiameseSashimiXWing,

		/// <summary>
		/// Indicates the franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Franken X-Wing")]
		FrankenXWing,

		/// <summary>
		/// Indicates the finned franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Finned Franken X-Wing")]
		FinnedFrankenXWing,

		/// <summary>
		/// Indicates the sashimi franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Sashimi Franken X-Wing")]
		SashimiFrankenXWing,

		/// <summary>
		/// Indicates the siamese finned franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Franken X-Wing")]
		SiameseFinnedFrankenXWing,

		/// <summary>
		/// Indicates the siamese sashimi franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Franken X-Wing")]
		SiameseSashimiFrankenXWing,

		/// <summary>
		/// Indicates the mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Mutant X-Wing")]
		MutantXWing,

		/// <summary>
		/// Indicates the finned mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Finned Mutant X-Wing")]
		FinnedMutantXWing,

		/// <summary>
		/// Indicates the sashimi mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Sashimi Mutant X-Wing")]
		SashimiMutantXWing,

		/// <summary>
		/// Indicates the siamese finned mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Mutant X-Wing")]
		SiameseFinnedMutantXWing,

		/// <summary>
		/// Indicates the siamese sashimi mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Mutant X-Wing")]
		SiameseSashimiMutantXWing,

		/// <summary>
		/// Indicates the swordfish.
		/// </summary>
		[TechniqueDisplay("Swordfish")]
		Swordfish,

		/// <summary>
		/// Indicates the finned swordfish.
		/// </summary>
		[TechniqueDisplay("Finned Swordfish")]
		FinnedSwordfish,

		/// <summary>
		/// Indicates the sashimi swordfish.
		/// </summary>
		[TechniqueDisplay("Sashimi Swordfish")]
		SashimiSwordfish,

		/// <summary>
		/// Indicates the siamese finned swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Swordfish")]
		SiameseFinnedSwordfish,

		/// <summary>
		/// Indicates the siamese sashimi swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Swordfish")]
		SiameseSashimiSwordfish,

		/// <summary>
		/// Indicates the swordfish.
		/// </summary>
		//[TechniqueDisplay("Franken Swordfish")]
		FrankenSwordfish,

		/// <summary>
		/// Indicates the finned franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Finned Franken Swordfish")]
		FinnedFrankenSwordfish,

		/// <summary>
		/// Indicates the sashimi franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Sashimi Franken Swordfish")]
		SashimiFrankenSwordfish,

		/// <summary>
		/// Indicates the siamese finned franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Franken Swordfish")]
		SiameseFinnedFrankenSwordfish,

		/// <summary>
		/// Indicates the siamese sashimi franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Franken Swordfish")]
		SiameseSashimiFrankenSwordfish,

		/// <summary>
		/// Indicates the mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Mutant Swordfish")]
		MutantSwordfish,

		/// <summary>
		/// Indicates the finned mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Finned Mutant Swordfish")]
		FinnedMutantSwordfish,

		/// <summary>
		/// Indicates the sashimi mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Sashimi Mutant Swordfish")]
		SashimiMutantSwordfish,

		/// <summary>
		/// Indicates the siamese finned mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Mutant Swordfish")]
		SiameseFinnedMutantSwordfish,

		/// <summary>
		/// Indicates the siamese sashimi mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Mutant Swordfish")]
		SiameseSashimiMutantSwordfish,

		/// <summary>
		/// Indicates the jellyfish.
		/// </summary>
		[TechniqueDisplay("Jellyfish")]
		Jellyfish,

		/// <summary>
		/// Indicates the finned jellyfish.
		/// </summary>
		[TechniqueDisplay("Finned Jellyfish")]
		FinnedJellyfish,

		/// <summary>
		/// Indicates the sashimi jellyfish.
		/// </summary>
		[TechniqueDisplay("Sashimi Jellyfish")]
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
		[TechniqueDisplay("XY-Wing")]
		XyWing,

		/// <summary>
		/// Indicates the XYZ-Wing.
		/// </summary>
		[TechniqueDisplay("XYZ-Wing")]
		XyzWing,

		/// <summary>
		/// Indicates the WXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("WXYZ-Wing")]
		WxyzWing,

		/// <summary>
		/// Indicates the VWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("VWXYZ-Wing")]
		VwxyzWing,

		/// <summary>
		/// Indicates the UVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("UVWXYZ-Wing")]
		UvwxyzWing,

		/// <summary>
		/// Indicates the TUVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("TUVWXYZ-Wing")]
		TuvwxyzWing,

		/// <summary>
		/// Indicates the STUVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("STUVWXYZ-Wing")]
		StuvwxyzWing,

		/// <summary>
		/// Indicates the RSTUVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("RSTUVWXYZ-Wing")]
		RstuvwxyzWing,

		/// <summary>
		/// Indicates the incomplete WXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Incomplete WXYZ-Wing")]
		IncompleteWxyzWing,

		/// <summary>
		/// Indicates the incomplete VWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Incomplete VWXYZ-Wing")]
		IncompleteVwxyzWing,

		/// <summary>
		/// Indicates the incomplete UVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Incomplete UVWXYZ-Wing")]
		IncompleteUvwxyzWing,

		/// <summary>
		/// Indicates the incomplete TUVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Incomplete TUVWXYZ-Wing")]
		IncompleteTuvwxyzWing,

		/// <summary>
		/// Indicates the incomplete STUVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Incomplete STUVWXYZ-Wing")]
		IncompleteStuvwxyzWing,

		/// <summary>
		/// Indicates the incomplete RSTUVWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Incomplete RSTUVWXYZ-Wing")]
		IncompleteRstuvwxyzWing,

		/// <summary>
		/// Indicates the W-Wing.
		/// </summary>
		[TechniqueDisplay("W-Wing")]
		WWing,

		/// <summary>
		/// Indicates the M-Wing.
		/// </summary>
		[TechniqueDisplay("M-Wing")]
		MWing,

		/// <summary>
		/// Indicates the local wing.
		/// </summary>
		[TechniqueDisplay("Local Wing")]
		LocalWing,

		/// <summary>
		/// Indicates the split wing.
		/// </summary>
		[TechniqueDisplay("Split Wing")]
		SplitWing,

		/// <summary>
		/// Indicates the hybrid wing.
		/// </summary>
		[TechniqueDisplay("Hybrid Wing")]
		HybridWing,

		/// <summary>
		/// Indicates the grouped XY-Wing.
		/// </summary>
		GroupedXyWing,

		/// <summary>
		/// Indicates the grouped W-Wing.
		/// </summary>
		[TechniqueDisplay("Grouped W-Wing")]
		GroupedWWing,

		/// <summary>
		/// Indicates the grouped M-Wing.
		/// </summary>
		[TechniqueDisplay("Grouped M-Wing")]
		GroupedMWing,

		/// <summary>
		/// Indicates the grouped local wing.
		/// </summary>
		[TechniqueDisplay("Grouped Local Wing")]
		GroupedLocalWing,

		/// <summary>
		/// Indicates the grouped split wing.
		/// </summary>
		[TechniqueDisplay("Grouped Split Wing")]
		GroupedSplitWing,

		/// <summary>
		/// Indicates the grouped hybrid wing.
		/// </summary>
		[TechniqueDisplay("Grouped Hybrid Wing")]
		GroupedHybridWing,

		/// <summary>
		/// Indicates the UR type 1.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 1")]
		UrType1,

		/// <summary>
		/// Indicates the UR type 2.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 2")]
		UrType2,

		/// <summary>
		/// Indicates the UR type 3.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 3")]
		UrType3,

		/// <summary>
		/// Indicates the UR type 4.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 4")]
		UrType4,

		/// <summary>
		/// Indicates the UR type 5.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 5")]
		UrType5,

		/// <summary>
		/// Indicates the UR type 6.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 6")]
		UrType6,

		/// <summary>
		/// Indicates the hidden UR.
		/// </summary>
		[TechniqueDisplay("Hidden Unique Rectangle")]
		HiddenUr,

		/// <summary>
		/// Indicates the UR + 2D.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 2D")]
		UrPlus2D,

		/// <summary>
		/// Indicates the UR + 2B / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 2B / 1SL")]
		UrPlus2B1SL,

		/// <summary>
		/// Indicates the UR + 2D / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 2D / 1SL")]
		UrPlus2D1SL,

		/// <summary>
		/// Indicates the UR + 3X.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3X")]
		UrPlus3X,

		/// <summary>
		/// Indicates the UR + 3x / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3x / 1SL")]
		UrPlus3x1SL_Lower,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3X / 1SL")]
		UrPlus3X1SL_Upper,

		/// <summary>
		/// Indicates the UR + 3X / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3X / 2SL")]
		UrPlus3X2SL,

		/// <summary>
		/// Indicates the UR + 3N / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3N / 2SL")]
		UrPlus3N2SL,

		/// <summary>
		/// Indicates the UR + 3U / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3U / 2SL")]
		UrPlus3U2SL,

		/// <summary>
		/// Indicates the UR + 3E / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3E / 2SL")]
		UrPlus3E2SL,

		/// <summary>
		/// Indicates the UR + 4x / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4x / 1SL")]
		UrPlus4x1SL_Lower,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4X / 1SL")]
		UrPlus4X1SL_Upper,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4x / 2SL")]
		UrPlus4x2SL_Lower,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4X / 2SL")]
		UrPlus4X2SL_Upper,

		/// <summary>
		/// Indicates the UR + 4X / 3SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4X / 3SL")]
		UrPlus4X3SL,

		/// <summary>
		/// Indicates the UR + 4C / 3SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4C / 3SL")]
		UrPlus4C3SL,

		/// <summary>
		/// Indicates the UR-XY-Wing.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + XY-Wing")]
		UrXyWing,

		/// <summary>
		/// Indicates the UR-XYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + XYZ-Wing")]
		UrXyzWing,

		/// <summary>
		/// Indicates the UR-WXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + WXYZ-Wing")]
		UrWxyzWing,

		/// <summary>
		/// Indicates the UR sue de coq.
		/// </summary>
		//[TechniqueDisplay("Unique Rectangle + Sue de Coq")]
		UrSdc,

		/// <summary>
		/// Indicates the AR type 1.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle Type 1")]
		ArType1,

		/// <summary>
		/// Indicates the AR type 2.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle Type 2")]
		ArType2,

		/// <summary>
		/// Indicates the AR type 3.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle Type 3")]
		ArType3,

		/// <summary>
		/// Indicates the AR type 5.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle Type 5")]
		ArType5,

		/// <summary>
		/// Indicates the hidden AR.
		/// </summary>
		[TechniqueDisplay("Avoidable Unique Rectangle")]
		HiddenAr,

		/// <summary>
		/// Indicates the AR + 2D.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle + 2D")]
		ArPlus2D,

		/// <summary>
		/// Indicates the AR + 3X.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle + 3X")]
		ArPlus3X,

		/// <summary>
		/// Indicates the AR-XY-Wing.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle + XY-Wing")]
		ArXyWing,

		/// <summary>
		/// Indicates the AR-XYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle + XYZ-Wing")]
		ArXyzWing,

		/// <summary>
		/// Indicates the AR-WXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Avoidable Rectangle + WXYZ-Wing")]
		ArWxyzWing,

		/// <summary>
		/// Indicates the AR sue de coq.
		/// </summary>
		//[TechniqueDisplay("Avoidable Rectangle + Sue de Coq")]
		ArSdc,

		/// <summary>
		/// Indicates the UL type 1.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 1")]
		UlType1,

		/// <summary>
		/// Indicates the UL type 2.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 2")]
		UlType2,

		/// <summary>
		/// Indicates the UL type 3.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 3")]
		UlType3,

		/// <summary>
		/// Indicates the UL type 4.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 4")]
		UlType4,

		/// <summary>
		/// Indicates the XR type 1.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 1")]
		XrType1,

		/// <summary>
		/// Indicates the XR type 2.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 2")]
		XrType2,

		/// <summary>
		/// Indicates the XR type 3.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 3")]
		XrType3,

		/// <summary>
		/// Indicates the XR type 4.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 4")]
		XrType4,

		/// <summary>
		/// Indicates the BUG type 1.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 1")]
		BugType1,

		/// <summary>
		/// Indicates the BUG type 2.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 2")]
		BugType2,

		/// <summary>
		/// Indicates the BUG type 3.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 3")]
		BugType3,

		/// <summary>
		/// Indicates the BUG type 4.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 4")]
		BugType4,

		/// <summary>
		/// Indicates the BUG + n.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave + n")]
		BugMultiple,

		/// <summary>
		/// Indicates the BUG + n with forcing chains.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave + n (+)")]
		BugMultipleFc,

		/// <summary>
		/// Indicates the BUG-XZ.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave XZ Rule")]
		BugXz,

		/// <summary>
		/// Indicates the BUG-XY-Wing.
		/// </summary>
		//[TechniqueDisplay("Bivalue Universal Grave XYZ-Wing")]
		BugXyzWing,

		/// <summary>
		/// Indicates the BDP type 1.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 1")]
		BdpType1,

		/// <summary>
		/// Indicates the BDP type 2.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 2")]
		BdpType2,

		/// <summary>
		/// Indicates the BDP type 3.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 3")]
		BdpType3,

		/// <summary>
		/// Indicates the BDP type 4.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 4")]
		BdpType4,

		/// <summary>
		/// Indicates the QDP type 1.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 1")]
		QdpType1,

		/// <summary>
		/// Indicates the QDP type 2.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 2")]
		QdpType2,

		/// <summary>
		/// Indicates the QDP type 3.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 3")]
		QdpType3,

		/// <summary>
		/// Indicates the QDP type 4.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 4")]
		QdpType4,

		/// <summary>
		/// Indicates the locked QDP.
		/// </summary>
		[TechniqueDisplay("Locked Qiu's Deadly Pattern")]
		LockedQdp,

		/// <summary>
		/// Indicates the US type 1.
		/// </summary>
		[TechniqueDisplay("Unique Square Type 1")]
		UsType1,

		/// <summary>
		/// Indicates the US type 2.
		/// </summary>
		[TechniqueDisplay("Unique Square Type 2")]
		UsType2,

		/// <summary>
		/// Indicates the US type 3.
		/// </summary>
		[TechniqueDisplay("Unique Square Type 3")]
		UsType3,

		/// <summary>
		/// Indicates the US type 4.
		/// </summary>
		[TechniqueDisplay("Unique Square Type 4")]
		UsType4,

		/// <summary>
		/// Indicates the SdC.
		/// </summary>
		[TechniqueDisplay("Sue de Coq")]
		Sdc,

		/// <summary>
		/// Indicates the 3-dimension SdC.
		/// </summary>
		[TechniqueDisplay("3 Dimension Sue de Coq")]
		Sdc3d,

		/// <summary>
		/// Indicates the cannibalized SdC.
		/// </summary>
		[TechniqueDisplay("Cannibalized Sue de Coq")]
		CannibalizedSdc,

		/// <summary>
		/// Indicates the skyscraper.
		/// </summary>
		[TechniqueDisplay("Skyscraper")]
		Skyscraper,

		/// <summary>
		/// Indicates the two-string kite.
		/// </summary>
		[TechniqueDisplay("Two-string Kite")]
		TwoStringKite,

		/// <summary>
		/// Indicates the turbot fish.
		/// </summary>
		[TechniqueDisplay("Turbot Fish")]
		TurbotFish,

		/// <summary>
		/// Indicates the empty rectangle.
		/// </summary>
		[TechniqueDisplay("Empty Rectangle")]
		EmptyRectangle,

		/// <summary>
		/// Indicates the guardian.
		/// </summary>
		[TechniqueDisplay("Guardian")]
		Guardian,

		/// <summary>
		/// Indicates the X-Chain.
		/// </summary>
		[TechniqueDisplay("X-Chain")]
		XChain,

		/// <summary>
		/// Indicates the Y-Chain.
		/// </summary>
		[TechniqueDisplay("Y-Chain")]
		YChain,

		/// <summary>
		/// Indicates the fishy cycle.
		/// </summary>
		[TechniqueDisplay("Fishy Cycle")]
		FishyCycle,

		/// <summary>
		/// Indicates the XY-Chain.
		/// </summary>
		[TechniqueDisplay("XY-Chain")]
		XyChain,

		/// <summary>
		/// Indicates the XY-Cycle.
		/// </summary>
		[TechniqueDisplay("XY-Cycle")]
		XyCycle,

		/// <summary>
		/// Indicates the XY-X-Chain.
		/// </summary>
		[TechniqueDisplay("XY-X-Chain")]
		XyXChain,

		/// <summary>
		/// Indicates the purple cow.
		/// </summary>
		[TechniqueDisplay("Purple Cow")]
		PurpleCow,

		/// <summary>
		/// Indicates the discontinuous nice loop.
		/// </summary>
		[TechniqueDisplay("Discontinuous Nice Loop")]
		DiscontinuousNiceLoop,

		/// <summary>
		/// Indicates the continuous nice loop.
		/// </summary>
		[TechniqueDisplay("Continuous Nice Loop")]
		ContinuousNiceLoop,

		/// <summary>
		/// Indicates the AIC.
		/// </summary>
		[TechniqueDisplay("Alternating Inference Chain")]
		Aic,

		/// <summary>
		/// Indicates the grouped X-Chain.
		/// </summary>
		[TechniqueDisplay("Grouped X-Chain")]
		GroupedXChain,

		/// <summary>
		/// Indicates the grouped fishy cycle.
		/// </summary>
		[TechniqueDisplay("Grouped Fishy Cycle")]
		GroupedFishyCycle,

		/// <summary>
		/// Indicates the grouped XY-Chain.
		/// </summary>
		[TechniqueDisplay("Grouped XY-Chain")]
		GroupedXyChain,

		/// <summary>
		/// Indicates the grouped XY-Cycle.
		/// </summary>
		[TechniqueDisplay("Grouped XY-Cycle")]
		GroupedXyCycle,

		/// <summary>
		/// Indicates the grouped XY-X-Chain.
		/// </summary>
		[TechniqueDisplay("Grouped XY-X-Chain")]
		GroupedXyXChain,

		/// <summary>
		/// Indicates the grouped purple cow.
		/// </summary>
		[TechniqueDisplay("Grouped Purple Cow")]
		GroupedPurpleCow,

		/// <summary>
		/// Indicates the grouped discontinuous nice loop.
		/// </summary>
		[TechniqueDisplay("Grouped Discontinuous Nice Loop")]
		GroupedDiscontinuousNiceLoop,

		/// <summary>
		/// Indicates the grouped continuous nice loop.
		/// </summary>
		[TechniqueDisplay("Grouped Continuous Nice Loop")]
		GroupedContinuousNiceLoop,

		/// <summary>
		/// Indicates the grouped AIC.
		/// </summary>
		[TechniqueDisplay("Grouped Alternating Inference Chain")]
		GroupedAic,

		/// <summary>
		/// Indicates the nishio FCs.
		/// </summary>
		[TechniqueDisplay("Nishio Forcing Chains")]
		NishioFc,

		/// <summary>
		/// Indicates the region FCs.
		/// </summary>
		[TechniqueDisplay("Region Forcing Chains")]
		RegionFc,

		/// <summary>
		/// Indicates the cell FCs.
		/// </summary>
		[TechniqueDisplay("Cell Forcing Chains")]
		CellFc,

		/// <summary>
		/// Indicates the dynamic region FCs.
		/// </summary>
		[TechniqueDisplay("Dynamic Region Forcing Chains")]
		DynamicRegionFc,

		/// <summary>
		/// Indicates the dynamic cell FCs.
		/// </summary>
		[TechniqueDisplay("Dynamic Cell Forcing Chains")]
		DynamicCellFc,

		/// <summary>
		/// Indicates the dynamic contradiction FCs.
		/// </summary>
		[TechniqueDisplay("Dynamic Contradiction Forcing Chains")]
		DynamicContradictionFc,

		/// <summary>
		/// Indicates the dynamic double FCs.
		/// </summary>
		[TechniqueDisplay("Dynamic Double Forcing Chains")]
		DynamicDoubleFc,

		/// <summary>
		/// Indicates the dynamic FCs.
		/// </summary>
		[TechniqueDisplay("Dynamic Forcing Chains")]
		DynamicFc,

		/// <summary>
		/// Indicates the ERIP.
		/// </summary>
		[TechniqueDisplay("Empty Rectangle Intersection Pair")]
		Erip,

		/// <summary>
		/// Indicates the ESP.
		/// </summary>
		[TechniqueDisplay("Extended Subset Principle")]
		Esp,

		/// <summary>
		/// Indicates the singly linked ALS-XZ.
		/// </summary>
		[TechniqueDisplay("Singly Linked Almost Locked Sets XZ Rule")]
		SinglyLinkedAlsXz,

		/// <summary>
		/// Indicates the doubly linked ALS-XZ.
		/// </summary>
		[TechniqueDisplay("Doubly Linked Almost Locked Sets XZ Rule")]
		DoublyLinkedAlsXz,

		/// <summary>
		/// Indicates the ALS-XY-Wing.
		/// </summary>
		[TechniqueDisplay("Almost Locked Sets XY-Wing")]
		AlsXyWing,

		/// <summary>
		/// Indicates the ALS-W-Wing.
		/// </summary>
		[TechniqueDisplay("Almost Locked Sets W-Wing")]
		AlsWWing,

		/// <summary>
		/// Indicates the death blossom.
		/// </summary>
		[TechniqueDisplay("Death Blossom")]
		DeathBlossom,

		/// <summary>
		/// Indicates the GSP.
		/// </summary>
		[TechniqueDisplay("Gurth's Symmetrical Placement")]
		Gsp,

		/// <summary>
		/// Indicates the GSP2.
		/// </summary>
		[TechniqueDisplay("Gurth's Symmetrical Placement 2")]
		Gsp2,

		/// <summary>
		/// Indicates the JE.
		/// </summary>
		[TechniqueDisplay("Junior Exocet")]
		Je,

		/// <summary>
		/// Indicates the SE.
		/// </summary>
		[TechniqueDisplay("Senior Exocet")]
		Se,

		/// <summary>
		/// Indicates the complex SE.
		/// </summary>
		[TechniqueDisplay("Complex Senior Exocet")]
		ComplexSe,

		/// <summary>
		/// Indicates the siamese JE. 
		/// </summary>
		//[TechniqueDisplay("Siamese Junior Exocet")]
		SiameseJe,

		/// <summary>
		/// Indicates the siamese SE.
		/// </summary>
		//[TechniqueDisplay("Siamese Senior Exocet")]
		SiameseSe,

		/// <summary>
		/// Indicates the SK-Loop.
		/// </summary>
		[TechniqueDisplay("Stephen Kurzhal's Loop")]
		SkLoop,

		/// <summary>
		/// Indicates the multi-sector locked sets.
		/// </summary>
		[TechniqueDisplay("Multi-sector Locked Sets")]
		Msls,

		/// <summary>
		/// Indicates the POM.
		/// </summary>
		[TechniqueDisplay("Pattern Overlay")]
		Pom,

		/// <summary>
		/// Indicates the template set.
		/// </summary>
		[TechniqueDisplay("Template Set")]
		TemplateSet,

		/// <summary>
		/// Indicates the template delete.
		/// </summary>
		[TechniqueDisplay("Template Delete")]
		TemplateDelete,

		/// <summary>
		/// Indicates the bowman's bingo.
		/// </summary>
		[TechniqueDisplay("Bowman's Bingo")]
		BowmanBingo,

		/// <summary>
		/// Indicates the brute force.
		/// </summary>
		[TechniqueDisplay("Brute Force")]
		BruteForce,
	}
}
