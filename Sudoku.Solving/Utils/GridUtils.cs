using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for grid calculating.
	/// </summary>
	public static class GridUtils
	{
		/// <summary>
		/// Checks whether the specified digit has given or modifiable values in
		/// the specified region.
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionOffset">The region.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		public static bool HasDigitValue(this Grid @this, int digit, int regionOffset)
		{
			return RegionUtils.GetCellOffsets(regionOffset).Any(
				o => @this.GetCellStatus(o) != CellStatus.Empty && @this[o] == digit);
		}

		/// <summary>
		/// Gets a mask of digit appearing in the specified region offset.
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionOffset">The region.</param>
		/// <returns>
		/// The mask. This value consists of 9 bits, which represents all nine cells
		/// in a specified region. The mask uses 1 to make the cell 'have this digit',
		/// and 0 to make the cell 'does not have this digit'.
		/// </returns>
		public static short GetDigitAppearingMask(this Grid @this, int digit, int regionOffset)
		{
			int result = 0, i = 0;
			foreach (int cellOffset in RegionUtils.GetCellOffsets(regionOffset))
			{
				result +=
					@this.GetCellStatus(cellOffset) == CellStatus.Empty && !@this[cellOffset, digit]
						? 1
						: 0;

				if (i++ != 8)
				{
					result <<= 1;
				}
			}

			// Now should reverse all bits. Note that this extension method
			// will be passed a ref value ('ref int', not 'int').
			result.ReverseBits();
			return (short)(result >> 23 & 511); // 23 == 32 - 9
		}
	}
}
