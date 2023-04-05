namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="LogicalSolver"/>.
/// </summary>
/// <seealso cref="LogicalSolver"/>
[DependencyProperty<bool>("EnableFullHouse", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.SingleStepSearcher_EnableFullHouse")]
[DependencyProperty<bool>("EnableLastDigit", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.SingleStepSearcher_EnableLastDigit")]
[DependencyProperty<bool>("HiddenSinglesInBlockFirst", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.SingleStepSearcher_HiddenSinglesInBlockFirst")]
[DependencyProperty<bool>("AnalyzerUseIttoryuMode", DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.SingleStepSearcher_UseIttoryuMode")]
[DependencyProperty<bool>("AllowIncompleteUniqueRectangles", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles")]
[DependencyProperty<bool>("SearchForExtendedUniqueRectangles", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles")]
[DependencyProperty<bool>("SearchExtendedBivalueUniversalGraveTypes", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes")]
[DependencyProperty<bool>("AllowCollisionOnAlmostLockedSetXzRule", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowCollision")]
[DependencyProperty<bool>("AllowLoopedPatternsOnAlmostLockedSetXzRule", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns")]
[DependencyProperty<bool>("AllowCollisionOnAlmostLockedSetXyWing", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.AlmostLockedSetsXyWingStepSearcher_AllowCollision")]
[DependencyProperty<bool>("LogicalSolverIsFullApplying", DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.IsFullApplying")]
[DependencyProperty<bool>("LogicalSolverIgnoresSlowAlgorithms", DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.IgnoreSlowAlgorithms")]
[DependencyProperty<bool>("LogicalSolverIgnoresHighAllocationAlgorithms", DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.IgnoreHighAllocationAlgorithms")]
[DependencyProperty<bool>("StepGathererOnlySearchSameLevelTechniquesInFindAllSteps", DefaultValue = true, DocReferencedMemberName = "global::Sudoku.Solving.Logical.StepGatherers.StepsGatherer.OnlyShowSameLevelTechniquesInFindAllSteps")]
[DependencyProperty<int>("MaxSizeOfRegularWing", DefaultValue = 5, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.RegularWingStepSearcher_MaxSize")]
[DependencyProperty<int>("MaxSizeOfComplexFish", DefaultValue = 5, DocReferencedMemberName = "global::Sudoku.Solving.Logical.LogicalSolver.ComplexFishStepSearcher_MaxSize")]
[DependencyProperty<int>("StepGathererMaxStepsGathered", DefaultValue = 1000, DocReferencedMemberName = "global::Sudoku.Solving.Logical.StepGatherers.StepsGatherer.MaxStepsGathered")]
public sealed partial class AnalysisPreferenceGroup : PreferenceGroup;
