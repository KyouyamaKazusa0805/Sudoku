namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/> or <see cref="Collector"/>.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="Collector"/>
public sealed partial class AnalysisPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly List<Technique> IttoryuSupportedTechniquesDefaultValue = [
		Technique.FullHouse,
		Technique.HiddenSingleBlock,
		Technique.HiddenSingleRow,
		Technique.HiddenSingleColumn,
		Technique.NakedSingle
	];


	/// <inheritdoc cref="SingleStepSearcher.EnableFullHouse"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool EnableFullHouse { get; set; }

	/// <inheritdoc cref="SingleStepSearcher.EnableLastDigit"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool EnableLastDigit { get; set; }

	/// <inheritdoc cref="SingleStepSearcher.HiddenSinglesInBlockFirst"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool HiddenSinglesInBlockFirst { get; set; }

	/// <inheritdoc cref="SingleStepSearcher.UseIttoryuMode"/>
	[AutoDependencyProperty]
	public partial bool AnalyzerUseIttoryuMode { get; set; }

	/// <inheritdoc cref="DirectIntersectionStepSearcher.AllowDirectPointing"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowDirectPointing { get; set; }

	/// <inheritdoc cref="DirectIntersectionStepSearcher.AllowDirectClaiming"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowDirectClaiming { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectLockedSubset"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowDirectLockedSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectNakedSubset"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowDirectNakedSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectLockedHiddenSubset"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowDirectLockedHiddenSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectHiddenSubset"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowDirectHiddenSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.DirectNakedSubsetMaxSize"/>
	[AutoDependencyProperty(DefaultValue = 2)]
	public partial int DirectNakedSubsetMaxSize { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.DirectHiddenSubsetMaxSize"/>
	[AutoDependencyProperty(DefaultValue = 2)]
	public partial int DirectHiddenSubsetMaxSize { get; set; }

	/// <inheritdoc cref="ComplexSingleStepSearcher.NakedSubsetMaxSize"/>
	[AutoDependencyProperty(DefaultValue = 4)]
	public partial int NakedSubsetMaxSizeInComplexSingle { get; set; }

	/// <inheritdoc cref="ComplexSingleStepSearcher.HiddenSubsetMaxSize"/>
	[AutoDependencyProperty(DefaultValue = 4)]
	public partial int HiddenSubsetMaxSizeInComplexSingle { get; set; }

	/// <inheritdoc cref="NormalFishStepSearcher.DisableFinnedOrSashimiXWing"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool DisableFinnedOrSashimiXWing { get; set; }

	/// <inheritdoc cref="NormalFishStepSearcher.AllowSiamese"/>
	[AutoDependencyProperty]
	public partial bool AllowSiameseNormalFish { get; set; }

	/// <inheritdoc cref="ComplexFishStepSearcher.AllowSiamese"/>
	[AutoDependencyProperty]
	public partial bool AllowSiameseComplexFish { get; set; }

	/// <inheritdoc cref="ComplexFishStepSearcher.MaxSize"/>
	[AutoDependencyProperty(DefaultValue = 5)]
	public partial int MaxSizeOfComplexFish { get; set; }

	/// <inheritdoc cref="UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowIncompleteUniqueRectangles { get; set; }

	/// <inheritdoc cref="UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool SearchForExtendedUniqueRectangles { get; set; }

	/// <inheritdoc cref="BivalueUniversalGraveStepSearcher.SearchExtendedTypes"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool SearchExtendedBivalueUniversalGraveTypes { get; set; }

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckValueTypes"/>
	[AutoDependencyProperty]
	public partial bool CheckValueTypes { get; set; }

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	[AutoDependencyProperty]
	public partial bool CheckAlmostLockedQuadruple { get; set; }

	/// <inheritdoc cref="XyzRingStepSearcher.AllowSiamese"/>
	[AutoDependencyProperty]
	public partial bool AllowSiameseXyzRing { get; set; }

	/// <inheritdoc cref="RegularWingStepSearcher.MaxSearchingPivotsCount"/>
	[AutoDependencyProperty(DefaultValue = 5)]
	public partial int MaxSizeOfRegularWing { get; set; }

	/// <inheritdoc cref="AlignedExclusionStepSearcher.MaxSearchingSize"/>
	[AutoDependencyProperty(DefaultValue = 3)]
	public partial int AlignedExclusionMaxSearchingSize { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowCollision"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowCollisionOnAlmostLockedSetsXzRule { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowLoopedPatternsOnAlmostLockedSetsXzRule { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsXyWingStepSearcher.AllowCollision"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowCollisionOnAlmostLockedSetsXyWing { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsWWingStepSearcher.AllowCollision"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool AllowCollisionOnAlmostLockedSetsWWing { get; set; }

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool SearchForReverseBugPartiallyUsedTypes { get; set; }

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount"/>
	[AutoDependencyProperty(DefaultValue = 2)]
	public partial int ReverseBugMaxSearchingEmptyCellsCount { get; set; }

	/// <inheritdoc cref="DeathBlossomStepSearcher.SearchExtendedTypes"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool SearchExtendedDeathBlossomTypes { get; set; }

	/// <summary>
	/// Indicates whether the step will be displayed its corresponding rating defined in program Sudoku Explainer.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool DisplayDifficultyRatingForSudokuExplainer { get; set; }


	/// <summary>
	/// Indicates whether the step analyzed will also display its English name of the technique used.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool AlsoDisplayEnglishNameOfStep { get; set; }

	/// <summary>
	/// Indicates whether the step will display its corresponding rating defined in program HoDoKu.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool DisplayDifficultyRatingForHodoku { get; set; }

	/// <summary>
	/// Indicates whether the step searchers will distinct for direct and indirect views,
	/// in order to re-order the step searchers' calculation, to make the experience better.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool DistinctDirectAndIndirectModes { get; set; }


	/// <inheritdoc cref="Analyzer.IsFullApplying"/>
	[AutoDependencyProperty]
	public partial bool AnalyzerIsFullApplying { get; set; }

	/// <inheritdoc cref="Analyzer.IgnoreSlowAlgorithms"/>
	[AutoDependencyProperty]
	public partial bool AnalyzerIgnoresSlowAlgorithms { get; set; }

	/// <inheritdoc cref="Analyzer.IgnoreHighAllocationAlgorithms"/>
	[AutoDependencyProperty]
	public partial bool AnalyzerIgnoresHighAllocationAlgorithms { get; set; }


	/// <inheritdoc cref="Collector.MaxStepsCollected"/>
	[AutoDependencyProperty(DefaultValue = 1000)]
	public partial int CollectorMaxStepsCollected { get; set; }

	/// <inheritdoc cref="Collector.DifficultyLevelMode"/>
	[AutoDependencyProperty(DefaultValue = 0)]
	public partial int DifficultyLevelMode { get; set; }


	/// <inheritdoc cref="DisorderedIttoryuFinder.SupportedTechniques"/>
	[AutoDependencyProperty]
	public partial List<Technique> IttoryuSupportedTechniques { get; set; }
}
