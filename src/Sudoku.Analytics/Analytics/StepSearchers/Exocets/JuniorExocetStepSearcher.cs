namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Junior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Junior Exocet</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class JuniorExocetStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates all patterns.
	/// </summary>
	private static readonly Exocet[] Patterns = new Exocet[ExocetTemplatesCount];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor'/>
	static JuniorExocetStepSearcher()
	{
		var s = new[,] { { 3, 4, 5, 6, 7, 8 }, { 0, 1, 2, 6, 7, 8 }, { 0, 1, 2, 3, 4, 5 } };
		var b = new[,]
		{
			{ 0, 1 }, { 0, 2 }, { 1, 2 }, { 9, 10 }, { 9, 11 }, { 10, 11 }, { 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 0, 9 }, { 0, 18 }, { 9, 18 }, { 1, 10 }, { 1, 19 }, { 10, 19 }, { 2, 11 }, { 2, 20 }, { 11, 20 }
		};
		var rq = new[,]
		{
			{ 9, 18 }, { 10, 19 }, { 11, 20 }, { 0, 18 }, { 1, 19 }, { 2, 20 }, { 0, 9 }, { 1, 10 }, { 2, 11 },
			{ 1, 2 }, { 10, 11 }, { 19, 20 }, { 0, 2 }, { 9, 11 }, { 18, 20 }, { 0, 1 }, { 9, 10 }, { 18, 19 }
		};
		var m = new[,]
		{
			{ 10, 11, 19, 20 }, { 9, 11, 18, 20 }, { 9, 10, 18, 19 },
			{ 1, 2, 19, 20 }, { 0, 2, 18, 20 }, { 0, 1, 18, 19 },
			{ 1, 2, 10, 11 }, { 0, 2, 9, 11 }, { 0, 1, 9, 10 },
			{ 10, 19, 11, 20 }, { 1, 19, 2, 20 }, { 1, 10, 2, 11 },
			{ 9, 18, 11, 20 }, { 0, 18, 2, 20 }, { 0, 9, 2, 11 },
			{ 9, 18, 10, 19 }, { 0, 18, 1, 19 }, { 0, 9, 1, 10 }
		};
		var bb = new[] { 0, 3, 6, 27, 30, 33, 54, 57, 60, 0, 27, 54, 3, 30, 57, 6, 33, 60 };
		var bc = new[,]
		{
			{ 1, 2 }, { 0, 2 }, { 0, 1 }, { 4, 5 }, { 3, 5 }, { 3, 4 }, { 7, 8 }, { 6, 8 }, { 6, 7 },
			{ 3, 6 }, { 0, 6 }, { 0, 3 }, { 4, 7 }, { 1, 7 }, { 1, 4 }, { 5, 8 }, { 2, 8 }, { 2, 5 }
		};

		scoped var t = (stackalloc int[3]);
		scoped var crossline = (stackalloc int[25]); // Only use [7..24].
		var n = 0;
		for (var i = 0; i < 18; i++)
		{
			for (int z = i / 9 * 9, j = z; j < z + 9; j++)
			{
				for (int y = j / 3 * 3, k = y; k < y + 3; k++)
				{
					for (var l = y; l < y + 3; l++)
					{
						var (b1, b2) = (bb[i] + b[j, 0], bb[i] + b[j, 1]);
						var (tq1, tr1) = (bb[bc[i, 0]] + rq[k, 0], bb[bc[i, 1]] + rq[l, 0]);

						var index = 6;
						var x = i / 3 % 3;
						var tt = i < 9 ? b1 % 9 + b2 % 9 : b1 / 9 + b2 / 9;
						tt = tt switch { < 4 => 3 - tt, < 13 => 12 - tt, _ => 21 - tt };

						(t[0], t[1], t[2]) = i < 9 ? (tt, tq1 % 9, tr1 % 9) : (tt, tq1 / 9, tr1 / 9);
						for (var index1 = 0; index1 < 3; index1++)
						{
							SkipInit(out int r);
							SkipInit(out int c);

							(i < 9 ? ref c : ref r) = t[index1];
							for (var index2 = 0; index2 < 6; index2++)
							{
								(i < 9 ? ref r : ref c) = s[x, index2];

								crossline[++index] = r * 9 + c;
							}
						}

						Patterns[n++] = new(
							b1,
							b2,
							tq1,
							bb[bc[i, 0]] + rq[k, 1],
							tr1,
							bb[bc[i, 1]] + rq[l, 1],
							(CellMap)crossline[7..],
							CellsMap[bb[bc[i, 1]] + m[l, 2]] + (bb[bc[i, 1]] + m[l, 3]),
							CellsMap[bb[bc[i, 1]] + m[l, 0]] + (bb[bc[i, 1]] + m[l, 1]),
							CellsMap[bb[bc[i, 0]] + m[k, 2]] + (bb[bc[i, 0]] + m[k, 3]),
							CellsMap[bb[bc[i, 0]] + m[k, 0]] + (bb[bc[i, 0]] + m[k, 1])
						);
					}
				}
			}
		}
	}


	/// <summary>
	/// Indicates whether the searcher will find advanced eliminations.
	/// </summary>
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		foreach (var currentJe in Patterns)
		{
			if (!EmptyCells.Contains(currentJe.Base1) || !EmptyCells.Contains(currentJe.Base2))
			{
				// Base cells should be empty cells.
				continue;
			}

			var baseCellsDigitsMask = grid.GetDigitsUnion(currentJe.BaseCellsMap);
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

			if (!CheckTargetCells(currentJe.TargetQ1, currentJe.TargetQ2, baseCellsDigitsMask, grid, out var otherDigitsMaskQArea))
			{
				continue;
			}

			if (!CheckTargetCells(currentJe.TargetR1, currentJe.TargetR2, baseCellsDigitsMask, grid, out var otherDigitsMaskRArea))
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
			foreach (var digit in grid.GetCandidates(currentJe.Base1))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, currentJe.Base1 * 9 + digit));
			}
			foreach (var digit in grid.GetCandidates(currentJe.Base2))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, currentJe.Base2 * 9 + digit));
			}
			foreach (var cell in currentJe.CrossLine)
			{
				cellOffsets.Add(new(DisplayColorKind.Auxiliary2, cell));

				foreach (var digit in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask))
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

			var step = new JuniorExocetStep(new[] { View.Empty | cellOffsets | candidateOffsets }, currentJe, baseCellsDigitsMask, eliminations.ToArray());
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);


			void gatherEliminations(int targetCell, Mask baseCellsDigits, Mask otherDigits, scoped in Grid grid)
			{
				var elimDigitsMask = (Mask)(grid.GetCandidates(targetCell) & ~(baseCellsDigits | otherDigits));

				if (EmptyCells.Contains(targetCell))
				{
					// Check existence of eliminations.
					if (elimDigitsMask != 0 && (grid.GetCandidates(targetCell) & baseCellsDigits) != 0)
					{
						foreach (var elimDigit in elimDigitsMask)
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
					foreach (var digit in (Mask)(grid.GetCandidates(targetCell) & ~elimDigitsMask))
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
	private unsafe bool CheckTargetCells(
		int targetCell1,
		int targetCell2,
		Mask baseCellsDigitsMask,
		scoped in Grid grid,
		out Mask resultOtherDigitsMask
	)
	{
		resultOtherDigitsMask = 0;

		var targetCell1Mask = grid.GetCandidates(targetCell1);
		var targetCell2Mask = grid.GetCandidates(targetCell2);
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

		var otherDigitsMask = (Mask)((targetCell1Mask | targetCell2Mask) & ~baseCellsDigitsMask);
		var housePair = stackalloc[]
		{
			targetCell1.ToHouseIndex(HouseType.Block),
			targetCell1.ToHouseIndex(HouseType.Row) == targetCell2.ToHouseIndex(HouseType.Row)
				? targetCell1.ToHouseIndex(HouseType.Row)
				: targetCell1.ToHouseIndex(HouseType.Column)
		};

		for (var i = 1; i <= PopCount((uint)otherDigitsMask); i++)
		{
			foreach (var digits in otherDigitsMask.GetAllSets().GetSubsets(i))
			{
				var currentDigitsMask = (Mask)0;
				foreach (var digit in digits)
				{
					currentDigitsMask |= (Mask)(1 << digit);
				}

				for (var j = 0; j < 2; j++)
				{
					var count = 0;
					for (var k = 0; k < 9; k++)
					{
						var tempCell = HouseCells[housePair[j]][k];
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
	private bool CheckCrossLineCells(scoped in Exocet currentJe, Mask digitsNeedChecking)
	{
		foreach (var digitNeedChecking in digitsNeedChecking)
		{
			var currentDigitSegment = currentJe.CrossLine & DigitsMap[digitNeedChecking];
			if (PopCount((uint)currentDigitSegment.RowMask) <= 2 || PopCount((uint)currentDigitSegment.ColumnMask) <= 2)
			{
				continue;
			}

			var flag = false;
			foreach (var currentRow in currentDigitSegment.RowMask)
			{
				foreach (var currentColumn in currentDigitSegment.ColumnMask)
				{
					if (!(currentDigitSegment - (HousesMap[currentRow + 9] | HousesMap[currentColumn + 18])))
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
