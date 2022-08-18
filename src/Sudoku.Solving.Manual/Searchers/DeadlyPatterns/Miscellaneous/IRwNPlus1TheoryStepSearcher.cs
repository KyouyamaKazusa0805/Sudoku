namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>RW's N + 1 Deadly Pattern Theory</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>RW's N + 1 Deadly Pattern Theory</item>
/// </list>
/// </summary>
public interface IRwNPlus1TheoryStepSearcher : IDeadlyPatternStepSearcher
{
}

[StepSearcher]
internal sealed partial class RwNPlus1TheoryStepSearcher : IRwNPlus1TheoryStepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <para><b>Developer Notes</b></para>
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
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int chuteIndex = 0; chuteIndex < 6; chuteIndex++)
		{
			var (chute, isRow, _) = Chutes[chuteIndex];
			var elimHouseType = isRow ? HouseType.Column : HouseType.Row;
			int valueDigitsSpannedHousesCount = 0;
			short valueDigitsMask = 0;
			bool isPassedPrechecking = true;
			var availableElimMap = Cells.Empty;
			for (int @base = (int)elimHouseType * 9, i = @base; i < @base + 9; i++)
			{
				var miniline = HouseMaps[i] & chute;
				if (miniline - EmptyCells is not (var currentNonemptyCells and not []))
				{
					continue;
				}

				var nonemptyCells = currentNonemptyCells;
				foreach (int cell in currentNonemptyCells)
				{
					if (grid.GetStatus(cell) == CellStatus.Modifiable)
					{
						nonemptyCells.Remove(cell);
					}
				}

				switch (nonemptyCells.Count)
				{
					case 0:
					{
						continue;
					}
					case 1:
					{
						isPassedPrechecking = false;
						goto CheckFlag;
					}
				}

				availableElimMap |= miniline & EmptyCells;
				valueDigitsMask |= grid.GetDigitsUnion(nonemptyCells);
				valueDigitsSpannedHousesCount++;
			}

		CheckFlag:
			if (!isPassedPrechecking)
			{
				continue;
			}

			if (PopCount((uint)valueDigitsMask) > valueDigitsSpannedHousesCount + 2)
			{
				continue;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in availableElimMap)
			{
				foreach (int digit in valueDigitsMask)
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

			var step = new RwNPlus1TheoryStep(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(View.Empty | new ChuteViewNode(DisplayColorKind.Normal, chuteIndex)),
				chute - EmptyCells,
				valueDigitsMask,
				chuteIndex
			);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}
}
