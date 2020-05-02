using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;
using static Sudoku.GridProcessings;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides an ALS with specified digits in a specified region.
	/// </summary>
	[DebuggerStepThrough]
	public readonly struct Als : IEquatable<Als>
	{
		/// <summary>
		/// The internal mask of this data structure.
		/// </summary>
		/// <remarks>
		/// The mask consists of 23 bits.
		/// The lower 9 bits is used for representing all digits used. for example,
		/// the mask 259 (or in binary <c>0b100_000_011</c>) represents an ALS holds
		/// digits { <c>1, 2, 9 </c> }; the middle 9 bits means the relative positions
		/// in a region, while the higher 5 bits means which region this ALS
		/// lies on (Region indices 0..27 is no more than 5 bits used in binary).
		/// The last 9 bits are reserved for future considerations.
		/// </remarks>
		private readonly int _mask;


		/// <summary>
		/// Initializes an instance with the specified mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		public Als(int mask) => _mask = mask;

		/// <summary>
		/// Initializes an instance with the specified region, relative positions
		/// and digits.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="relativePos">All relative positions.</param>
		/// <param name="digits">All digits.</param>
		public Als(int region, IEnumerable<int> relativePos, IEnumerable<int> digits)
		{
			int mask = 0;
			setMask(relativePos);
			mask <<= 9;
			setMask(digits);
			_mask = mask | (region << 18);

			void setMask(IEnumerable<int> values)
			{
				foreach (int value in values)
				{
					mask |= 1 << value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the current ALS is a bi-value cell ALS.
		/// </summary>
		public bool IsBivalueCellAls => (RelativePosMask & (RelativePosMask - 1)) == 0;

		/// <summary>
		/// Indicates the region .
		/// </summary>
		public int Region => _mask >> 18 & 31;

		/// <summary>
		/// Indicates the relative positions mask.
		/// </summary>
		public short RelativePosMask => (short)(_mask >> 9 & 511);

		/// <summary>
		/// Indicates the digits mask.
		/// </summary>
		public short DigitsMask => (short)(_mask & 511);

		/// <summary>
		/// Indicates the cells used in this ALS.
		/// </summary>
		public GridMap Map => new GridMap(Cells);

		/// <summary>
		/// Indicates all digits used.
		/// </summary>
		public IEnumerable<int> Digits => DigitsMask.GetAllSets();

		/// <summary>
		/// Indicates the relative positions (offsets) in a region.
		/// </summary>
		public IEnumerable<int> RelativePos => RelativePosMask.GetAllSets();

		/// <summary>
		/// Indicates all strong links in this ALS. The result will be represented
		/// as a <see cref="short"/> mask of 9 bits indicating which bits used.
		/// </summary>
		public IEnumerable<short> StrongLinksMask
		{
			get
			{
				int[] digits = Digits.ToArray();
				for (int i = 0, length = digits.Length; i < length - 1; i++)
				{
					for (int j = i + 1; j < length; j++)
					{
						yield return (short)(1 << digits[i] | 1 << digits[j]);
					}
				}
			}
		}

		/// <summary>
		/// Indicates all cells used.
		/// </summary>
		public IEnumerable<int> Cells
		{
			get
			{
				foreach (int pos in RelativePos)
				{
					yield return RegionCells[Region][pos];
				}
			}
		}


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="region">(<see langword="out"/> parameter) The region.</param>
		/// <param name="relativePos">
		/// (<see langword="out"/> parameter) The relative positions.
		/// </param>
		/// <param name="digits">(<see langword="out"/> parameter) The digits.</param>
		/// <param name="map">
		/// (<see langword="out"/> parameter) The map of all cells used.
		/// </param>
		public void Deconstruct(
			out int region, out IEnumerable<int> relativePos, out IEnumerable<int> digits, out GridMap map) =>
			(region, relativePos, digits, map) = (Region, RelativePos, Digits, Map);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="region">(<see langword="out"/> parameter) The region.</param>
		/// <param name="digitsMask">
		/// (<see langword="out"/> parameter) The mask of all digits used.
		/// </param>
		/// <param name="relativePosMask">
		/// (<see langword="out"/> parameter) The mask of all relative positions used.
		/// </param>
		/// <param name="relativePos">
		/// (<see langword="out"/> parameter) The relative positions.
		/// </param>
		/// <param name="digits">(<see langword="out"/> parameter) The digits.</param>
		/// <param name="map">
		/// (<see langword="out"/> parameter) The map of all cells used.
		/// </param>
		public void Deconstruct(
			out int region, out short relativePosMask, out short digitsMask,
			out IEnumerable<int> relativePos, out IEnumerable<int> digits, out GridMap map) =>
			(region, relativePos, digits, relativePosMask, digitsMask, map) = (Region, RelativePos, Digits, RelativePosMask, DigitsMask, Map);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Als comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Als other)
		{
			return IsBivalueCellAls && other.IsBivalueCellAls
				? (_mask & 511) == (other._mask & 511)
					&& RegionCells[Region][RelativePosMask.FindFirstSet()]
					== RegionCells[other.Region][other.RelativePosMask.FindFirstSet()]
				: _mask == other._mask;
		}

		/// <summary>
		/// Indicates whether the specified grid contains the digit.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsDigit(IReadOnlyGrid grid, int digit, out GridMap result)
		{
			result = GridMap.Empty;
			foreach (int cell in Cells)
			{
				if ((grid.GetCandidatesReversal(cell) >> digit & 1) != 0)
				{
					result.Add(cell);
				}
			}

			return result.IsNotEmpty;
		}

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override int GetHashCode() => _mask;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString()
		{
			return IsBivalueCellAls
				? new StringBuilder()
					.Append(new DigitCollection(Digits).ToString(null))
					.Append("/")
					.Append(new CellCollection(Cells).ToString())
					.ToString()
				: new StringBuilder()
					.Append(new DigitCollection(Digits).ToString(null))
					.Append("/")
					.Append(new CellCollection(Cells).ToString())
					.Append($" in {new RegionCollection(stackalloc[] { Region }).ToString()}")
					.ToString();
		}


		/// <summary>
		/// To search for all ALSes in the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All ALSes searched.</returns>
		public static IEnumerable<Als>[] GetAllAlses(IReadOnlyGrid grid)
		{
			var dic = new IEnumerable<Als>[27];
			for (int region = 0; region < 27; region++)
			{
				dic[region] = GetAllAlses(grid, region);
			}

			return dic;
		}

		/// <summary>
		/// To search for all ALSes in the specified grid and the region to iterate on.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="region">The region.</param>
		/// <returns>All ALSes searched.</returns>
		[SuppressMessage("", "IDE0004")]
		private static IEnumerable<Als> GetAllAlses(IReadOnlyGrid grid, int region)
		{
			short posMask = 0;
			int i = 0;
			foreach (int cell in RegionCells[region])
			{
				if (grid.GetStatus(cell) == Empty)
				{
					posMask |= (short)(1 << i);
				}

				i++;
			}
			if (posMask == 0)
			{
				yield break;
			}

			int count = posMask.CountSet();
			for (int size = 1; size <= count; size++)
			{
				foreach (short mask in GetCombinations(count, size))
				{
					short realMask = 0;
					foreach (int index in mask.GetAllSets())
					{
						realMask |= (short)(1 << posMask.SetAt(index));
					}

					short digitsMask = 0;
					foreach (int cell in MaskExtensions.GetCells(region, realMask))
					{
						digitsMask |= grid.GetCandidatesReversal(cell);
					}
					if (digitsMask.CountSet() - 1 != size)
					{
						// Not an ALS.
						continue;
					}

					yield return new Als((int)digitsMask | realMask << 9 | region << 18);
				}
			}
		}

		/// <summary>
		/// Get combinations.
		/// </summary>
		/// <param name="count">The number of cells in total.</param>
		/// <param name="size">The number of cells you want to take.</param>
		/// <returns>All masks.</returns>
		private static IEnumerable<short> GetCombinations(int count, int size)
		{
			foreach (short mask in new BitCombinationGenerator(count, size))
			{
				yield return mask;
			}
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Als left, Als right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Als left, Als right) => !(left == right);
	}
}
