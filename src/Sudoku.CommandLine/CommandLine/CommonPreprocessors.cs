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
}
