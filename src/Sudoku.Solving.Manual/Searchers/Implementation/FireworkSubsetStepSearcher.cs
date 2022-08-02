namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed partial class FireworkSubsetStepSearcher : IFireworkSubsetStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		foreach (var pattern in IFireworkSubsetStepSearcher.Patterns)
		{
			if ((EmptyCells & pattern.Map) != pattern.Map)
			{
				// At least one cell is not empty cell. Just skip this structure.
				continue;
			}

			// Checks for the number of kinds of digits in three cells.
			short digitsMask = grid.GetDigitsUnion(pattern.Map);
			if (PopCount((uint)digitsMask) < 3)
			{
				continue;
			}

			switch (pattern.Pivot)
			{
				case { } pivot when PopCount((uint)digitsMask) >= 3:
				{
					if (CheckTriple(accumulator, grid, onlyFindOne, pattern, digitsMask, pivot) is { } step)
					{
						return step;
					}

					break;
				}
				case null when PopCount((uint)digitsMask) >= 4:
				{
					if (CheckQuadruple(accumulator, grid, onlyFindOne, pattern) is { } step)
					{
						return step;
					}

					break;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Checks for firework triple steps.
	/// </summary>
	private Step? CheckTriple(
		ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in FireworkPattern pattern, short digitsMask, int pivot)
	{
		var nonPivotCells = pattern.Map - pivot;
		int cell1 = nonPivotCells[0], cell2 = nonPivotCells[1];
		short satisfiedDigitsMask = IFireworkSubsetStepSearcher.IsFirework(
			cell1,
			cell2,
			pivot,
			grid,
			out var house1CellsExcluded,
			out var house2CellsExcluded
		);
		if (satisfiedDigitsMask == 0)
		{
			// No possible digits found as a firework digit.
			return null;
		}

		foreach (int[] digits in digitsMask.GetAllSets().GetSubsets(3))
		{
			short currentDigitsMask = (short)(1 << digits[0] | 1 << digits[1] | 1 << digits[2]);
			if (satisfiedDigitsMask != currentDigitsMask)
			{
				continue;
			}

			// Firework Triple is found.
			int pivotCellBlock = pivot.ToHouseIndex(HouseType.Block);
			var pivotRowCells = HouseMaps[pivot.ToHouseIndex(HouseType.Row)];
			var pivotColumnCells = HouseMaps[pivot.ToHouseIndex(HouseType.Column)];

			// Now check eliminations.
			var conclusions = new List<Conclusion>(18);
			foreach (int digit in (short)(grid.GetCandidates(pivot) & ~currentDigitsMask))
			{
				conclusions.Add(new(ConclusionType.Elimination, pivot, digit));
			}
			foreach (int digit in (short)(grid.GetCandidates(cell1) & ~currentDigitsMask))
			{
				conclusions.Add(new(ConclusionType.Elimination, cell1, digit));
			}
			foreach (int digit in (short)(grid.GetCandidates(cell2) & ~currentDigitsMask))
			{
				conclusions.Add(new(ConclusionType.Elimination, cell2, digit));
			}
			foreach (int digit in currentDigitsMask)
			{
				var possibleBlockCells = HouseMaps[pivotCellBlock] & EmptyCells & CandidatesMap[digit];
				foreach (int cell in possibleBlockCells - pivotRowCells - pivotColumnCells)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, digit));
				}
			}
			if (conclusions.Count == 0)
			{
				// No eliminations found.
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (int digit in (short)(grid.GetCandidates(pivot) & currentDigitsMask))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, pivot * 9 + digit));
			}
			foreach (int digit in (short)(grid.GetCandidates(cell1) & currentDigitsMask))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell1 * 9 + digit));
			}
			foreach (int digit in (short)(grid.GetCandidates(cell2) & currentDigitsMask))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell2 * 9 + digit));
			}

			var cellOffsets = new List<CellViewNode>(12);
			foreach (int house1CellExcluded in house1CellsExcluded)
			{
				cellOffsets.Add(new(DisplayColorKind.Elimination, house1CellExcluded));
			}
			foreach (int house2CellExcluded in house2CellsExcluded)
			{
				cellOffsets.Add(new(DisplayColorKind.Elimination, house2CellExcluded));
			}

			var unknowns = new List<UnknownViewNode>(4);
			int house1 = (Cells.Empty + cell1 + pivot).CoveredLine;
			int house2 = (Cells.Empty + cell2 + pivot).CoveredLine;
			foreach (int cell in (HouseMaps[house1] & HouseMaps[pivotCellBlock] & EmptyCells) - pivot)
			{
				unknowns.Add(new(DisplayColorKind.Normal, cell, (Utf8Char)'y', currentDigitsMask));
			}
			foreach (int cell in (HouseMaps[house2] & HouseMaps[pivotCellBlock] & EmptyCells) - pivot)
			{
				unknowns.Add(new(DisplayColorKind.Normal, cell, (Utf8Char)'x', currentDigitsMask));
			}

			var step = new FireworkTripleStep(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(
					View.Empty | candidateOffsets,
					View.Empty
						| cellOffsets
						| unknowns
						| new UnknownViewNode[]
						{
							new(
								DisplayColorKind.Normal,
								pivot,
								(Utf8Char)'z',
								(short)(grid.GetCandidates(pivot) & currentDigitsMask)
							),
							new(
								DisplayColorKind.Normal,
								cell1,
								(Utf8Char)'x',
								(short)(grid.GetCandidates(cell1) & currentDigitsMask)
							),
							new(
								DisplayColorKind.Normal,
								cell2,
								(Utf8Char)'y',
								(short)(grid.GetCandidates(cell2) & currentDigitsMask)
							)
						}
				),
				pattern.Map,
				currentDigitsMask
			);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}

	/// <summary>
	/// Checks for firework quadruple steps.
	/// </summary>
	private Step? CheckQuadruple(
		ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in FireworkPattern pattern)
	{
		return null;
	}
}
