using System.Runtime.CompilerServices;
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void MakeIttoryu(this scoped ref Grid @this, DigitPath ittoryuPath)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(ittoryuPath.IsComplete, true);

		var result = @this.SolutionGrid;
		@this = result
			.SwapTwoDigits(ittoryuPath.Digits[0], 0)
			.SwapTwoDigits(ittoryuPath.Digits[1], 1)
			.SwapTwoDigits(ittoryuPath.Digits[2], 2)
			.SwapTwoDigits(ittoryuPath.Digits[3], 3)
			.SwapTwoDigits(ittoryuPath.Digits[4], 4)
			.SwapTwoDigits(ittoryuPath.Digits[5], 5)
			.SwapTwoDigits(ittoryuPath.Digits[6], 6)
			.SwapTwoDigits(ittoryuPath.Digits[7], 7)
			.SwapTwoDigits(ittoryuPath.Digits[8], 8)
			.ResetGrid;
	}
}
