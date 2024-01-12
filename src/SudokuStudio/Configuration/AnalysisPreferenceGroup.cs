namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/> or <see cref="StepCollector"/>.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="StepCollector"/>
[DependencyProperty<bool>(RuntimeIdentifier.EnableFullHouse, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.EnableFullHouse)}")]
[DependencyProperty<bool>(RuntimeIdentifier.EnableLastDigit, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.EnableLastDigit)}")]
[DependencyProperty<bool>(RuntimeIdentifier.HiddenSinglesInBlockFirst, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.HiddenSinglesInBlockFirst)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AnalyzerUseIttoryuMode, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.UseIttoryuMode)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowIncompleteUniqueRectangles, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(UniqueRectangleStepSearcher)}.{nameof(UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles)}")]
[DependencyProperty<bool>(RuntimeIdentifier.SearchForExtendedUniqueRectangles, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(UniqueRectangleStepSearcher)}.{nameof(UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles)}")]
[DependencyProperty<bool>(RuntimeIdentifier.SearchExtendedBivalueUniversalGraveTypes, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(BivalueUniversalGraveStepSearcher)}.{nameof(BivalueUniversalGraveStepSearcher.SearchExtendedTypes)}")]
[DependencyProperty<bool>(RuntimeIdentifier.CheckValueTypes, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedCandidatesStepSearcher)}.{nameof(AlmostLockedCandidatesStepSearcher.CheckValueTypes)}")]
[DependencyProperty<bool>(RuntimeIdentifier.CheckAlmostLockedQuadruple, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedCandidatesStepSearcher)}.{nameof(AlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXzRule, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowLoopedPatternsOnAlmostLockedSetXzRule, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXyWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXyWingStepSearcher)}.{nameof(AlmostLockedSetsXyWingStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>(RuntimeIdentifier.LogicalSolverIsFullApplying, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IsFullApplying)}")]
[DependencyProperty<bool>(RuntimeIdentifier.LogicalSolverIgnoresSlowAlgorithms, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreSlowAlgorithms)}")]
[DependencyProperty<bool>(RuntimeIdentifier.LogicalSolverIgnoresHighAllocationAlgorithms, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreHighAllocationAlgorithms)}")]
[DependencyProperty<bool>(RuntimeIdentifier.SearchForReverseBugPartiallyUsedTypes, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes)}")]
[DependencyProperty<bool>(RuntimeIdentifier.DisableFinnedOrSashimiXWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(NormalFishStepSearcher)}.{nameof(NormalFishStepSearcher.DisableFinnedOrSashimiXWing)}")]
[DependencyProperty<bool>(RuntimeIdentifier.SearchExtendedDeathBlossomTypes, DefaultValue = false, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(DeathBlossomStepSearcher)}.{nameof(DeathBlossomStepSearcher.SearchExtendedTypes)}")]
[DependencyProperty<bool>("AlsoDisplayEnglishNameOfStep", DocSummary = "Indicates whether the step analyzed will also display its English name of the technique used.")]
[DependencyProperty<bool>("DisplayDifficultyRatingForHodoku", DocSummary = "Indicates whether the step will display its corresponding rating defined in program HoDoKu.")]
[DependencyProperty<bool>("DisplayDifficultyRatingForSudokuExplainer", DocSummary = "Indicates whether the step will be displayed its corresponding rating defined in program Sudoku Explainer.")]
[DependencyProperty<bool>("DistinctDirectAndIndirectModes", DocSummary = "Indicates whether the step searchers will distinct for direct and indirect views, in order to re-order the step searchers' calculation, to make the experience better.")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowSiameseNormalFish, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(NormalFishStepSearcher)}.{nameof(NormalFishStepSearcher.AllowSiamese)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowSiameseComplexFish, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexFishStepSearcher)}.{nameof(NormalFishStepSearcher.AllowSiamese)}")]
[DependencyProperty<int>(RuntimeIdentifier.MaxSizeOfRegularWing, DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(RegularWingStepSearcher)}.{nameof(RegularWingStepSearcher.MaxSearchingPivotsCount)}")]
[DependencyProperty<int>(RuntimeIdentifier.AlignedExclusionMaxSearchingSize, DefaultValue = 3, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlignedExclusionStepSearcher)}.{nameof(AlignedExclusionStepSearcher.MaxSearchingSize)}")]
[DependencyProperty<int>(RuntimeIdentifier.ReverseBugMaxSearchingEmptyCellsCount, DefaultValue = 2, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount)}")]
[DependencyProperty<int>(RuntimeIdentifier.MaxSizeOfComplexFish, DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexFishStepSearcher)}.{nameof(ComplexFishStepSearcher.MaxSize)}")]
[DependencyProperty<int>(RuntimeIdentifier.StepGathererMaxStepsGathered, DefaultValue = 1000, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(StepCollector)}.{nameof(StepCollector.MaxStepsGathered)}")]
[DependencyProperty<int>(RuntimeIdentifier.DifficultyLevelMode, DefaultValue = 0, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(StepCollector)}.{nameof(StepCollector.DifficultyLevelMode)}")]
[DependencyProperty<List<Technique>>(RuntimeIdentifier.IttoryuSupportedTechniques, DocReferencedMemberName = $"global::Sudoku.Algorithm.Ittoryu.{nameof(IttoryuPathFinder)}.{nameof(IttoryuPathFinder.SupportedTechniques)}")]
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
