namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Extended Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Rectangle Type 1</item>
/// <item>Extended Rectangle Type 2</item>
/// <item>Extended Rectangle Type 3</item>
/// <item>Extended Rectangle Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_ExtendedRectangleStepSearcher",
	Technique.ExtendedRectangleType1, Technique.ExtendedRectangleType2,
	Technique.ExtendedRectangleType3, Technique.ExtendedRectangleType3Cannibalism, Technique.ExtendedRectangleType4,
	SupportedSudokuTypes = SudokuType.Standard,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class ExtendedRectangleStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var (isFatType, patternCells, pairs, size) in ExtendedRectanglePattern.AllPatterns)
		{
			if ((EmptyCells & patternCells) != patternCells)
			{
				continue;
			}

			// Check each pair.
			// Ensures all pairs should contains same digits and the kind of digits must be greater than 2.
			var checkKindsFlag = true;
			foreach (var (l, r) in pairs)
			{
				var tempMask = (Mask)(grid.GetCandidates(l) & grid.GetCandidates(r));
				if (tempMask == 0 || Mask.IsPow2(tempMask))
				{
					checkKindsFlag = false;
					break;
				}
			}
			if (!checkKindsFlag)
			{
				// Failed to check.
				continue;
			}

			// Check the mask of cells from two houses.
			var (m1, m2) = ((Mask)0, (Mask)0);
			foreach (var (l, r) in pairs)
			{
				m1 |= grid.GetCandidates(l);
				m2 |= grid.GetCandidates(r);
			}

			var (resultMask, normalDigits, extraDigits) = ((Mask)(m1 | m2), (Mask)0, (Mask)0);
			foreach (var digit in resultMask)
			{
				var count = 0;
				foreach (var (l, r) in pairs)
				{
					if (((grid.GetCandidates(l) & grid.GetCandidates(r)) >> digit & 1) != 0)
					{
						// Both two cells contain same digit.
						count++;
					}
				}

				(count >= 2 ? ref normalDigits : ref extraDigits) |= (Mask)(1 << digit);
			}

			if (Mask.PopCount(normalDigits) != size)
			{
				// The number of normal digits are not enough.
				continue;
			}

			if (Mask.PopCount(resultMask) == size + 1)
			{
				// Possible type 1 or 2 found. Now check extra cells.
				var extraDigit = Mask.TrailingZeroCount(extraDigits);
				var extraCellsMap = patternCells & CandidatesMap[extraDigit];
				if (!extraCellsMap)
				{
					continue;
				}

				if (extraCellsMap.Count == 1)
				{
					if (CheckType1(accumulator, in grid, ref context, in patternCells, in extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step1)
					{
						return step1;
					}
				}

				if (CheckType2(accumulator, in grid, ref context, in patternCells, in extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step2)
				{
					return step2;
				}
			}
			else
			{
				var extraCellsMap = CellMap.Empty;
				foreach (var cell in patternCells)
				{
					foreach (var digit in extraDigits)
					{
						if (grid.GetExistence(cell, digit))
						{
							extraCellsMap.Add(cell);
							break;
						}
					}
				}

				if (extraCellsMap.FirstSharedHouse == 32)
				{
					continue;
				}

				if (CheckType3Naked(
					accumulator, in grid, ref context, in patternCells, normalDigits,
					extraDigits, in extraCellsMap, isFatType, onlyFindOne
				) is { } step3)
				{
					return step3;
				}

				if (CheckType14(accumulator, in grid, ref context, in patternCells, normalDigits, in extraCellsMap, onlyFindOne) is { } step14)
				{
					return step14;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="patternCells">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private ExtendedRectangleType1Step? CheckType1(
		List<Step> accumulator,
		ref readonly Grid grid,
		ref StepAnalysisContext context,
		ref readonly CellMap patternCells,
		ref readonly CellMap extraCells,
		Mask normalDigits,
		Digit extraDigit,
		bool onlyFindOne
	)
	{
		var (conclusions, candidateOffsets) = (new List<Conclusion>(), new List<CandidateViewNode>());
		foreach (var cell in patternCells)
		{
			if (cell == extraCells[0])
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					if (digit != extraDigit)
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
			}
			else
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
				}
			}
		}

		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var step = new ExtendedRectangleType1Step(
			conclusions.AsReadOnlyMemory(),
			[[.. candidateOffsets]],
			context.Options,
			in patternCells,
			normalDigits
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
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="patternCells">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private ExtendedRectangleType2Step? CheckType2(
		List<Step> accumulator,
		ref readonly Grid grid,
		ref StepAnalysisContext context,
		ref readonly CellMap patternCells,
		ref readonly CellMap extraCells,
		Mask normalDigits,
		Digit extraDigit,
		bool onlyFindOne
	)
	{
		if ((extraCells.PeerIntersection & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in patternCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(digit == extraDigit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, cell * 9 + digit));
			}
		}

		var step = new ExtendedRectangleType2Step(
			(from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)).ToArray(),
			[[.. candidateOffsets]],
			context.Options,
			in patternCells,
			normalDigits,
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

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="patternCells">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigits">The extra digits mask.</param>
	/// <param name="extraCells">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <param name="isFatType">Indicates whether the type is fat type.</param>
	/// <returns>The first found step if worth.</returns>
	private ExtendedRectangleType3Step? CheckType3Naked(
		List<Step> accumulator,
		ref readonly Grid grid,
		ref StepAnalysisContext context,
		ref readonly CellMap patternCells,
		Mask normalDigits,
		Mask extraDigits,
		ref readonly CellMap extraCells,
		bool isFatType,
		bool onlyFindOne
	)
	{
		// Test examples:
		// Cannibalism
		// .3.1....+21...5...9..4..8..+1+3+15.2.8.+6+46.+8+1..+232..36..1.7.+1....6..9+3..12...+42.7.+1.8:611 913 517 418 428 631 537 338 467 569 576 477 577 581 588 596 996 598
		// +53..4+9.6.84.+6.+7.95...2.5..+4..43.85..3..+5.+4..9.+589.14+3.+4+2.7.3.+5.78+5+4.+6.23.6+3.5+2.4.:113 117 133 135 137 142 153 253 157 177 179 197 897

		foreach (var isCannibalism in (false, true))
		{
			// Iterate on each shared house.
			foreach (var house in extraCells.SharedHouses)
			{
				// For cannibalism mode, check whether the side is the in the direction of the pattern lying.
				var patternCellsCoveredInThisHouse = HousesMap[house] & patternCells;
				if (isCannibalism && patternCellsCoveredInThisHouse.Count <= 2)
				{
					continue;
				}

				// Find all possible cells that are out of relation with the extended rectangle pattern.
				var otherCells = HousesMap[house] & EmptyCells & ~patternCells;

				// Iterate on size of the pattern.
				// Please note that the cannibalism mode may use all empty cells recorded in variable 'otherCells'.
				for (var size = 1; size < (isCannibalism ? otherCells.Count + 1 : otherCells.Count); size++)
				{
					// Iterate on each combination of the pattern.
					foreach (ref readonly var cells in otherCells & size)
					{
						var mask = grid[in cells];
						if ((mask & extraDigits) != extraDigits || Mask.PopCount(mask) != size + 1)
						{
							// The extra cells must contain all possible digits appeared in extended rectangle pattern.
							continue;
						}

						if (!isCannibalism) // Non-cannibalism check.
						{
							// Now a step is formed. Check for elimination.
							var elimMap = HousesMap[house] & EmptyCells & ~patternCells & ~cells;
							if (!elimMap)
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

							g(in patternCells, in cells, in extraCells, in grid, mask, out var candidateOffsets);

							var step = new ExtendedRectangleType3Step(
								conclusions.AsReadOnlyMemory(),
								[[.. candidateOffsets, new HouseViewNode(0, house)]],
								context.Options,
								in patternCells,
								normalDigits,
								in cells,
								mask,
								house,
								isCannibalism
							);
							if (onlyFindOne)
							{
								return step;
							}

							accumulator.Add(step);
						}
						else if (isFatType) // Cannibalism check.
						{
							// Because we cannot fill with any digits elsewhere the empty cells in the house,
							// the intersection digit must be appeared in 'otherCells' instead of the extended rectangle pattern.
							// Therefore, the pattern forms as a cannibalism.
							//
							// We should check whether the size of the pattern. The extra digits appeared in the pattern
							// must cover (n - 1) cells, where 'n' is the length of the cells covered in this shared house.
							var digitsToCheck = (Mask)(mask & ~normalDigits);
							var finalCellsContainingExtraDigits = patternCellsCoveredInThisHouse;
							foreach (var cell in patternCellsCoveredInThisHouse)
							{
								if ((grid.GetCandidates(cell) & digitsToCheck) == 0)
								{
									// No extra cells found.
									finalCellsContainingExtraDigits.Remove(cell);
								}
							}
							if (finalCellsContainingExtraDigits.Count != patternCellsCoveredInThisHouse.Count - 1)
							{
								continue;
							}

							var intersectedDigitsMask = (Mask)(mask & normalDigits);
							if (!Mask.IsPow2(intersectedDigitsMask))
							{
								continue;
							}

							// This digit will be cannibalism. Checks for elimination.
							var intersectedDigit = Mask.Log2(intersectedDigitsMask);
							var elimMap = patternCellsCoveredInThisHouse & CandidatesMap[intersectedDigit];
							if (!elimMap)
							{
								continue;
							}

							g(in patternCells, in cells, in extraCells, in grid, mask, out var candidateOffsets);

							var step = new ExtendedRectangleType3Step(
								(from cell in elimMap select new Conclusion(Elimination, cell * 9 + intersectedDigit)).ToArray(),
								[[.. candidateOffsets, new HouseViewNode(0, house)]],
								context.Options,
								in patternCells,
								normalDigits,
								in cells,
								mask,
								house,
								isCannibalism
							);
							if (onlyFindOne)
							{
								return step;
							}

							accumulator.Add(step);
						}
					}
				}
			}
		}

		return null;


		static void g(
			ref readonly CellMap patternCells,
			ref readonly CellMap cells,
			ref readonly CellMap extraCells,
			ref readonly Grid grid,
			Mask mask,
			out List<CandidateViewNode> candidateOffsets
		)
		{
			candidateOffsets = [];
			foreach (var cell in patternCells & ~extraCells)
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
				}
			}
			foreach (var cell in extraCells)
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(
						new(
							(mask >> digit & 1) != 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
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
		}
	}

	/// <summary>
	/// Check type 4 and some type 1 patterns cannot be found.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="patternCells">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraCells">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private Step? CheckType14(
		List<Step> accumulator,
		ref readonly Grid grid,
		ref StepAnalysisContext context,
		ref readonly CellMap patternCells,
		Mask normalDigits,
		ref readonly CellMap extraCells,
		bool onlyFindOne
	)
	{
		switch (extraCells)
		{
			case [var extraCell]:
			{
				// Type 1 found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (var digit in normalDigits)
				{
					if (CandidatesMap[digit].Contains(extraCell))
					{
						conclusions.Add(new(Elimination, extraCell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					goto ReturnNull;
				}

				// Gather all highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in patternCells)
				{
					if (cell == extraCell)
					{
						continue;
					}

					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}
				}

				var step = new ExtendedRectangleType1Step(
					conclusions.AsReadOnlyMemory(),
					[[.. candidateOffsets]],
					context.Options,
					in patternCells,
					normalDigits
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);

				break;
			}
			case [var extraCell1, var extraCell2]:
			{
				// Type 4.
				var m1 = grid.GetCandidates(extraCell1);
				var m2 = grid.GetCandidates(extraCell2);
				var conjugateMask = (Mask)(m1 & m2 & normalDigits);
				if (conjugateMask == 0)
				{
					goto ReturnNull;
				}

				foreach (var conjugateDigit in conjugateMask)
				{
					var house = extraCells.FirstSharedHouse;
					var map = HousesMap[house] & extraCells;
					if (map != extraCells || map != (CandidatesMap[conjugateDigit] & HousesMap[house]))
					{
						continue;
					}

					var elimDigits = (Mask)(normalDigits & ~(1 << conjugateDigit));
					var conclusions = new List<Conclusion>();
					foreach (var digit in elimDigits)
					{
						foreach (var cell in extraCells & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in patternCells & ~extraCells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
						}
					}
					foreach (var cell in extraCells)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + conjugateDigit));
					}

					var step = new ExtendedRectangleType4Step(
						conclusions.AsReadOnlyMemory(),
						[
							[
								.. candidateOffsets,
								new ConjugateLinkViewNode(ColorIdentifier.Normal, extraCells[0], extraCells[1], conjugateDigit)
							]
						],
						context.Options,
						in patternCells,
						normalDigits,
						new(in extraCells, conjugateDigit)
					);
					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
				break;
			}
		}

	ReturnNull:
		return null;
	}
}
