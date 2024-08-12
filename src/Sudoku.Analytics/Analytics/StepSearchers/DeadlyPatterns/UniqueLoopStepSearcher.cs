#define USE_BREADTH_FIRST_SEARCHING
#undef USE_DEPTH_FIRST_SEARCHING
#undef IGNORE_MULTIVALUE_CELL_CHECKING_LIMIT
#if USE_BREADTH_FIRST_SEARCHING && USE_DEPTH_FIRST_SEARCHING
#undef USE_DEPTH_FIRST_SEARCHING
#warning Both flags 'USE_BREADTH_FIRST_SEARCHING' and 'USE_DEPTH_FIRST_SEARCHING' are set. Only 'USE_BREADTH_FIRST_SEARCHING' will be used.
#elif !USE_BREADTH_FIRST_SEARCHING && !USE_DEPTH_FIRST_SEARCHING
#error Both flags 'USE_BREADTH_FIRST_SEARCHING' and 'USE_DEPTH_FIRST_SEARCHING' are not set. You must set one flag to enable its algorithm (BFS or DFS).
#endif
#if IGNORE_MULTIVALUE_CELL_CHECKING_LIMIT && (USE_BREADTH_FIRST_SEARCHING || !USE_DEPTH_FIRST_SEARCHING)
#warning Symbol 'IGNORE_MULTIVALUE_CELL_CHECKING_LIMIT' can only be available when symbol 'USE_DEPTH_FIRST_SEARCHING' is enabled.
#endif

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
	"StepSearcherName_UniqueLoopStepSearcher",
	Technique.UniqueLoopType1, Technique.UniqueLoopType2, Technique.UniqueLoopType3, Technique.UniqueLoopType4,
	SupportedSudokuTypes = SudokuType.Standard,
	RuntimeFlags = StepSearcherRuntimeFlags.TimeComplexity,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class UniqueLoopStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var typeCheckers = (TypeChecker[])[CheckType1, CheckType2, CheckType3, CheckType4];

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		ref readonly var grid = ref context.Grid;
		var tempAccumulator = new List<UniqueLoopStep>();
		foreach (ref readonly var pattern in FindLoops(in grid))
		{
			var (loop, path, comparer) = pattern;
			if ((loop & ~BivalueCells) is not (var extraCellsMap and not []))
			{
				// The current puzzle has multiple solutions.
				throw new PuzzleInvalidException(in grid, typeof(UniqueLoopStepSearcher));
			}

			var d1 = Mask.TrailingZeroCount(comparer);
			var d2 = comparer.GetNextSet(d1);
			foreach (var typeChecker in typeCheckers)
			{
				if (typeChecker(tempAccumulator, in grid, ref context, d1, d2, in loop, in extraCellsMap, comparer, context.OnlyFindOne, path) is { } step)
				{
					return step;
				}
			}
		}

		if (tempAccumulator.Count == 0)
		{
			return null;
		}

		var resultList = StepMarshal.RemoveDuplicateItems(tempAccumulator).ToList();
		StepMarshal.SortItems(resultList);

		if (context.OnlyFindOne)
		{
			return resultList[0];
		}

		context.Accumulator.AddRange(resultList);
		return null;
	}

	/// <summary>
	/// Try to find all possible loops appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <returns>A list of <see cref="UniqueLoop"/> instances.</returns>
#if USE_BREADTH_FIRST_SEARCHING
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	private ReadOnlySpan<UniqueLoop> FindLoops(ref readonly Grid grid)
	{
#if USE_BREADTH_FIRST_SEARCHING
		return bfs(in grid);
#else
		var patterns = new HashSet<UniqueLoop>();
		foreach (var cell in BivalueCells)
		{
			var mask = grid.GetCandidates(cell);
			var d1 = Mask.TrailingZeroCount(mask);
			var d2 = mask.GetNextSet(d1);

			var tempLoop = new List<Cell>(14);
			var loopMap = CellMap.Empty;
			dfs(in grid, cell, d1, d2, tempLoop, ref loopMap, patterns);
		}
		return patterns.ToArray();
#endif


#if USE_BREADTH_FIRST_SEARCHING && !USE_DEPTH_FIRST_SEARCHING
		static ReadOnlySpan<UniqueLoop> bfs(ref readonly Grid grid)
		{
			var result = new HashSet<UniqueLoop>();
			foreach (var cell in BivalueCells)
			{
				var queue = LinkedList.Singleton(LinkedList.Singleton(cell));
				var comparer = grid.GetCandidates(cell);
				var d1 = Mask.TrailingZeroCount(comparer);
				var d2 = comparer.GetNextSet(d1);
				var pairMap = CandidatesMap[d1] & CandidatesMap[d2];
				while (queue.Count != 0)
				{
					var currentBranch = queue.RemoveFirstNode();

					// The node should be appended after the last node, and its start node is the first node of the linked list.
					foreach (var currentCell in PeersMap[currentBranch.LastValue()] & pairMap)
					{
						// Determine whether the current cell iterated is the first node.
						// If so, check whether the loop is of length greater than 6, and validity of the loop.
						if (currentCell == cell && currentBranch.Count is 6 or 8 or 10 or 12 or 14 && UniqueLoop.IsValid(currentBranch))
						{
							result.Add(new(currentBranch.AsCellMap(), [.. currentBranch], comparer));
							break;
						}

						if (!currentBranch.Contains(currentCell))
						{
							// Create a new link with original value, and a new value at the last position.
							queue.AddLast(LinkedList.Create(currentBranch, currentCell));
						}
					}
				}
			}
			return result.ToArray();
		}
#endif

#if USE_DEPTH_FIRST_SEARCHING && !USE_BREADTH_FIRST_SEARCHING
		static void dfs(
			ref readonly Grid grid,
			Cell cell,
			Digit d1,
			Digit d2,
			List<Cell> loopPath,
			ref CellMap loopMap,
			HashSet<UniqueLoop> result,
			Mask extraDigitsMask = Grid.MaxCandidatesMask,
#if !IGNORE_MULTIVALUE_CELL_CHECKING_LIMIT
			int allowExtraDigitsCellsCount = 2,
#endif
			HouseType lastHouseType = (HouseType)byte.MaxValue
		)
		{
			loopPath.Add(cell);
			loopMap.Add(cell);

			var comparer = (Mask)(1 << d1 | 1 << d2);
			foreach (var houseType in HouseTypes)
			{
				if (houseType == lastHouseType)
				{
					continue;
				}

				foreach (var next in (HousesMap[cell.ToHouse(houseType)] & EmptyCells) - cell)
				{
					if (loopPath[0] == next && loopPath.Count >= 6 && UniqueLoop.IsValid(loopPath))
					{
						// Yeah. The loop is closed.
						result.Add(new(in loopMap, [.. loopPath], comparer));
					}
					else if (!loopMap.Contains(next))
					{
						var digitsMask = grid.GetCandidates(next);
						if ((digitsMask >> d1 & 1) != 0 && (digitsMask >> d2 & 1) != 0)
						{
							extraDigitsMask = (Mask)((extraDigitsMask | digitsMask) & ~comparer);

#if !IGNORE_MULTIVALUE_CELL_CHECKING_LIMIT
							// We can continue if:
							//   1) The cell has exactly the 2 values of the loop.
							//   2) Only for type 2:
							//      The cell has one extra value, the same as all previous cells with an extra value.
							//   3) The cell has extra values and the maximum number of cells with extra values 2 is not reached.
							var digitsCount = Mask.PopCount(digitsMask);
							if (digitsCount == 2 || allowExtraDigitsCellsCount != 0)
#endif
							{
								// Make recursion.
								dfs(
									in grid,
									next,
									d1,
									d2,
									loopPath,
									ref loopMap,
									result,
									extraDigitsMask,
#if !IGNORE_MULTIVALUE_CELL_CHECKING_LIMIT
									digitsCount == 2 || Mask.IsPow2((Mask)(digitsMask & ~comparer))
										? allowExtraDigitsCellsCount
										: allowExtraDigitsCellsCount - 1,
#endif
									houseType
								);
							}
						}
					}
				}
			}

			loopPath.RemoveAt(^1);
			loopMap.Remove(cell);
		}
#endif
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <inheritdoc cref="TypeChecker"/>
	private UniqueLoopType1Step? CheckType1(
		List<UniqueLoopStep> accumulator,
		ref readonly Grid grid,
		ref AnalysisContext context,
		Digit d1,
		Digit d2,
		ref readonly CellMap loop,
		ref readonly CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne,
		Cell[] path
	)
	{
		if (extraCellsMap is not [var extraCell])
		{
			return null;
		}

		var conclusions = new List<Conclusion>(2);
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
			candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + d1));
			candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + d2));
		}

		var step = new UniqueLoopType1Step(
			[.. conclusions],
			[[.. candidateOffsets, .. GetLoopLinks(path)]],
			context.Options,
			d1,
			d2,
			in loop,
			path
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
	/// Check type 2.
	/// </summary>
	/// <inheritdoc cref="TypeChecker"/>
	private UniqueLoopType2Step? CheckType2(
		List<UniqueLoopStep> accumulator,
		ref readonly Grid grid,
		ref AnalysisContext context,
		Digit d1,
		Digit d2,
		ref readonly CellMap loop,
		ref readonly CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne,
		Cell[] path
	)
	{
		var mask = (Mask)(grid[in extraCellsMap] & ~comparer);
		if (!Mask.IsPow2(mask))
		{
			return null;
		}

		var extraDigit = Mask.TrailingZeroCount(mask);
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
				candidateOffsets.Add(new(digit == extraDigit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, cell * 9 + digit));
			}
		}

		var step = new UniqueLoopType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
			[[.. candidateOffsets, .. GetLoopLinks(path)]],
			context.Options,
			d1,
			d2,
			in loop,
			extraDigit,
			path
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
	/// <inheritdoc cref="TypeChecker"/>
	private UniqueLoopType3Step? CheckType3(
		List<UniqueLoopStep> accumulator,
		ref readonly Grid grid,
		ref AnalysisContext context,
		Digit d1,
		Digit d2,
		ref readonly CellMap loop,
		ref readonly CellMap extraCellsMap,
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
		var m = grid[in extraCellsMap];
		if ((m & comparer) != comparer)
		{
			return null;
		}

		CellMap otherCells;
		var otherDigitsMask = (Mask)(m & ~comparer);
		if (extraCellsMap.InOneHouse(out _))
		{
			if (extraCellsMap.Count != 2)
			{
				return null;
			}

			// All extra cells lie in a same house. This is the basic subtype of type 3.
			foreach (var houseIndex in extraCellsMap.SharedHouses)
			{
				if ((ValuesMap[d1] || ValuesMap[d2]) && HousesMap[houseIndex])
				{
					continue;
				}

				otherCells = HousesMap[houseIndex] & EmptyCells & ~loop;
				for (var size = Mask.PopCount(otherDigitsMask) - 1; size < otherCells.Count; size++)
				{
					foreach (ref readonly var cells in otherCells & size)
					{
						var mask = grid[in cells];
						if (Mask.PopCount(mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						if ((HousesMap[houseIndex] & EmptyCells & ~cells & ~loop) is not (var elimMap and not []))
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
										(otherDigitsMask >> digit & 1) != 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
										cell * 9 + digit
									)
								);
							}
						}
						foreach (var cell in cells)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
							}
						}

						var step = new UniqueLoopType3Step(
							[.. conclusions],
							[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, houseIndex), .. GetLoopLinks(path)]],
							context.Options,
							d1,
							d2,
							in loop,
							in cells,
							mask,
							path
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

		// Extra cells may not lie in a same house. However the type 3 can form in this case.
		otherCells = extraCellsMap.PeerIntersection & ~loop & EmptyCells;
		if (!otherCells)
		{
			return null;
		}

		for (var size = Mask.PopCount(otherDigitsMask) - 1; size < otherCells.Count; size++)
		{
			foreach (ref readonly var cells in otherCells & size)
			{
				var mask = grid[in cells];
				if (Mask.PopCount(mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
				{
					continue;
				}

				if (((extraCellsMap | cells).PeerIntersection & ~loop) is not (var elimMap and not []))
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
								(otherDigitsMask >> digit & 1) != 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in cells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
					}
				}

				var step = new UniqueLoopType3Step(
					[.. conclusions],
					[[.. candidateOffsets, .. GetLoopLinks(path)]],
					context.Options,
					d1,
					d2,
					in loop,
					in cells,
					mask,
					path
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
	/// Check type 4.
	/// </summary>
	/// <inheritdoc cref="TypeChecker"/>
	private UniqueLoopType4Step? CheckType4(
		List<UniqueLoopStep> accumulator,
		ref readonly Grid grid,
		ref AnalysisContext context,
		Digit d1,
		Digit d2,
		ref readonly CellMap loop,
		ref readonly CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne,
		Cell[] path
	)
	{
		if (extraCellsMap.Count != 2 || !extraCellsMap.InOneHouse(out _))
		{
			return null;
		}

		foreach (var houseIndex in extraCellsMap.SharedHouses)
		{
			foreach (var (digit, otherDigit) in ((d1, d2), (d2, d1)))
			{
				var map = HousesMap[houseIndex] & CandidatesMap[digit];
				if (map != (HousesMap[houseIndex] & loop))
				{
					continue;
				}

				var first = extraCellsMap[0];
				var second = extraCellsMap[1];
				var conclusions = new List<Conclusion>(2);
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
				foreach (var cell in loop & ~extraCellsMap)
				{
					foreach (var d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + d));
					}
				}
				foreach (var cell in extraCellsMap)
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
				}

				var step = new UniqueLoopType4Step(
					[.. conclusions],
					[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, houseIndex), .. GetLoopLinks(path)]],
					context.Options,
					d1,
					d2,
					in loop,
					new(first, second, digit),
					path
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
	/// Gets all possible links of the loop.
	/// </summary>
	/// <param name="path">The loop, specified as its path.</param>
	/// <returns>A list of <see cref="CellLinkViewNode"/> instances.</returns>
	private static ReadOnlySpan<CellLinkViewNode> GetLoopLinks(Cell[] path)
	{
		var result = new List<CellLinkViewNode>();
		for (var i = 0; i < path.Length; i++)
		{
			result.Add(new(ColorIdentifier.Normal, path[i], path[i + 1 == path.Length ? 0 : i + 1]));
		}
		return result.AsReadOnlySpan();
	}
}

/// <summary>
/// The local delegate type that creates a <see cref="UniqueLoopStep"/> instance on checking subtypes.
/// </summary>
/// <param name="accumulator">The technique accumulator.</param>
/// <param name="grid">The grid to be checked.</param>
/// <param name="context">The context.</param>
/// <param name="d1">The digit 1.</param>
/// <param name="d2">The digit 2.</param>
/// <param name="loop">The loop.</param>
/// <param name="extraCellsMap">The extra cells map.</param>
/// <param name="comparer">The digits used by unique loop pattern.</param>
/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
/// <param name="path">The path of the loop.</param>
/// <returns>The first found step.</returns>
file delegate UniqueLoopStep? TypeChecker(
	List<UniqueLoopStep> accumulator,
	ref readonly Grid grid,
	ref AnalysisContext context,
	Digit d1,
	Digit d2,
	ref readonly CellMap loop,
	ref readonly CellMap extraCellsMap,
	Mask comparer,
	bool onlyFindOne,
	Cell[] path
);
