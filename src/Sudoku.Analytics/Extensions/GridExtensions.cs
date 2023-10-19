using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Algorithm.Ittoryu;
using Sudoku.Analytics.Categorization;

namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// Try to reproduce ittoryu ordering for the specified grid whose path can be found by <see cref="IttoryuPathFinder"/>.
	/// </summary>
	/// <param name="this">The grid to be adjusted.</param>
	/// <param name="ittoryuPath">
	/// The path to be used. The argument isn't required to be a complete path. It's considered to be OK if a path contains at least 2 digits.
	/// </param>
	/// <exception cref="ArgumentException">Throws when the ittoryu path contains a digit series of length 0 or 1.</exception>
	/// <seealso cref="IttoryuPathFinder"/>
	public static void MakeIttoryu(this scoped ref Grid @this, DigitPath ittoryuPath)
	{
		if (ittoryuPath.Digits is not { Length: >= 2 })
		{
			throw new ArgumentException($"The argument '{nameof(ittoryuPath)}' requires a digit series of length greater than 1.", nameof(ittoryuPath));
		}

		if (ittoryuPath == [0, 1, 2, 3, 4, 5, 6, 7, 8])
		{
			// The puzzle won't be changed.
			return;
		}

		// Try to replace digits.
		var result = Grid.Empty;
		var valuesMap = @this.ValuesMap;
		for (var digit = 0; digit < ittoryuPath.Digits.Length; digit++)
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

	/// <summary>
	/// Determine whether the specified grid is an ittoryu puzzle, meaning the puzzle can be finished via digits one by one.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="path">The first found ittoryu path.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIttoryu(this scoped ref readonly Grid @this, [NotNullWhen(true)] out DigitPath? path)
		=> @this.IsIttoryu(TechniqueSets.IttoryuTechniques, out path);

	/// <summary>
	/// Determine whether the specified grid is an ittoryu puzzle, meaning the puzzle can be finished via digits one by one,
	/// with checking using the specified techniques.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="techniques">
	/// <para>The techniques supported.</para>
	/// <para>
	/// The value is a <see cref="TechniqueSet"/> instance. You can only chose one or more fields in the following list:
	/// <list type="bullet">
	/// <item><see cref="Technique.FullHouse"/> (Full House)</item>
	/// <item><see cref="Technique.HiddenSingleBlock"/> (Hidden Single in Block)</item>
	/// <item><see cref="Technique.HiddenSingleRow"/> (Hidden Single in Row)</item>
	/// <item><see cref="Technique.HiddenSingleColumn"/> (Hidden Single in Column)</item>
	/// <item><see cref="Technique.NakedSingle"/> (Naked Single)</item>
	/// </list>
	/// </para>
	/// <para>For example, <c>[Technique.FullHouse, Technique.NakedSingle]</c> is valid argument, but <c>[Technique.NakedPair]</c> is not.</para>
	/// </param>
	/// <param name="path">The first found ittoryu path.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIttoryu(this scoped ref readonly Grid @this, TechniqueSet techniques, [NotNullWhen(true)] out DigitPath? path)
	{
		var pathFinder = new IttoryuPathFinder { SupportedTechniques = techniques };
		var foundPath = pathFinder.FindPath(in @this);
		if (foundPath.IsComplete)
		{
			path = foundPath;
			return true;
		}
		else
		{
			path = null;
			return false;
		}
	}

	/// <summary>
	/// Try to get the maximum times that the specified digit, describing it can be filled with the specified houses in maximal case.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="limitCount">The maximum number of filling with <paramref name="digit"/> in <paramref name="cells"/>.</param>
	/// <returns>
	/// <para>The number of times that the digit can be filled with the specified houses, at most.</para>
	/// </returns>
	public static bool MaxPlacementsOf(this scoped ref readonly Grid @this, Digit digit, scoped ref readonly CellMap cells, int limitCount)
	{
		var activeCells = @this.CandidatesMap[digit] & cells;
		var inactiveCells = @this.ValuesMap[digit] & cells;
		if (!activeCells && limitCount == inactiveCells.Count)
		{
			return true;
		}

		for (var i = activeCells.Count; i >= 1; i--)
		{
			foreach (ref readonly var cellsCombination in activeCells.GetSubsets(i).EnumerateRef())
			{
				if (!cellsCombination.CanSeeEachOther && ((cellsCombination.ExpandedPeers | cellsCombination) & activeCells) == activeCells)
				{
					return i + inactiveCells.Count == limitCount;
				}
			}
		}

		return false;
	}
}
