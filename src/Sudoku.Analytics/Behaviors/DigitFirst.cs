namespace Sudoku.Behaviors;

/// <summary>
/// Represents a way to finish puzzle with digit-first rule.
/// </summary>
public sealed class DigitFirst : IBehaviorMetric
{
	/// <inheritdoc/>
	public static UserBehavior MeasurableBehavior => UserBehavior.DigitFirst;


	/// <inheritdoc/>
	public static ReadOnlySpan<int> GetDistanceArray(Collector collector, ref readonly Grid grid, out ReadOnlySpan<KeyValuePair<SingleStep, Grid>> steps)
	{
		var (playground, solution, result) = (grid, grid.GetSolutionGrid(), (List<int>)[]);
		var (tempSteps, lastStep) = ((List<KeyValuePair<SingleStep, Grid>>)[], default(SingleStep)!);
		while (!playground.IsSolved)
		{
			var (z, s) = findNearestStep(
				in grid,
				in playground,
				lastStep,
				from step in collector.Collect(in playground).Cast<Step, SingleStep>()
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
