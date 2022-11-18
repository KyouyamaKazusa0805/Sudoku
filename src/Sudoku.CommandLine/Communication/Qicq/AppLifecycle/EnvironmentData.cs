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
	/// The gaming cancellation token.
	/// </summary>
	public static bool? GamingCancellationToken = null;

	/// <summary>
	/// The current executing command.
	/// </summary>
	public static string? EnvironmentCommandExecuting = null;

	/// <summary>
	/// The puzzle.
	/// </summary>
	public static Grid Puzzle = Grid.Empty;

	/// <summary>
	/// The answer data.
	/// </summary>
	public static ConcurrentBag<UserPuzzleAnswerData> AnswerData = new();

	/// <summary>
	/// Users has already answered the puzzle. This field is used for checking whether the user has already been replied.
	/// </summary>
	public static ConcurrentBag<string> AnsweredUsers = new();

	/// <summary>
	/// The painter.
	/// </summary>
	public static ISudokuPainter? Painter = null;
}
