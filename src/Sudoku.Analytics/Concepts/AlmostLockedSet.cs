using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Text.Notation;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Concepts;

/// <summary>
/// Defines a data pattern that describes an ALS.
/// </summary>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="possibleEliminationMap">Gets the possible cells that can store eliminations for the ALS.</param>
/// <remarks>
/// An <b>Almost Locked Set</b> is a sudoku concept, which describes a case that
/// <c>n</c> cells contains <c>(n + 1)</c> kinds of different digits.
/// The special case is a bi-value cell.
/// </remarks>
public sealed partial class AlmostLockedSet(
	[DataMember] Mask digitsMask,
	[DataMember] scoped in CellMap cells,
	[DataMember] scoped in CellMap possibleEliminationMap
)
{
	/// <summary>
	/// Indicates an array of the total number of the strong relations in an ALS of the different size.
	/// The field is only unused in the property <see cref="StrongLinks"/>.
	/// </summary>
	/// <seealso cref="StrongLinks"/>
	private static readonly int[] StrongRelationsCount = [0, 1, 3, 6, 10, 15, 21, 28, 36, 45];


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public House House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			_ = Cells.InOneHouse(out var houseIndex);
			return houseIndex;
		}
	}

	/// <summary>
	/// Indicates whether the ALS only uses a bi-value cell.
	/// </summary>
	public bool IsBivalueCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Count == 1;
	}

	/// <summary>
	/// Indicates all strong links in this ALS.
	/// The result will be represented as a <see cref="Mask"/> mask of 9 bits indicating which bits used.
	/// </summary>
	public Mask[] StrongLinks
	{
		get
		{
			scoped var digits = DigitsMask.GetAllSets();
			var result = new Mask[StrongRelationsCount[digits.Length - 1]];
			for (var (i, x, l) = (0, 0, digits.Length); i < l - 1; i++)
			{
				for (var j = i + 1; j < l; j++)
				{
					result[x++] = (Mask)(1 << digits[i] | 1 << digits[j]);
				}
			}

			return result;
		}
	}


	/// <summary>
	/// Indicates whether the specified grid contains the digit.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="result">The result.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool ContainsDigit(scoped in Grid grid, Digit digit, out CellMap result)
	{
		result = CellMap.Empty;
		foreach (var cell in Cells)
		{
			if ((grid.GetCandidates(cell) >> digit & 1) != 0)
			{
				result.Add(cell);
			}
		}

		return !!result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var digitsStr = DigitNotation.ToString(DigitsMask, DigitNotation.Kind.Compact);
		var houseStr = HouseNotation.ToString(House);
		return IsBivalueCell ? $"{digitsStr}/{Cells}" : $"{digitsStr}/{Cells} {GetString("KeywordIn")} {houseStr}";
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
				foreach (var map in tempMap.GetSubsets(size))
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && house >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					var digitsMask = grid[map];
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					var coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							map,
							house < 9 && coveredLine is >= 9 and not InvalidTrailingZeroCountMethodFallback
								? ((HousesMap[house] | HousesMap[coveredLine]) & emptyMap) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return [.. result];
	}
}
