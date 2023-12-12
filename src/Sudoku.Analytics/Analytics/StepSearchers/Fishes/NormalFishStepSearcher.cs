using System.Runtime.CompilerServices;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearcherModules;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Runtime.MaskServices;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Normal Fish</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Normal fishes:
/// <list type="bullet">
/// <item>X-Wing</item>
/// <item>Swordfish</item>
/// <item>Jellyfish</item>
/// </list>
/// </item>
/// <item>
/// Finned fishes:
/// <list type="bullet">
/// <item>
/// Finned normal fishes:
/// <list type="bullet">
/// <item>Finned X-Wing</item>
/// <item>Finned Swordfish</item>
/// <item>Finned Jellyfish</item>
/// </list>
/// </item>
/// <item>
/// Finned sashimi fishes:
/// <list type="bullet">
/// <item>Sashimi X-Wing</item>
/// <item>Sashimi Swordfish</item>
/// <item>Sashimi Jellyfish</item>
/// </list>
/// </item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.XWing, Technique.Swordfish, Technique.Jellyfish,
	Technique.Squirmbag, Technique.Whale, Technique.Leviathan,
	Technique.FinnedXWing, Technique.FinnedSwordfish, Technique.FinnedJellyfish,
	Technique.FinnedSquirmbag, Technique.FinnedWhale, Technique.FinnedLeviathan,
	Technique.SashimiXWing, Technique.SashimiSwordfish, Technique.SashimiJellyfish,
	Technique.SashimiSquirmbag, Technique.SashimiWhale, Technique.SashimiLeviathan,
	Technique.SiameseFinnedXWing, Technique.SiameseFinnedSwordfish, Technique.SiameseFinnedJellyfish,
	Technique.SiameseFinnedSquirmbag, Technique.SiameseFinnedWhale, Technique.SiameseFinnedLeviathan,
	Technique.SiameseSashimiXWing, Technique.SiameseSashimiSwordfish, Technique.SiameseSashimiJellyfish,
	Technique.SiameseSashimiSquirmbag, Technique.SiameseSashimiWhale, Technique.SiameseSashimiLeviathan)]
[StepSearcherRuntimeName("StepSearcherName_NormalFishStepSearcher")]
public sealed partial class NormalFishStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether Finned X-Wing and Sashimi X-Wing should be disabled.
	/// </summary>
	/// <remarks>
	/// This option may be used when you are used to spotting skyscrapers (grouped or non-grouped).
	/// All Finned X-Wings can be replaced with Grouped Skyscrapers, and all Sashimi X-Wings can be replaced with Non-grouped Skyscrapers.
	/// </remarks>
	[RuntimeIdentifier(RuntimeIdentifier.DisableFinnedOrSashimiXWing)]
	public bool DisableFinnedOrSashimiXWing { get; set; }


	/// <inheritdoc/>
	protected internal override unsafe Step? Collect(scoped ref AnalysisContext context)
	{
		var r = stackalloc House*[9];
		var c = stackalloc House*[9];
		Unsafe.InitBlock(r, 0, (uint)sizeof(House*) * 9);
		Unsafe.InitBlock(c, 0, (uint)sizeof(House*) * 9);

		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		for (var digit = 0; digit < 9; digit++)
		{
			if (ValuesMap[digit].Count > 5)
			{
				continue;
			}

			// Gather.
			for (var house = 9; house < 27; house++)
			{
				if (HousesMap[house] & CandidatesMap[digit])
				{
#pragma warning disable CA2014
					if (house < 18)
					{
						if (r[digit] == null)
						{
							var ptr = stackalloc House[10];
							Unsafe.InitBlock(ptr, 0, 10 * sizeof(House));

							r[digit] = ptr;
						}

						r[digit][++r[digit][0]] = house;
					}
					else
					{
						if (c[digit] == null)
						{
							var ptr = stackalloc House[10];
							Unsafe.InitBlock(ptr, 0, 10 * sizeof(House));

							c[digit] = ptr;
						}

						c[digit][++c[digit][0]] = house;
					}
#pragma warning restore CA2014
				}
			}
		}

		for (var size = 2; size <= 4; size++)
		{
			if (Collect(accumulator, in grid, ref context, size, r, c, false, true, onlyFindOne) is { } finlessRowFish)
			{
				return finlessRowFish;
			}
			if (Collect(accumulator, in grid, ref context, size, r, c, false, false, onlyFindOne) is { } finlessColumnFish)
			{
				return finlessColumnFish;
			}
			if (Collect(accumulator, in grid, ref context, size, r, c, true, true, onlyFindOne) is { } finnedRowFish)
			{
				return finnedRowFish;
			}
			if (Collect(accumulator, in grid, ref context, size, r, c, true, false, onlyFindOne) is { } finnedColumnFish)
			{
				return finnedColumnFish;
			}
		}

		return null;
	}

	/// <summary>
	/// Get all possible normal fishes.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="size">The size.</param>
	/// <param name="r">The possible row table to iterate.</param>
	/// <param name="c">The possible column table to iterate.</param>
	/// <param name="withFin">Indicates whether the searcher will check for the existence of fins.</param>
	/// <param name="searchRow">
	/// Indicates whether the searcher searches for fishes in the direction of rows.
	/// </param>
	/// <param name="onlyFindOne">Indicates whether the method only searches for one step.</param>
	/// <returns>The first found step.</returns>
	private unsafe NormalFishStep? Collect(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		int size,
		House** r,
		House** c,
		bool withFin,
		bool searchRow,
		bool onlyFindOne
	)
	{
		// Iterate on each digit.
		for (var digit = 0; digit < 9; digit++)
		{
			// Check the validity of the distribution for the current digit.
			var pBase = searchRow ? r[digit] : c[digit];
			var pCover = searchRow ? c[digit] : r[digit];
			if (pBase == null || pBase[0] <= size)
			{
				continue;
			}

			// Iterate on the base set combination.
			foreach (var bs in Pointer.Slice(pBase, 1, 10, true).GetSubsets(size))
			{
				// 'baseLine' is the map that contains all base set cells.
				var baseLine = size switch
				{
					2 => CandidatesMap[digit] & (HousesMap[bs[0]] | HousesMap[bs[1]]),
					3 => CandidatesMap[digit] & (HousesMap[bs[0]] | HousesMap[bs[1]] | HousesMap[bs[2]]),
					4 => CandidatesMap[digit] & (HousesMap[bs[0]] | HousesMap[bs[1]] | HousesMap[bs[2]] | HousesMap[bs[3]])
				};

				// Iterate on the cover set combination.
				foreach (var cs in Pointer.Slice(pCover, 1, 10, true).GetSubsets(size))
				{
					// 'coverLine' is the map that contains all cover set cells.
					var coverLine = size switch
					{
						2 => CandidatesMap[digit] & (HousesMap[cs[0]] | HousesMap[cs[1]]),
						3 => CandidatesMap[digit] & (HousesMap[cs[0]] | HousesMap[cs[1]] | HousesMap[cs[2]]),
						4 => CandidatesMap[digit] & (HousesMap[cs[0]] | HousesMap[cs[1]] | HousesMap[cs[2]] | HousesMap[cs[3]])
					};

					// Now check the fins and the elimination cells.
					CellMap elimMap, fins = [];
					if (!withFin)
					{
						// If the current searcher doesn't check fins, we'll just get the pure check:
						// 1. Base set contain more cells than cover sets.
						// 2. Elimination cells set isn't empty.
						if (baseLine - coverLine)
						{
							continue;
						}

						elimMap = coverLine - baseLine;
						if (!elimMap)
						{
							continue;
						}
					}
					else // Should check fins.
					{
						// All fins should be in the same block.
						var blockMask = (fins = baseLine - coverLine).BlockMask;
						if (!fins || !IsPow2(blockMask))
						{
							continue;
						}

						// Cover set shouldn't overlap with the block of all fins lying in.
						var finBlock = TrailingZeroCount(blockMask);
						if (!(coverLine & HousesMap[finBlock]))
						{
							continue;
						}

						// Don't intersect.
						if (!(HousesMap[finBlock] & coverLine - baseLine))
						{
							continue;
						}

						// Finally, get the elimination cells.
						elimMap = coverLine - baseLine & HousesMap[finBlock];
					}

					if (DisableFinnedOrSashimiXWing && size == 2 && !!fins)
					{
						// We should disallow collecting Finned X-Wing and Sashimi X-Wings
						// when option 'DisableFinnedOrSashimiXWing' is configured.
						continue;
					}

					// Gather the result.
					var step = new NormalFishStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, digit)],
						[
							[
								..
								from cell in withFin ? baseLine - fins : baseLine
								select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
								.. withFin ? from cell in fins select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit) : [],
								.. from baseSet in bs select new HouseViewNode(WellKnownColorIdentifier.Normal, baseSet),
								.. from coverSet in cs select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, coverSet),
							],
							GetDirectView(digit, bs, cs, in fins, searchRow)
						],
						context.PredefinedOptions,
						digit,
						HouseMaskOperations.Create(bs),
						HouseMaskOperations.Create(cs),
						in fins,
						FishModule.IsSashimi(bs, in fins, digit)
					);
					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Get the direct fish view with the specified grid and the base sets.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="coverSets">The cover sets.</param>
	/// <param name="fins">The cells of the fin in the current fish.</param>
	/// <param name="searchRow">Indicates whether the current searcher searches row.</param>
	/// <returns>The view.</returns>
	private static View GetDirectView(Digit digit, House[] baseSets, House[] coverSets, scoped ref readonly CellMap fins, bool searchRow)
	{
		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = fins ? new List<CandidateViewNode>() : null;
		foreach (var baseSet in baseSets)
		{
			foreach (var cell in HousesMap[baseSet])
			{
				switch (CandidatesMap[digit].Contains(cell))
				{
					case true when fins.Contains(cell):
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell));
						break;
					}
					default:
					{
						var flag = false;
						foreach (var c in ValuesMap[digit])
						{
							if (HousesMap[c.ToHouseIndex(searchRow ? HouseType.Column : HouseType.Row)].Contains(cell))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}

						var (baseMap, coverMap) = (CellMap.Empty, CellMap.Empty);
						foreach (var b in baseSets)
						{
							baseMap |= HousesMap[b];
						}
						foreach (var c in coverSets)
						{
							coverMap |= HousesMap[c];
						}
						baseMap &= coverMap;
						if (baseMap.Contains(cell))
						{
							continue;
						}

						cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell) { RenderingMode = RenderingMode.BothDirectAndPencilmark });
						break;
					}
				}
			}
		}

		foreach (var cell in ValuesMap[digit])
		{
			cellOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell) { RenderingMode = RenderingMode.BothDirectAndPencilmark });
		}
		foreach (var cell in fins)
		{
			candidateOffsets!.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
		}

		return [.. cellOffsets, .. candidateOffsets ?? []];
	}
}

/// <summary>
/// The local method provider for pointer types.
/// </summary>
file static class Pointer
{
	/// <summary>
	/// Get the new array from the pointer, with the specified start index.
	/// </summary>
	/// <typeparam name="T">The type of the pointer element.</typeparam>
	/// <param name="ptr">The pointer.</param>
	/// <param name="index">The start index that you want to pick from.</param>
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// For example, the pointer is the address of the first element in an array <c>[0, 1, 3, 6, 10]</c>,
	/// if parameter <paramref name="index"/> is 2, the return array will be <c>[3, 6, 10]</c>. Note that
	/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
	/// 5 elements in this case.
	/// </remarks>
	public static unsafe ReadOnlySpan<T> Slice<T>(T* ptr, int index, int length)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = new T[length - index];
		for (var i = index; i < length; i++)
		{
			result[i - index] = ptr[i];
		}

		return result;
	}

	/// <summary>
	/// Get the new array from the pointer, with the specified start index.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <param name="index">The start index that you want to pick from.</param>
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <param name="removeTrailingZeros">
	/// Indicates whether the method will remove the trailing zeros. If <see langword="false"/>,
	/// the method will be same as <see cref="Slice{T}(T*, int, int)"/>.
	/// </param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// For example, the pointer is the address of the first element in an array <c>[0, 1, 3, 6, 10]</c>,
	/// if parameter <paramref name="index"/> is 2, the return array will be <c>[3, 6, 10]</c>. Note that
	/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
	/// 5 elements in this case.
	/// </remarks>
	/// <seealso cref="Slice{T}(T*, int, int)"/>
	public static unsafe ReadOnlySpan<int> Slice(int* ptr, int index, int length, bool removeTrailingZeros)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		if (removeTrailingZeros)
		{
			var count = 0;
			var p = ptr + length - 1;
			for (var i = length - 1; i >= 0; i--, p--, count++)
			{
				if (*p != 0)
				{
					break;
				}
			}

			var result = new int[length - count - index];
			for (var i = index; i < length - count; i++)
			{
				result[i - index] = ptr[i];
			}

			return result;
		}

		return Slice(ptr, index, length);
	}
}
