namespace Sudoku.CommandLine;

/// <summary>
/// Provides a list of commonly-used preprocessor methods.
/// </summary>
internal static class CommonPreprocessors
{
	/// <summary>
	/// Output invalid puzzle on puzzle after having been checked.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="solver">The solver.</param>
	/// <param name="solution">The solution.</param>
	public static void OutputIfPuzzleNotUnique(in Grid grid, ISolver solver, out Grid solution)
	{
		var result = solver.Solve(grid, out solution);
		if (result is true)
		{
			return;
		}

		var text = result is false ? "The puzzle has multiple solutions." : "The puzzle has no valid solutions.";
		Console.WriteLine($"\e[31m{text}\e[0m");
		solution = Grid.Undefined;
	}

	/// <summary>
	/// Creates a <see cref="CancellationTokenSource"/> instance with a timeout.
	/// Set <see cref="Timeout.Infinite"/> (i.e. -1) to set infinity timeout.
	/// </summary>
	/// <param name="timeout">The timeout in milliseconds.</param>
	/// <returns>A <see cref="CancellationTokenSource"/> instance.</returns>
	/// <seealso cref="Timeout.Infinite"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CancellationTokenSource CreateCancellationTokenSource(int timeout)
		=> timeout == Timeout.Infinite ? new() : new(timeout);
}
