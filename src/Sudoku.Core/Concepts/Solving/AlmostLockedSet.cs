namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines a data structure that describes an ALS.
/// </summary>
/// <remarks>
/// An <b>Almost Locked Set</b> is a sudoku concept, which describes a case that
/// <c>n</c> cells contains <c>(n + 1)</c> kinds of different digits.
/// The special case is a bi-value cell.
/// </remarks>
public sealed class AlmostLockedSet :
	IEquatable<AlmostLockedSet>,
	ITechniquePattern<AlmostLockedSet>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<AlmostLockedSet, AlmostLockedSet>
#endif
{
	/// <summary>
	/// Indicates an array of the total number of the strong relations in an ALS of the different size.
	/// The field is only unsed in the property <see cref="StrongLinksMask"/>.
	/// </summary>
	/// <seealso cref="StrongLinksMask"/>
	private static readonly int[] StrongRelationsCount = { 0, 1, 3, 6, 10, 15, 21, 34, 45 };


	/// <summary>
	/// Initializes an <see cref="AlmostLockedSet"/> instance
	/// via the specified digit mask and the map of cells.
	/// </summary>
	/// <param name="digitMask">The digit mask.</param>
	/// <param name="map">The map.</param>
	/// <param name="possibleEliminationMap">
	/// Indicates the possible cells that can be as the elimination.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlmostLockedSet(short digitMask, in Cells map, in Cells possibleEliminationMap) =>
		(DigitsMask, Map, PossibleEliminationMap) = (digitMask, map, possibleEliminationMap);


	/// <summary>
	/// Indicates the region used.
	/// </summary>
	public int Region
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			_ = Map.AllSetsAreInOneRegion(out int region);
			return region;
		}
	}

	/// <summary>
	/// Indicates whether the ALS only uses a bi-value cell.
	/// </summary>
	public bool IsBivalueCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.Count == 1;
	}

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public short DigitsMask { get; }

	/// <summary>
	/// Indicates all strong links in this ALS. The result will be represented
	/// as a <see cref="short"/> mask of 9 bits indicating which bits used.
	/// </summary>
	public short[] StrongLinksMask
	{
		get
		{
			var digits = DigitsMask.GetAllSets();
			short[] result = new short[StrongRelationsCount[digits.Length - 1]];
			for (int i = 0, x = 0, l = digits.Length, iterationLength = l - 1; i < iterationLength; i++)
			{
				for (int j = i + 1; j < l; j++)
				{
					result[x++] = (short)(1 << digits[i] | 1 << digits[j]);
				}
			}

			return result;
		}
	}

	/// <inheritdoc/>
	public Cells Map { get; }

	/// <summary>
	/// Gets the possible cells that can store eliminations for the ALS.
	/// </summary>
	public Cells PossibleEliminationMap { get; }


	/// <summary>
	/// Indicates whether the specified grid contains the digit.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="result">The result.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool ContainsDigit(in Grid grid, int digit, out Cells result)
	{
		result = Cells.Empty;
		foreach (int cell in Map)
		{
			if ((grid.GetCandidates(cell) >> digit & 1) != 0)
			{
				result.Add(cell);
			}
		}

		return result is not [];
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as AlmostLockedSet);

	/// <summary>
	/// Determine whether the specified <see cref="AlmostLockedSet"/> instance holds the same
	/// <see cref="DigitsMask"/> and <see cref="Map"/> property values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] AlmostLockedSet? other) =>
		other is not null && DigitsMask == other.DigitsMask && Map == other.Map;

	/// <inheritdoc/>
	/// <remarks>
	/// If you want to determine the equality of two instance, I recommend you
	/// <b>should</b> use method <see cref="Equals(AlmostLockedSet?)"/> instead of this method.
	/// </remarks>
	/// <seealso cref="Equals(AlmostLockedSet?)"/>
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string digitsStr = new DigitCollection(DigitsMask).ToSimpleString();
		string regionStr = new RegionCollection(Region).ToString();
		return IsBivalueCell ? $"{digitsStr}/{Map}" : $"{digitsStr}/{Map} in {regionStr}";
	}


	/// <summary>
	/// Gathers all possible <see cref="AlmostLockedSet"/>s in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All possible found <see cref="AlmostLockedSet"/>.</returns>
	public static AlmostLockedSet[] Gather(in Grid grid)
	{
		_ = grid is { EmptyCells: var emptyMap, BivalueCells: var bivalueMap };

		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (int cell in bivalueMap)
		{
			result.Add(new(grid.GetCandidates(cell), Cells.Empty + cell, PeerMaps[cell] & emptyMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (int region = 0; region < 27; region++)
		{
			if ((RegionMaps[region] & emptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (int size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					short blockMask = map.BlockMask;
					if (IsPow2(blockMask) && region >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					short digitsMask = 0;
					foreach (int cell in map)
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
							region < 9 && coveredLine is >= 9 and not InvalidFirstSet
								? ((RegionMaps[region] | RegionMaps[coveredLine]) & emptyMap) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return result.ToArray();
	}


	/// <summary>
	/// Determine whether two <see cref="AlmostLockedSet"/>s hold a same <see cref="DigitsMask"/>
	/// and <see cref="Map"/> property values.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(AlmostLockedSet left, AlmostLockedSet right) => left.Equals(right);

	/// <summary>
	/// Determine whether two <see cref="AlmostLockedSet"/>s don't hold a same <see cref="DigitsMask"/>
	/// and <see cref="Map"/> property values.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(AlmostLockedSet left, AlmostLockedSet right) => !(left == right);
}
