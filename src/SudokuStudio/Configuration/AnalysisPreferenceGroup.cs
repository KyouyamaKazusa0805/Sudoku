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
[DependencyProperty<bool>(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXzRule, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowLoopedPatternsOnAlmostLockedSetXzRule, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns)}")]
[DependencyProperty<bool>(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXyWing, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXyWingStepSearcher)}.{nameof(AlmostLockedSetsXyWingStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>(RuntimeIdentifier.LogicalSolverIsFullApplying, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IsFullApplying)}")]
[DependencyProperty<bool>(RuntimeIdentifier.LogicalSolverIgnoresSlowAlgorithms, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreSlowAlgorithms)}")]
[DependencyProperty<bool>(RuntimeIdentifier.LogicalSolverIgnoresHighAllocationAlgorithms, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreHighAllocationAlgorithms)}")]
[DependencyProperty<bool>(RuntimeIdentifier.StepGathererOnlySearchSameLevelTechniquesInFindAllSteps, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(StepCollector)}.{nameof(StepCollector.OnlyShowSameLevelTechniquesInFindAllSteps)}")]
[DependencyProperty<bool>(RuntimeIdentifier.SearchForReverseBugPartiallyUsedTypes, DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes)}")]
[DependencyProperty<bool>("AlsoDisplayEnglishNameOfStep", DocSummary = "Indicates whether the step analyzed will also display its English name of the technique used.")]
[DependencyProperty<bool>("DisplayDifficultyRatingForHodoku", DocSummary = "Indicates whether the step will display its corresponding rating defined in program HoDoKu.")]
[DependencyProperty<bool>("DisplayDifficultyRatingForSudokuExplainer", DocSummary = "Indicates whether the step will be displayed its corresponding rating defined in program Sudoku Explainer.")]
[DependencyProperty<int>(RuntimeIdentifier.MaxSizeOfRegularWing, DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(RegularWingStepSearcher)}.{nameof(RegularWingStepSearcher.MaxSearchingPivotsCount)}")]
[DependencyProperty<int>(RuntimeIdentifier.AlignedExclusionMaxSearchingSize, DefaultValue = 3, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlignedExclusionStepSearcher)}.{nameof(AlignedExclusionStepSearcher.MaxSearchingSize)}")]
[DependencyProperty<int>(RuntimeIdentifier.ReverseBugMaxSearchingEmptyCellsCount, DefaultValue = 2, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount)}")]
[DependencyProperty<int>(RuntimeIdentifier.MaxSizeOfComplexFish, DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexFishStepSearcher)}.{nameof(ComplexFishStepSearcher.MaxSize)}")]
[DependencyProperty<int>(RuntimeIdentifier.StepGathererMaxStepsGathered, DefaultValue = 1000, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(StepCollector)}.{nameof(StepCollector.MaxStepsGathered)}")]
public sealed partial class AnalysisPreferenceGroup : PreferenceGroup;
