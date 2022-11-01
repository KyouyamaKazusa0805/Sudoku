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
	/// The current executing command.
	/// </summary>
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
