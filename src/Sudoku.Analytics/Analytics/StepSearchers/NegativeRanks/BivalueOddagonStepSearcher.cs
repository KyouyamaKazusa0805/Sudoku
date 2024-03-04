namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// <para>
/// Provides with a <b>Bi-value Oddagon</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Bi-value Oddagon Type 2</item>
/// <item>Bi-value Oddagon Type 3</item>
/// </list>
/// </para>
/// <para>
/// <i>
/// In practicing, type 1 and 4 do not exist. A bi-value oddagon type 1 is a remote pair
/// and a type 4 cannot be formed as a stable technique pattern.
/// </i>
/// </para>
/// <para><i>A Remote Pair is a XY-Chain that only uses two digits.</i></para>
/// </summary>
[StepSearcher(Technique.BivalueOddagonType2, Technique.BivalueOddagonType3)]
[StepSearcherFlags(StepSearcherFlags.TimeComplexity)]
[StepSearcherRuntimeName("StepSearcherName_BivalueOddagonStepSearcher")]
public sealed partial class BivalueOddagonStepSearcher : StepSearcher
{
	/// <summary>
	/// The maximum number of loops can be searched for in code.
	/// </summary>
	private const int MaximumCount = 100;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (BivalueCells.Count < 4)
		{
			return null;
		}

		var resultAccumulator = new List<BivalueOddagonStep>();

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		scoped ref readonly var grid = ref context.Grid;
		var onlyFindOne = context.OnlyFindOne;
		if (collect(in grid) is not { Count: not 0 } oddagonInfoList)
		{
			return null;
		}

		foreach (var (currentLoop, extraCells, comparer) in oddagonInfoList)
		{
			var d1 = TrailingZeroCount(comparer);
			var d2 = comparer.GetNextSet(d1);
			switch (extraCells.Count)
			{
				case 0:
				{
					// This puzzle has no puzzle solutions.
					throw new PuzzleInvalidException(in grid, typeof(BivalueOddagonStepSearcher));
				}
				case not 1:
				{
					// Type 2, 3.
					// Here use default label to ensure the order of the handling will be 1->2->3.
					if (CheckType2(resultAccumulator, in grid, ref context, d1, d2, in currentLoop, in extraCells, comparer, onlyFindOne) is { } step2)
					{
						return step2;
					}

					if (extraCells.Count == 2
						&& CheckType3(resultAccumulator, in grid, ref context, d1, d2, in currentLoop, in extraCells, comparer, onlyFindOne) is { } step3)
					{
						return step3;
					}

					break;
				}
			}
		}

		if (resultAccumulator.Count == 0)
		{
			return null;
		}

		var resultList = Step.RemoveDuplicateItems(resultAccumulator).ToList();
		Step.SortItems(resultList);
		if (context.OnlyFindOne)
		{
			return resultList.First();
		}

		context.Accumulator.AddRange(resultList);
		return null;


		static HashSet<BivalueOddagonInfo> collect(scoped ref readonly Grid grid)
		{
			var (foundLoopsCount, result) = (-1, new HashSet<BivalueOddagonInfo>(MaximumCount));
			for (var d1 = 0; d1 < 8; d1++)
			{
				for (var d2 = d1 + 1; d2 < 9; d2++)
				{
					var comparer = (Mask)(1 << d1 | 1 << d2);
					var cellsContainingBothTwoDigits = CandidatesMap[d1] & CandidatesMap[d2];
					foreach (var cell in cellsContainingBothTwoDigits)
					{
						dfs(
							in grid,
							cell,
							cell,
							-1,
							in cellsContainingBothTwoDigits,
							[cell],
							PopCount((uint)grid.GetCandidates(cell)) > 2 ? [cell] : [],
							result,
							ref foundLoopsCount,
							comparer,
							0
						);
					}
				}
			}

			return result;
		}

		static void dfs(
			scoped ref readonly Grid grid,
			Cell startCell,
			Cell previousCell,
			House previousHouse,
			scoped ref readonly CellMap cellsContainingBothDigits,
			scoped ref readonly CellMap loop,
			scoped ref readonly CellMap extraCells,
			HashSet<BivalueOddagonInfo> result,
			scoped ref int loopsCount,
			Mask comparer,
			Mask extraDigitsMask
		)
		{
			if (loopsCount > 100)
			{
				// There is no need to iterate more loops because they are same.
				return;
			}

			scoped var h = (stackalloc House[3]);
			foreach (var houseType in HouseTypes)
			{
				var nextHouse = previousCell.ToHouseIndex(houseType);
				if (nextHouse == previousHouse)
				{
					continue;
				}

				var loopCellsInThisHouse = loop & HousesMap[nextHouse];
				if (loopCellsInThisHouse.Count >= 2 && !loopCellsInThisHouse.Contains(startCell))
				{
					continue;
				}

				var otherCellsCanBeIterated = cellsContainingBothDigits - loop + startCell & HousesMap[nextHouse];
				if (!otherCellsCanBeIterated)
				{
					continue;
				}

				foreach (var cell in otherCellsCanBeIterated)
				{
					cell.CopyHouseInfo(ref h[0]);

					if (((loop & HousesMap[h[0]]).Count >= 2 || (loop & HousesMap[h[1]]).Count >= 2 || (loop & HousesMap[h[2]]).Count >= 2)
						&& startCell != cell)
					{
						// All valid loops can only at most 2 cells from all houses that the current loop uses.
						continue;
					}

					if (startCell == cell)
					{
						if ((loop.Count & 1) != 0 && loop.Count > 4)
						{
							// The pattern is found.
							if (++loopsCount < MaximumCount)
							{
								result.Add(new(in loop, in extraCells, comparer));
							}

							return;
						}
					}
					else
					{
						var newExtraDigitsMask = (Mask)(extraDigitsMask | (Mask)(grid.GetCandidates(cell) & ~comparer));
						var newExtraCells = PopCount((uint)grid.GetCandidates(cell)) > 2 ? extraCells + cell : extraCells;
						if (newExtraCells.InOneHouse(out _)
							|| IsPow2(newExtraDigitsMask) && !!(newExtraCells.PeerIntersection & CandidatesMap[Log2((uint)newExtraDigitsMask)])
							|| newExtraCells.Count < 3)
						{
							dfs(
								in grid,
								startCell,
								cell,
								nextHouse,
								in cellsContainingBothDigits,
								loop + cell,
								in newExtraCells,
								result,
								ref loopsCount,
								comparer,
								newExtraDigitsMask
							);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Check for type 2.
	/// </summary>
	private BivalueOddagonType2Step? CheckType2(
		List<BivalueOddagonStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Digit d1,
		Digit d2,
		scoped ref readonly CellMap loop,
		scoped ref readonly CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne
	)
	{
		var mask = (Mask)(grid[in extraCellsMap] & ~comparer);
		if (!IsPow2(mask))
		{
			goto ReturnNull;
		}

		var extraDigit = TrailingZeroCount(mask);
		if (extraCellsMap % CandidatesMap[extraDigit] is not (var elimMap and not []))
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

		var step = new BivalueOddagonType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
			[[.. candidateOffsets]],
			context.PredefinedOptions,
			in loop,
			d1,
			d2,
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
	/// Check for type 3.
	/// </summary>
	private BivalueOddagonType3Step? CheckType3(
		List<BivalueOddagonStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Digit d1,
		Digit d2,
		scoped ref readonly CellMap loop,
		scoped ref readonly CellMap extraCellsMap,
		Mask comparer,
		bool onlyFindOne
	)
	{
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

		if (!extraCellsMap.InOneHouse(out _) || notSatisfiedType3)
		{
			goto ReturnNull;
		}

		var m = grid[in extraCellsMap];
		if ((m & comparer) != comparer)
		{
			goto ReturnNull;
		}

		var otherDigitsMask = (Mask)(m & ~comparer);
		foreach (var house in extraCellsMap.SharedHouses)
		{
			if ((ValuesMap[d1] | ValuesMap[d2]) & HousesMap[house])
			{
				goto ReturnNull;
			}

			var otherCells = (HousesMap[house] & EmptyCells) - loop;
			for (var size = PopCount((uint)otherDigitsMask) - 1; size < otherCells.Count; size++)
			{
				foreach (ref readonly var cells in otherCells.GetSubsets(size))
				{
					var mask = grid[in cells];
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((HousesMap[house] & EmptyCells) - cells - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
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

					var step = new BivalueOddagonType3Step(
						[.. conclusions],
						[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
						context.PredefinedOptions,
						in loop,
						d1,
						d2,
						in cells,
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

	ReturnNull:
		return null;
	}
}

/// <summary>
/// Defines a temporary type that records a pair of data for a bi-value oddagon.
/// </summary>
/// <param name="LoopCells">Indicates the cells of the whole loop.</param>
/// <param name="ExtraCells">Indicates the extra cells.</param>
/// <param name="DigitsMask">Indicates the mask of digits that the loop used.</param>
file sealed record BivalueOddagonInfo(scoped ref readonly CellMap LoopCells, scoped ref readonly CellMap ExtraCells, Mask DigitsMask)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] BivalueOddagonInfo? other) => other is not null && LoopCells == other.LoopCells;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => LoopCells.GetHashCode();
}
