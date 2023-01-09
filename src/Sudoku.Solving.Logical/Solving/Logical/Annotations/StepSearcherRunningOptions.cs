namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines a list of options that is used by type <see cref="StepSearcherRunningOptionsAttribute"/>,
/// indicating the extra options that are configured and used by <see cref="LogicalSolver"/>
/// to control some extra operation that cannot do on basic options being marked <see cref="StepSearcherPropertyAttribute"/>.
/// </summary>
/// <seealso cref="StepSearcherRunningOptionsAttribute"/>
/// <seealso cref="LogicalSolver"/>
/// <seealso cref="StepSearcherPropertyAttribute"/>
[Flags]
public enum StepSearcherRunningOptions : byte
{
	/// <summary>
	/// Indicates the options is none.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the step searcher will be temporarily disabled due to some reason,
	/// e.g. the step searcher algorithm has bugs waiting to be fixed.
	/// </summary>
	TemporarilyDisabled = 1,

	/// <summary>
	/// Indicates the step searcher is running only on standard sudoku puzzles.
	/// If a puzzle is sukaku, the step searcher will be skipped.
	/// </summary>
	OnlyForStandardSudoku = 2,

	/// <summary>
	/// Indicates the step searcher chooses for a slow algorithm. If <see cref="LogicalSolver.IgnoreSlowAlgorithms"/>
	/// is configured, the step searcher will be ignored.
	/// </summary>
	/// <seealso cref="LogicalSolver.IgnoreSlowAlgorithms"/>
	SlowAlgorithm = 4,

	/// <summary>
	/// Indicates the step searcher chooses for an algorithm that will raise high memory allocation.
	/// If <see cref="LogicalSolver.IgnoreHighAllocationAlgorithms"/> is configured, the step searcher will be ignored.
	/// </summary>
	/// <seealso cref="LogicalSolver.IgnoreHighAllocationAlgorithms"/>
	HighMemoryAllocation = 8
}
