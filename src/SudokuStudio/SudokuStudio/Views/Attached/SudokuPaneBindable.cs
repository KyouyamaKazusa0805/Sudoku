#pragma warning disable CS1591

namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
[AttachedProperty<LogicalSolver>("ProgramSolver", DefaultValueGeneratingMemberName = nameof(ProgramSolverDefaultValue))]
[AttachedProperty<StepsGatherer>("ProgramStepGatherer", DefaultValueGeneratingMemberName = nameof(ProgramStepGathererDefaultValue))]
public static partial class SudokuPaneBindable
{
	private static readonly LogicalSolver ProgramSolverDefaultValue = CommonLogicalSolvers.Suitable;
	private static readonly StepsGatherer ProgramStepGathererDefaultValue = new();
}
