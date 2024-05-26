namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents a list of <see cref="string"/>s indicating the runtime identifier recognized by UI,
/// used by <see cref="SettingItemNameAttribute"/>.
/// </summary>
/// <seealso cref="SettingItemNameAttribute"/>
public static class SettingItemNames
{
	//
	// Step searcher property names
	//
	/// <inheritdoc cref="SingleStepSearcher.EnableFullHouse"/>
	public const string EnableFullHouse = nameof(EnableFullHouse);

	/// <inheritdoc cref="SingleStepSearcher.EnableLastDigit"/>
	public const string EnableLastDigit = nameof(EnableLastDigit);

	/// <inheritdoc cref="SingleStepSearcher.HiddenSinglesInBlockFirst"/>
	public const string HiddenSinglesInBlockFirst = nameof(HiddenSinglesInBlockFirst);

	/// <inheritdoc cref="SingleStepSearcher.UseIttoryuMode"/>
	public const string AnalyzerUseIttoryuMode = nameof(AnalyzerUseIttoryuMode);

	/// <inheritdoc cref="DirectIntersectionStepSearcher.AllowDirectPointing"/>
	public const string AllowDirectPointing = nameof(AllowDirectPointing);

	/// <inheritdoc cref="DirectIntersectionStepSearcher.AllowDirectClaiming"/>
	public const string AllowDirectClaiming = nameof(AllowDirectClaiming);

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectHiddenSubset"/>
	public const string AllowDirectHiddenSubset = nameof(AllowDirectHiddenSubset);

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectLockedHiddenSubset"/>
	public const string AllowDirectLockedHiddenSubset = nameof(AllowDirectLockedHiddenSubset);

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectNakedSubset"/>
	public const string AllowDirectNakedSubset = nameof(AllowDirectNakedSubset);

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectLockedSubset"/>
	public const string AllowDirectLockedSubset = nameof(AllowDirectLockedSubset);

	/// <inheritdoc cref="DirectSubsetStepSearcher.DirectNakedSubsetMaxSize"/>
	public const string DirectNakedSubsetMaxSize = nameof(DirectNakedSubsetMaxSize);

	/// <inheritdoc cref="DirectSubsetStepSearcher.DirectHiddenSubsetMaxSize"/>
	public const string DirectHiddenSubsetMaxSize = nameof(DirectHiddenSubsetMaxSize);

	/// <inheritdoc cref="ComplexSingleStepSearcher.NakedSubsetMaxSize"/>
	public const string NakedSubsetMaxSizeInComplexSingle = nameof(NakedSubsetMaxSizeInComplexSingle);

	/// <inheritdoc cref="ComplexSingleStepSearcher.HiddenSubsetMaxSize"/>
	public const string HiddenSubsetMaxSizeInComplexSingle = nameof(HiddenSubsetMaxSizeInComplexSingle);

	/// <inheritdoc cref="NormalFishStepSearcher.DisableFinnedOrSashimiXWing"/>
	public const string DisableFinnedOrSashimiXWing = nameof(DisableFinnedOrSashimiXWing);

	/// <inheritdoc cref="NormalFishStepSearcher.AllowSiamese"/>
	public const string AllowSiameseNormalFish = nameof(AllowSiameseNormalFish);

	/// <inheritdoc cref="ComplexFishStepSearcher.AllowSiamese"/>
	public const string AllowSiameseComplexFish = nameof(AllowSiameseComplexFish);

	/// <inheritdoc cref="XyzRingStepSearcher.AllowSiamese"/>
	public const string AllowSiameseXyzRing = nameof(AllowSiameseXyzRing);

	/// <inheritdoc cref="IrregularWingStepSearcher.AllowWWing"/>
	public const string AllowWWing = nameof(AllowWWing);

	/// <inheritdoc cref="IrregularWingStepSearcher.AllowMWing"/>
	public const string AllowMWing = nameof(AllowMWing);

	/// <inheritdoc cref="IrregularWingStepSearcher.AllowSWing"/>
	public const string AllowSWing = nameof(AllowSWing);

	/// <inheritdoc cref="IrregularWingStepSearcher.AllowLWing"/>
	public const string AllowLWing = nameof(AllowLWing);

	/// <inheritdoc cref="IrregularWingStepSearcher.AllowHWing"/>
	public const string AllowHWing = nameof(AllowHWing);

	/// <inheritdoc cref="UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles"/>
	public const string AllowIncompleteUniqueRectangles = nameof(AllowIncompleteUniqueRectangles);

	/// <inheritdoc cref="UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles"/>
	public const string SearchForExtendedUniqueRectangles = nameof(SearchForExtendedUniqueRectangles);

	/// <inheritdoc cref="BivalueUniversalGraveStepSearcher.SearchExtendedTypes"/>
	public const string SearchExtendedBivalueUniversalGraveTypes = nameof(SearchExtendedBivalueUniversalGraveTypes);

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowCollision"/>
	public const string AllowCollisionOnAlmostLockedSetXzRule = nameof(AllowCollisionOnAlmostLockedSetXzRule);

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns"/>
	public const string AllowLoopedPatternsOnAlmostLockedSetXzRule = nameof(AllowLoopedPatternsOnAlmostLockedSetXzRule);

	/// <inheritdoc cref="AlmostLockedSetsXyWingStepSearcher.AllowCollision"/>
	public const string AllowCollisionOnAlmostLockedSetXyWing = nameof(AllowCollisionOnAlmostLockedSetXyWing);

	/// <inheritdoc cref="AlmostLockedSetsWWingStepSearcher.AllowCollision"/>
	public const string AllowCollisionOnAlmostLockedSetWWing = nameof(AllowCollisionOnAlmostLockedSetWWing);

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes"/>
	public const string SearchForReverseBugPartiallyUsedTypes = nameof(SearchForReverseBugPartiallyUsedTypes);

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount"/>
	public const string ReverseBugMaxSearchingEmptyCellsCount = nameof(ReverseBugMaxSearchingEmptyCellsCount);

	/// <inheritdoc cref="AlignedExclusionStepSearcher.MaxSearchingSize"/>
	public const string AlignedExclusionMaxSearchingSize = nameof(AlignedExclusionMaxSearchingSize);

	/// <inheritdoc cref="RegularWingStepSearcher.MaxSearchingPivotsCount"/>
	public const string MaxSizeOfRegularWing = nameof(MaxSizeOfRegularWing);

	/// <inheritdoc cref="ComplexFishStepSearcher.MaxSize"/>
	public const string MaxSizeOfComplexFish = nameof(MaxSizeOfComplexFish);

	/// <inheritdoc cref="TemplateStepSearcher.TemplateDeleteOnly"/>
	public const string TemplateDeleteOnly = nameof(TemplateDeleteOnly);

	/// <inheritdoc cref="BowmanBingoStepSearcher.MaxLength"/>
	public const string BowmanBingoMaxLength = nameof(BowmanBingoMaxLength);

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	public const string CheckAlmostLockedQuadruple = nameof(CheckAlmostLockedQuadruple);

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckValueTypes"/>
	public const string CheckValueTypes = nameof(CheckValueTypes);

	/// <inheritdoc cref="DeathBlossomStepSearcher.SearchExtendedTypes"/>
	public const string SearchExtendedDeathBlossomTypes = nameof(SearchExtendedDeathBlossomTypes);

	//
	// Analyzer & Collector property names
	//
	/// <inheritdoc cref="Analyzer.IsFullApplying"/>
	public const string LogicalSolverIsFullApplying = nameof(LogicalSolverIsFullApplying);

	/// <inheritdoc cref="Analyzer.IgnoreSlowAlgorithms"/>
	public const string LogicalSolverIgnoresSlowAlgorithms = nameof(LogicalSolverIgnoresSlowAlgorithms);

	/// <inheritdoc cref="Analyzer.IgnoreHighAllocationAlgorithms"/>
	public const string LogicalSolverIgnoresHighAllocationAlgorithms = nameof(LogicalSolverIgnoresHighAllocationAlgorithms);

	/// <inheritdoc cref="Collector.DifficultyLevelMode"/>
	public const string DifficultyLevelMode = nameof(DifficultyLevelMode);

	/// <inheritdoc cref="Collector.MaxStepsGathered"/>
	public const string StepGathererMaxStepsGathered = nameof(StepGathererMaxStepsGathered);

	/// <summary>
	/// Indicates the supported techniques used in ittoryu mode.
	/// </summary>
	public const string IttoryuSupportedTechniques = nameof(IttoryuSupportedTechniques);
}
