namespace Sudoku.Solving;

/// <summary>
/// Defines a solver that can solve a sudoku puzzle, using LINQ.
/// </summary>
[Algorithm("LINQ")]
public sealed class LinqSolver : ISimpleSolver
{
	/// <inheritdoc/>
	public bool? Solve(in Grid grid, out Grid result)
	{
		Unsafe.SkipInit(out result);
		switch (solve(grid.ToString("0")))
		{
			case []:
			{
				return null;
			}
			case [var resultString]:
			{
				result = Grid.Parse(resultString);
				return true;
			}
			default:
			{
				return false;
			}
		}


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
