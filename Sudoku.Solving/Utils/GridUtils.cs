using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
		/// <para>
		/// Indicates whether the specified grid contains the candidate.
		/// Note that given and modifiable values always make this method
		/// return <c>false</c>.
		/// </para>
		/// <para>
		/// If you want to check the reversal case, please use the method
		/// <see cref="CandidateDoesNotExist(Grid, int, int)"/> instead
		/// of '<c>!grid.CandidateExists</c>'.
		/// </para>
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="CandidateDoesNotExist(Grid, int, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CandidateExists(this Grid @this, int cellOffset, int digit)
		{
			return @this.GetCellStatus(cellOffset) == CellStatus.Empty
				&& !@this[cellOffset, digit];
		}

		/// <summary>
		/// <para>
		/// Indicates whether the specified grid <b>does not</b> contains the candidate.
		/// Note that given and modifiable values always make this method
		/// return <c>false</c>.
		/// </para>
		/// <para>
		/// If you want to check the reversal case, please use the method
		/// <see cref="CandidateExists(Grid, int, int)"/> instead
		/// of '<c>!grid.CandidateDoesNotExist</c>'.
		/// </para>
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="CandidateExists(Grid, int, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CandidateDoesNotExist(this Grid @this, int cellOffset, int digit)
		{
			return @this.GetCellStatus(cellOffset) == CellStatus.Empty
				&& @this[cellOffset, digit];
		}

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
				result += @this.CandidateExists(cellOffset, digit) ? 1 : 0;

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

		/// <summary>
		/// Find all bivalue cells displaying with a <see cref="GridMap"/>.
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <param name="count">
		/// (out parameter) The number of bivalue cells.
		/// This parameter is only used for quickening the code running.
		/// </param>
		/// <returns>The grid map.</returns>
		public static GridMap GetBivalueCellsMap(this Grid @this, out int count)
		{
			var bivalueCellsMap = new GridMap();
			count = 0;
			int i = 0;
			foreach (var (status, mask) in @this)
			{
				if (status == CellStatus.Empty && (~mask & 511).CountSet() == 2)
				{
					bivalueCellsMap[i] = true;
					count++;
				}

				i++;
			}

			return bivalueCellsMap;
		}

		/// <summary>
		/// Find all conjugate pairs in a grid.
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <returns>All conjugate pairs.</returns>
		public static IReadOnlyList<ConjugatePair> GetAllConjugatePairs(this Grid @this)
		{
			var list = new List<ConjugatePair>();
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					short mask = @this.GetDigitAppearingMask(digit, region);
					if (mask.CountSet() == 2)
					{
						// Conjugate pair found.
						int[] z = mask.GetAllSets().ToArray();
						int p1 = RegionUtils.GetCellOffset(region, z[0]);
						int p2 = RegionUtils.GetCellOffset(region, z[1]);
						list.Add(new ConjugatePair(p1, p2, digit));
					}
				}
			}

			return list;
		}
	}
}
