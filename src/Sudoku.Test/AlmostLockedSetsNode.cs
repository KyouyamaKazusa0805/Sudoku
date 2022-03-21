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
	/// <param name="extraCells">
	/// Indicates the extra cells that the current ALS used, but the current digit doesn't used.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlmostLockedSetNode(byte digit, in Cells cells, in Cells extraCells) :
		base(NodeType.AlmostLockedSets, digit, cells, InitOtherMask(extraCells))
	{
	}


	/// <summary>
	/// Indicates the whole ALS map.
	/// </summary>
	public Cells FullCells
	{
		get
		{
			var result = Cells;
			long otherCells = _other >> 7 & ((1L << 57) - 1);
			while (otherCells != 0)
			{
				int cell = (int)(otherCells >> 7 & 127);
				result.AddAnyway(cell);

				otherCells >>= 7;
			}

			return result;
		}
	}


	/// <summary>
	/// Gets the part of the other mask value that represents the extra cells used.
	/// </summary>
	/// <param name="extraCells">The extra cells used.</param>
	/// <returns>The mask of type <see cref="long"/>.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the number of elements in the argument <paramref name="extraCells"/>
	/// is greater than 8.
	/// </exception>
	private static long InitOtherMask(in Cells extraCells)
	{
		if (extraCells.Count > 8)
		{
			// 8 * 7 <= 64 - 7
			throw new ArgumentException(
				$"The length of the argument '{nameof(extraCells)}' must be lower than 8.",
				nameof(extraCells));
		}

		long finalMask = 0;
		int i = 0;
		foreach (int cell in extraCells)
		{
			finalMask |= (long)(cell << (7 * i++));
		}

		// Preserve the last 4 bits being zeroed.
		return finalMask << 4;
	}
}
