namespace Sudoku.Solving.Logical.Implementations.Searchers;

[StepSearcher]
internal sealed partial class FireworkStepSearcher : IFireworkStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var pattern in IFireworkStepSearcher.Patterns)
		{
			if ((EmptyCells & pattern.Map) != pattern.Map)
			{
				// At least one cell is not empty cell. Just skip this structure.
				continue;
			}

			// Checks for the number of kinds of digits in three cells.
			var digitsMask = grid.GetDigitsUnion(pattern.Map);
			if (PopCount((uint)digitsMask) < 3)
			{
				continue;
			}

			switch (pattern.Pivot)
			{
				case { } pivot when PopCount((uint)digitsMask) >= 3:
				{
					if (CheckPairType1(accumulator, grid, onlyFindOne, pattern, pivot) is { } stepPairType1)
					{
						return stepPairType1;
					}

					if (CheckTriple(accumulator, grid, onlyFindOne, pattern, digitsMask, pivot) is { } stepTriple)
					{
						return stepTriple;
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
	/// Checks for firework pair type 1 steps.
	/// </summary>
	private IStep? CheckPairType1(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in FireworkPattern pattern, int pivot)
	{
		var map = pattern.Map;
		var nonPivotCells = map - pivot;
		int cell1 = nonPivotCells[0], cell2 = nonPivotCells[1];
		var satisfiedDigitsMask = IFireworkStepSearcher.GetFireworkDigits(
			cell1,
			cell2,
			pivot,
			grid,
			out var house1CellsExcluded,
			out var house2CellsExcluded
		);
		if (PopCount((uint)satisfiedDigitsMask) < 2)
		{
			// No possible digits found as a firework digit.
			return null;
		}

		var elimCell = ((PeersMap[cell1] & PeersMap[cell2]) - pivot)[0];
		foreach (var digits in satisfiedDigitsMask.GetAllSets().GetSubsets(2))
		{
			var currentDigitsMask = (short)(1 << digits[0] | 1 << digits[1]);
			var cell1TheOtherLine = cell1.ToHouseIndex(
				(CellsMap[cell1] + pivot).CoveredLine.ToHouseType() == HouseType.Row
					? HouseType.Column
					: HouseType.Row
			);
			var cell2TheOtherLine = cell2.ToHouseIndex(
				(CellsMap[cell2] + pivot).CoveredLine.ToHouseType() == HouseType.Row
					? HouseType.Column
					: HouseType.Row
			);

			foreach (var extraCell1 in (HousesMap[cell1TheOtherLine] & EmptyCells) - cell1)
			{
				foreach (var extraCell2 in (HousesMap[cell2TheOtherLine] & EmptyCells) - cell2)
				{
					if (extraCell1 == extraCell2)
					{
						// Cannot be a same cell.
						continue;
					}

					if (grid.GetCandidates(extraCell1) != currentDigitsMask
						|| grid.GetCandidates(extraCell2) != currentDigitsMask)
					{
						continue;
					}

					// Firework pair type 1 found.
					var elimMap = PeersMap[extraCell1] & PeersMap[extraCell2] & EmptyCells;
					if (!elimMap)
					{
						// No elimination cells.
						continue;
					}

					var conclusions = new List<Conclusion>(2);
					if (CandidatesMap[digits[0]].Contains(elimCell))
					{
						conclusions.Add(new(Elimination, elimCell * 9 + digits[0]));
					}
					if (CandidatesMap[digits[1]].Contains(elimCell))
					{
						conclusions.Add(new(Elimination, elimCell * 9 + digits[1]));
					}
					if (conclusions.Count == 0)
					{
						// No eliminations found.
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>(10);
					foreach (var cell in map)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & currentDigitsMask))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}
					foreach (var digit in grid.GetCandidates(extraCell1))
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, extraCell1 * 9 + digit));
					}
					foreach (var digit in grid.GetCandidates(extraCell2))
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, extraCell2 * 9 + digit));
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (var cell in house1CellsExcluded)
					{
						cellOffsets.Add(new(DisplayColorKind.Elimination, cell));
					}
					foreach (var cell in house2CellsExcluded)
					{
						cellOffsets.Add(new(DisplayColorKind.Elimination, cell));
					}

					var step = new FireworkPairType1Step(
						conclusions.ToImmutableArray(),
						ImmutableArray.Create(
							View.Empty | candidateOffsets,
							View.Empty | candidateOffsets | cellOffsets
						),
						map,
						currentDigitsMask,
						extraCell1,
						extraCell2
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

	/// <summary>
	/// Checks for firework triple steps.
	/// </summary>
	private IStep? CheckTriple(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in FireworkPattern pattern, short digitsMask, int pivot)
	{
		var nonPivotCells = pattern.Map - pivot;
		int cell1 = nonPivotCells[0], cell2 = nonPivotCells[1];
		var satisfiedDigitsMask = IFireworkStepSearcher.GetFireworkDigits(
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

		foreach (var digits in digitsMask.GetAllSets().GetSubsets(3))
		{
			var currentDigitsMask = (short)(1 << digits[0] | 1 << digits[1] | 1 << digits[2]);
			if ((satisfiedDigitsMask & currentDigitsMask) != currentDigitsMask)
			{
				continue;
			}

			// Firework Triple is found.
			var pivotCellBlock = pivot.ToHouseIndex(HouseType.Block);
			var pivotRowCells = HousesMap[pivot.ToHouseIndex(HouseType.Row)];
			var pivotColumnCells = HousesMap[pivot.ToHouseIndex(HouseType.Column)];

			// Now check eliminations.
			var conclusions = new List<Conclusion>(18);
			foreach (var digit in (short)(grid.GetCandidates(pivot) & ~currentDigitsMask))
			{
				conclusions.Add(new(Elimination, pivot, digit));
			}
			foreach (var digit in (short)(grid.GetCandidates(cell1) & ~currentDigitsMask))
			{
				conclusions.Add(new(Elimination, cell1, digit));
			}
			foreach (var digit in (short)(grid.GetCandidates(cell2) & ~currentDigitsMask))
			{
				conclusions.Add(new(Elimination, cell2, digit));
			}
			foreach (var digit in currentDigitsMask)
			{
				var possibleBlockCells = HousesMap[pivotCellBlock] & EmptyCells & CandidatesMap[digit];
				foreach (var cell in possibleBlockCells - pivotRowCells - pivotColumnCells)
				{
					conclusions.Add(new(Elimination, cell, digit));
				}
			}
			if (conclusions.Count == 0)
			{
				// No eliminations found.
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var digit in (short)(grid.GetCandidates(pivot) & currentDigitsMask))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, pivot * 9 + digit));
			}
			foreach (var digit in (short)(grid.GetCandidates(cell1) & currentDigitsMask))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell1 * 9 + digit));
			}
			foreach (var digit in (short)(grid.GetCandidates(cell2) & currentDigitsMask))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell2 * 9 + digit));
			}

			var cellOffsets = new List<CellViewNode>(12);
			foreach (var house1CellExcluded in house1CellsExcluded)
			{
				cellOffsets.Add(new(DisplayColorKind.Elimination, house1CellExcluded));
			}
			foreach (var house2CellExcluded in house2CellsExcluded)
			{
				cellOffsets.Add(new(DisplayColorKind.Elimination, house2CellExcluded));
			}

			var unknowns = new List<UnknownViewNode>(4);
			var house1 = (CellsMap[cell1] + pivot).CoveredLine;
			var house2 = (CellsMap[cell2] + pivot).CoveredLine;
			foreach (var cell in (HousesMap[house1] & HousesMap[pivotCellBlock] & EmptyCells) - pivot)
			{
				unknowns.Add(new(DisplayColorKind.Normal, cell, (Utf8Char)'y', currentDigitsMask));
			}
			foreach (var cell in (HousesMap[house2] & HousesMap[pivotCellBlock] & EmptyCells) - pivot)
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
	private IStep? CheckQuadruple(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne, scoped in FireworkPattern pattern)
	{
		if (pattern is not { Map: [var c1, var c2, var c3, var c4] map })
		{
			return null;
		}

		var digitsMask = grid.GetDigitsUnion(map);
		if (PopCount((uint)digitsMask) < 4)
		{
			return null;
		}

		foreach (var digits in digitsMask.GetAllSets().GetSubsets(4))
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
					HousesMap[pivot1.ToHouseIndex(HouseType.Row)]
						| HousesMap[pivot1.ToHouseIndex(HouseType.Column)]
				);
				var nonPivot2Cells = (map - pivot2) & (
					HousesMap[pivot2.ToHouseIndex(HouseType.Row)]
						| HousesMap[pivot2.ToHouseIndex(HouseType.Column)]
				);
				int cell1Pivot1 = nonPivot1Cells[0], cell2Pivot1 = nonPivot1Cells[1];
				int cell1Pivot2 = nonPivot2Cells[0], cell2Pivot2 = nonPivot2Cells[1];

				foreach (var ((d1, d2), (d3, d4)) in cases)
				{
					var pair1DigitsMask = (short)(1 << d1 | 1 << d2);
					var pair2DigitsMask = (short)(1 << d3 | 1 << d4);
					var satisfiedDigitsMaskPivot1 = IFireworkStepSearcher.GetFireworkDigits(
						cell1Pivot1,
						cell2Pivot1,
						pivot1,
						grid,
						out var house1CellsExcludedPivot1,
						out var house2CellsExcludedPivot1
					);
					var satisfiedDigitsMaskPivot2 = IFireworkStepSearcher.GetFireworkDigits(
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
					var fourDigitsMask = (short)(pair1DigitsMask | pair2DigitsMask);

					var conclusions = new List<Conclusion>(20);
					foreach (var digit in (short)(grid.GetCandidates(pivot1) & ~pair1DigitsMask))
					{
						conclusions.Add(new(Elimination, pivot1, digit));
					}
					foreach (var digit in (short)(grid.GetCandidates(pivot2) & ~pair2DigitsMask))
					{
						conclusions.Add(new(Elimination, pivot2, digit));
					}
					foreach (var cell in map - pivot1 - pivot2)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & ~fourDigitsMask))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					foreach (var cell in
						(
							HousesMap[pivot1.ToHouseIndex(HouseType.Block)]
								- HousesMap[pivot1.ToHouseIndex(HouseType.Row)]
								- HousesMap[pivot1.ToHouseIndex(HouseType.Column)]
						) & EmptyCells)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & pair1DigitsMask))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					foreach (var cell in
						(
							HousesMap[pivot2.ToHouseIndex(HouseType.Block)]
								- HousesMap[pivot2.ToHouseIndex(HouseType.Row)]
								- HousesMap[pivot2.ToHouseIndex(HouseType.Column)]
						) & EmptyCells)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & pair2DigitsMask))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						// No eliminations found.
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var digit in (short)(grid.GetCandidates(pivot1) & fourDigitsMask))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, pivot1 * 9 + digit));
					}
					foreach (var digit in (short)(grid.GetCandidates(pivot2) & fourDigitsMask))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, pivot2 * 9 + digit));
					}
					foreach (var cell in map - pivot1 - pivot2)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & fourDigitsMask))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var candidateOffsetsView2 = new List<CandidateViewNode>();
					foreach (var cell in map - pivot2)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & pair1DigitsMask))
						{
							candidateOffsetsView2.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}
					var candidateOffsetsView3 = new List<CandidateViewNode>();
					foreach (var cell in map - pivot1)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & pair2DigitsMask))
						{
							candidateOffsetsView3.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var cellOffsets1 = new List<CellViewNode>();
					foreach (var cell in house1CellsExcludedPivot1 | house2CellsExcludedPivot1)
					{
						cellOffsets1.Add(new(DisplayColorKind.Elimination, cell));
					}
					var cellOffsets2 = new List<CellViewNode>();
					foreach (var cell in house1CellsExcludedPivot2 | house2CellsExcludedPivot2)
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
