namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/> or <see cref="Collector"/>.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="Collector"/>
[DependencyProperty<bool>(SettingItemNames.EnableFullHouse, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.EnableFullHouse)}")]
[DependencyProperty<bool>(SettingItemNames.EnableLastDigit, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.EnableLastDigit)}")]
[DependencyProperty<bool>(SettingItemNames.HiddenSinglesInBlockFirst, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.HiddenSinglesInBlockFirst)}")]
[DependencyProperty<bool>(SettingItemNames.AnalyzerUseIttoryuMode, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.UseIttoryuMode)}")]
[DependencyProperty<bool>(SettingItemNames.AllowDirectPointing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectIntersectionStepSearcher)}.{nameof(DirectIntersectionStepSearcher.AllowDirectPointing)}")]
[DependencyProperty<bool>(SettingItemNames.AllowDirectClaiming, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectIntersectionStepSearcher)}.{nameof(DirectIntersectionStepSearcher.AllowDirectClaiming)}")]
[DependencyProperty<bool>(SettingItemNames.AllowDirectLockedSubset, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectSubsetStepSearcher)}.{nameof(DirectSubsetStepSearcher.AllowDirectLockedSubset)}")]
[DependencyProperty<bool>(SettingItemNames.AllowDirectNakedSubset, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectSubsetStepSearcher)}.{nameof(DirectSubsetStepSearcher.AllowDirectNakedSubset)}")]
[DependencyProperty<bool>(SettingItemNames.AllowDirectLockedHiddenSubset, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectSubsetStepSearcher)}.{nameof(DirectSubsetStepSearcher.AllowDirectLockedHiddenSubset)}")]
[DependencyProperty<bool>(SettingItemNames.AllowDirectHiddenSubset, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectSubsetStepSearcher)}.{nameof(DirectSubsetStepSearcher.AllowDirectHiddenSubset)}")]
[DependencyProperty<bool>(SettingItemNames.AllowIncompleteUniqueRectangles, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(UniqueRectangleStepSearcher)}.{nameof(UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles)}")]
[DependencyProperty<bool>(SettingItemNames.SearchForExtendedUniqueRectangles, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(UniqueRectangleStepSearcher)}.{nameof(UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles)}")]
[DependencyProperty<bool>(SettingItemNames.SearchExtendedBivalueUniversalGraveTypes, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(BivalueUniversalGraveStepSearcher)}.{nameof(BivalueUniversalGraveStepSearcher.SearchExtendedTypes)}")]
[DependencyProperty<bool>(SettingItemNames.CheckValueTypes, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedCandidatesStepSearcher)}.{nameof(AlmostLockedCandidatesStepSearcher.CheckValueTypes)}")]
[DependencyProperty<bool>(SettingItemNames.CheckAlmostLockedQuadruple, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedCandidatesStepSearcher)}.{nameof(AlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple)}")]
[DependencyProperty<bool>(SettingItemNames.AllowCollisionOnAlmostLockedSetXzRule, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>(SettingItemNames.AllowLoopedPatternsOnAlmostLockedSetXzRule, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns)}")]
[DependencyProperty<bool>(SettingItemNames.AllowCollisionOnAlmostLockedSetXyWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXyWingStepSearcher)}.{nameof(AlmostLockedSetsXyWingStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>(SettingItemNames.LogicalSolverIsFullApplying, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IsFullApplying)}")]
[DependencyProperty<bool>(SettingItemNames.LogicalSolverIgnoresSlowAlgorithms, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreSlowAlgorithms)}")]
[DependencyProperty<bool>(SettingItemNames.LogicalSolverIgnoresHighAllocationAlgorithms, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreHighAllocationAlgorithms)}")]
[DependencyProperty<bool>(SettingItemNames.SearchForReverseBugPartiallyUsedTypes, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes)}")]
[DependencyProperty<bool>(SettingItemNames.DisableFinnedOrSashimiXWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(NormalFishStepSearcher)}.{nameof(NormalFishStepSearcher.DisableFinnedOrSashimiXWing)}")]
[DependencyProperty<bool>(SettingItemNames.SearchExtendedDeathBlossomTypes, DefaultValue = false, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DeathBlossomStepSearcher)}.{nameof(DeathBlossomStepSearcher.SearchExtendedTypes)}")]
[DependencyProperty<bool>("AlsoDisplayEnglishNameOfStep", DocSummary = "Indicates whether the step analyzed will also display its English name of the technique used.")]
[DependencyProperty<bool>("DisplayDifficultyRatingForHodoku", DocSummary = "Indicates whether the step will display its corresponding rating defined in program HoDoKu.")]
[DependencyProperty<bool>("DisplayDifficultyRatingForSudokuExplainer", DocSummary = "Indicates whether the step will be displayed its corresponding rating defined in program Sudoku Explainer.")]
[DependencyProperty<bool>("DistinctDirectAndIndirectModes", DocSummary = "Indicates whether the step searchers will distinct for direct and indirect views, in order to re-order the step searchers' calculation, to make the experience better.")]
[DependencyProperty<bool>(SettingItemNames.AllowSiameseNormalFish, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(NormalFishStepSearcher)}.{nameof(NormalFishStepSearcher.AllowSiamese)}")]
[DependencyProperty<bool>(SettingItemNames.AllowSiameseComplexFish, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexFishStepSearcher)}.{nameof(NormalFishStepSearcher.AllowSiamese)}")]
[DependencyProperty<bool>(SettingItemNames.AllowSiameseXyzRing, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(XyzRingStepSearcher)}.{nameof(XyzRingStepSearcher.AllowSiamese)}")]
[DependencyProperty<bool>(SettingItemNames.AllowWWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(IrregularWingStepSearcher)}.{nameof(IrregularWingStepSearcher.AllowWWing)}")]
[DependencyProperty<bool>(SettingItemNames.AllowMWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(IrregularWingStepSearcher)}.{nameof(IrregularWingStepSearcher.AllowMWing)}")]
[DependencyProperty<bool>(SettingItemNames.AllowSWing, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(IrregularWingStepSearcher)}.{nameof(IrregularWingStepSearcher.AllowSWing)}")]
[DependencyProperty<bool>(SettingItemNames.AllowLWing, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(IrregularWingStepSearcher)}.{nameof(IrregularWingStepSearcher.AllowLWing)}")]
[DependencyProperty<bool>(SettingItemNames.AllowHWing, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(IrregularWingStepSearcher)}.{nameof(IrregularWingStepSearcher.AllowHWing)}")]
[DependencyProperty<int>(SettingItemNames.DirectNakedSubsetMaxSize, DefaultValue = 2, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectSubsetStepSearcher)}.{nameof(DirectSubsetStepSearcher.DirectNakedSubsetMaxSize)}")]
[DependencyProperty<int>(SettingItemNames.DirectHiddenSubsetMaxSize, DefaultValue = 2, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DirectSubsetStepSearcher)}.{nameof(DirectSubsetStepSearcher.DirectHiddenSubsetMaxSize)}")]
[DependencyProperty<int>(SettingItemNames.NakedSubsetMaxSizeInComplexSingle, DefaultValue = 4, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexSingleStepSearcher)}.{nameof(ComplexSingleStepSearcher.NakedSubsetMaxSize)}")]
[DependencyProperty<int>(SettingItemNames.HiddenSubsetMaxSizeInComplexSingle, DefaultValue = 4, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexSingleStepSearcher)}.{nameof(ComplexSingleStepSearcher.HiddenSubsetMaxSize)}")]
[DependencyProperty<int>(SettingItemNames.MaxSizeOfRegularWing, DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(RegularWingStepSearcher)}.{nameof(RegularWingStepSearcher.MaxSearchingPivotsCount)}")]
[DependencyProperty<int>(SettingItemNames.AlignedExclusionMaxSearchingSize, DefaultValue = 3, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlignedExclusionStepSearcher)}.{nameof(AlignedExclusionStepSearcher.MaxSearchingSize)}")]
[DependencyProperty<int>(SettingItemNames.ReverseBugMaxSearchingEmptyCellsCount, DefaultValue = 2, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount)}")]
[DependencyProperty<int>(SettingItemNames.MaxSizeOfComplexFish, DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexFishStepSearcher)}.{nameof(ComplexFishStepSearcher.MaxSize)}")]
[DependencyProperty<int>(SettingItemNames.StepGathererMaxStepsGathered, DefaultValue = 1000, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Collector)}.{nameof(Collector.MaxStepsGathered)}")]
[DependencyProperty<int>(SettingItemNames.DifficultyLevelMode, DefaultValue = 0, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Collector)}.{nameof(Collector.DifficultyLevelMode)}")]
[DependencyProperty<List<Technique>>(SettingItemNames.IttoryuSupportedTechniques, DocReferencedMemberName = $"global::Sudoku.Ittoryu.{nameof(DisorderedIttoryuFinder)}.{nameof(DisorderedIttoryuFinder.SupportedTechniques)}")]
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
}
