using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Unique Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Loop Type 1</item>
/// <item>Unique Loop Type 2</item>
/// <item>Unique Loop Type 3</item>
/// <item>Unique Loop Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.UniqueLoopType1, Technique.UniqueLoopType2, Technique.UniqueLoopType3, Technique.UniqueLoopType4,
	Flags = ConditionalFlags.Standard | ConditionalFlags.TimeComplexity)]
public sealed partial class UniqueLoopStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (BivalueCells.Count < 5)
		{
			return null;
		}

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		var resultAccumulator = new List<UniqueLoopStep>();
		foreach (var cell in BivalueCells)
		{
			var mask = grid.GetCandidates(cell);
			var d1 = TrailingZeroCount(mask);
			var d2 = mask.GetNextSet(d1);

			using scoped var tempLoop = new ValueList<Cell>(14);
			var loopMap = CellMap.Empty;
			var patterns = new HashSet<Pattern>();
			CollectUniqueLoops(grid, cell, d1, d2, tempLoop, ref loopMap, patterns);

			if (patterns.Count == 0)
			{
				continue;
			}

			var comparer = (Mask)(1 << d1 | 1 << d2);
			foreach (var (loop, path, _) in patterns)
			{
				var extraCellsMap = loop - BivalueCells;
				switch (extraCellsMap.Count)
				{
					case 0:
					{
						// The current puzzle has multiple solutions.
						throw new StepSearcherProcessException<UniqueLoopStepSearcher>();
					}
					case 1:
					{
						if (CheckType1(resultAccumulator, d1, d2, loop, extraCellsMap, onlyFindOne, path) is { } step1)
						{
							return step1;
						}

						break;
					}
					default:
					{
						// Type 2, 3, 4.
						if (CheckType2(resultAccumulator, grid, d1, d2, loop, extraCellsMap, comparer, onlyFindOne, path) is { } step2)
						{
							return step2;
						}
						if (CheckType3(resultAccumulator, grid, d1, d2, loop, extraCellsMap, comparer, onlyFindOne, path) is { } step3)
						{
							return step3;
						}
						if (CheckType4(resultAccumulator, grid, d1, d2, loop, extraCellsMap, comparer, onlyFindOne, path) is { } step4)
						{
							return step4;
						}

						break;
					}
				}
			}
		}

		if (resultAccumulator.Count == 0)
		{
			return null;
		}

		var resultList = from step in resultAccumulator.Distinct() orderby step.Loop.Count select step;
		if (onlyFindOne)
		{
			return resultList.First();
		}

		accumulator.AddRange(resultList);

		return null;
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <param name="path">The path of the loop.</param>
	/// <returns>The step is worth.</returns>
	private UniqueLoopType1Step? CheckType1(
		List<UniqueLoopStep> accumulator,
		Digit d1,
		Digit d2,
		scoped in CellMap loop,
		scoped in CellMap extraCellsMap,
		bool onlyFindOne,
		Cell[] path
	)
	{
		var extraCell = extraCellsMap[0];
		using scoped var conclusions = new ValueList<Conclusion>(2);
		if (CandidatesMap[d1].Contains(extraCell))
		{
			conclusions.Add(new(Elimination, extraCell, d1));
		}
		if (CandidatesMap[d2].Contains(extraCell))
		{
			conclusions.Add(new(Elimination, extraCell, d2));
		}
		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in loop - extraCell)
		{
			candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d1));
			candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d2));
		}

		var step = new UniqueLoopType1Step([.. conclusions], [[.. candidateOffsets, .. GetLoopLinks(path)]], d1, d2, loop);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <param name="path">The path of the loop.</param>
	/// <returns>The step is worth.</returns>
	private UniqueLoopType2Step? CheckType2(
		List<UniqueLoopStep> accumulator,
		scoped in Grid grid,
		Digit d1,
		Digit d2,
		scoped in CellMap loop,
		scoped in CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne,
		Cell[] path
	)
	{
		var mask = (Mask)(grid[extraCellsMap] & ~comparer);
		if (!IsPow2(mask))
		{
			return null;
		}

		var extraDigit = TrailingZeroCount(mask);
		var elimMap = extraCellsMap % CandidatesMap[extraDigit];
		if (!elimMap)
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in loop)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(digit == extraDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + digit));
			}
		}

		var step = new UniqueLoopType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
			[[.. candidateOffsets, .. GetLoopLinks(path)]],
			d1,
			d2,
			loop,
			extraDigit
		);

		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <param name="path">The path of the loop.</param>
	/// <returns>The step is worth.</returns>
	private UniqueLoopType3Step? CheckType3(
		List<UniqueLoopStep> accumulator,
		scoped in Grid grid,
		Digit d1,
		Digit d2,
		scoped in CellMap loop,
		scoped in CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne,
		Cell[] path
	)
	{
		// Check whether the extra cells contain at least one digit of digit 1 and 2,
		// and extra digits not appeared in two digits mentioned above.
		var notSatisfiedType3 = false;
		foreach (var cell in extraCellsMap)
		{
			var mask = grid.GetCandidates(cell);
			if ((mask & comparer) == 0 || mask == comparer)
			{
				notSatisfiedType3 = true;
				break;
			}
		}
		if (notSatisfiedType3)
		{
			return null;
		}

		// Gather the union result of digits appeared, and check whether the result mask
		// contains both digit 1 and 2.
		var m = grid[extraCellsMap];
		if ((m & comparer) != comparer)
		{
			return null;
		}

		var otherDigitsMask = (Mask)(m & ~comparer);
		if (extraCellsMap.InOneHouse(out _))
		{
			if (extraCellsMap.Count != 2)
			{
				return null;
			}

			// All extra cells lie in a same house. This is the basic subtype of type 3.
			foreach (var houseIndex in extraCellsMap.CoveredHouses)
			{
				if ((ValuesMap[d1] || ValuesMap[d2]) && HousesMap[houseIndex])
				{
					continue;
				}

				var otherCells = (HousesMap[houseIndex] & EmptyCells) - loop;
				for (var size = PopCount((uint)otherDigitsMask) - 1; size < otherCells.Count; size++)
				{
					foreach (var cells in otherCells.GetSubsets(size))
					{
						var mask = grid[cells];
						if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						if ((HousesMap[houseIndex] & EmptyCells) - cells - loop is not (var elimMap and not []))
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (var digit in mask)
						{
							foreach (var cell in elimMap & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in loop)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(otherDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
										cell * 9 + digit
									)
								);
							}
						}
						foreach (var cell in cells)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
							}
						}

						var step = new UniqueLoopType3Step(
							[.. conclusions],
							[[.. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Normal, houseIndex), .. GetLoopLinks(path)]],
							d1,
							d2,
							loop,
							cells,
							mask
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

		{
			// Extra cells may not lie in a same house. However the type 3 can form in this case.
			var otherCells = extraCellsMap.PeerIntersection - loop & EmptyCells;
			if (!otherCells)
			{
				return null;
			}

			for (var size = PopCount((uint)otherDigitsMask) - 1; size < otherCells.Count; size++)
			{
				foreach (var cells in otherCells.GetSubsets(size))
				{
					var mask = grid[cells];
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((extraCellsMap | cells).PeerIntersection - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (var cell in elimMap)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & otherDigitsMask))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in loop)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									(otherDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new UniqueLoopType3Step(
						[.. conclusions],
						[[.. candidateOffsets, .. GetLoopLinks(path)]],
						d1,
						d2,
						loop,
						cells,
						mask
					);
					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <param name="path">The path of the loop.</param>
	/// <returns>The step is worth.</returns>
	private UniqueLoopType4Step? CheckType4(
		List<UniqueLoopStep> accumulator,
		scoped in Grid grid,
		Digit d1,
		Digit d2,
		scoped in CellMap loop,
		scoped in CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne,
		Cell[] path
	)
	{
		if (extraCellsMap.Count != 2 || !extraCellsMap.InOneHouse(out _))
		{
			return null;
		}

		foreach (var houseIndex in extraCellsMap.CoveredHouses)
		{
			foreach (var (digit, otherDigit) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				var map = HousesMap[houseIndex] & CandidatesMap[digit];
				if (map != (HousesMap[houseIndex] & loop))
				{
					continue;
				}

				var first = extraCellsMap[0];
				var second = extraCellsMap[1];
				using scoped var conclusions = new ValueList<Conclusion>(2);
				if (CandidatesMap[otherDigit].Contains(first))
				{
					conclusions.Add(new(Elimination, first, otherDigit));
				}
				if (CandidatesMap[otherDigit].Contains(second))
				{
					conclusions.Add(new(Elimination, second, otherDigit));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in loop - extraCellsMap)
				{
					foreach (var d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d));
					}
				}
				foreach (var cell in extraCellsMap)
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
				}

				var step = new UniqueLoopType4Step(
					[.. conclusions],
					[[.. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Normal, houseIndex), .. GetLoopLinks(path)]],
					d1,
					d2,
					loop,
					new(first, second, digit)
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}


	/// <summary>
	/// To collect all possible unique loop patterns in the current grid.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="cell">The cell where the loop starts.</param>
	/// <param name="d1">The first digit to be used.</param>
	/// <param name="d2">The second digit to be used.</param>
	/// <param name="loopPath">The path of the loop. This is a temporary variable.</param>
	/// <param name="loopMap">
	/// A <see cref="CellMap"/> instance recording which cells are used for the current loop. This is a temporary variable.
	/// </param>
	/// <param name="result">The result.</param>
	/// <param name="extraDigits">The extra digits to be checked.</param>
	/// <param name="allowedEx">Indicates how many cells the current loop can exist, with extra digits stored.</param>
	/// <param name="lastHouseType">The last house type. This is a temporary variable.</param>
	private static void CollectUniqueLoops(
		scoped in Grid grid,
		Cell cell,
		Digit d1,
		Digit d2,
		scoped ValueList<Cell> loopPath,
		scoped ref CellMap loopMap,
		HashSet<Pattern> result,
		Mask extraDigits = Grid.MaxCandidatesMask,
		int allowedEx = 2,
		HouseType lastHouseType = unchecked((HouseType)(-1))
	)
	{
		loopPath.Add(cell);
		loopMap.Add(cell);

		foreach (var houseType in HouseTypes)
		{
			if (houseType == lastHouseType)
			{
				continue;
			}

			foreach (var next in HousesMap[cell.ToHouseIndex(houseType)] & EmptyCells)
			{
				if (loopPath[0] == next && loopPath.Count >= 6 && IsValidLoop(loopPath))
				{
					// Yeah. The loop is closed.
					result.Add(new(loopMap, [.. loopPath], (Mask)(1 << d1 | 1 << d2)));
				}
				else if (!loopMap.Contains(next))
				{
					var mask = grid.GetCandidates(next);
					if ((mask >> d1 & 1) != 0 && (mask >> d2 & 1) != 0)
					{
						extraDigits = (Mask)((Mask)(extraDigits | mask) & (Mask)~(1 << d1 | 1 << d2));

						var count = PopCount((uint)mask);

						// We can continue if:
						//   1. The cell has exactly the 2 values of the loop.
						//   2. The cell has one extra value, the same as all previous cells with an extra value (for type 2 only).
						//   3. The cell has extra values and the maximum number of cells with extra values 2 is not reached.
						if (count == 2 || IsPow2(extraDigits) || allowedEx != 0)
						{
							var newAllowedEx = count > 2 ? allowedEx - 1 : allowedEx;
							CollectUniqueLoops(grid, next, d1, d2, loopPath, ref loopMap, result, extraDigits, newAllowedEx, houseType);
						}
					}
				}
			}
		}

		loopPath.Remove();
		loopMap.Remove(cell);
	}


	/// <summary>
	/// Determine whether the specified loop is a valid unique loop or unique rectangle pattern.
	/// </summary>
	/// <param name="loopPath">The path of the loop.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <remarks>
	/// <para>
	/// This method uses a trick way (node-coloring) to check whether a loop is a unique loop.
	/// The parameter <paramref name="loopPath"/> holds a list of cells, which are nodes of the loop.
	/// Then we begin to colorize each node with 2 different colors <b><i>A</i></b> and <b><i>B</i></b>.
	/// The only point we should notice is that the color between 2 adjacent nodes should be different
	/// (i.e. one is colored with <b><i>A</i></b> and the other should be colored with <b><i>B</i></b>).
	/// If at least one pair of cells in a same house are colored with a same color, it won't be a valid unique loop.
	/// Those colors stands for the final filling digits. Therefore, "Two cells in a same house are colored same"
	/// means "Two cells in a same house are filled with a same digit", which disobeys the basic sudoku rule.
	/// </para>
	/// <para>
	/// This method won't check for whether the loop is a unique rectangle (of length 4). It means, a valid unique rectangle
	/// can also make this method return <see langword="true"/>.
	/// </para>
	/// </remarks>
	internal static bool IsValidLoop(scoped in ValueList<Cell> loopPath)
	{
		var (visitedOdd, visitedEven, isOdd) = (0, 0, false);
		foreach (var cell in loopPath)
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouseIndex(houseType);
				if (isOdd)
				{
					if ((visitedOdd >> house & 1) != 0)
					{
						return false;
					}

					visitedOdd |= 1 << house;
				}
				else
				{
					if ((visitedEven >> house & 1) != 0)
					{
						return false;
					}

					visitedEven |= 1 << house;
				}
			}

			isOdd = !isOdd;
		}

		return visitedEven == visitedOdd;
	}

	/// <summary>
	/// Gets all possible links of the loop.
	/// </summary>
	/// <param name="path">The loop, specified as its path.</param>
	/// <returns>A list of <see cref="LinkViewNode"/> instances.</returns>
	private static List<LinkViewNode> GetLoopLinks(Cell[] path)
	{
		const Digit digit = 4;
		var result = new List<LinkViewNode>();
		for (var i = 0; i < path.Length; i++)
		{
			result.Add(
				new(
					WellKnownColorIdentifier.Normal,
					new(digit, CellsMap[path[i]]),
					new(digit, CellsMap[path[i + 1 == path.Length ? 0 : i + 1]]),
					Inference.ConjugatePair
				)
			);
		}

		return result;
	}


	/// <summary>
	/// Represents for a data set that describes the complete information about a unique loop technique.
	/// </summary>
	/// <param name="Loop">Indicates the cells used in this whole unique loop.</param>
	/// <param name="Path">Indicates the detail path of the loop.</param>
	/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="Mask"/>.</param>
	private readonly record struct Pattern(scoped in CellMap Loop, Cell[] Path, Mask DigitsMask)
	{
		/// <inheritdoc/>
		public override int GetHashCode() => Loop.GetHashCode();

		/// <inheritdoc/>
		public bool Equals(Pattern other) => Loop == other.Loop;
	}
}
