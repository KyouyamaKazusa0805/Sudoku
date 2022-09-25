namespace Sudoku.Solving.Algorithms.EnumerableQuery;

/// <summary>
/// Defines a solver that can solve a sudoku puzzle, using LINQ.
/// </summary>
public sealed class LinqSolver : ISimpleSolver
{
	/// <inheritdoc/>
	public static string? UriLink => null;


	/// <inheritdoc/>
	public bool? Solve(scoped in Grid grid, out Grid result)
	{
		Unsafe.SkipInit(out result);
		var (_, @return) = solve(grid.ToString("0")) switch
		{
			[] => (Grid.Undefined, default(bool?)),
			[var resultString] => (result = Grid.Parse(resultString), true),
			_ => (Grid.Undefined, false)
		};
		return @return;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int indexOfZeroIgnoringCase(string s) => s.IndexOf('0', StringComparison.OrdinalIgnoreCase);

		static IReadOnlyList<string> solve(string puzzle)
		{
			const string digits = "123456789";
			var result = new List<string> { puzzle };

			while (result is [var r, ..] && indexOfZeroIgnoringCase(r) != -1)
			{
				result = (
					from solution in result
					let index = indexOfZeroIgnoringCase(solution)
					let pair = (Column: index % 9, Block: index - index % 27 + index % 9 - index % 3)
					from digit in digits
					let duplicateCases =
						from i in Enumerable.Range(0, 9)
						let duplicatesInRow = solution[index - pair.Column + i] == digit
						let duplicatesInColumn = solution[pair.Column + i * 9] == digit
						let duplicatesInBlock = solution[pair.Block + i % 3 + (int)(i / 3F) * 9] == digit
						where duplicatesInRow || duplicatesInColumn || duplicatesInBlock
						select i
					where !duplicateCases.Any()
					select solution.ReplaceAt(index, digit)
				).ToList();
			}

			return result;
		}
	}
}
