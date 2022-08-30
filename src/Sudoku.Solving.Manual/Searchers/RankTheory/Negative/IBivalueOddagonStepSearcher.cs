namespace Sudoku.Solving.Manual.Searchers;

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
/// In practicing, type 1 and 4 do not exist. A bi-value oddagon type 1 is a remote pair
/// and a type 4 cannot be formed as a stable technique structure.
/// </para>
/// <para>
/// A remote pair is a XY-Chain that only uses two digits.
/// This technique will be resolved by <see cref="IAlternatingInferenceChainStepSearcher"/>.
/// </para>
/// </summary>
/// <seealso cref="IAlternatingInferenceChainStepSearcher"/>
public interface IBivalueOddagonStepSearcher : INegativeRankStepSearcher, ICellLinkingLoopStepSearcher
{
}

[StepSearcher]
internal sealed unsafe partial class BivalueOddagonStepSearcher : IBivalueOddagonStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (BivalueCells.Count < 4)
		{
			return null;
		}

		var resultAccumulator = new List<BivalueOddagonStep>();

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		IOrderedEnumerable<BivalueOddagonStep> resultList = null!;
		foreach (int cell in BivalueCells)
		{
			short mask = grid.GetCandidates(cell);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			short comparer = (short)(1 << d1 | 1 << d2);
			var foundData = ICellLinkingLoopStepSearcher.GatherBivalueOddagons(comparer);
			if (foundData.Length == 0)
			{
				continue;
			}

			foreach (var (currentLoop, digitsMask) in foundData)
			{
				var extraCellsMap = currentLoop - BivalueCells;
				switch (extraCellsMap.Count)
				{
					case 0:
					{
						throw new InvalidOperationException("The current grid has no solution.");
					}
					case not 1:
					{
						// Type 2, 3.
						// Here use default label to ensure the order of
						// the handling will be 1->2->3.
						if (CheckType2(resultAccumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer, onlyFindOne) is { } step2)
						{
							return step2;
						}

						if (extraCellsMap.Count == 2)
						{
							if (CheckType3(resultAccumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer, onlyFindOne) is { } step3)
							{
								return step3;
							}
						}

						break;
					}
				}
			}

			if (resultAccumulator.Count == 0)
			{
				continue;
			}

			resultList =
				from step in IDistinctableStep<BivalueOddagonStep>.Distinct(resultAccumulator)
				orderby step.Loop.Count, step.TechniqueCode
				select step;

			if (onlyFindOne)
			{
				return resultList.First();
			}

			accumulator.AddRange(resultList);
		}

		return null;
	}

	private IStep? CheckType2(
		ICollection<BivalueOddagonStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, short comparer, bool onlyFindOne)
	{
		short mask = (short)(grid.GetDigitsUnion(extraCellsMap) & ~comparer);
		if (!IsPow2(mask))
		{
			goto ReturnNull;
		}

		int extraDigit = TrailingZeroCount(mask);
		if (extraCellsMap % CandidatesMap[extraDigit] is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int cell in loop)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		var step = new BivalueOddagonType2Step(
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			loop,
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

	private IStep? CheckType3(
		ICollection<BivalueOddagonStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, short comparer, bool onlyFindOne)
	{
		bool notSatisfiedType3 = false;
		foreach (int cell in extraCellsMap)
		{
			short mask = grid.GetCandidates(cell);
			if ((mask & comparer) == 0 || mask == comparer)
			{
				notSatisfiedType3 = true;
				break;
			}
		}

		if (!extraCellsMap.InOneHouse || notSatisfiedType3)
		{
			goto ReturnNull;
		}

		short m = grid.GetDigitsUnion(extraCellsMap);
		if ((m & comparer) != comparer)
		{
			goto ReturnNull;
		}

		short otherDigitsMask = (short)(m & ~comparer);
		foreach (int house in extraCellsMap.CoveredHouses)
		{
			if ((ValuesMap[d1] | ValuesMap[d2]) & HousesMap[house])
			{
				goto ReturnNull;
			}

			var otherCells = (HousesMap[house] & EmptyCells) - loop;
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = otherCells.Count; size < count; size++)
			{
				foreach (var cells in otherCells & size)
				{
					short mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((HousesMap[house] & EmptyCells) - cells - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
					foreach (int digit in mask)
					{
						foreach (int cell in elimMap & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int cell in loop)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									(otherDigitsMask >> digit & 1) != 0
										? DisplayColorKind.Auxiliary1
										: DisplayColorKind.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new BivalueOddagonType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, house)
						),
						loop,
						d1,
						d2,
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

	ReturnNull:
		return null;
	}
}
