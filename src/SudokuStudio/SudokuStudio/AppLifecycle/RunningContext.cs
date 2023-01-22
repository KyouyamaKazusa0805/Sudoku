namespace SudokuStudio.AppLifecycle;

/// <summary>
/// Defines a context type that stores the basic information of project running.
/// </summary>
public sealed class RunningContext
{
	/// <summary>
	/// Indicates the current running assembly.
	/// </summary>
	public Assembly Assembly { get; } = typeof(RunningContext).Assembly;

	/// <summary>
	/// Defines a logical solver.
	/// </summary>
	public LogicalSolver Solver { get; } = CommonLogicalSolvers.Suitable;

	/// <summary>
	/// Indicates the version of the current assembly.
	/// </summary>
	public Version AssemblyVersion => Assembly.GetName().Version!;

	/// <summary>
	/// Indicates the main window that the program is running.
	/// </summary>
	public Window MainWindow { get; internal set; } = null!;
}
