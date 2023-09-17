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

		if (ittoryuPath == [0, 1, 2, 3, 4, 5, 6, 7, 8])
		{
			// The puzzle won't be changed.
			return;
		}

		// Try to replace digits.
		var result = Grid.Empty;
		var valuesMap = @this.ValuesMap;
		for (var digit = 0; digit < 9; digit++)
		{
			scoped ref readonly var valueMap = ref valuesMap[ittoryuPath.Digits[digit]];
			foreach (var cell in valueMap)
			{
				result.SetDigit(cell, digit);
			}
		}

		// Fix the grid for the initial state.
		for (var cell = 0; cell < 81; cell++)
		{
			if (@this.GetState(cell) == CellState.Given)
			{
				result.SetState(cell, CellState.Given);
			}
		}

		@this = result.ResetGrid;
	}
}
