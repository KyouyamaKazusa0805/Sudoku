namespace Sudoku.Techniques
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
		FullHouse,

		/// <summary>
		/// Indicates the last digit.
		/// </summary>
		LastDigit,

		/// <summary>
		/// Indicates the hidden single.
		/// </summary>
		HiddenSingleBlock,

		/// <summary>
		/// Indicates the hidden single.
		/// </summary>
		HiddenSingleRow,

		/// <summary>
		/// Indicates the hidden single.
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
		UrType1,

		/// <summary>
		/// Indicates the UR type 2.
		/// </summary>
		UrType2,

		/// <summary>
		/// Indicates the UR type 3.
		/// </summary>
		UrType3,

		/// <summary>
		/// Indicates the UR type 4.
		/// </summary>
		UrType4,

		/// <summary>
		/// Indicates the UR type 5.
		/// </summary>
		UrType5,

		/// <summary>
		/// Indicates the UR type 6.
		/// </summary>
		UrType6,

		/// <summary>
		/// Indicates the hidden UR.
		/// </summary>
		HiddenUr,

		/// <summary>
		/// Indicates the UR + 2D.
		/// </summary>
		UrPlus2D,

		/// <summary>
		/// Indicates the UR + 2B / 1SL.
		/// </summary>
		UrPlus2B1SL,

		/// <summary>
		/// Indicates the UR + 2D / 1SL.
		/// </summary>
		UrPlus2D1SL,

		/// <summary>
		/// Indicates the UR + 3X.
		/// </summary>
		UrPlus3X,

		/// <summary>
		/// Indicates the UR + 3x / 1SL.
		/// </summary>
		UrPlus3x1SL_Lower,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		UrPlus3X1SL_Upper,

		/// <summary>
		/// Indicates the UR + 3X / 2SL.
		/// </summary>
		UrPlus3X2SL,

		/// <summary>
		/// Indicates the UR + 3N / 2SL.
		/// </summary>
		UrPlus3N2SL,

		/// <summary>
		/// Indicates the UR + 3U / 2SL.
		/// </summary>
		UrPlus3U2SL,

		/// <summary>
		/// Indicates the UR + 3E / 2SL.
		/// </summary>
		UrPlus3E2SL,

		/// <summary>
		/// Indicates the UR + 4x / 1SL.
		/// </summary>
		UrPlus4x1SL_Lower,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		UrPlus4X1SL_Upper,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		UrPlus4x2SL_Lower,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		UrPlus4X2SL_Upper,

		/// <summary>
		/// Indicates the UR + 4X / 3SL.
		/// </summary>
		UrPlus4X3SL,

		/// <summary>
		/// Indicates the UR + 4C / 3SL.
		/// </summary>
		UrPlus4C3SL,

		/// <summary>
		/// Indicates the UR-XY-Wing.
		/// </summary>
		UrXyWing,

		/// <summary>
		/// Indicates the UR-XYZ-Wing.
		/// </summary>
		UrXyzWing,

		/// <summary>
		/// Indicates the UR-WXYZ-Wing.
		/// </summary>
		UrWxyzWing,

		/// <summary>
		/// Indicates the UR sue de coq.
		/// </summary>
		UrSdc,

		/// <summary>
		/// Indicates the AR type 1.
		/// </summary>
		ArType1,

		/// <summary>
		/// Indicates the AR type 2.
		/// </summary>
		ArType2,

		/// <summary>
		/// Indicates the AR type 3.
		/// </summary>
		ArType3,

		/// <summary>
		/// Indicates the AR type 5.
		/// </summary>
		ArType5,

		/// <summary>
		/// Indicates the hidden AR.
		/// </summary>
		HiddenAr,

		/// <summary>
		/// Indicates the AR + 2D.
		/// </summary>
		ArPlus2D,

		/// <summary>
		/// Indicates the AR + 3X.
		/// </summary>
		ArPlus3X,

		/// <summary>
		/// Indicates the AR-XY-Wing.
		/// </summary>
		ArXyWing,

		/// <summary>
		/// Indicates the AR-XYZ-Wing.
		/// </summary>
		ArXyzWing,

		/// <summary>
		/// Indicates the AR-WXYZ-Wing.
		/// </summary>
		ArWxyzWing,

		/// <summary>
		/// Indicates the AR sue de coq.
		/// </summary>
		ArSdc,

		/// <summary>
		/// Indicates the UL type 1.
		/// </summary>
		UlType1,

		/// <summary>
		/// Indicates the UL type 2.
		/// </summary>
		UlType2,

		/// <summary>
		/// Indicates the UL type 3.
		/// </summary>
		UlType3,

		/// <summary>
		/// Indicates the UL type 4.
		/// </summary>
		UlType4,

		/// <summary>
		/// Indicates the XR type 1.
		/// </summary>
		XrType1,

		/// <summary>
		/// Indicates the XR type 2.
		/// </summary>
		XrType2,

		/// <summary>
		/// Indicates the XR type 3.
		/// </summary>
		XrType3,

		/// <summary>
		/// Indicates the XR type 4.
		/// </summary>
		XrType4,

		/// <summary>
		/// Indicates the BUG type 1.
		/// </summary>
		BugType1,

		/// <summary>
		/// Indicates the BUG type 2.
		/// </summary>
		BugType2,

		/// <summary>
		/// Indicates the BUG type 3.
		/// </summary>
		BugType3,

		/// <summary>
		/// Indicates the BUG type 4.
		/// </summary>
		BugType4,

		/// <summary>
		/// Indicates the BUG + n.
		/// </summary>
		BugMultiple,

		/// <summary>
		/// Indicates the BUG + n with forcing chains.
		/// </summary>
		BugMultipleFc,

		/// <summary>
		/// Indicates the BUG-XZ.
		/// </summary>
		BugXz,

		/// <summary>
		/// Indicates the BUG-XY-Wing.
		/// </summary>
		BugXyzWing,

		/// <summary>
		/// Indicates the BDP type 1.
		/// </summary>
		BdpType1,

		/// <summary>
		/// Indicates the BDP type 2.
		/// </summary>
		BdpType2,

		/// <summary>
		/// Indicates the BDP type 3.
		/// </summary>
		BdpType3,

		/// <summary>
		/// Indicates the BDP type 4.
		/// </summary>
		BdpType4,

		/// <summary>
		/// Indicates the QDP type 1.
		/// </summary>
		QdpType1,

		/// <summary>
		/// Indicates the QDP type 2.
		/// </summary>
		QdpType2,

		/// <summary>
		/// Indicates the QDP type 3.
		/// </summary>
		QdpType3,

		/// <summary>
		/// Indicates the QDP type 4.
		/// </summary>
		QdpType4,

		/// <summary>
		/// Indicates the locked QDP.
		/// </summary>
		LockedQdp,

		/// <summary>
		/// Indicates the US type 1.
		/// </summary>
		UsType1,

		/// <summary>
		/// Indicates the US type 2.
		/// </summary>
		UsType2,

		/// <summary>
		/// Indicates the US type 3.
		/// </summary>
		UsType3,

		/// <summary>
		/// Indicates the US type 4.
		/// </summary>
		UsType4,

		/// <summary>
		/// Indicates the reverse UR type 1.
		/// </summary>
		ReverseUrType1,

		/// <summary>
		/// Indicates the reverse UR type 2.
		/// </summary>
		ReverseUrType2,

		/// <summary>
		/// Indicates the reverse UR type 3.
		/// </summary>
		ReverseUrType3,

		/// <summary>
		/// Indicates the reverse UR type 4.
		/// </summary>
		ReverseUrType4,

		/// <summary>
		/// Indicates the reverse UL type 1.
		/// </summary>
		ReverseUlType1,

		/// <summary>
		/// Indicates the reverse UL type 2.
		/// </summary>
		ReverseUlType2,

		/// <summary>
		/// Indicates the reverse UL type 3.
		/// </summary>
		ReverseUlType3,

		/// <summary>
		/// Indicates the reverse UL type 4.
		/// </summary>
		ReverseUlType4,

		/// <summary>
		/// Indicates the SdC.
		/// </summary>
		Sdc,

		/// <summary>
		/// Indicates the 3-dimension SdC.
		/// </summary>
		Sdc3d,

		/// <summary>
		/// Indicates the cannibalized SdC.
		/// </summary>
		CannibalizedSdc,

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
		Guardian,

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
		Aic,

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
		GroupedAic,

		/// <summary>
		/// Indicates the nishio FCs.
		/// </summary>
		NishioFc,

		/// <summary>
		/// Indicates the region FCs.
		/// </summary>
		RegionFc,

		/// <summary>
		/// Indicates the cell FCs.
		/// </summary>
		CellFc,

		/// <summary>
		/// Indicates the dynamic region FCs.
		/// </summary>
		DynamicRegionFc,

		/// <summary>
		/// Indicates the dynamic cell FCs.
		/// </summary>
		DynamicCellFc,

		/// <summary>
		/// Indicates the dynamic contradiction FCs.
		/// </summary>
		DynamicContradictionFc,

		/// <summary>
		/// Indicates the dynamic double FCs.
		/// </summary>
		DynamicDoubleFc,

		/// <summary>
		/// Indicates the dynamic FCs.
		/// </summary>
		DynamicFc,

		/// <summary>
		/// Indicates the ERIP.
		/// </summary>
		Erip,

		/// <summary>
		/// Indicates the ESP.
		/// </summary>
		Esp,

		/// <summary>
		/// Indicates the singly linked ALS-XZ.
		/// </summary>
		SinglyLinkedAlsXz,

		/// <summary>
		/// Indicates the doubly linked ALS-XZ.
		/// </summary>
		DoublyLinkedAlsXz,

		/// <summary>
		/// Indicates the ALS-XY-Wing.
		/// </summary>
		AlsXyWing,

		/// <summary>
		/// Indicates the ALS-W-Wing.
		/// </summary>
		AlsWWing,

		/// <summary>
		/// Indicates the death blossom.
		/// </summary>
		DeathBlossom,

		/// <summary>
		/// Indicates the GSP.
		/// </summary>
		Gsp,

		/// <summary>
		/// Indicates the GSP2.
		/// </summary>
		Gsp2,

		/// <summary>
		/// Indicates the JE.
		/// </summary>
		Je,

		/// <summary>
		/// Indicates the SE.
		/// </summary>
		Se,

		/// <summary>
		/// Indicates the complex SE.
		/// </summary>
		ComplexSe,

		/// <summary>
		/// Indicates the siamese JE. 
		/// </summary>
		SiameseJe,

		/// <summary>
		/// Indicates the siamese SE.
		/// </summary>
		SiameseSe,

		/// <summary>
		/// Indicates the SK-Loop.
		/// </summary>
		SkLoop,

		/// <summary>
		/// Indicates the multi-sector locked sets.
		/// </summary>
		Msls,

		/// <summary>
		/// Indicates the POM.
		/// </summary>
		Pom,

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
}
