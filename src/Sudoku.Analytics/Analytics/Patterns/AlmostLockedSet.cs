namespace Sudoku.Analytics.Patterns;

/// <summary>
/// Defines a data structure that describes an ALS.
/// </summary>
/// <param name="digitMask">The digit mask.</param>
/// <param name="map">The map.</param>
/// <param name="possibleEliminationMap">Indicates the possible cells that can be as the elimination.</param>
/// <remarks>
/// An <b>Almost Locked Set</b> is a sudoku concept, which describes a case that
/// <c>n</c> cells contains <c>(n + 1)</c> kinds of different digits.
/// The special case is a bi-value cell.
/// </remarks>
public sealed class AlmostLockedSet(short digitMask, scoped in CellMap map, scoped in CellMap possibleEliminationMap)
{
	/// <summary>
	/// Indicates an array of the total number of the strong relations in an ALS of the different size.
	/// The field is only unused in the property <see cref="StrongLinks"/>.
	/// </summary>
	/// <seealso cref="StrongLinks"/>
	private static readonly int[] StrongRelationsCount = { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45 };


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public int House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			_ = Map.AllSetsAreInOneHouse(out var houseIndex);
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
	public short DigitsMask { get; } = digitMask;

	/// <summary>
	/// Indicates all strong links in this ALS.
	/// The result will be represented as a <see cref="short"/> mask of 9 bits indicating which bits used.
	/// </summary>
	public short[] StrongLinks
	{
		get
		{
			scoped var digits = DigitsMask.GetAllSets();
			var result = new short[StrongRelationsCount[digits.Length - 1]];
			for (int i = 0, x = 0, l = digits.Length; i < l - 1; i++)
			{
				for (var j = i + 1; j < l; j++)
				{
					result[x++] = (short)(1 << digits[i] | 1 << digits[j]);
				}
			}

			return result;
		}
	}

	/// <inheritdoc/>
	public CellMap Map { get; } = map;

	/// <summary>
	/// Gets the possible cells that can store eliminations for the ALS.
	/// </summary>
	public CellMap PossibleEliminationMap { get; } = possibleEliminationMap;


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
		foreach (var cell in Map)
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
	public override string ToString()
	{
		var digitsStr = DigitMaskFormatter.Format(DigitsMask);
		var houseStr = HouseFormatter.Format(1 << House);
		return IsBivalueCell ? $"{digitsStr}/{Map}" : $"{digitsStr}/{Map} {R["KeywordIn"]} {houseStr}";
	}


	/// <summary>
	/// Gathers all possible <see cref="AlmostLockedSet"/>s in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All possible found <see cref="AlmostLockedSet"/> instances.</returns>
	public static AlmostLockedSet[] Gather(scoped in Grid grid)
	{
		_ = grid is { EmptyCells: var emptyMap, BivalueCells: var bivalueMap };

		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (var cell in bivalueMap)
		{
			result.Add(new(grid.GetCandidates(cell), CellsMap[cell], PeersMap[cell] & emptyMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & emptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (var size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && house >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					var digitsMask = grid.GetDigitsUnion(map);
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					var coveredLine = map.CoveredLine;
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
}
