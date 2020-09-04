using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates a normal ALS.
	/// </summary>
	public readonly struct Als : IEquatable<Als>
	{
		/// <summary>
		/// Initializes an instance with the specified digit mask and the map of cells.
		/// </summary>
		/// <param name="digitMask">The digit mask.</param>
		/// <param name="map">The map.</param>
		public Als(short digitMask, GridMap map) : this(digitMask, map, default)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified digit mask and the map of cells.
		/// </summary>
		/// <param name="digitMask">The digit mask.</param>
		/// <param name="map">The map.</param>
		/// <param name="possibleEliminationSet">The possible elimination set.</param>
		public Als(short digitMask, GridMap map, GridMap possibleEliminationSet)
		{
			(DigitsMask, Map, IsBivalueCell, PossibleEliminationSet) = (digitMask, map, map.Count == 1, possibleEliminationSet);
			Map.AllSetsAreInOneRegion(out int region);
			Region = region;
		}


		/// <summary>
		/// Indicates whether this instance is a bi-value-cell ALS.
		/// </summary>
		public bool IsBivalueCell { get; }

		/// <summary>
		/// Indicates the region that the instance lies on.
		/// </summary>
		public int Region { get; }

		/// <summary>
		/// Indicates the mask of each digit.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates the map that ALS lying on.
		/// </summary>
		public GridMap Map { get; }

		/// <summary>
		/// Indicates the possible elimination set.
		/// </summary>
		public GridMap PossibleEliminationSet { get; }

		/// <summary>
		/// Indicates all strong links in this ALS. The result will be represented
		/// as a <see cref="short"/> mask of 9 bits indicating which bits used.
		/// </summary>
		public IEnumerable<short> StrongLinksMask
		{
			get
			{
				int[] digits = DigitsMask.GetAllSets().ToArray();
				for (int i = 0, length = digits.Length; i < length - 1; i++)
				{
					for (int j = i + 1; j < length; j++)
					{
						yield return (short)(1 << digits[i] | 1 << digits[j]);
					}
				}
			}
		}


		/// <include file='....\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="region">(<see langword="out"/> parameter) The region.</param>
		/// <param name="digitsMask">(<see langword="out"/> parameter) The digits mask.</param>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		public void Deconstruct(out int region, out short digitsMask, out GridMap map) =>
			(region, digitsMask, map) = (Region, DigitsMask, Map);

		/// <include file='....\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
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
			out GridMap map, out GridMap possibleEliminations, out IEnumerable<short> strongLinksMask) =>
			(isBivalueCell, region, digitsMask, map, possibleEliminations, strongLinksMask) = (IsBivalueCell, Region, DigitsMask, Map, PossibleEliminationSet, StrongLinksMask);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Als comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the specified grid contains the digit.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsDigit(Grid grid, int digit, out GridMap result)
		{
			result = GridMap.Empty;
			foreach (int cell in Map)
			{
				if ((grid.GetCandidateMask(cell) >> digit & 1) != 0)
				{
					result.Add(cell);
				}
			}

			return result.IsNotEmpty;
		}

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(Als other) => DigitsMask == other.DigitsMask && Map == other.Map;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <remarks>
		/// If you want to determine the equality of two instance, I recommend you
		/// <b>should</b> use method <see cref="Equals(Als)"/> instead of this method.
		/// </remarks>
		[SuppressMessage("", "IDE0004")]
		public override int GetHashCode()
		{
			short mask = 0;
			int i = 0;
			foreach (int cell in RegionCells[Region])
			{
				if (Map[cell])
				{
					mask |= (short)(1 << i);
				}

				i++;
			}

			return Region << 18 | (int)mask << 9 | (int)DigitsMask;
		}

		/// <inheritdoc/>
		public override string ToString() =>
			IsBivalueCell
				? new StringBuilder()
					.Append(new DigitCollection(DigitsMask.GetAllSets()).ToString(null))
					.Append("/")
					.Append(new CellCollection(Map).ToString())
					.ToString()
				: new StringBuilder()
					.Append(new DigitCollection(DigitsMask.GetAllSets()).ToString(null))
					.Append("/")
					.Append(new CellCollection(Map).ToString())
					.Append($" in {new RegionCollection(Region).ToString()}")
					.ToString();


		/// <summary>
		/// To search for all ALSes in the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All ALSes searched.</returns>
		public static IEnumerable<Als> GetAllAlses(Grid grid)
		{
			var bivalueMap = TechniqueSearcher.BivalueMap;
			var emptyMap = TechniqueSearcher.EmptyMap;

			// Get all bi-value-cell ALSes.
			foreach (int cell in bivalueMap)
			{
				yield return new(grid.GetCandidateMask(cell), new() { cell }, PeerMaps[cell] & emptyMap);
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
					foreach (int[] cells in list.ToArray().GetSubsets(size))
					{
						var map = new GridMap(cells);
						if (map.BlockMask.IsPowerOfTwo() && region >= 9)
						{
							// All ALS cells lying on a box-row or a box-column
							// will be processed as a block ALS.
							continue;
						}

						// Get all candidates in these cells.
						short digitsMask = 0;
						foreach (int cell in cells)
						{
							digitsMask |= grid.GetCandidateMask(cell);
						}
						if (digitsMask.CountSet() - 1 != size)
						{
							continue;
						}

						int coveredLine = map.CoveredLine;
						yield return new(
							digitsMask,
							map,
							(region, coveredLine) is ( < 9, >= 9)
								? ((regionMap | RegionMaps[coveredLine]) & emptyMap) - map
								: tempMap - map);
					}
				}
			}
		}


		/// <include file='....\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Als left, Als right) => left.Equals(right);

		/// <include file='....\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Als left, Als right) => !(left == right);
	}
}
