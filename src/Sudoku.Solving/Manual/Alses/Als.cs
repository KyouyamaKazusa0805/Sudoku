using System;
using System.Collections.Generic;
using System.Extensions;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a normal ALS.
	/// </summary>
	public readonly struct Als : IValueEquatable<Als>
	{
		/// <summary>
		/// Initializes an instance with the specified digit mask and the map of cells.
		/// </summary>
		/// <param name="digitMask">The digit mask.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		public Als(short digitMask, in Cells map) : this(digitMask, map, default)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified digit mask and the map of cells.
		/// </summary>
		/// <param name="digitMask">The digit mask.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <param name="possibleEliminationSet">
		/// (<see langword="in"/> parameter) The possible elimination set.
		/// </param>
		public Als(short digitMask, in Cells map, in Cells possibleEliminationSet)
		{
			DigitsMask = digitMask;
			Map = map;
			IsBivalueCell = map.Count == 1;
			PossibleEliminationSet = possibleEliminationSet;
			Map.AllSetsAreInOneRegion(out int region);
			Region = region;
		}


		/// <summary>
		/// Indicates whether this instance is a bi-value-cell ALS.
		/// </summary>
		public bool IsBivalueCell { get; }

		/// <summary>
		/// Indicates the region that the instance lies in.
		/// </summary>
		public int Region { get; }

		/// <summary>
		/// Indicates the mask of each digit.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates the map that ALS lying on.
		/// </summary>
		public Cells Map { get; }

		/// <summary>
		/// Indicates the possible elimination set.
		/// </summary>
		public Cells PossibleEliminationSet { get; }

		/// <summary>
		/// Indicates all strong links in this ALS. The result will be represented
		/// as a <see cref="short"/> mask of 9 bits indicating which bits used.
		/// </summary>
		public IEnumerable<short> StrongLinksMask
		{
			get
			{
				var digits = DigitsMask.GetAllSets().ToArray();
				for (int i = 0, length = digits.Length; i < length - 1; i++)
				{
					for (int j = i + 1; j < length; j++)
					{
						yield return (short)(1 << digits[i] | 1 << digits[j]);
					}
				}
			}
		}


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="region">(<see langword="out"/> parameter) The region.</param>
		/// <param name="digitsMask">(<see langword="out"/> parameter) The digits mask.</param>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		public void Deconstruct(out int region, out short digitsMask, out Cells map)
		{
			region = Region;
			digitsMask = DigitsMask;
			map = Map;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="isBivalueCell">
		/// (<see langword="out"/> parameter) Indicates whether the specified ALS is bi-value.
		/// </param>
		/// <param name="region">(<see langword="out"/> parameter) The region.</param>
		/// <param name="digitsMask">(<see langword="out"/> parameter) The digits mask.</param>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		/// <param name="possibleEliminations">(<see langword="out"/> parameter) The possible eliminations.</param>
		/// <param name="strongLinksMask">(<see langword="out"/> parameter) The strong links mask.</param>
		public void Deconstruct(
			out bool isBivalueCell, out int region, out short digitsMask,
			out Cells map, out Cells possibleEliminations, out IEnumerable<short> strongLinksMask)
		{
			isBivalueCell = IsBivalueCell;
			region = Region;
			digitsMask = DigitsMask;
			map = Map;
			possibleEliminations = PossibleEliminationSet;
			strongLinksMask = StrongLinksMask;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Als comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the specified grid contains the digit.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsDigit(in SudokuGrid grid, int digit, out Cells result)
		{
			result = Cells.Empty;
			foreach (int cell in Map)
			{
				if ((grid.GetCandidates(cell) >> digit & 1) != 0)
				{
					result.AddAnyway(cell);
				}
			}

			return !result.IsEmpty;
		}

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public bool Equals(in Als other) => DigitsMask == other.DigitsMask && Map == other.Map;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <remarks>
		/// If you want to determine the equality of two instance, I recommend you
		/// <b>should</b> use method <see cref="Equals(in Als)"/> instead of this method.
		/// </remarks>
		public override int GetHashCode()
		{
			short mask = 0;
			int i = 0;
			foreach (int cell in RegionCells[Region])
			{
				if (Map.Contains(cell))
				{
					mask |= (short)(1 << i);
				}

				i++;
			}

			return Region << 18 | mask << 9 | (int)DigitsMask;
		}

		/// <inheritdoc/>
		public override string ToString() =>
			IsBivalueCell
			? new StringBuilder()
				.Append(new DigitCollection(DigitsMask).ToString(null))
				.Append('/')
				.Append(Map.ToString())
				.ToString()
			: new StringBuilder()
				.Append(new DigitCollection(DigitsMask).ToString(null))
				.Append('/')
				.Append(Map.ToString())
				.Append(' ')
				.Append("in")
				.Append(' ')
				.Append(new RegionCollection(Region).ToString())
				.ToString();


		/// <summary>
		/// To search for all ALSes in the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All ALSes searched.</returns>
		public static Als[] GetAllAlses(SudokuGrid grid)
		{
			var bivalueMap = BivalueMap;
			var emptyMap = EmptyMap;

			// Get all bi-value-cell ALSes.
			var result = new List<Als>();
			foreach (int cell in bivalueMap)
			{
				result.Add(new(grid.GetCandidates(cell), new() { cell }, PeerMaps[cell] & emptyMap));
			}

			// Get all non-bi-value-cell ALSes.
			var list = new List<int>();
			for (int region = 0; region < 27; region++)
			{
				var regionMap = RegionMaps[region];
				var tempMap = regionMap & emptyMap;
				if (tempMap.Count < 3)
				{
					continue;
				}

				int[] emptyCells = tempMap.ToArray();
				list.Clear();
				list.AddRange(emptyCells);
				for (int size = 2; size <= emptyCells.Length - 1; size++)
				{
					foreach (int[] cells in list.GetSubsets(size))
					{
						var map = new Cells(cells);
						if (map.BlockMask != 0 && (map.BlockMask & map.BlockMask - 1) == 0 && region >= 9)
						{
							// All ALS cells lying on a box-row or a box-column
							// will be processed as a block ALS.
							continue;
						}

						// Get all candidates in these cells.
						short digitsMask = 0;
						foreach (int cell in cells)
						{
							digitsMask |= grid.GetCandidates(cell);
						}
						if (PopCount((uint)digitsMask) - 1 != size)
						{
							continue;
						}

						int coveredLine = map.CoveredLine;
						result.Add(
							new(
								digitsMask,
								map,
								(region, coveredLine) is ( < 9, >= 9 and not Constants.InvalidFirstSet)
								? ((regionMap | RegionMaps[coveredLine]) & emptyMap) - map
								: tempMap - map));
					}
				}
			}

			return result.ToArray();
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in Als left, in Als right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in Als left, in Als right) => !(left == right);
	}
}
