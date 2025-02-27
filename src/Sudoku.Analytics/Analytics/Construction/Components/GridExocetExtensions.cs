namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExocetExtensions
{
	/// <summary>
	/// Check whether all intersected cells by original cross-line cells and extra house cells are non-empty,
	/// and cannot be of value appeared in base cells.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="crossline">The cross-line cells.</param>
	/// <param name="baseCellsDigitsMask">The mask that holds a list of digits appeared in base cells.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool CheckCrossLineIntersectionLeaveEmpty(this in Grid @this, in CellMap crossline, Mask baseCellsDigitsMask)
	{
		foreach (var cell in crossline)
		{
			if (@this.GetDigit(cell) is not (var valueDigit and not -1) || (baseCellsDigitsMask >> valueDigit & 1) != 0)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Check whether all digits appeared in base cells can be filled in target empty cells.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="targetCellsToBeChecked">The target cells to be checked.</param>
	/// <param name="baseCellsDigitsMask">A mask that holds a list of digits appeared in base cells.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CheckTargetCellsDigitsValidity(
		this in Grid @this,
		in CellMap targetCellsToBeChecked,
		Mask baseCellsDigitsMask
	) => targetCellsToBeChecked switch
	{
		// If the selected target contains two valid cells, we should check for its intersected value and union value,
		// determining whether the union value contains all digits from base cells,
		// and intersected value contain at least 2 kinds of digits appeared from base cells.
		// Why is 2? Because the target cells should be filled two different digits appeared from base cells.
		{ Count: 2 } when (
			(Mask)(@this[targetCellsToBeChecked] & baseCellsDigitsMask),
			(Mask)(@this[targetCellsToBeChecked, false, MaskAggregator.And] & baseCellsDigitsMask)
		) is var (u, i) => u == baseCellsDigitsMask && i != 0,

		// A conjugate pair or AHS may be formed in such target cells. The will be used in a senior exocet.
		// Today we don't check for it.
		_ => false
	};

	/// <summary>
	/// Try to get all possible digits as value representation in cross-line cells.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="crossline">The cross-line cells to be checked.</param>
	/// <param name="baseCellsDigitsMask">The digits appeared in base cells.</param>
	/// <returns>A list of digits appeared in cross-line cells as value representation.</returns>
	public static Mask GetValueDigitsAppearedInCrossline(this in Grid @this, in CellMap crossline, Mask baseCellsDigitsMask)
	{
		var result = (Mask)0;
		foreach (var cell in crossline)
		{
			if (@this.GetDigit(cell) is var digit && (baseCellsDigitsMask >> digit & 1) != 0)
			{
				result |= (Mask)(1 << digit);
			}
		}
		return result;
	}
}
