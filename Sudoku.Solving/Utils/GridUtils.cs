using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Utils
{
	public static class GridUtils
	{
		public static bool HasDigitValue(this Grid @this, int digit, int regionOffset)
		{
			return RegionUtils.GetCellOffsets(regionOffset).Any(
				o => @this.GetCellStatus(o) != CellStatus.Empty && @this[o] == digit);
		}

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
			return (short)(result >> 23); // 23 == 32 - 9
		}
	}
}
