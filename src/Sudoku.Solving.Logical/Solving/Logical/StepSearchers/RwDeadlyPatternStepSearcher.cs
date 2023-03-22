namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.OnlyForStandardSudoku)]
internal sealed partial class RwDeadlyPatternStepSearcher : IRwDeadlyPatternStepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// <para>
	/// This implementation is based on the theory mentioned from RW:
	/// </para>
	/// <para>
	/// If a group of all instances of N different digits in a chute is spread
	/// over max N+1 mini- rows/columns, then the group will contain at least one unavoidable set.
	/// </para>
	/// <para>
	/// This technique too complex to be proved, but I find this link to describe about this technique:
	/// <see href="http://forum.enjoysudoku.com/yet-another-crazy-uniqueness-technique-t5589.html"/>
	/// </para>
	/// </remarks>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var chuteIndex = 0; chuteIndex < 6; chuteIndex++)
		{
			var (chute, isRow, _) = Chutes[chuteIndex];
			var elimHouseType = isRow ? HouseType.Column : HouseType.Row;
			var emptyMinilineSpannedHousesCount = 0;
			var valueDigitsMask = (short)0;
			var isPassedPrechecking = true;
			var availableElimMap = CellMap.Empty;
			for (var i = (int)elimHouseType * 9; i < (int)elimHouseType * 9 + 9; i++)
			{
				var miniline = HousesMap[i] & chute;
				if (miniline - EmptyCells is not (var currentNonemptyCells and not []))
				{
					emptyMinilineSpannedHousesCount++;
					continue;
				}

				var nonemptyCells = currentNonemptyCells;
				foreach (var cell in currentNonemptyCells)
				{
					if (grid.GetStatus(cell) == CellStatus.Modifiable)
					{
						nonemptyCells.Remove(cell);
					}
				}

				switch (nonemptyCells.Count)
				{
					case 1:
					{
						isPassedPrechecking = false;
						goto CheckFlag;
					}
					case 2:
					{
						emptyMinilineSpannedHousesCount++;
						break;
					}
				}

				availableElimMap |= miniline & EmptyCells;
				valueDigitsMask |= grid.GetDigitsUnion(nonemptyCells);
			}

		CheckFlag:
			if (!isPassedPrechecking)
			{
				continue;
			}

			if (emptyMinilineSpannedHousesCount >= PopCount((uint)(Grid.MaxCandidatesMask & ~valueDigitsMask)) + 2)
			{
				continue;
			}

			var conclusions = new List<Conclusion>();
			foreach (var cell in availableElimMap)
			{
				foreach (var digit in valueDigitsMask)
				{
					if (CandidatesMap[digit].Contains(cell))
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var step = new RwDeadlyPatternStep(
				conclusions.ToArray(),
				new[] { View.Empty | new ChuteViewNode(DisplayColorKind.Normal, chuteIndex) },
				chute - EmptyCells,
				valueDigitsMask,
				chuteIndex
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}
}
