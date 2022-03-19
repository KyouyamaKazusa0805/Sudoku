using Sudoku.Collections;

namespace Sudoku.Test;

/// <summary>
/// Defines a chain node that provides with the data for an almost locked set.
/// </summary>
public sealed class AlmostLockedSetNode : Node
{
	/// <summary>
	/// Initializes an <see cref="AlmostLockedSetNode"/> instance via the digit used
	/// and the cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="alsDigitsMask">The mask that holds the digits in the cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlmostLockedSetNode(byte digit, in Cells cells) : base(NodeType.AlmostLockedSets, digit, cells)
	{
	}


	/// <summary>
	/// Try to get all possible digits appeared in the specified cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cells">The cells.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The <see cref="Cells"/> instance as the result.</returns>
	public static Cells GetCells(in Grid grid, in Cells cells, int digit)
	{
		var result = Cells.Empty;
		foreach (int cell in cells)
		{
			if (grid.Exists(cell, digit) is true)
			{
				result.AddAnyway(cell);
			}
		}

		return result;
	}
}
