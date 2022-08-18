namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Junior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Junior Exocet</item>
/// </list>
/// </summary>
public interface IJuniorExocetStepSearcher : IExocetStepSearcher
{
}

[StepSearcher]
internal sealed unsafe partial class JuniorExocetStepSearcher : IJuniorExocetStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var exocetPatterns = IExocetStepSearcher.Patterns;
		foreach (/*scoped*/ ref readonly var currentJe in exocetPatterns.EnumerateRef())
		{
			if (!EmptyCells.Contains(currentJe.Base1) || !EmptyCells.Contains(currentJe.Base2))
			{
				// Base cells should be empty cells.
				continue;
			}

			short baseCellsDigitsMask = grid.GetDigitsUnion(currentJe.BaseCellsMap);
			if (PopCount((uint)baseCellsDigitsMask) > 5)
			{
				// The number of kinds of digits should not be greater than 5.
				continue;
			}

			if (!EmptyCells.Contains(currentJe.TargetQ1) && !EmptyCells.Contains(currentJe.TargetQ2)
				&& !EmptyCells.Contains(currentJe.TargetR1) && !EmptyCells.Contains(currentJe.TargetR2))
			{
				// In target cells, at least one cell should be empty cell.
				continue;
			}

			if (!CheckTargetCells(
					currentJe.TargetQ1, currentJe.TargetQ2, baseCellsDigitsMask,
					grid, out short otherDigitsMaskQArea))
			{
				continue;
			}
			if (!CheckTargetCells(
					currentJe.TargetR1, currentJe.TargetR2, baseCellsDigitsMask,
					grid, out short otherDigitsMaskRArea))
			{
				continue;
			}

			if (!CheckCrossLineCells(currentJe, baseCellsDigitsMask))
			{
				continue;
			}

			var eliminations = new List<ExocetElimination>();
			var cellOffsets = new List<CellViewNode>
			{
				new(DisplayColorKind.Normal, currentJe.Base1),
				new(DisplayColorKind.Normal, currentJe.Base2),
				new(DisplayColorKind.Auxiliary1, currentJe.TargetQ1),
				new(DisplayColorKind.Auxiliary1, currentJe.TargetQ2),
				new(DisplayColorKind.Auxiliary1, currentJe.TargetR1),
				new(DisplayColorKind.Auxiliary1, currentJe.TargetR2)
			};
			var candidateOffsets = new List<CandidateViewNode>();
			foreach (int digit in grid.GetCandidates(currentJe.Base1))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, currentJe.Base1 * 9 + digit));
			}
			foreach (int digit in grid.GetCandidates(currentJe.Base2))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, currentJe.Base2 * 9 + digit));
			}
			foreach (int cell in currentJe.CrossLine)
			{
				cellOffsets.Add(new(DisplayColorKind.Auxiliary2, cell));

				foreach (int digit in grid.GetCandidates(cell) & baseCellsDigitsMask)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + digit));
				}
			}

			gatherEliminations(currentJe.TargetQ1, baseCellsDigitsMask, otherDigitsMaskQArea, grid);
			gatherEliminations(currentJe.TargetQ2, baseCellsDigitsMask, otherDigitsMaskQArea, grid);
			gatherEliminations(currentJe.TargetR1, baseCellsDigitsMask, otherDigitsMaskRArea, grid);
			gatherEliminations(currentJe.TargetR2, baseCellsDigitsMask, otherDigitsMaskRArea, grid);
			if (eliminations.Count == 0)
			{
				continue;
			}

			var step = new JuniorExocetStep(
				ImmutableArray.Create(View.Empty | cellOffsets | candidateOffsets),
				currentJe,
				baseCellsDigitsMask,
				eliminations.ToImmutableArray()
			);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);


			void gatherEliminations(int targetCell, short baseCellsDigits, short otherDigits, scoped in Grid grid)
			{
				short elimDigitsMask = (short)(grid.GetCandidates(targetCell) & ~(baseCellsDigits | otherDigits));

				if (EmptyCells.Contains(targetCell))
				{
					// Check existence of eliminations.
					if (elimDigitsMask != 0 && (grid.GetCandidates(targetCell) & baseCellsDigits) != 0)
					{
						foreach (int elimDigit in elimDigitsMask)
						{
							eliminations.Add(
								new(
									new Conclusion[] { new(Elimination, targetCell, elimDigit) },
									ExocetEliminatedReason.Basic
								)
							);
						}
					}

					// Highlight candidates.
					foreach (int digit in (short)(grid.GetCandidates(targetCell) & ~elimDigitsMask))
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, targetCell * 9 + digit));
					}
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Checks for the validity of the target cells, guaranteeing the target cells is valid in the full exocet map.
	/// </summary>
	/// <param name="targetCell1">The first target cell to be checked.</param>
	/// <param name="targetCell2">The second target cell to be checked.</param>
	/// <param name="baseCellsDigitsMask">The mask that holds the digits appearing in the base cells.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="resultOtherDigitsMask">
	/// The other digits found in the exocet pattern, which means the digits that is not the valid digits
	/// as the base digits.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private bool CheckTargetCells(
		int targetCell1, int targetCell2, short baseCellsDigitsMask, scoped in Grid grid,
		out short resultOtherDigitsMask)
	{
		resultOtherDigitsMask = 0;

		short targetCell1Mask = grid.GetCandidates(targetCell1);
		short targetCell2Mask = grid.GetCandidates(targetCell2);
		if ((baseCellsDigitsMask & targetCell1Mask) != 0 ^ (baseCellsDigitsMask & targetCell2Mask) != 0)
		{
			// One of two target cells only contains the digits appearing in base cells,
			// and the other target cell doesn't.
			return true;
		}

		if ((baseCellsDigitsMask & targetCell1Mask) == 0 && (baseCellsDigitsMask & targetCell2Mask) == 0
			|| (baseCellsDigitsMask & ~targetCell1Mask) == 0 && (baseCellsDigitsMask & ~targetCell2Mask) == 0)
		{
			// Neither two target cells don't contain any possible digits appearing in base cells,
			// all both only contain digits appearing in base cells.
			return false;
		}

		short otherDigitsMask = (short)((targetCell1Mask | targetCell2Mask) & ~baseCellsDigitsMask);
		int* housePair = stackalloc[]
		{
			targetCell1.ToHouseIndex(HouseType.Block),
			targetCell1.ToHouseIndex(HouseType.Row) == targetCell2.ToHouseIndex(HouseType.Row)
				? targetCell1.ToHouseIndex(HouseType.Row)
				: targetCell1.ToHouseIndex(HouseType.Column)
		};

		for (int i = 1; i <= PopCount((uint)otherDigitsMask); i++)
		{
			foreach (int[] digits in otherDigitsMask.GetAllSets().GetSubsets(i))
			{
				short currentDigitsMask = 0;
				foreach (int digit in digits)
				{
					currentDigitsMask |= (short)(1 << digit);
				}

				for (int j = 0; j < 2; j++)
				{
					int count = 0;
					for (int k = 0; k < 9; k++)
					{
						int tempCell = HouseCells[housePair[j]][k];
						if (tempCell == targetCell1 || tempCell == targetCell2)
						{
							// Cannot be itself.
							continue;
						}

						if (!EmptyCells.Contains(tempCell))
						{
							// The cell must be empty.
							continue;
						}

						if ((grid.GetCandidates(tempCell) & currentDigitsMask) == 0)
						{
							// The current cell does not contain any possible digits enumerated.
							continue;
						}

						count++;
					}

					if (count == PopCount((uint)currentDigitsMask) - 1)
					{
						resultOtherDigitsMask = currentDigitsMask;
						return true;
					}
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Checks for the validity of the cross-line cells, guaranteeing the target cells is valid
	/// in the full exocet map.
	/// </summary>
	/// <param name="currentJe">The current JE pattern.</param>
	/// <param name="digitsNeedChecking">The digits need checking.</param>
	/// <returns>A <see cref="bool"/> indicating that.</returns>
	private bool CheckCrossLineCells(scoped in ExocetPattern currentJe, short digitsNeedChecking)
	{
		foreach (int digitNeedChecking in digitsNeedChecking)
		{
			var currentDigitSegment = currentJe.CrossLine & DigitsMap[digitNeedChecking];
			if (PopCount((uint)currentDigitSegment.RowMask) <= 2
				|| PopCount((uint)currentDigitSegment.ColumnMask) <= 2)
			{
				continue;
			}

			bool flag = false;
			foreach (int currentRow in currentDigitSegment.RowMask)
			{
				foreach (int currentColumn in currentDigitSegment.ColumnMask)
				{
					if (currentDigitSegment << (HouseMaps[currentRow + 9] | HouseMaps[currentColumn + 18]))
					{
						flag = true;
						goto CheckFlag;
					}
				}
			}

		CheckFlag:
			if (!flag)
			{
				return false;
			}
		}

		return true;
	}
}
