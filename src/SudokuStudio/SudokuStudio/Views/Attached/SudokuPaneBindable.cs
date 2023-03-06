#pragma warning disable CS1591
namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
[AttachedProperty<LogicalSolver>("ProgramSolver")]
[AttachedProperty<StepsGatherer>("ProgramStepGatherer")]
public static partial class SudokuPaneBindable
{
	[DefaultValue]
	private static readonly LogicalSolver ProgramSolverDefaultValue = CommonLogicalSolvers.Suitable;

	[DefaultValue]
	private static readonly StepsGatherer ProgramStepGathererDefaultValue = new();
}
