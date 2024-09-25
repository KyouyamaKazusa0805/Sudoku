namespace Sudoku.Measuring;

/// <summary>
/// Represents a type that can calculate filling distance of a puzzle.
/// </summary>
public static class FillingDistance
{
	/// <summary>
	/// Try to return a list of integers representing the logical filling distance between two adjacent steps,
	/// by using block-first filling rule.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="steps">The steps and corresponding grid states.</param>
	/// <returns>A list of block distances.</returns>
	public static ReadOnlySpan<int> GetBlockDistanceArray(this Collector @this, ref readonly Grid grid, out ReadOnlySpan<KeyValuePair<SingleStep, Grid>> steps)
	{
		var (playground, solution, result) = (grid, grid.GetSolutionGrid(), (List<int>)[]);
		var (tempSteps, lastStep) = ((List<KeyValuePair<SingleStep, Grid>>)[], default(SingleStep)!);
		while (!playground.IsSolved)
		{
			var (z, s) = findNearestStep(
				in grid,
				in playground,
				lastStep,
				from step in @this.Collect(in playground).Cast<Step, SingleStep>()
				group step by step.Code into stepGroup
				orderby stepGroup.Key
				select stepGroup
			);
			result.Add(s);
			tempSteps.Add(KeyValuePair.Create(lastStep = z, playground));
			playground.Apply(new(Assignment, lastStep.Cell, solution.GetDigit(lastStep.Cell)));
		}

		steps = tempSteps.AsReadOnlySpan();
		return result.AsReadOnlySpan();


		(SingleStep Step, int Score) findNearestStep(
			ref readonly Grid grid,
			ref readonly Grid playground,
			SingleStep lastStep,
			SpanOrderedEnumerable<SpanGrouping<SingleStep, Technique>> stepGroups
		)
		{
			var (minScore, minStep) = (int.MaxValue, default(SingleStep)!);
			foreach (var step in stepGroups[0])
			{
				if (lastStep is { Cell: var lastCell, Digit: var lastDigit }
					&& step.Cell.ToHouse(HouseType.Block) == lastCell.ToHouse(HouseType.Block)
					&& lastDigit > step.Digit
					&& lastCell.ToHouse(HouseType.Block) is var block
					&& (HousesMap[block] & playground.EmptyCells).Count != 1)
				{
					// Special rule:
					// If a user has found a new step that will revert filling rule,
					// it will be allowed if and only if the step is a full house.
					continue;
				}

				var newScore = getScore(in grid, in playground, lastStep?.Cell ?? -1, step.Cell);
				if (newScore <= minScore)
				{
					minScore = newScore;
					minStep = step;
				}
			}

			// Last resort: If a 'minStep' is not found, it must be treated as a new loop to find a new step.
			if (minStep is null)
			{
				minScore = int.MaxValue;
				minStep = default!;
				foreach (var step in stepGroups[0])
				{
					var s = getBottomingScore(step, in playground);
					if (s <= minScore)
					{
						minScore = s;
						minStep = step;
					}
				}
				return (minStep, minScore);


				int getBottomingScore(SingleStep step, ref readonly Grid playground)
				{
					var lastDigit = solution.GetDigit(lastStep!.Cell);
					var currentDigit = solution.GetDigit(step.Cell);
					var currentCellBlock = step.Cell.ToHouse(HouseType.Block);
					var baseScore = 0;
					for (var (currentBlock, i) = (currentCellBlock, 0); i < 9; currentBlock = (currentBlock + 1) % 9, i++)
					{
						baseScore += (playground.EmptyCells & HousesMap[currentBlock]).Count;
					}

					var mask = (Mask)0;
					for (var d = 0; d < currentDigit; d++)
					{
						if (!!(playground.CandidatesMap[d] & HousesMap[currentCellBlock]))
						{
							mask |= (Mask)(1 << d);
						}
					}
					return baseScore + Mask.PopCount(mask);
				}
			}
			return (minStep, minScore);
		}

		static int getScore(ref readonly Grid grid, ref readonly Grid playground, Cell lastCell, Cell currentCell)
		{
			var solution = grid.GetSolutionGrid();

			// Create index table. e.g. If block 1 is missing digit 2, 5 and 7, we should make them in queue.
			var indexedList = new List<Cell>();
			var emptyCells = playground.EmptyCells;
			var startBlock = lastCell == -1 ? 0 : lastCell.ToHouse(HouseType.Block);
			for (var (block, i) = (startBlock, 0); i < 9; block = (block + 1) % 9, i++)
			{
				var emptyCellsInHouse = emptyCells & HousesMap[block];
				if (!emptyCellsInHouse)
				{
					continue;
				}

				foreach (var digit in playground[in emptyCellsInHouse])
				{
					foreach (var cell in emptyCellsInHouse)
					{
						if (solution.GetDigit(cell) == digit && !indexedList.Contains(cell))
						{
							indexedList.Add(cell);
							break;
						}
					}
				}
			}

			// If a step is from the middle of the digit, we should check whether the previous step uses a different digit,
			// and remove them to find the number of digits in the removed set.
			var mask = playground[emptyCells & HousesMap[startBlock]];
			if (lastCell != -1)
			{
				var maskCopied = mask;
				foreach (var digit in maskCopied)
				{
					if (digit > solution.GetDigit(lastCell))
					{
						mask &= (Mask)~(1 << digit);
					}
				}
			}

			if (lastCell != -1
				&& Mask.IsPow2((lastCell.AsCellMap() + currentCell).BlockMask)
				&& solution.GetDigit(lastCell) is var lastDigit
				&& solution.GetDigit(currentCell) is var currentDigit
				&& currentDigit < lastDigit
				&& (emptyCells & HousesMap[lastCell.ToHouse(HouseType.Block)]).Count == 1)
			{
				// Directly measure the distance, ignoring whether two digits are filled with having reverted or not.
				return 1;
			}

			var index = indexedList.FindIndex(cell => cell == currentCell);
			return lastCell == -1 ? index + 1 : index + 1 - Mask.PopCount(mask);
		}
	}

	/// <summary>
	/// Try to return a list of integers representing the logical filling distance between two adjacent steps,
	/// by using digit-first filling rule.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="steps">The steps and corresponding grid states.</param>
	/// <returns>A list of block distances.</returns>
	public static ReadOnlySpan<int> GetDigitDistanceArray(this Collector @this, ref readonly Grid grid, out ReadOnlySpan<KeyValuePair<SingleStep, Grid>> steps)
	{
		var (playground, solution, result) = (grid, grid.GetSolutionGrid(), (List<int>)[]);
		var (tempSteps, lastStep) = ((List<KeyValuePair<SingleStep, Grid>>)[], default(SingleStep)!);
		while (!playground.IsSolved)
		{
			var (z, s) = findNearestStep(
				in grid,
				in playground,
				lastStep,
				from step in @this.Collect(in playground).Cast<Step, SingleStep>()
				group step by step.Code into stepGroup
				orderby stepGroup.Key
				select stepGroup
			);
			result.Add(s);
			tempSteps.Add(KeyValuePair.Create(lastStep = z, playground));
			playground.Apply(new(Assignment, lastStep.Cell, solution.GetDigit(lastStep.Cell)));
		}

		steps = tempSteps.AsReadOnlySpan();
		return result.AsReadOnlySpan();


		static (SingleStep Step, int Score) findNearestStep(
			ref readonly Grid grid,
			ref readonly Grid playground,
			SingleStep lastStep,
			SpanOrderedEnumerable<SpanGrouping<SingleStep, Technique>> stepGroups
		)
		{
			var (minScore, minStep) = (int.MaxValue, default(SingleStep)!);
			foreach (var step in stepGroups[0])
			{
				var newScore = getScore(in grid, in playground, lastStep?.Cell ?? -1, step.Cell);
				if (newScore <= minScore)
				{
					minScore = newScore;
					minStep = step;
				}
			}
			return (minStep, minScore);
		}

		static int getScore(ref readonly Grid grid, ref readonly Grid playground, Cell lastCell, Cell currentCell)
		{
			var solution = grid.GetSolutionGrid();

			// Create index table. e.g. If block 1 is missing digit 2, 5 and 7, we should make them in queue.
			var indexedList = new List<Cell>();
			var emptyCells = playground.EmptyCells;
			var valuesMap = playground.ValuesMap;
			var startDigit = lastCell == -1 ? 0 : solution.GetDigit(lastCell);
			for (var (digit, i) = (startDigit, 0); i < 9; digit = (digit + 1) % 9, i++)
			{
				if (valuesMap[digit].Count == 9)
				{
					continue;
				}

				for (var block = 0; block < 9; block++)
				{
					if (!(valuesMap[digit] & HousesMap[block]))
					{
						foreach (var cell in emptyCells & HousesMap[block])
						{
							if (solution.GetDigit(cell) == digit)
							{
								indexedList.Add(cell);
								break;
							}
						}
					}
				}
			}

			// If a step is from the middle of the digit, we should check whether the previous step uses a different digit,
			// and remove them to find the number of digits in the removed set.
			var mask = Grid.MaxCandidatesMask;
			if (lastCell != -1)
			{
				var lastBlock = lastCell.ToHouse(HouseType.Block);
				for (var block = 0; block < 9; block++)
				{
					if (lastBlock < block || !!(valuesMap[startDigit] & HousesMap[block]))
					{
						mask &= (Mask)~(1 << block);
					}
				}
			}

			if (lastCell != -1
				&& solution.GetDigit(lastCell) is var lastDigit
				&& solution.GetDigit(currentCell) is var currentDigit
				&& lastCell.ToHouse(HouseType.Block) is var lastHouse
				&& currentCell.ToHouse(HouseType.Block) is var currentHouse
				&& currentDigit == lastDigit && currentHouse < lastHouse)
			{
				// Directly measure the distance, ignoring whether two digits are filled with having reverted or not.
				return 1;
			}

			var index = indexedList.FindIndex(cell => cell == currentCell);
			return lastCell == -1 ? index + 1 : index + 1 - Mask.PopCount(mask);
		}
	}
}
