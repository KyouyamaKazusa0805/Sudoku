namespace Sudoku.Algorithms.Solving.EnumerableQuery;

/// <summary>
/// Defines a solver that can solve a sudoku puzzle, using LINQ.
/// </summary>
public sealed class EnumerableQuerySolver : ISolver
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
		Unsafe.SkipInit(out result);
		var (_, @return) = solve(grid.ToString()) switch
		{
			[] => (Grid.Undefined, default(bool?)),
			[var resultString] => (result = Grid.Parse(resultString), true),
			_ => (Grid.Undefined, false)
		};
		return @return;


		static ReadOnlySpan<string> solve(string puzzle)
		{
			var result = (ReadOnlySpan<string>)(string[])[puzzle];
			while (result is [var r, ..] && r.Contains('.'))
			{
				result =
					from solution in result
					let index = solution.IndexOf('.')
					let column = index % 9
					let block = index - index % 27 + column - index % 3
					from digit in DigitCharacters.ToCharArray()
					let duplicateCases =
						// Here we cannot replace it with a read-only span (i.e. 'Digits' -> '(ReadOnlySpan<Digit>)Digits')
						// because it will make the query expression create a new unsafe object in nested query expression,
						// which will cause the solver to be unsafe.
						from pos in Digits
						let duplicatesInRow = solution[index - column + pos] == digit
						let duplicatesInColumn = solution[column + pos * 9] == digit
						let duplicatesInBlock = solution[block + pos % 3 + pos / 3 * 9] == digit
						where duplicatesInRow || duplicatesInColumn || duplicatesInBlock
						select pos
					where duplicateCases.Length == 0
					select $"{solution[..index]}{digit}{solution[(index + 1)..]}";
			}

			// Return the result value.
			// Because we generate the target value inside the query expression,
			// we may not consider the value having been deleted by GC.
			// In C# 11, we can use keyword 'scoped' to describe whether a value or reference can be used local-scoped.
			// If a local does not contain a 'scoped' keyword, the value can be escaped the whole method lifecycle.
			return result;
		}
	}
}
