namespace Sudoku.Solving.EnumerableQuery;

/// <summary>
/// Defines a solver that can solve a sudoku puzzle, using LINQ.
/// </summary>
public sealed class EnumerableQuerySolver : ISolver, IMultipleSolutionSolver
{
	/// <summary>
	/// Indicates the characters for 1 to 9.
	/// </summary>
	private const string DigitCharacters = "123456789";


	/// <inheritdoc/>
	public static string? UriLink => null;


	/// <inheritdoc/>
	public bool? Solve(ref readonly Grid grid, out Grid result)
	{
#pragma warning disable format
		(result, var @return) = SolveCore(grid.ToString()) switch
		{
			[] => (Grid.Undefined, default(bool?)),
			[var resultString] => (Grid.Parse(resultString), true),
			_ => (Grid.Undefined, false)
		};
#pragma warning restore format
		return @return;
	}

	/// <inheritdoc/>
	public ReadOnlySpan<Grid> SolveAll(ref readonly Grid grid)
	{
		var results = SolveCore(grid.ToString());
		return results.IsEmpty ? [] : from result in results select Grid.Parse(result);
	}


	/// <summary>
	/// The core method to solve puzzles.
	/// </summary>
	/// <param name="puzzle">Indicates the puzzles.</param>
	/// <returns>A list of puzzles found.</returns>
	private static ReadOnlySpan<string> SolveCore(string puzzle)
	{
		var result = (ReadOnlySpan<string>)(string[])[puzzle];
		while (result is [var r, ..] && r.Contains('.'))
		{
			result =
				from solution in result
				let index = solution.IndexOf('.')
				let column = index % 9
				let block = index - index % 27 + column - index % 3
				from digit in DigitCharacters.AsSpan()
				let duplicateCases =
					from pos in Digits
					let rowContainsDuplicateDigits = solution[index - column + pos] == digit
					let columnContainsDuplicateDigits = solution[column + pos * 9] == digit
					let blockContainsDuplicateDigits = solution[block + pos % 3 + pos / 3 * 9] == digit
					where rowContainsDuplicateDigits || columnContainsDuplicateDigits || blockContainsDuplicateDigits
					select pos
				where duplicateCases.Length == 0
				select $"{solution[..index]}{digit}{solution[(index + 1)..]}";
		}
		return result;
	}
}
