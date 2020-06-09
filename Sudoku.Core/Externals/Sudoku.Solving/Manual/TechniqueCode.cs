using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Represents a technique instance, which is used for comparison.
	/// </summary>
	public enum TechniqueCode : short
	{
		/// <summary>
		/// The placeholder of this enumeration type.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the full house.
		/// </summary>
		[TechniqueDisplay("Full House", Category = "Singles")]
		FullHouse,

		/// <summary>
		/// Indicates the last digit.
		/// </summary>
		[TechniqueDisplay("Last Digit", Category = "Singles")]
		LastDigit,

		/// <summary>
		/// Indicates the hidden single.
		/// </summary>
		[TechniqueDisplay("Hidden Single", Category = "Singles")]
		HiddenSingle,

		/// <summary>
		/// Indicates the naked single.
		/// </summary>
		[TechniqueDisplay("Naked Single", Category = "Singles")]
		NakedSingle,

		/// <summary>
		/// Indicates the pointing.
		/// </summary>
		[TechniqueDisplay("Pointing", Category = "Intersections>Locked Candidates")]
		Pointing,

		/// <summary>
		/// Indicates the claiming.
		/// </summary>
		[TechniqueDisplay("Claiming", Category = "Intersections>Locked Candidates")]
		Claiming,

		/// <summary>
		/// Indicates the ALP.
		/// </summary>
		[TechniqueDisplay("Almost Locked Pair", Category = "Intersections>Almost Locked Candidates")]
		AlmostLockedPair,

		/// <summary>
		/// Indicates the ALT.
		/// </summary>
		[TechniqueDisplay("Almost Locked Triple", Category = "Intersections>Almost Locked Candidates")]
		AlmostLockedTriple,

		/// <summary>
		/// Indicates the ALQ.
		/// </summary>
		[TechniqueDisplay("Almost Locked Quadruple", Category = "Intersections>Almost Locked Candidates")]
		AlmostLockedQuadruple,

		/// <summary>
		/// Indicates the naked pair.
		/// </summary>
		[TechniqueDisplay("Naked Pair", Category = "Subsets")]
		NakedPair,

		/// <summary>
		/// Indicates the naked pair plus (naked pair (+)).
		/// </summary>
		[TechniqueDisplay("Naked Pair (+)", Category = "Subsets")]
		NakedPairPlus,

		/// <summary>
		/// Indicates the locked pair.
		/// </summary>
		[TechniqueDisplay("Locked Pair", Category = "Subsets")]
		LockedPair,

		/// <summary>
		/// Indicates the hidden pair.
		/// </summary>
		[TechniqueDisplay("Hidden Pair", Category = "Subsets")]
		HiddenPair,

		/// <summary>
		/// Indicates the naked triple.
		/// </summary>
		[TechniqueDisplay("Naked Triple", Category = "Subsets")]
		NakedTriple,

		/// <summary>
		/// Indicates the naked triple plus (naked triple (+)).
		/// </summary>
		[TechniqueDisplay("Naked Triple (+)", Category = "Subsets")]
		NakedTriplePlus,

		/// <summary>
		/// Indicates the locked triple.
		/// </summary>
		[TechniqueDisplay("Locked Triple", Category = "Subsets")]
		LockedTriple,

		/// <summary>
		/// Indicates the hidden triple.
		/// </summary>
		[TechniqueDisplay("Hidden Triple", Category = "Subsets")]
		HiddenTriple,

		/// <summary>
		/// Indicates the naked quadruple.
		/// </summary>
		[TechniqueDisplay("Naked Quadruple", Category = "Subsets")]
		NakedQuadruple,

		/// <summary>
		/// Indicates the naked quadruple plus (naked quadruple (+)).
		/// </summary>
		[TechniqueDisplay("Naked Quadruple (+)", Category = "Subsets")]
		NakedQuadruplePlus,

		/// <summary>
		/// Indicates the hidden quadruple.
		/// </summary>
		[TechniqueDisplay("Hidden Quadruple", Category = "Subsets")]
		HiddenQuadruple,

		/// <summary>
		/// Indicates the X-Wing.
		/// </summary>
		[TechniqueDisplay("X-Wing", Category = "Fishes>Normal Fishes")]
		XWing,

		/// <summary>
		/// Indicates the finned X-Wing.
		/// </summary>
		[TechniqueDisplay("Finned X-Wing", Category = "Fishes>Normal Fishes")]
		FinnedXWing,

		/// <summary>
		/// Indicates the sashimi X-Wing.
		/// </summary>
		[TechniqueDisplay("Sashimi X-Wing", Category = "Fishes>Normal Fishes")]
		SashimiXWing,

		/// <summary>
		/// Indicates the siamese finned X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned X-Wing", Category = "Fishes>Normal Fishes")]
		SiameseFinnedXWing,

		/// <summary>
		/// Indicates the siamese sashimi X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi X-Wing", Category = "Fishes>Normal Fishes")]
		SiameseSashimiXWing,

		/// <summary>
		/// Indicates the franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Franken X-Wing", Category = "Fishes>Franken Fishes")]
		FrankenXWing,

		/// <summary>
		/// Indicates the finned franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Finned Franken X-Wing", Category = "Fishes>Franken Fishes")]
		FinnedFrankenXWing,

		/// <summary>
		/// Indicates the sashimi franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Sashimi Franken X-Wing", Category = "Fishes>Franken Fishes")]
		SashimiFrankenXWing,

		/// <summary>
		/// Indicates the siamese finned franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Franken X-Wing", Category = "Fishes>Franken Fishes")]
		SiameseFinnedFrankenXWing,

		/// <summary>
		/// Indicates the siamese sashimi franken X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Franken X-Wing", Category = "Fishes>Franken Fishes")]
		SiameseSashimiFrankenXWing,

		/// <summary>
		/// Indicates the mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Mutant X-Wing", Category = "Fishes>Mutant Fishes")]
		MutantXWing,

		/// <summary>
		/// Indicates the finned mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Finned Mutant X-Wing", Category = "Fishes>Mutant Fishes")]
		FinnedMutantXWing,

		/// <summary>
		/// Indicates the sashimi mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Sashimi Mutant X-Wing", Category = "Fishes>Mutant Fishes")]
		SashimiMutantXWing,

		/// <summary>
		/// Indicates the siamese finned mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Mutant X-Wing", Category = "Fishes>Mutant Fishes")]
		SiameseFinnedMutantXWing,

		/// <summary>
		/// Indicates the siamese sashimi mutant X-Wing.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Mutant X-Wing", Category = "Fishes>Mutant Fishes")]
		SiameseSashimiMutantXWing,

		/// <summary>
		/// Indicates the swordfish.
		/// </summary>
		[TechniqueDisplay("Swordfish", Category = "Fishes>Normal Fishes")]
		Swordfish,

		/// <summary>
		/// Indicates the finned swordfish.
		/// </summary>
		[TechniqueDisplay("Finned Swordfish", Category = "Fishes>Normal Fishes")]
		FinnedSwordfish,

		/// <summary>
		/// Indicates the sashimi swordfish.
		/// </summary>
		[TechniqueDisplay("Sashimi Swordfish", Category = "Fishes>Normal Fishes")]
		SashimiSwordfish,

		/// <summary>
		/// Indicates the siamese finned swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Swordfish", Category = "Fishes>Normal Fishes")]
		SiameseFinnedSwordfish,

		/// <summary>
		/// Indicates the siamese sashimi swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Swordfish", Category = "Fishes>Normal Fishes")]
		SiameseSashimiSwordfish,

		/// <summary>
		/// Indicates the swordfish.
		/// </summary>
		//[TechniqueDisplay("Franken Swordfish", Category = "Fishes>Franken Fishes")]
		FrankenSwordfish,

		/// <summary>
		/// Indicates the finned franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Finned Franken Swordfish", Category = "Fishes>Franken Fishes")]
		FinnedFrankenSwordfish,

		/// <summary>
		/// Indicates the sashimi franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Sashimi Franken Swordfish", Category = "Fishes>Franken Fishes")]
		SashimiFrankenSwordfish,

		/// <summary>
		/// Indicates the siamese finned franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Franken Swordfish", Category = "Fishes>Franken Fishes")]
		SiameseFinnedFrankenSwordfish,

		/// <summary>
		/// Indicates the siamese sashimi franken swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Franken Swordfish", Category = "Fishes>Franken Fishes")]
		SiameseSashimiFrankenSwordfish,

		/// <summary>
		/// Indicates the mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Mutant Swordfish", Category = "Fishes>Mutant Fishes")]
		MutantSwordfish,

		/// <summary>
		/// Indicates the finned mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Finned Mutant Swordfish", Category = "Fishes>Mutant Fishes")]
		FinnedMutantSwordfish,

		/// <summary>
		/// Indicates the sashimi mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Sashimi Mutant Swordfish", Category = "Fishes>Mutant Fishes")]
		SashimiMutantSwordfish,

		/// <summary>
		/// Indicates the siamese finned mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Finned Mutant Swordfish", Category = "Fishes>Mutant Fishes")]
		SiameseFinnedMutantSwordfish,

		/// <summary>
		/// Indicates the siamese sashimi mutant swordfish.
		/// </summary>
		//[TechniqueDisplay("Siamese Sashimi Mutant Swordfish", Category = "Fishes>Mutant Fishes")]
		SiameseSashimiMutantSwordfish,

		/// <summary>
		/// Indicates the jellyfish.
		/// </summary>
		[TechniqueDisplay("Jellyfish", Category = "Fishes>Normal Fishes")]
		Jellyfish,

		/// <summary>
		/// Indicates the finned jellyfish.
		/// </summary>
		[TechniqueDisplay("Finned Jellyfish", Category = "Fishes>Normal Fishes")]
		FinnedJellyfish,

		/// <summary>
		/// Indicates the sashimi jellyfish.
		/// </summary>
		[TechniqueDisplay("Sashimi Jellyfish", Category = "Fishes>Normal Fishes")]
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
		[TechniqueDisplay("XY-Wing", Category = "Wings>Regular Wings")]
		XyWing,

		/// <summary>
		/// Indicates the XYZ-Wing.
		/// </summary>
		[TechniqueDisplay("XYZ-Wing", Category = "Wings>Regular Wings")]
		XyzWing,

		/// <summary>
		/// Indicates the WXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("WXYZ-Wing", Category = "Wings>Regular Wings")]
		WxyzWing,

		/// <summary>
		/// Indicates the VWXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("VWXYZ-Wing", Category = "Wings>Regular Wings")]
		VwxyzWing,

		/// <summary>
		/// Indicates the UVWXYZ-Wing.
		/// </summary>
		//[TechniqueDisplay("UVWXYZ-Wing", Category = "Wings>Regular Wings")]
		UvwxyzWing,

		/// <summary>
		/// Indicates the TUVWXYZ-Wing.
		/// </summary>
		//[TechniqueDisplay("TUVWXYZ-Wing", Category = "Wings>Regular Wings")]
		TuvwxyzWing,

		/// <summary>
		/// Indicates the STUVWXYZ-Wing.
		/// </summary>
		//[TechniqueDisplay("STUVWXYZ-Wing", Category = "Wings>Regular Wings")]
		StuvwxyzWing,

		/// <summary>
		/// Indicates the RSTUVWXYZ-Wing.
		/// </summary>
		//[TechniqueDisplay("RSTUVWXYZ-Wing", Category = "Wings>Regular Wings")]
		RstuvwxyzWing,

		/// <summary>
		/// Indicates the W-Wing.
		/// </summary>
		[TechniqueDisplay("W-Wing", Category = "Wings>Irregular Wings")]
		WWing,

		/// <summary>
		/// Indicates the M-Wing.
		/// </summary>
		[TechniqueDisplay("M-Wing", Category = "Wings>Irregular Wings")]
		MWing,

		/// <summary>
		/// Indicates the local wing.
		/// </summary>
		[TechniqueDisplay("Local Wing", Category = "Wings>Irregular Wings")]
		LocalWing,

		/// <summary>
		/// Indicates the split wing.
		/// </summary>
		[TechniqueDisplay("Split Wing", Category = "Wings>Irregular Wings")]
		SplitWing,

		/// <summary>
		/// Indicates the hybrid wing.
		/// </summary>
		[TechniqueDisplay("Hybrid Wing", Category = "Wings>Irregular Wings")]
		HybridWing,

		/// <summary>
		/// Indicates the grouped XY-Wing.
		/// </summary>
		GroupedXyWing,

		/// <summary>
		/// Indicates the grouped W-Wing.
		/// </summary>
		[TechniqueDisplay("Grouped W-Wing", Category = "Wings>Irregular Wings")]
		GroupedWWing,

		/// <summary>
		/// Indicates the grouped M-Wing.
		/// </summary>
		[TechniqueDisplay("Grouped M-Wing", Category = "Wings>Irregular Wings")]
		GroupedMWing,

		/// <summary>
		/// Indicates the grouped local wing.
		/// </summary>
		[TechniqueDisplay("Grouped Local Wing", Category = "Wings>Irregular Wings")]
		GroupedLocalWing,

		/// <summary>
		/// Indicates the grouped split wing.
		/// </summary>
		[TechniqueDisplay("Grouped Split Wing", Category = "Wings>Irregular Wings")]
		GroupedSplitWing,

		/// <summary>
		/// Indicates the grouped hybrid wing.
		/// </summary>
		[TechniqueDisplay("Grouped Hybrid Wing", Category = "Wings>Irregular Wings")]
		GroupedHybridWing,

		/// <summary>
		/// Indicates the UR type 1.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 1", Category = "Uniqueness>Rectangles")]
		UrType1,

		/// <summary>
		/// Indicates the UR type 2.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 2", Category = "Uniqueness>Rectangles")]
		UrType2,

		/// <summary>
		/// Indicates the UR type 3.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 3", Category = "Uniqueness>Rectangles")]
		UrType3,

		/// <summary>
		/// Indicates the UR type 4.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 4", Category = "Uniqueness>Rectangles")]
		UrType4,

		/// <summary>
		/// Indicates the UR type 5.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 5", Category = "Uniqueness>Rectangles")]
		UrType5,

		/// <summary>
		/// Indicates the UR type 6.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle Type 6", Category = "Uniqueness>Rectangles")]
		UrType6,

		/// <summary>
		/// Indicates the hidden UR.
		/// </summary>
		[TechniqueDisplay("Hidden Unique Rectangle", Category = "Uniqueness>Rectangles")]
		HiddenUr,

		/// <summary>
		/// Indicates the UR + 2D.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 2D", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus2D,

		/// <summary>
		/// Indicates the UR + 2B / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 2B / 1SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus2B1SL,

		/// <summary>
		/// Indicates the UR + 2D / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 2D / 1SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus2D1SL,

		/// <summary>
		/// Indicates the UR + 3X.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3X", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3X,

		/// <summary>
		/// Indicates the UR + 3x / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3x / 1SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3x1SL,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3X / 1SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3X1SL,

		/// <summary>
		/// Indicates the UR + 3X / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3X / 2SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3X2SL,

		/// <summary>
		/// Indicates the UR + 3N / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3N / 2SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3N2SL,

		/// <summary>
		/// Indicates the UR + 3U / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3U / 2SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3U2SL,

		/// <summary>
		/// Indicates the UR + 3E / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 3E / 2SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus3E2SL,

		/// <summary>
		/// Indicates the UR + 4x / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4x / 1SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus4x1SL,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4X / 1SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus4X1SL,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4x / 2SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus4x2SL,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4X / 2SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus4X2SL,

		/// <summary>
		/// Indicates the UR + 4X / 3SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4X / 3SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus4X3SL,

		/// <summary>
		/// Indicates the UR + 4C / 3SL.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + 4C / 3SL", Category = "Uniqueness>Rectangles>Extended")]
		UrPlus4C3SL,

		/// <summary>
		/// Indicates the UR-XY-Wing.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + XY-Wing", Category = "Uniqueness>Rectangles>Extended")]
		UrXyWing,

		/// <summary>
		/// Indicates the UR-XYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + XYZ-Wing", Category = "Uniqueness>Rectangles>Extended")]
		UrXyzWing,

		/// <summary>
		/// Indicates the UR-WXYZ-Wing.
		/// </summary>
		[TechniqueDisplay("Unique Rectangle + WXYZ-Wing", Category = "Uniqueness>Rectangles>Extended")]
		UrWxyzWing,

		/// <summary>
		/// Indicates the UR sue de coq.
		/// </summary>
		//[TechniqueDisplay("Unique Rectangle + Sue de Coq", Category = "Uniqueness>Rectangles>Extended")]
		UrSdc,

		/// <summary>
		/// Indicates the UL type 1.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 1", Category = "Uniqueness>Loops")]
		UlType1,

		/// <summary>
		/// Indicates the UL type 2.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 2", Category = "Uniqueness>Loops")]
		UlType2,

		/// <summary>
		/// Indicates the UL type 3.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 3", Category = "Uniqueness>Loops")]
		UlType3,

		/// <summary>
		/// Indicates the UL type 4.
		/// </summary>
		[TechniqueDisplay("Unique Loop Type 4", Category = "Uniqueness>Loops")]
		UlType4,

		/// <summary>
		/// Indicates the XR type 1.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 1", Category = "Uniqueness>Extended Rectangles")]
		XrType1,

		/// <summary>
		/// Indicates the XR type 2.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 2", Category = "Uniqueness>Extended Rectangles")]
		XrType2,

		/// <summary>
		/// Indicates the XR type 3.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 3", Category = "Uniqueness>Extended Rectangles")]
		XrType3,

		/// <summary>
		/// Indicates the XR type 4.
		/// </summary>
		[TechniqueDisplay("Extended Rectangle Type 4", Category = "Uniqueness>Extended Rectangles")]
		XrType4,

		/// <summary>
		/// Indicates the BUG type 1.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 1", Category = "Uniqueness>Bivalue Universal Graves")]
		BugType1,

		/// <summary>
		/// Indicates the BUG type 2.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 2", Category = "Uniqueness>Bivalue Universal Graves")]
		BugType2,

		/// <summary>
		/// Indicates the BUG type 3.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 3", Category = "Uniqueness>Bivalue Universal Graves")]
		BugType3,

		/// <summary>
		/// Indicates the BUG type 4.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave Type 4", Category = "Uniqueness>Bivalue Universal Graves")]
		BugType4,

		/// <summary>
		/// Indicates the BUG + n.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave + n", Category = "Uniqueness>Bivalue Universal Graves>Extended")]
		BugMultiple,

		/// <summary>
		/// Indicates the BUG-XZ.
		/// </summary>
		[TechniqueDisplay("Bivalue Universal Grave XZ Rule", Category = "Uniqueness>Bivalue Universal Graves>Extended")]
		BugXz,

		/// <summary>
		/// Indicates the BUG-XY-Wing.
		/// </summary>
		//[TechniqueDisplay("Bivalue Universal Grave XYZ-Wing", Category = "Uniqueness>Bivalue Universal Graves>Extended")]
		BugXyzWing,

		/// <summary>
		/// Indicates the BDP type 1.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 1", Category = "Uniqueness>Borescoper's Deadly Patterns")]
		BdpType1,

		/// <summary>
		/// Indicates the BDP type 2.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 2", Category = "Uniqueness>Borescoper's Deadly Patterns")]
		BdpType2,

		/// <summary>
		/// Indicates the BDP type 3.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 3", Category = "Uniqueness>Borescoper's Deadly Patterns")]
		BdpType3,

		/// <summary>
		/// Indicates the BDP type 4.
		/// </summary>
		[TechniqueDisplay("Borescoper's Deadly Pattern Type 4", Category = "Uniqueness>Borescoper's Deadly Patterns")]
		BdpType4,

		/// <summary>
		/// Indicates the QDP type 1.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 1", Category = "Uniqueness>Qiu's Deadly Patterns")]
		QdpType1,

		/// <summary>
		/// Indicates the QDP type 2.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 2", Category = "Uniqueness>Qiu's Deadly Patterns")]
		QdpType2,

		/// <summary>
		/// Indicates the QDP type 3.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 3", Category = "Uniqueness>Qiu's Deadly Patterns")]
		QdpType3,

		/// <summary>
		/// Indicates the QDP type 4.
		/// </summary>
		[TechniqueDisplay("Qiu's Deadly Pattern Type 4", Category = "Uniqueness>Qiu's Deadly Patterns")]
		QdpType4,

		/// <summary>
		/// Indicates the locked QDP.
		/// </summary>
		[TechniqueDisplay("Locked Qiu's Deadly Pattern", Category = "Uniqueness>Qiu's Deadly Patterns")]
		LockedQdp,

		/// <summary>
		/// Indicates the SdC.
		/// </summary>
		[TechniqueDisplay("Sue de Coq", Category = "Almost Locked Sets")]
		Sdc,

		/// <summary>
		/// Indicates the cannibalized SdC.
		/// </summary>
		[TechniqueDisplay("Cannibalized Sue de Coq", Category = "Almost Locked Sets")]
		CannibalizedSdc,

		/// <summary>
		/// Indicates the skyscraper.
		/// </summary>
		[TechniqueDisplay("Skyscraper", Category = "Single Digit Patterns")]
		Skyscraper,

		/// <summary>
		/// Indicates the two-string kite.
		/// </summary>
		[TechniqueDisplay("Two-string Kite", Category = "Single Digit Patterns")]
		TwoStringKite,

		/// <summary>
		/// Indicates the turbot fish.
		/// </summary>
		[TechniqueDisplay("Turbot Fish", Category = "Single Digit Patterns")]
		TurbotFish,

		/// <summary>
		/// Indicates the empty rectangle.
		/// </summary>
		[TechniqueDisplay("Empty Rectangle", Category = "Single Digit Patterns")]
		EmptyRectangle,

		/// <summary>
		/// Indicates the guardian.
		/// </summary>
		[TechniqueDisplay("Guardian", Category = "Single Digit Patterns")]
		Guardian,

		/// <summary>
		/// Indicates the X-Chain.
		/// </summary>
		[TechniqueDisplay("X-Chain", Category = "Chains>Alternating Inference Chains")]
		XChain,

		/// <summary>
		/// Indicates the XY-Chain.
		/// </summary>
		[TechniqueDisplay("XY-Chain", Category = "Chains>Alternating Inference Chains")]
		XyChain,

		/// <summary>
		/// Indicates the XY-X-Chain.
		/// </summary>
		[TechniqueDisplay("XY-X-Chain", Category = "Chains>Alternating Inference Chains")]
		XyXChain,

		/// <summary>
		/// Indicates the purple cow.
		/// </summary>
		[TechniqueDisplay("Purple Cow", Category = "Chains>Alternating Inference Chains")]
		PurpleCow,

		/// <summary>
		/// Indicates the discontinuous nice loop.
		/// </summary>
		[TechniqueDisplay("Discontinuous Nice Loop", Category = "Chains>Alternating Inference Chains")]
		DiscontinuousNiceLoop,

		/// <summary>
		/// Indicates the continuous nice loop.
		/// </summary>
		[TechniqueDisplay("Continuous Nice Loop", Category = "Chains>Alternating Inference Chains")]
		ContinuousNiceLoop,

		/// <summary>
		/// Indicates the AIC.
		/// </summary>
		[TechniqueDisplay("Alternating Inference Chain", Category = "Chains>Alternating Inference Chains")]
		Aic,

		/// <summary>
		/// Indicates the grouped X-Chain.
		/// </summary>
		[TechniqueDisplay("Grouped X-Chain", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedXChain,

		/// <summary>
		/// Indicates the grouped XY-Chain.
		/// </summary>
		[TechniqueDisplay("Grouped XY-Chain", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedXyChain,

		/// <summary>
		/// Indicates the grouped XY-X-Chain.
		/// </summary>
		[TechniqueDisplay("Grouped XY-X-Chain", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedXyXChain,

		/// <summary>
		/// Indicates the grouped purple cow.
		/// </summary>
		[TechniqueDisplay("Grouped Purple Cow", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedPurpleCow,

		/// <summary>
		/// Indicates the grouped discontinuous nice loop.
		/// </summary>
		[TechniqueDisplay("Grouped Discontinuous Nice Loop", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedDiscontinuousNiceLoop,

		/// <summary>
		/// Indicates the grouped continuous nice loop.
		/// </summary>
		[TechniqueDisplay("Grouped Continuous Nice Loop", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedContinuousNiceLoop,

		/// <summary>
		/// Indicates the grouped AIC.
		/// </summary>
		[TechniqueDisplay("Grouped Alternating Inference Chain", Category = "Chains>Grouped Alternating Inference Chains")]
		GroupedAic,

		/// <summary>
		/// Indicates the ERIP.
		/// </summary>
		[TechniqueDisplay("Empty Rectangle Intersection Pair", Category = "Almost Locked Sets")]
		Erip,

		/// <summary>
		/// Indicates the ESP.
		/// </summary>
		[TechniqueDisplay("Extended Subset Principle", Category = "Almost Locked Sets")]
		Esp,

		/// <summary>
		/// Indicates the singly linked ALS-XZ.
		/// </summary>
		[TechniqueDisplay("Singly Linked Almost Locked Sets XZ Rule", Category = "Almost Locked Sets")]
		SinglyLinkedAlsXz,

		/// <summary>
		/// Indicates the doubly linked ALS-XZ.
		/// </summary>
		[TechniqueDisplay("Doubly Linked Almost Locked Sets XZ Rule", Category = "Almost Locked Sets")]
		DoublyLinkedAlsXz,

		/// <summary>
		/// Indicates the ALS-XY-Wing.
		/// </summary>
		[TechniqueDisplay("Almost Locked Sets XY-Wing", Category = "Almost Locked Sets")]
		AlsXyWing,

		/// <summary>
		/// Indicates the ALS-W-Wing.
		/// </summary>
		[TechniqueDisplay("Almost Locked Sets W-Wing", Category = "Almost Locked Sets")]
		AlsWWing,

		/// <summary>
		/// Indicates the death blossom.
		/// </summary>
		[TechniqueDisplay("Death Blossom", Category = "Almost Locked Sets")]
		DeathBlossom,

		/// <summary>
		/// Indicates the GSP.
		/// </summary>
		[TechniqueDisplay("Gurth's Symmetrical Placement", Category = "Symmetry")]
		Gsp,

		/// <summary>
		/// Indicates the JE.
		/// </summary>
		[TechniqueDisplay("Junior Exocet", Category = "Exocets")]
		Je,

		/// <summary>
		/// Indicates the SE.
		/// </summary>
		[TechniqueDisplay("Senior Exocet", Category = "Exocets")]
		Se,

		/// <summary>
		/// Indicates the complex SE.
		/// </summary>
		[TechniqueDisplay("Complex Senior Exocet", Category = "Exocets")]
		ComplexSe,

		/// <summary>
		/// Indicates the siamese JE. 
		/// </summary>
		//[TechniqueDisplay("Siamese Junior Exocet", Category = "Exocets")]
		SiameseJe,

		/// <summary>
		/// Indicates the siamese SE.
		/// </summary>
		//[TechniqueDisplay("Siamese Senior Exocet", Category = "Exocets")]
		SiameseSe,

		/// <summary>
		/// Indicates the SK-Loop.
		/// </summary>
		[TechniqueDisplay("Stephen Kurzhal's Loop", Category = "Multi-sector Locked Sets")]
		SkLoop,

		/// <summary>
		/// Indicates the multi-sector locked sets.
		/// </summary>
		[TechniqueDisplay("Multi-sector Locked Sets", Category = "Multi-sector Locked Sets")]
		Msls,

		/// <summary>
		/// Indicates the POM.
		/// </summary>
		[TechniqueDisplay("Pattern Overlay", Category = "Last Resorts")]
		Pom,

		/// <summary>
		/// Indicates the template set.
		/// </summary>
		[TechniqueDisplay("Template Set", Category = "Last Resorts")]
		TemplateSet,

		/// <summary>
		/// Indicates the template delete.
		/// </summary>
		[TechniqueDisplay("Template Delete", Category = "Last Resorts")]
		TemplateDelete,

		/// <summary>
		/// Indicates the bowman's bingo.
		/// </summary>
		[TechniqueDisplay("Bowman's Bingo", Category = "Last Resorts")]
		BowmanBingo,

		/// <summary>
		/// Indicates the CCC.
		/// </summary>
		[TechniqueDisplay("Chute Clue Cover", Category = "Last Resorts")]
		Ccc,

		/// <summary>
		/// Indicates the brute force.
		/// </summary>
		[TechniqueDisplay("Brute Force", Category = "Last Resorts")]
		BruteForce,
	}
}
