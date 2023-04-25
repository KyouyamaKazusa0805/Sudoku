namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/> or <see cref="StepCollector"/>.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="StepCollector"/>
[DependencyProperty<bool>("EnableFullHouse", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.EnableFullHouse)}")]
[DependencyProperty<bool>("EnableLastDigit", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.EnableLastDigit)}")]
[DependencyProperty<bool>("HiddenSinglesInBlockFirst", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.HiddenSinglesInBlockFirst)}")]
[DependencyProperty<bool>("AnalyzerUseIttoryuMode", DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(SingleStepSearcher)}.{nameof(SingleStepSearcher.UseIttoryuMode)}")]
[DependencyProperty<bool>("AllowIncompleteUniqueRectangles", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(UniqueRectangleStepSearcher)}.{nameof(UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles)}")]
[DependencyProperty<bool>("SearchForExtendedUniqueRectangles", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(UniqueRectangleStepSearcher)}.{nameof(UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles)}")]
[DependencyProperty<bool>("SearchExtendedBivalueUniversalGraveTypes", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(BivalueUniversalGraveStepSearcher)}.{nameof(BivalueUniversalGraveStepSearcher.SearchExtendedTypes)}")]
[DependencyProperty<bool>("AllowCollisionOnAlmostLockedSetXzRule", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>("AllowLoopedPatternsOnAlmostLockedSetXzRule", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXzStepSearcher)}.{nameof(AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns)}")]
[DependencyProperty<bool>("AllowCollisionOnAlmostLockedSetXyWing", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlmostLockedSetsXyWingStepSearcher)}.{nameof(AlmostLockedSetsXyWingStepSearcher.AllowCollision)}")]
[DependencyProperty<bool>("LogicalSolverIsFullApplying", DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IsFullApplying)}")]
[DependencyProperty<bool>("LogicalSolverIgnoresSlowAlgorithms", DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreSlowAlgorithms)}")]
[DependencyProperty<bool>("LogicalSolverIgnoresHighAllocationAlgorithms", DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(Analyzer)}.{nameof(Analyzer.IgnoreHighAllocationAlgorithms)}")]
[DependencyProperty<bool>("StepGathererOnlySearchSameLevelTechniquesInFindAllSteps", DefaultValue = true, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(StepCollector)}.{nameof(StepCollector.OnlyShowSameLevelTechniquesInFindAllSteps)}")]
[DependencyProperty<int>("MaxSizeOfRegularWing", DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(RegularWingStepSearcher)}.{nameof(RegularWingStepSearcher.MaxSearchingPivotsCount)}")]
[DependencyProperty<int>("AlignedExclusionMaxSearchingSize", DefaultValue = 3, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(AlignedExclusionStepSearcher)}.{nameof(AlignedExclusionStepSearcher.MaxSearchingSize)}")]
[DependencyProperty<int>("ReverseBugMaxSearchingEmptyCellsCount", DefaultValue = 2, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ReverseBivalueUniversalGraveStepSearcher)}.{nameof(ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount)}")]
[DependencyProperty<int>("MaxSizeOfComplexFish", DefaultValue = 5, DocReferencedMemberName = $"global::Sudoku.Analytics.StepSearchers.{nameof(ComplexFishStepSearcher)}.{nameof(ComplexFishStepSearcher.MaxSize)}")]
[DependencyProperty<int>("StepGathererMaxStepsGathered", DefaultValue = 1000, DocReferencedMemberName = $"global::Sudoku.Analytics.{nameof(StepCollector)}.{nameof(StepCollector.MaxStepsGathered)}")]
public sealed partial class AnalysisPreferenceGroup : PreferenceGroup;
