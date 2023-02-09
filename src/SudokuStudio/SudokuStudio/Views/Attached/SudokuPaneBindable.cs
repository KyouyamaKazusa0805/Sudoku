#pragma warning disable CS1591

namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
public static class SudokuPaneBindable
{
	public static readonly DependencyProperty ProgramSolverProperty =
		DependencyProperty.RegisterAttached("ProgramSolver", typeof(LogicalSolver), typeof(SudokuPaneBindable), new(CommonLogicalSolvers.Suitable));

	public static readonly DependencyProperty ProgramStepGathererProperty =
		DependencyProperty.RegisterAttached("ProgramStepGatherer", typeof(StepsGatherer), typeof(SudokuPaneBindable), new(new StepsGatherer()));


	public static void SetProgramSolver(DependencyObject obj, LogicalSolver value) => obj.SetValue(ProgramSolverProperty, value);

	public static void SetProgramStepGatherer(DependencyObject obj, StepsGatherer value) => obj.SetValue(ProgramStepGathererProperty, value);

	public static LogicalSolver GetProgramSolver(DependencyObject obj) => (LogicalSolver)obj.GetValue(ProgramSolverProperty);

	public static StepsGatherer GetProgramStepGatherer(DependencyObject obj) => (StepsGatherer)obj.GetValue(ProgramStepGathererProperty);
}
