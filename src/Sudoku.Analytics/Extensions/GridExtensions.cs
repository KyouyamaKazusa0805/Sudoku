using Sudoku.Algorithm.Ittoryu;

namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// Try to recover the specified grid to an ittoryu grid via the specified path calculated by <see cref="IttoryuPathFinder"/>.
	/// </summary>
	/// <param name="this">The grid to be adjusted.</param>
	/// <param name="ittoryuPath">The path to be used.</param>
	/// <seealso cref="IttoryuPathFinder"/>
	public static void MakeIttoryu(this scoped ref Grid @this, DigitPath ittoryuPath)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(ittoryuPath.IsComplete, true);

		if (ittoryuPath == (Digit[])[0, 1, 2, 3, 4, 5, 6, 7, 8])
		{
			// The puzzle won't be changed.
			return;
		}

		var result = @this;
		for (var i = 0; i < 9; i++)
		{
			result.SwapTwoDigits(ittoryuPath.Digits[i], Array.IndexOf(ittoryuPath.Digits, i));
		}

		@this = result;
	}
}
