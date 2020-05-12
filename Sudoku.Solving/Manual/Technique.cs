namespace Sudoku.Solving.Manual
{
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
		/// Indicates the hidden single.
		/// </summary>
		HiddenSingle,

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
		MutantFinnedJellyfish,

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

		// TODO: Miss all high-ordered fishes.

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
		/// Indicates the purple cow.
		/// </summary>
		PurpleCow,

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
		UrPlus3x1SL,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		UrPlus3X1SL,

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
		UrPlus4x1SL,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		UrPlus4X1SL,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		UrPlus4x2SL,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		UrPlus4X2SL,

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
		/// Indicates the SDC.
		/// </summary>
		Sdc,

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
		/// Indicates the X-Chain.
		/// </summary>
		XChain,

		/// <summary>
		/// Indicates the XY-Chain.
		/// </summary>
		XyChain,

		/// <summary>
		/// Indicates the XY-X-Chain.
		/// </summary>
		XyXChain,

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
		/// Indicates the grouped XY-Chain.
		/// </summary>
		GroupedXyChain,

		/// <summary>
		/// Indicates the grouped XY-X-Chain.
		/// </summary>
		GroupedXyXChain,

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
		/// Indicates the ERIP.
		/// </summary>
		Erip,

		/// <summary>
		/// Indicates the GSP.
		/// </summary>
		Gsp,

		/// <summary>
		/// Indicates ALs-XZ.
		/// </summary>
		AlsXz,

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
		/// Indicates the JE.
		/// </summary>
		Je,

		/// <summary>
		/// Indicates the SE.
		/// </summary>
		Se,

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
		/// Indicates the CCC.
		/// </summary>
		Ccc,

		/// <summary>
		/// Indicates the brute force.
		/// </summary>
		BruteForce,
	}
}
