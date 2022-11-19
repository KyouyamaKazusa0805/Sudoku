namespace Sudoku.Communication.Qicq.AppLifecycle;

/// <summary>
/// Provides with environment data.
/// </summary>
internal static class EnvironmentData
{
	/// <summary>
	/// The random number generator.
	/// </summary>
	public static readonly Random Rng = new();

	/// <summary>
	/// The generator.
	/// </summary>
	public static readonly PatternBasedPuzzleGenerator Generator = new();

	/// <summary>
	/// The solver.
	/// </summary>
	public static readonly LogicalSolver Solver = new();

	/// <summary>
	/// The internal answering context. The field is only used for command <see cref="StartGamingCommand"/>.
	/// </summary>
	/// <remarks>
	/// This field uses a concurrent dictionary, grouping all answering contexts by group QQ number.
	/// No matter what thread the current program use, this field can always provide the available,
	/// although the target result may not be created.
	/// </remarks>
	internal static readonly ConcurrentDictionary<string, AnsweringContext> AnsweringContexts = new();


	/// <summary>
	/// The current executing command.
	/// </summary>
	/// <remarks><b><i>This field will be re-considered.</i></b></remarks>
	public static string? EnvironmentCommandExecuting = null;

	/// <summary>
	/// The puzzle.
	/// </summary>
	public static Grid Puzzle = Grid.Empty;

	/// <summary>
	/// The painter.
	/// </summary>
	public static ISudokuPainter? Painter = null;
}
