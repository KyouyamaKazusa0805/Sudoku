namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
[AttachedProperty<Analyzer>("ProgramSolver")]
[AttachedProperty<StepCollector>("ProgramStepGatherer")]
public static partial class SudokuPaneBindable
{
	[DefaultValue]
	private static readonly Analyzer ProgramSolverDefaultValue = PredefinedAnalyzers.Balanced;

	[DefaultValue]
	private static readonly StepCollector ProgramStepGathererDefaultValue = new();
}
