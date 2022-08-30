namespace Sudoku.Solving.Patterns;

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
	IEqualityOperators<AlmostLockedSet, AlmostLockedSet>,
	ITechniquePattern<AlmostLockedSet>,
	ITechniquePatternGatherable<AlmostLockedSet>
{
	/// <summary>
	/// Indicates an array of the total number of the strong relations in an ALS of the different size.
	/// The field is only unused in the property <see cref="StrongLinks"/>.
	/// </summary>
	/// <seealso cref="StrongLinks"/>
	private static readonly int[] StrongRelationsCount = { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45 };


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
	internal AlmostLockedSet(short digitMask, scoped in CellMap map, scoped in CellMap possibleEliminationMap)
		=> (DigitsMask, Map, PossibleEliminationMap) = (digitMask, map, possibleEliminationMap);


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public int House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			_ = Map.AllSetsAreInOneHouse(out int houseIndex);
			return houseIndex;
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
	/// Indicates all strong links in this ALS.
	/// The result will be represented as a <see cref="short"/> mask of 9 bits indicating which bits used.
	/// </summary>
	public short[] StrongLinks
	{
		get
		{
			scoped var digits = DigitsMask.GetAllSets();
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
	public CellMap Map { get; }

	/// <summary>
	/// Gets the possible cells that can store eliminations for the ALS.
	/// </summary>
	public CellMap PossibleEliminationMap { get; }


	/// <summary>
	/// Indicates whether the specified grid contains the digit.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="result">The result.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool ContainsDigit(scoped in Grid grid, int digit, out CellMap result)
	{
		result = CellMap.Empty;
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
	public bool Equals([NotNullWhen(true)] AlmostLockedSet? other)
		=> other is not null && DigitsMask == other.DigitsMask && Map == other.Map;

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
		foreach (int cell in HouseCells[House])
		{
			if (Map.Contains(cell))
			{
				mask |= (short)(1 << i);
			}

			i++;
		}

		return House << 18 | mask << 9 | (int)DigitsMask;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string digitsStr = DigitMaskFormatter.Format(DigitsMask);
		string houseStr = HouseFormatter.Format(1 << House);
		return IsBivalueCell ? $"{digitsStr}/{Map}" : $"{digitsStr}/{Map} {R["KeywordIn"]} {houseStr}";
	}


	/// <inheritdoc/>
	public static AlmostLockedSet[] Gather(scoped in Grid grid)
	{
		_ = grid is { EmptyCells: var emptyMap, BivalueCells: var bivalueMap };

		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (int cell in bivalueMap)
		{
			result.Add(new(grid.GetCandidates(cell), CellMap.Empty + cell, PeersMap[cell] & emptyMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (int house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & emptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (int size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					short blockMask = map.BlockMask;
					if (IsPow2(blockMask) && house >= 9)
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
							house < 9 && coveredLine is >= 9 and not InvalidValidOfTrailingZeroCountMethodFallback
								? ((HousesMap[house] | HousesMap[coveredLine]) & emptyMap) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return result.ToArray();
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(AlmostLockedSet left, AlmostLockedSet right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(AlmostLockedSet left, AlmostLockedSet right) => !(left == right);
}
