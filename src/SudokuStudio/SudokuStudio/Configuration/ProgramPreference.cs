namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a user-defined program preference.
/// </summary>
public sealed class ProgramPreference
{
	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_EnableFullHouse"/>
	public bool EnableFullHouse = true;

	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_EnableLastDigit"/>
	public bool EnableLastDigit = true;

	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_HiddenSinglesInBlockFirst"/>
	public bool HiddenSinglesInBlockFirst = true;

	/// <inheritdoc cref="LogicalSolver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles"/>
	public bool AllowIncompleteUniqueRectangles = true;

	/// <inheritdoc cref="LogicalSolver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles"/>
	public bool SearchForExtendedUniqueRectangles = true;

	/// <inheritdoc cref="LogicalSolver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes"/>
	public bool SearchExtendedBivalueUniversalGraveTypes = true;

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnAlmostLockedSetXzRule = true;

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns"/>
	public bool AllowLoopedPatternsOnAlmostLockedSetXzRule = true;

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXyWingStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnAlmostLockedSetXyWing = true;

	/// <inheritdoc cref="LogicalSolver.IsFullApplying"/>
	public bool LogicalSolverIsFullApplying = false;

	/// <inheritdoc cref="LogicalSolver.IgnoreSlowAlgorithms"/>
	public bool LogicalSolverIgnoresSlowAlgorithms = false;

	/// <inheritdoc cref="LogicalSolver.IgnoreHighAllocationAlgorithms"/>
	public bool LogicalSolverIgnoresHighAllocationAlgorithms = false;

	/// <inheritdoc cref="LogicalSolver.RegularWingStepSearcher_MaxSize"/>
	public int MaxSizeOfRegularWing = 5;

	/// <inheritdoc cref="LogicalSolver.ComplexFishStepSearcher_MaxSize"/>
	public int MaxSizeOfComplexFish = 5;
}
