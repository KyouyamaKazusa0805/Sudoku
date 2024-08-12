namespace Sudoku.Analytics.StepSearchers;

public partial class UniqueLoopStepSearcher
{
	/// <summary>
	/// Check type 1.
	/// </summary>
	private static partial UniqueLoopStep? CheckType1(
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
	private static partial UniqueLoopStep? CheckType2(
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
	private static partial UniqueLoopStep? CheckType3(
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
	private static partial UniqueLoopStep? CheckType4(
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
}
