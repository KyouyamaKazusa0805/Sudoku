namespace Sudoku.Solving.BruteForces;

/// <summary>
/// Provides a solver using LINQ method.
/// </summary>
public sealed class LinqSolver : IPuzzleSolver
{
	/// <inheritdoc/>
	public ISolverResult Solve(in Grid puzzle, CancellationToken cancellationToken = default)
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();
		var results = SolveStrings($"{puzzle:0}");
		stopwatch.Stop();

		var solverResult = new BruteForceSolverResult(puzzle, ElapsedTime: stopwatch.Elapsed);
		return results switch
		{
			[] => solverResult with { IsSolved = false, FailedReason = FailedReason.PuzzleHasNoSolution },
			[var result] => solverResult with { Solution = Grid.Parse(result) },
			_ => solverResult with { IsSolved = false, FailedReason = FailedReason.PuzzleHasNoSolution }
		};
	}

	/// <inheritdoc/>
	public ValueTask<ISolverResult> SolveAsync(in Grid puzzle, CancellationToken cancellationToken = default) =>
		new(Solve(puzzle, cancellationToken));


	/// <summary>
	/// Internal solving method.
	/// </summary>
	/// <param name="puzzle">The puzzle string, with placeholder character '0'.</param>
	/// <returns>The result strings (i.e. All solutions).</returns>
	private static IReadOnlyList<string> SolveStrings(string puzzle)
	{
		const string digits = "123456789";
		var result = new List<string> { puzzle };

		while (result is [var r, ..] && r.IndexOf('0', StringComparison.OrdinalIgnoreCase) != -1)
		{
			result = (
				from solution in result
				let index = solution.IndexOf('0', StringComparison.OrdinalIgnoreCase)
				let pair = (Column: index % 9, Block: index - index % 27 + index % 9 - index % 3)
				from digit in digits
				let duplicateCases =
					from i in Enumerable.Range(0, 9)
					let duplicatesInRow = solution[index - pair.Column + i] == digit
					let duplicatesInColumn = solution[pair.Column + i * 9] == digit
					let duplicatesInBlock = solution[pair.Block + i % 3 + (int)Floor(i / 3F) * 9] == digit
					where duplicatesInRow || duplicatesInColumn || duplicatesInBlock
					select i
				where !duplicateCases.Any()
				select solution.ReplaceAt(index, digit)
			).ToList();
		}

		return result;
	}

	/// <summary>
	/// Internal solving method.
	/// </summary>
	/// <param name="handler">The puzzle string, with placeholder character '0'.</param>
	/// <returns>The result strings (i.e. All solutions).</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static IReadOnlyList<string> SolveStrings(
		[InterpolatedStringHandlerArgument] in StringHandler handler
	) => SolveStrings(handler.ToStringAndClear());
}
