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
		short satisfiedDigitsMask = IFireworkStepSearcher.GetFireworkDigits(
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
			if ((satisfiedDigitsMask & currentDigitsMask) != currentDigitsMask)
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
		if (pattern is not { Map: [var c1, var c2, var c3, var c4] map })
		{
			return null;
		}

		short digitsMask = grid.GetDigitsUnion(map);
		if (PopCount((uint)digitsMask) < 4)
		{
			return null;
		}

		foreach (int[] digits in digitsMask.GetAllSets().GetSubsets(4))
		{
			var cases = (stackalloc[]
			{
				((digits[0], digits[1]), (digits[2], digits[3])),
				((digits[0], digits[2]), (digits[1], digits[3])),
				((digits[0], digits[3]), (digits[1], digits[2])),
				((digits[1], digits[2]), (digits[0], digits[3])),
				((digits[1], digits[3]), (digits[0], digits[2])),
				((digits[2], digits[3]), (digits[0], digits[1]))
			});

			foreach (var (pivot1, pivot2) in stackalloc[] { (c1, c4), (c2, c3) })
			{
				var nonPivot1Cells = (map - pivot1) & (
					HouseMaps[pivot1.ToHouseIndex(HouseType.Row)]
						| HouseMaps[pivot1.ToHouseIndex(HouseType.Column)]
				);
				var nonPivot2Cells = (map - pivot2) & (
					HouseMaps[pivot2.ToHouseIndex(HouseType.Row)]
						| HouseMaps[pivot2.ToHouseIndex(HouseType.Column)]
				);
				int cell1Pivot1 = nonPivot1Cells[0], cell2Pivot1 = nonPivot1Cells[1];
				int cell1Pivot2 = nonPivot2Cells[0], cell2Pivot2 = nonPivot2Cells[1];

				foreach (var ((d1, d2), (d3, d4)) in cases)
				{
					short pair1DigitsMask = (short)(1 << d1 | 1 << d2);
					short pair2DigitsMask = (short)(1 << d3 | 1 << d4);
					short satisfiedDigitsMaskPivot1 = IFireworkStepSearcher.GetFireworkDigits(
						cell1Pivot1,
						cell2Pivot1,
						pivot1,
						grid,
						out var house1CellsExcludedPivot1,
						out var house2CellsExcludedPivot1
					);
					short satisfiedDigitsMaskPivot2 = IFireworkStepSearcher.GetFireworkDigits(
						cell1Pivot2,
						cell2Pivot2,
						pivot2,
						grid,
						out var house1CellsExcludedPivot2,
						out var house2CellsExcludedPivot2
					);
					if ((satisfiedDigitsMaskPivot1 & pair1DigitsMask) != pair1DigitsMask
						|| (satisfiedDigitsMaskPivot2 & pair2DigitsMask) != pair2DigitsMask)
					{
						continue;
					}

					// Firework quadruple found.
					short fourDigitsMask = (short)(pair1DigitsMask | pair2DigitsMask);

					var conclusions = new List<Conclusion>(20);
					foreach (int digit in (short)(grid.GetCandidates(pivot1) & ~pair1DigitsMask))
					{
						conclusions.Add(new(ConclusionType.Elimination, pivot1, digit));
					}
					foreach (int digit in (short)(grid.GetCandidates(pivot2) & ~pair2DigitsMask))
					{
						conclusions.Add(new(ConclusionType.Elimination, pivot2, digit));
					}
					foreach (int cell in map - pivot1 - pivot2)
					{
						foreach (int digit in (short)(grid.GetCandidates(cell) & ~fourDigitsMask))
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					foreach (int cell in
						(
							HouseMaps[pivot1.ToHouseIndex(HouseType.Block)]
								- HouseMaps[pivot1.ToHouseIndex(HouseType.Row)]
								- HouseMaps[pivot1.ToHouseIndex(HouseType.Column)]
						) & EmptyCells)
					{
						foreach (int digit in (short)(grid.GetCandidates(cell) & pair1DigitsMask))
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					foreach (int cell in
						(
							HouseMaps[pivot2.ToHouseIndex(HouseType.Block)]
								- HouseMaps[pivot2.ToHouseIndex(HouseType.Row)]
								- HouseMaps[pivot2.ToHouseIndex(HouseType.Column)]
						) & EmptyCells)
					{
						foreach (int digit in (short)(grid.GetCandidates(cell) & pair2DigitsMask))
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
					foreach (int digit in (short)(grid.GetCandidates(pivot1) & fourDigitsMask))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, pivot1 * 9 + digit));
					}
					foreach (int digit in (short)(grid.GetCandidates(pivot2) & fourDigitsMask))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, pivot2 * 9 + digit));
					}
					foreach (int cell in map - pivot1 - pivot2)
					{
						foreach (int digit in (short)(grid.GetCandidates(cell) & fourDigitsMask))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var candidateOffsetsView2 = new List<CandidateViewNode>();
					foreach (int cell in map - pivot2)
					{
						foreach (int digit in (short)(grid.GetCandidates(cell) & pair1DigitsMask))
						{
							candidateOffsetsView2.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}
					var candidateOffsetsView3 = new List<CandidateViewNode>();
					foreach (int cell in map - pivot1)
					{
						foreach (int digit in (short)(grid.GetCandidates(cell) & pair2DigitsMask))
						{
							candidateOffsetsView3.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var cellOffsets1 = new List<CellViewNode>();
					foreach (int cell in house1CellsExcludedPivot1 | house2CellsExcludedPivot1)
					{
						cellOffsets1.Add(new(DisplayColorKind.Elimination, cell));
					}
					var cellOffsets2 = new List<CellViewNode>();
					foreach (int cell in house1CellsExcludedPivot2 | house2CellsExcludedPivot2)
					{
						cellOffsets2.Add(new(DisplayColorKind.Elimination, cell));
					}

					var step = new FireworkQuadrupleStep(
						conclusions.ToImmutableArray(),
						ImmutableArray.Create(
							View.Empty | candidateOffsets,
							View.Empty
								| cellOffsets1
								| candidateOffsetsView2,
							View.Empty
								| cellOffsets2
								| candidateOffsetsView3
						),
						map,
						fourDigitsMask
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
}
