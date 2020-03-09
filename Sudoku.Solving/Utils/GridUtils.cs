using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for grid calculating.
	/// </summary>
	[DebuggerStepThrough]
	public static class GridUtils
	{
		/// <summary>
		/// <para>Indicates whether the specified cell is a bivalue cell.</para>
		/// <para>
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBivalueCell(this IReadOnlyGrid @this, int cellOffset)
		{
			return @this.GetCellStatus(cellOffset) == CellStatus.Empty
				&& @this.GetCandidatesReversal(cellOffset).CountSet() == 2;
		}

		/// <summary>
		/// <para>
		/// Indicates whether the specified region is a bi-location region
		/// for the specified digit.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="region">The region.</param>
		/// <param name="mask">
		/// (<see langword="out"/> parameter) The mask off digit appearing mask.
		/// If the region has the value cell with this digit, this value will be 0.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBilocationRegion(
			this IReadOnlyGrid @this, int digit, int region, out short mask)
		{
			if (@this.HasDigitValue(digit, region))
			{
				mask = 0;
				return false;
			}

			mask = @this.GetDigitAppearingMask(digit, region);
			return mask.CountSet() == 2;
		}

		/// <summary>
		/// <para>
		/// Indicates whether the specified grid contains the candidate.
		/// </para>
		/// <para>
		/// If you want to check the reversal case, please use the method
		/// <see cref="CandidateDoesNotExist(IReadOnlyGrid, int, int)"/> instead
		/// of '<c>!grid.CandidateExists</c>'.
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="CandidateDoesNotExist(IReadOnlyGrid, int, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CandidateExists(
			this IReadOnlyGrid @this, int cellOffset, int digit) =>
			@this.GetCellStatus(cellOffset) == CellStatus.Empty && !@this[cellOffset, digit];

		/// <summary>
		/// <para>
		/// Indicates whether the specified grid <b>does not</b> contain the candidate.
		/// </para>
		/// <para>
		/// If you want to check the reversal case, please use the method
		/// <see cref="CandidateExists(IReadOnlyGrid, int, int)"/> instead
		/// of '<c>!grid.CandidateDoesNotExist</c>'.
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="CandidateExists(IReadOnlyGrid, int, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CandidateDoesNotExist(
			this IReadOnlyGrid @this, int cellOffset, int digit) =>
			@this.GetCellStatus(cellOffset) == CellStatus.Empty && @this[cellOffset, digit];

		/// <summary>
		/// Checks whether the specified digit has given or modifiable values in
		/// the specified region.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionOffset">The region.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasDigitValue(
			this IReadOnlyGrid @this, int digit, int regionOffset)
		{
			return GridMap.GetCellsIn(regionOffset).Any(
				o => @this.GetCellStatus(o) != CellStatus.Empty && @this[o] == digit);
		}

		/// <summary>
		/// <para>
		/// Gets a mask of digit appearing in the specified region offset.
		/// </para>
		/// <para>
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionOffset">The region.</param>
		/// <returns>
		/// The mask. This value consists of 9 bits, which represents all nine cells
		/// in a specified region. The mask uses 1 to make the cell 'have this digit',
		/// and 0 to make the cell 'does not have this digit'.
		/// </returns>
		public static short GetDigitAppearingMask(
			this IReadOnlyGrid @this, int digit, int regionOffset)
		{
			int result = 0, i = 0;
			foreach (int cellOffset in GridMap.GetCellsIn(regionOffset))
			{
				result += @this.CandidateExists(cellOffset, digit) ? 1 : 0;

				if (i++ == 8)
				{
					continue;
				}

				result <<= 1;
			}

			// Now should reverse all bits. Note that this extension method
			// will be passed a ref value ('ref int', not 'int').
			result.ReverseBits();
			return (short)(result >> 23 & 511); // 23 == 32 - 9
		}

		/// <summary>
		/// <para>
		/// Gets a <see cref="GridMap"/> of cells whose input digit appearing
		/// in the specified region offset.
		/// </para>
		/// <para>
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionOffset">The region.</param>
		/// <returns>The cells' map.</returns>
		public static GridMap GetDigitAppearingCells(
			this IReadOnlyGrid @this, int digit, int regionOffset)
		{
			var result = GridMap.Empty;

			foreach (int cell in GridMap.GetCellsIn(regionOffset))
			{
				if (!@this.CandidateExists(cell, digit))
				{
					continue;
				}

				result[cell] = true;
			}

			return result;
		}

		/// <summary>
		/// <para>
		/// Gets a mask of digit appearing in the specified region offset.
		/// If the cell is not in <paramref name="map"/>, its mask will
		/// not calculated to the result.
		/// </para>
		/// <para>
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionOffset">The region offset.</param>
		/// <param name="map">The grid map.</param>
		/// <returns>
		/// The mask. This value consists of 9 bits, which represents all nine cells
		/// in a specified region. The mask uses 1 to make the cell 'have this digit',
		/// and 0 to make the cell 'does not have this digit'.
		/// </returns>
		public static short GetDigitAppearingMask(
			this IReadOnlyGrid @this, int digit, int regionOffset, GridMap map)
		{
			int result = 0, i = 0;
			foreach (int cell in GridMap.GetCellsIn(regionOffset))
			{
				result += @this.CandidateExists(cell, digit) && map[cell] ? 1 : 0;

				if (i++ != 8)
				{
					result <<= 1;
				}
			}

			result.ReverseBits();
			return (short)(result >> 23 & 511); // 23 == 32 - 9
		}

		/// <summary>
		/// Find all bivalue cells displaying with a <see cref="GridMap"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="count">
		/// (<see langword="out"/> parameter) The number of bivalue cells.
		/// This parameter is only used for quickening the code running.
		/// </param>
		/// <returns>The grid map.</returns>
		public static GridMap GetBivalueCellsMap(this IReadOnlyGrid @this, out int count)
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
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>All conjugate pairs.</returns>
		public static IReadOnlyList<ConjugatePair> GetAllConjugatePairs(this IReadOnlyGrid @this)
		{
			var list = new List<ConjugatePair>();
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					if (!@this.IsBilocationRegion(digit, region, out short mask))
					{
						continue;
					}

					// Conjugate pair found.
					int[] z = mask.GetAllSets().ToArray();
					int p1 = RegionUtils.GetCellOffset(region, z[0]);
					int p2 = RegionUtils.GetCellOffset(region, z[1]);
					list.Add(new ConjugatePair(p1, p2, digit));
				}
			}

			return list;
		}
	}
}
