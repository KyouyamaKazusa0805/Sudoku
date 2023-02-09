#if false
namespace SudokuStudio.AppLifecycle;

/// <summary>
/// Provides with a set of properties or fields that is used by the program in its lifecycle.
/// </summary>
internal sealed class EnvironmentVariable
{
	/// <summary>
	/// Defines a step gatherer.
	/// </summary>
	public StepsGatherer Gatherer { get; } = new();

	/// <summary>
	/// Defines a logical solver.
	/// </summary>
	public LogicalSolver Solver { get; } = CommonLogicalSolvers.Suitable with { };
}
#endif