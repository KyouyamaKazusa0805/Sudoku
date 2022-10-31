namespace Sudoku.Communication.Qicq.AppLifecycle;

/// <summary>
/// Provides with environment data.
/// </summary>
internal static class EnvironmentData
{
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


	/// <summary>
	/// Determines whether the current command environment is being drawing.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Painter))]
	public static bool IsDrawingEnvironment => EnvironmentCommandExecuting == R["_Command_Draw"];
}
