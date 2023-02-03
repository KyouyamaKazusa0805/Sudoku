namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="LogicalSolver"/>.
/// </summary>
/// <seealso cref="LogicalSolver"/>
public sealed class AnalysisPreferenceGroup : PreferenceGroup
{
	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_EnableFullHouse"/>
	public bool EnableFullHouse { get; set; }

	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_EnableLastDigit"/>
	public bool EnableLastDigit { get; set; }

	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_HiddenSinglesInBlockFirst"/>
	public bool HiddenSinglesInBlockFirst { get; set; }

	/// <inheritdoc cref="LogicalSolver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles"/>
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <inheritdoc cref="LogicalSolver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles"/>
	public bool SearchForExtendedUniqueRectangles { get; set; }

	/// <inheritdoc cref="LogicalSolver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes"/>
	public bool SearchExtendedBivalueUniversalGraveTypes { get; set; }

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnAlmostLockedSetXzRule { get; set; }

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns"/>
	public bool AllowLoopedPatternsOnAlmostLockedSetXzRule { get; set; }

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXyWingStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnAlmostLockedSetXyWing { get; set; }

	/// <inheritdoc cref="LogicalSolver.IsFullApplying"/>
	public bool LogicalSolverIsFullApplying { get; set; }

	/// <inheritdoc cref="LogicalSolver.IgnoreSlowAlgorithms"/>
	public bool LogicalSolverIgnoresSlowAlgorithms { get; set; }

	/// <inheritdoc cref="LogicalSolver.IgnoreHighAllocationAlgorithms"/>
	public bool LogicalSolverIgnoresHighAllocationAlgorithms { get; set; }

	/// <inheritdoc cref="StepsGatherer.OnlyShowSameLevelTechniquesInFindAllSteps"/>
	public bool StepGathererOnlySearchSameLevelTechniquesInFindAllSteps { get; set; }

	/// <inheritdoc cref="LogicalSolver.RegularWingStepSearcher_MaxSize"/>
	public int MaxSizeOfRegularWing { get; set; }

	/// <inheritdoc cref="LogicalSolver.ComplexFishStepSearcher_MaxSize"/>
	public int MaxSizeOfComplexFish { get; set; }

	/// <inheritdoc cref="StepsGatherer.MaxStepsGathered"/>
	public int StepGathererMaxStepsGathered { get; set; }


	/// <inheritdoc/>
	public override void CoverProperties()
	{
		var solver = ((App)Application.Current).EnvironmentVariables.Solver;
		solver.SingleStepSearcher_EnableFullHouse = EnableFullHouse;
		solver.SingleStepSearcher_EnableLastDigit = EnableLastDigit;
		solver.SingleStepSearcher_HiddenSinglesInBlockFirst = HiddenSinglesInBlockFirst;
		solver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = AllowIncompleteUniqueRectangles;
		solver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = SearchForExtendedUniqueRectangles;
		solver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes = SearchExtendedBivalueUniversalGraveTypes;
		solver.AlmostLockedSetsXzStepSearcher_AllowCollision = AllowCollisionOnAlmostLockedSetXzRule;
		solver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = AllowLoopedPatternsOnAlmostLockedSetXzRule;
		solver.AlmostLockedSetsXyWingStepSearcher_AllowCollision = AllowCollisionOnAlmostLockedSetXyWing;
		solver.IsFullApplying = LogicalSolverIsFullApplying;
		solver.IgnoreSlowAlgorithms = LogicalSolverIgnoresSlowAlgorithms;
		solver.IgnoreHighAllocationAlgorithms = LogicalSolverIgnoresHighAllocationAlgorithms;
		solver.RegularWingStepSearcher_MaxSize = MaxSizeOfRegularWing;
		solver.ComplexFishStepSearcher_MaxSize = MaxSizeOfComplexFish;

		var gatherer = ((App)Application.Current).EnvironmentVariables.Gatherer;
		gatherer.OnlyShowSameLevelTechniquesInFindAllSteps = StepGathererOnlySearchSameLevelTechniquesInFindAllSteps;
		gatherer.MaxStepsGathered = StepGathererMaxStepsGathered;
	}
}
