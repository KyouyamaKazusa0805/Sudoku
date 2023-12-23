namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Firework</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Firework Pair:
/// <list type="bullet">
/// <item>Firework Pair Type 1 (Single Firework + 2 Bi-value cells)</item>
/// <item>Firework Pair Type 2 (Double Fireworks)</item>
/// <item>Firework Pair Type 3 (Single Fireworks + Empty Rectangle)</item>
/// </list>
/// </item>
/// <item>Firework Triple</item>
/// <item>Firework Quadruple</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.FireworkPairType1, Technique.FireworkPairType2, Technique.FireworkPairType3,
	Technique.FireworkTriple, Technique.FireworkQuadruple, Flags = ConditionalFlags.TimeComplexity)]
[StepSearcherRuntimeName("StepSearcherName_FireworkStepSearcher")]
public sealed partial class FireworkStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the patterns used.
	/// </summary>
	private static readonly Pattern[] Patterns;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static FireworkStepSearcher()
	{
		var houses = (int[][])[
			[0, 1, 3, 4],
			[0, 2, 3, 5],
			[1, 2, 4, 5],
			[0, 1, 6, 7],
			[0, 2, 6, 8],
			[1, 2, 7, 8],
			[3, 4, 6, 7],
			[3, 5, 6, 8],
			[4, 5, 7, 8]
		];

		Patterns = new Pattern[3645];

		var i = 0;
		foreach (var houseQuad in houses)
		{
			// Gather triples.
			foreach (var triple in houseQuad.GetSubsets(3))
			{
				foreach (var a in HousesMap[triple[0]])
				{
					foreach (var b in HousesMap[triple[1]])
					{
						foreach (var c in HousesMap[triple[2]])
						{
							if ((CellsMap[a] + b).InOneHouse(out _) && (CellsMap[a] + c).InOneHouse(out _))
							{
								Patterns[i++] = new([a, b, c], a);
								continue;
							}

							if ((CellsMap[a] + b).InOneHouse(out _) && (CellsMap[b] + c).InOneHouse(out _))
							{
								Patterns[i++] = new([a, b, c], b);
								continue;
							}

							if ((CellsMap[a] + c).InOneHouse(out _) && (CellsMap[b] + c).InOneHouse(out _))
							{
								Patterns[i++] = new([a, b, c], c);
							}
						}
					}
				}
			}

			// Gather quadruples.
			foreach (var a in HousesMap[houseQuad[0]])
			{
				foreach (var b in HousesMap[houseQuad[1]])
				{
					foreach (var c in HousesMap[houseQuad[2]])
					{
						foreach (var d in HousesMap[houseQuad[3]])
						{
							if (!(CellsMap[a] + b).InOneHouse(out _) || !(CellsMap[a] + c).InOneHouse(out _)
								|| !(CellsMap[b] + d).InOneHouse(out _) || !(CellsMap[c] + d).InOneHouse(out _))
							{
								continue;
							}

							Patterns[i++] = new([a, b, c, d], null);
						}
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var pattern in Patterns)
		{
			if ((EmptyCells & pattern.Map) != pattern.Map)
			{
				// At least one cell is not empty cell. Just skip this pattern.
				continue;
			}

			// Checks for the number of kinds of digits in three cells.
			var digitsMask = grid[pattern.Map];
			if (PopCount((uint)digitsMask) < 3)
			{
				continue;
			}

			switch (pattern.Pivot)
			{
				case { } pivot when PopCount((uint)digitsMask) >= 3:
				{
#if false
					if (CheckPairType1(accumulator, in grid, ref context, onlyFindOne, pattern, pivot) is { } stepPairType1)
					{
						return stepPairType1;
					}
#endif

					if (CheckTriple(accumulator, in grid, ref context, onlyFindOne, in pattern, digitsMask, pivot) is { } stepTriple)
					{
						return stepTriple;
					}

					break;
				}
				case null when PopCount((uint)digitsMask) >= 4:
				{
					if (CheckQuadruple(accumulator, in grid, ref context, onlyFindOne, in pattern) is { } step)
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
	private FireworkPairType1Step? CheckPairType1(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		bool onlyFindOne,
		scoped ref readonly Pattern pattern,
		Cell pivot
	)
	{
		var map = pattern.Map;
		var nonPivotCells = map - pivot;
		var cell1 = nonPivotCells[0];
		var cell2 = nonPivotCells[1];
		var satisfiedDigitsMask = GetFireworkDigits(cell1, cell2, pivot, in grid, out var house1CellsExcluded, out var house2CellsExcluded);
		if (PopCount((uint)satisfiedDigitsMask) < 2)
		{
			// No possible digits found as a firework digit.
			return null;
		}

		var elimCell = ((PeersMap[cell1] & PeersMap[cell2]) - pivot)[0];
		foreach (var digits in satisfiedDigitsMask.GetAllSets().GetSubsets(2))
		{
			var currentDigitsMask = (Mask)(1 << digits[0] | 1 << digits[1]);
			var cell1TheOtherLine = cell1.ToHouseIndex((CellsMap[cell1] + pivot).CoveredLine.ToHouseType() == HouseType.Row ? HouseType.Column : HouseType.Row);
			var cell2TheOtherLine = cell2.ToHouseIndex((CellsMap[cell2] + pivot).CoveredLine.ToHouseType() == HouseType.Row ? HouseType.Column : HouseType.Row);

			foreach (var extraCell1 in (HousesMap[cell1TheOtherLine] & EmptyCells) - cell1)
			{
				foreach (var extraCell2 in (HousesMap[cell2TheOtherLine] & EmptyCells) - cell2 - extraCell1)
				{
					if (grid.GetCandidates(extraCell1) != currentDigitsMask || grid.GetCandidates(extraCell2) != currentDigitsMask)
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
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & currentDigitsMask))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}
					foreach (var digit in grid.GetCandidates(extraCell1))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, extraCell1 * 9 + digit));
					}
					foreach (var digit in grid.GetCandidates(extraCell2))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, extraCell2 * 9 + digit));
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (var cell in house1CellsExcluded)
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Elimination, cell));
					}
					foreach (var cell in house2CellsExcluded)
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Elimination, cell));
					}

					var step = new FireworkPairType1Step(
						[.. conclusions],
						[[.. candidateOffsets], [.. candidateOffsets, .. cellOffsets]],
						context.PredefinedOptions,
						in map,
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
	private FireworkTripleStep? CheckTriple(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		bool onlyFindOne,
		scoped ref readonly Pattern pattern,
		Mask digitsMask,
		Cell pivot
	)
	{
		var nonPivotCells = pattern.Map - pivot;
		var cell1 = nonPivotCells[0];
		var cell2 = nonPivotCells[1];
		var satisfiedDigitsMask = GetFireworkDigits(cell1, cell2, pivot, in grid, out var house1CellsExcluded, out var house2CellsExcluded);
		if (satisfiedDigitsMask == 0)
		{
			// No possible digits found as a firework digit.
			return null;
		}

		foreach (var digits in digitsMask.GetAllSets().GetSubsets(3))
		{
			var currentDigitsMask = (Mask)(1 << digits[0] | 1 << digits[1] | 1 << digits[2]);
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
			foreach (var digit in (Mask)(grid.GetCandidates(pivot) & ~currentDigitsMask))
			{
				conclusions.Add(new(Elimination, pivot, digit));
			}
			foreach (var digit in (Mask)(grid.GetCandidates(cell1) & ~currentDigitsMask))
			{
				conclusions.Add(new(Elimination, cell1, digit));
			}
			foreach (var digit in (Mask)(grid.GetCandidates(cell2) & ~currentDigitsMask))
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

			var step = new FireworkTripleStep(
				[.. conclusions],
				[
					[
						..
						from digit in (Mask)(grid.GetCandidates(pivot) & currentDigitsMask)
						select new CandidateViewNode(WellKnownColorIdentifier.Normal, pivot * 9 + digit),
						..
						from digit in (Mask)(grid.GetCandidates(cell1) & currentDigitsMask)
						select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell1 * 9 + digit),
						..
						from digit in (Mask)(grid.GetCandidates(cell2) & currentDigitsMask)
						select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell2 * 9 + digit)
					],
					[
						..
						from house1CellExcluded in house1CellsExcluded
						select new CellViewNode(WellKnownColorIdentifier.Elimination, house1CellExcluded),
						..
						from house2CellExcluded in house2CellsExcluded
						select new CellViewNode(WellKnownColorIdentifier.Elimination, house2CellExcluded),
						..
						from cell in (HousesMap[(CellsMap[cell1] + pivot).CoveredLine] & HousesMap[pivotCellBlock] & EmptyCells) - pivot
						select new BabaGroupViewNode(cell, (Utf8Char)'y', currentDigitsMask),
						..
						from cell in (HousesMap[(CellsMap[cell2] + pivot).CoveredLine] & HousesMap[pivotCellBlock] & EmptyCells) - pivot
						select new BabaGroupViewNode(cell, (Utf8Char)'x', currentDigitsMask),
						new BabaGroupViewNode(pivot, (Utf8Char)'z', (Mask)(grid.GetCandidates(pivot) & currentDigitsMask)),
						new BabaGroupViewNode(cell1, (Utf8Char)'x', (Mask)(grid.GetCandidates(cell1) & currentDigitsMask)),
						new BabaGroupViewNode(cell2, (Utf8Char)'y', (Mask)(grid.GetCandidates(cell2) & currentDigitsMask))
					]
				],
				context.PredefinedOptions,
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
	private FireworkQuadrupleStep? CheckQuadruple(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		bool onlyFindOne,
		scoped ref readonly Pattern pattern
	)
	{
		if (pattern is not { Map: [var c1, var c2, var c3, var c4] map })
		{
			return null;
		}

		var digitsMask = grid[in map];
		if (PopCount((uint)digitsMask) < 4)
		{
			return null;
		}

		foreach (var digits in digitsMask.GetAllSets().GetSubsets(4))
		{
			scoped var cases = (ReadOnlySpan<((Digit, Digit), (Digit, Digit))>)([
				((digits[0], digits[1]), (digits[2], digits[3])),
				((digits[0], digits[2]), (digits[1], digits[3])),
				((digits[0], digits[3]), (digits[1], digits[2])),
				((digits[1], digits[2]), (digits[0], digits[3])),
				((digits[1], digits[3]), (digits[0], digits[2])),
				((digits[2], digits[3]), (digits[0], digits[1]))
			]);

			foreach (var (pivot1, pivot2) in ((c1, c4), (c2, c3)))
			{
				var nonPivot1Cells = (map - pivot1) & (HousesMap[pivot1.ToHouseIndex(HouseType.Row)] | HousesMap[pivot1.ToHouseIndex(HouseType.Column)]);
				var nonPivot2Cells = (map - pivot2) & (HousesMap[pivot2.ToHouseIndex(HouseType.Row)] | HousesMap[pivot2.ToHouseIndex(HouseType.Column)]);
				var cell1Pivot1 = nonPivot1Cells[0];
				var cell2Pivot1 = nonPivot1Cells[1];
				var cell1Pivot2 = nonPivot2Cells[0];
				var cell2Pivot2 = nonPivot2Cells[1];
				foreach (var ((d1, d2), (d3, d4)) in cases)
				{
					var pair1DigitsMask = (Mask)(1 << d1 | 1 << d2);
					var pair2DigitsMask = (Mask)(1 << d3 | 1 << d4);
					var satisfiedDigitsMaskPivot1 = GetFireworkDigits(
						cell1Pivot1, cell2Pivot1, pivot1, in grid, out var house1CellsExcludedPivot1, out var house2CellsExcludedPivot1
					);
					var satisfiedDigitsMaskPivot2 = GetFireworkDigits(
						cell1Pivot2, cell2Pivot2, pivot2, in grid, out var house1CellsExcludedPivot2, out var house2CellsExcludedPivot2
					);
					if ((satisfiedDigitsMaskPivot1 & pair1DigitsMask) != pair1DigitsMask
						|| (satisfiedDigitsMaskPivot2 & pair2DigitsMask) != pair2DigitsMask)
					{
						continue;
					}

					// Firework quadruple found.
					var fourDigitsMask = (Mask)(pair1DigitsMask | pair2DigitsMask);
					var conclusions = new List<Conclusion>(20);
					foreach (var digit in (Mask)(grid.GetCandidates(pivot1) & ~pair1DigitsMask))
					{
						conclusions.Add(new(Elimination, pivot1, digit));
					}
					foreach (var digit in (Mask)(grid.GetCandidates(pivot2) & ~pair2DigitsMask))
					{
						conclusions.Add(new(Elimination, pivot2, digit));
					}
					foreach (var cell in map - pivot1 - pivot2)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~fourDigitsMask))
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
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & pair1DigitsMask))
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
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & pair2DigitsMask))
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
					foreach (var digit in (Mask)(grid.GetCandidates(pivot1) & fourDigitsMask))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, pivot1 * 9 + digit));
					}
					foreach (var digit in (Mask)(grid.GetCandidates(pivot2) & fourDigitsMask))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, pivot2 * 9 + digit));
					}
					foreach (var cell in map - pivot1 - pivot2)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & fourDigitsMask))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}

					var candidateOffsetsView2 = new List<CandidateViewNode>();
					foreach (var cell in map - pivot2)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & pair1DigitsMask))
						{
							candidateOffsetsView2.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}
					var candidateOffsetsView3 = new List<CandidateViewNode>();
					foreach (var cell in map - pivot1)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & pair2DigitsMask))
						{
							candidateOffsetsView3.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}

					var cellOffsets1 = new List<CellViewNode>();
					foreach (var cell in house1CellsExcludedPivot1 | house2CellsExcludedPivot1)
					{
						cellOffsets1.Add(new(WellKnownColorIdentifier.Elimination, cell));
					}
					var cellOffsets2 = new List<CellViewNode>();
					foreach (var cell in house1CellsExcludedPivot2 | house2CellsExcludedPivot2)
					{
						cellOffsets2.Add(new(WellKnownColorIdentifier.Elimination, cell));
					}

					var step = new FireworkQuadrupleStep(
						[.. conclusions],
						[[.. candidateOffsets], [.. cellOffsets1, .. candidateOffsetsView2], [.. cellOffsets2, .. candidateOffsetsView3]],
						context.PredefinedOptions,
						in map,
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


	/// <summary>
	/// <para>Checks for all digits which the cells containing form a firework pattern.</para>
	/// <para>
	/// This method returns the digits that satisfied the condition. If none found,
	/// this method will return 0.
	/// </para>
	/// </summary>
	/// <param name="c1">The cell 1 used in this pattern.</param>
	/// <param name="c2">The cell 2 used in this pattern.</param>
	/// <param name="pivot">The pivot cell.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="house1CellsExcluded">
	/// The excluded cells that is out of the firework pattern in the <paramref name="c1"/>'s house.
	/// </param>
	/// <param name="house2CellsExcluded">
	/// The excluded cells that is out of the firework pattern in the <paramref name="c2"/>'s house.
	/// </param>
	/// <returns>All digits that satisfied the firework rule. If none found, 0.</returns>
	private static Mask GetFireworkDigits(
		Cell c1,
		Cell c2,
		Cell pivot,
		scoped ref readonly Grid grid,
		out CellMap house1CellsExcluded,
		out CellMap house2CellsExcluded
	)
	{
		var pivotCellBlock = pivot.ToHouseIndex(HouseType.Block);
		var excluded1 = HousesMap[(CellsMap[c1] + pivot).CoveredLine] - HousesMap[pivotCellBlock] - c1;
		var excluded2 = HousesMap[(CellsMap[c2] + pivot).CoveredLine] - HousesMap[pivotCellBlock] - c2;
		var finalMask = (Mask)0;
		foreach (var digit in grid[[c1, c2, pivot]])
		{
			if (isFireworkFor(digit, in excluded1, in grid) && isFireworkFor(digit, in excluded2, in grid))
			{
				finalMask |= (Mask)(1 << digit);
			}
		}

		(house1CellsExcluded, house2CellsExcluded) = (excluded1, excluded2);
		return finalMask;


		static bool isFireworkFor(Digit digit, scoped ref readonly CellMap houseCellsExcluded, scoped ref readonly Grid grid)
		{
			foreach (var cell in houseCellsExcluded)
			{
				switch (grid.GetDigit(cell))
				{
					case -1 when CandidatesMap[digit].Contains(cell):
					case var cellValue when cellValue == digit:
					{
						return false;
					}
				}
			}

			return true;
		}
	}


	/// <summary>
	/// Indicates a firework pattern. The pattern will be like:
	/// <code><![CDATA[
	/// .-------.-------.-------.
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// :-------+-------+-------:
	/// | . . . | B . . | . C . |
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// :-------+-------+-------:
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// | . . . | A . . | .(D). |
	/// '-------'-------'-------'
	/// ]]></code>
	/// </summary>
	/// <param name="Map">Indicates the full map of all cells used.</param>
	/// <param name="Pivot">The pivot cell. This property can be <see langword="null"/> if four cells are used.</param>
	private readonly record struct Pattern(scoped ref readonly CellMap Map, Cell? Pivot);
}
