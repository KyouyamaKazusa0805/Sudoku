using Sudoku.Data.Meta;

namespace Sudoku.Solving.Utils
{
	public static class GridUtils
	{
		public static readonly int[] PointingMask = { 0b000_000_111, 0b000_111_000, 0b111_000_000 };

		public static readonly int[] ClaimingMask = { 0b100_100_100, 0b010_010_010, 0b001_001_001 };

		public static int GetDigitAppearingMask(this Grid @this, int digit, int regionOffset)
		{
			int result = 0, i = 0;
			foreach (int cellOffset in RegionUtils.GetCellOffsets(regionOffset))
			{
				result += @this.GetCellStatus(cellOffset) == CellStatus.Empty
					&& @this[cellOffset, digit] ? 1 : 0;

				if (i++ != 8)
				{
					result <<= 1;
				}
			}

			return result;
		}

		public static bool SimplyValidate(this Grid @this)
		{
			int count = 0;
			for (int i = 0; i < 81; i++)
			{
				if (@this.GetCellStatus(i) == CellStatus.Given)
				{
					count++;
				}

				int curDigit, peerDigit;
				if ((curDigit = @this[i]) != -1)
				{
					foreach (int peerOffset in new GridMap(i).Offsets)
					{
						if (peerOffset == i)
						{
							continue;
						}

						if ((peerDigit = @this[peerOffset]) != -1 && curDigit == peerDigit)
						{
							return false;
						}
					}
				}
			}

			return count >= 17;
		}

		public static int? GetNakedSingleDigit(this Grid @this, int offset)
		{
			short value = @this.GetMask(offset);
			if (@this.GetCellStatus(offset) == CellStatus.Empty)
			{
				int count = 0, pos = -1;
				for (int i = 0; i < 9; i++, value >>= 1)
				{
					if ((value & 1) == 0)
					{
						count++;
						pos = i;
					}
				}

				if (count == 1)
				{
					return pos;
				}
			}

			return null;
		}
	}
}
