namespace Sudoku.Platforms.QQ.AppLifecycle;

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
	/// The internal running context.
	/// </summary>
	internal static readonly ConcurrentDictionary<string, BotRunningContext> RunningContexts = new();
}
