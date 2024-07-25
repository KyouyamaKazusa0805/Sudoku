namespace Sudoku.Runtime.SolvingServices.EnumerableQuery;

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
}
