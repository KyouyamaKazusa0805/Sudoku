namespace Sudoku.Analytics.StepSearchers;

public partial class UniqueRectangleStepSearcher
{
	/// <summary>
	/// Check UR + 2B/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (ab )  (ab )
	///  |
	///  | a
	///  |
	///  abx    aby
	/// ]]></code>
	/// </remarks>
	private partial void Check2B1SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var digits = (ReadOnlySpan<Digit>)([d1, d2]);
		foreach (var cell in (corner1, corner2))
		{
			foreach (var otherCell in otherCellsMap)
			{
				if (!IsSameHouseCell(cell, otherCell, out var houses))
				{
					continue;
				}

				foreach (var house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					for (var digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						var digit = digits[digitIndex];
						if (!IsConjugatePair(digit, [cell, otherCell], house))
						{
							continue;
						}

						var elimCell = (otherCellsMap - otherCell)[0];
						if (!CandidatesMap[digit].Contains(otherCell))
						{
							continue;
						}

						var elimDigit = Mask.TrailingZeroCount((Mask)(comparer ^ (1 << digit)));
						var conclusions = new List<Conclusion>(4);
						if (CandidatesMap[elimDigit].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, elimDigit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (var urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								var coveredHouses = (urCell.AsCellMap() + otherCell).SharedHouses;
								if ((coveredHouses >> house & 1) != 0)
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(ColorIdentifier.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (CandidatesMap[d1].Contains(urCell))
								{
									if (urCell != elimCell || d1 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? ColorIdentifier.Normal
													: d1 == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
												urCell * 9 + d1
											)
										);
									}
								}
								if (CandidatesMap[d2].Contains(urCell))
								{
									if (urCell != elimCell || d2 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? ColorIdentifier.Normal
													: d2 == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
												urCell * 9 + d2
											)
										);
									}
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleConjugatePairStep(
								[.. conclusions],
								[
									[
										.. arMode ? GetHighlightCells(urCells) : [],
										.. candidateOffsets,
										new HouseViewNode(ColorIdentifier.Normal, house)
									]
								],
								context.Options,
								Technique.UniqueRectangle2B1,
								d1,
								d2,
								[.. urCells],
								arMode,
								[new(cell, otherCell, digit)],
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 2D/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )   aby
	///  |
	///  | a
	///  |
	///  abx   (ab )
	///          ↑ corner2
	/// ]]></code>
	/// </remarks>
	private partial void Check2D1SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		foreach (var cell in (corner1, corner2))
		{
			foreach (var otherCell in otherCellsMap)
			{
				if (!IsSameHouseCell(cell, otherCell, out var houses))
				{
					continue;
				}

				foreach (var house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					foreach (var digit in (d1, d2))
					{
						if (!IsConjugatePair(digit, [cell, otherCell], house))
						{
							continue;
						}

						var elimCell = (otherCellsMap - otherCell)[0];
						if (!CandidatesMap[digit].Contains(otherCell))
						{
							continue;
						}

						var conclusions = new List<Conclusion>(4);
						if (CandidatesMap[digit].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (var urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								var flag = false;
								foreach (var r in (urCell.AsCellMap() + otherCell).SharedHouses)
								{
									if (r == house)
									{
										flag = true;
										break;
									}
								}

								if (flag)
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(ColorIdentifier.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (CandidatesMap[d1].Contains(urCell) && (urCell != elimCell || d1 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? ColorIdentifier.Normal
												: d1 == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
											urCell * 9 + d1
										)
									);
								}
								if (CandidatesMap[d2].Contains(urCell) && (urCell != elimCell || d2 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? ColorIdentifier.Normal
												: d2 == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
											urCell * 9 + d2
										)
									);
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleConjugatePairStep(
								[.. conclusions],
								[
									[
										.. arMode ? GetHighlightCells(urCells) : [],
										.. candidateOffsets,
										new HouseViewNode(ColorIdentifier.Normal, house)
									]
								],
								context.Options,
								Technique.UniqueRectangle2D1,
								d1,
								d2,
								[.. urCells],
								arMode,
								[new(cell, otherCell, digit)],
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 3X/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )    abx
	///           |
	///           | b
	///       a   |
	///  aby-----abz
	/// ]]></code>
	/// </remarks>
	private partial void Check3X2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		foreach (var (a, b) in ((d1, d2), (d2, d1)))
		{
			var abxCell = adjacentCellsMap[0];
			var abyCell = adjacentCellsMap[1];
			var map1 = abzCell.AsCellMap() + abxCell;
			var map2 = abzCell.AsCellMap() + abyCell;
			if (!IsConjugatePair(b, in map1, map1.SharedLine) || !IsConjugatePair(a, in map2, map2.SharedLine))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(2);
			if (CandidatesMap[a].Contains(abxCell))
			{
				conclusions.Add(new(Elimination, abxCell, a));
			}
			if (CandidatesMap[b].Contains(abyCell))
			{
				conclusions.Add(new(Elimination, abyCell, b));
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (var digit in grid.GetCandidates(abxCell))
			{
				if ((digit == d1 || digit == d2) && digit != a)
				{
					candidateOffsets.Add(new(digit == b ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, abxCell * 9 + digit));
				}
			}
			foreach (var digit in grid.GetCandidates(abyCell))
			{
				if ((digit == d1 || digit == d2) && digit != b)
				{
					candidateOffsets.Add(new(digit == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, abyCell * 9 + digit));
				}
			}
			foreach (var digit in grid.GetCandidates(abzCell))
			{
				if (digit == a || digit == b)
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, abzCell * 9 + digit));
				}
			}
			foreach (var digit in comparer)
			{
				candidateOffsets.Add(new(ColorIdentifier.Normal, cornerCell * 9 + digit));
			}
			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 6)
			{
				continue;
			}

			accumulator.Add(
				new UniqueRectangleConjugatePairStep(
					[.. conclusions],
					[
						[
							.. arMode ? GetHighlightCells(urCells) : [],
							.. candidateOffsets,
							new HouseViewNode(ColorIdentifier.Normal, map1.SharedLine),
							new HouseViewNode(ColorIdentifier.Auxiliary1, map2.SharedLine)
						]
					],
					context.Options,
					Technique.UniqueRectangle3X2,
					d1,
					d2,
					[.. urCells],
					arMode,
					[new(abxCell, abzCell, b), new(abyCell, abzCell, a)],
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 3N/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a   |
	///            | b
	///            |
	///  aby      abz
	/// ]]></code>
	/// </remarks>
	private partial void Check3N2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Step 1: Get the diagonal cell of 'cornerCell' and determine the existence of strong link.
		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		var digitPairs = (ReadOnlySpan<(Digit, Digit)>)([(d1, d2), (d2, d1)]);
		var digits = (ReadOnlySpan<Digit>)([d1, d2]);
		foreach (var (begin, end) in ((abxCell, abyCell), (abyCell, abxCell)))
		{
			var linkMap = begin.AsCellMap() + abzCell;
			foreach (var (a, b) in digitPairs)
			{
				if (!IsConjugatePair(b, in linkMap, linkMap.SharedLine))
				{
					continue;
				}

				// Step 2: Get the link cell that is adjacent to 'cornerCell' and check the strong link.
				var secondLinkMap = cornerCell.AsCellMap() + begin;
				if (!IsConjugatePair(a, in secondLinkMap, secondLinkMap.SharedLine))
				{
					continue;
				}

				// Step 3: Check eliminations.
				if (!CandidatesMap[a].Contains(end))
				{
					continue;
				}

				// Step 4: Check highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (var d in comparer)
				{
					candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, cornerCell * 9 + d));
				}
				foreach (var d in digits)
				{
					if (CandidatesMap[d].Contains(abzCell))
					{
						candidateOffsets.Add(new(d == b ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, abzCell * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(cornerCell, begin, a), new(begin, abzCell, b)];
				accumulator.Add(
					new UniqueRectangleConjugatePairStep(
						[new(Elimination, end, a)],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairs[1].Line)
							]
						],
						context.Options,
						Technique.UniqueRectangle3N2,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 3U/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        b
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	private partial void Check3U2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		foreach (var (begin, end) in ((abxCell, abyCell), (abyCell, abxCell)))
		{
			var linkMap = begin.AsCellMap() + abzCell;
			foreach (var (a, b) in ((d1, d2), (d2, d1)))
			{
				if (!IsConjugatePair(b, in linkMap, linkMap.SharedLine))
				{
					continue;
				}

				var secondLinkMap = cornerCell.AsCellMap() + end;
				if (!IsConjugatePair(a, in secondLinkMap, secondLinkMap.SharedLine))
				{
					continue;
				}

				if (!CandidatesMap[a].Contains(begin))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (var d in comparer)
				{
					candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, cornerCell * 9 + d));
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, end * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(abzCell))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == b ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, abzCell * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(cornerCell, end, a), new(begin, abzCell, b)];
				accumulator.Add(
					new UniqueRectangleConjugatePairStep(
						[new(Elimination, begin, a)],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairs[1].Line)
							]
						],
						context.Options,
						Technique.UniqueRectangle3U2,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 3E/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        a
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	private partial void Check3E2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		foreach (var (begin, end) in ((abxCell, abyCell), (abyCell, abxCell)))
		{
			var linkMap = begin.AsCellMap() + abzCell;
			foreach (var (a, b) in ((d1, d2), (d2, d1)))
			{
				if (!IsConjugatePair(a, in linkMap, linkMap.SharedLine))
				{
					continue;
				}

				var secondLinkMap = cornerCell.AsCellMap() + end;
				if (!IsConjugatePair(a, in secondLinkMap, secondLinkMap.SharedLine))
				{
					continue;
				}

				if (!CandidatesMap[b].Contains(abzCell))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (var d in comparer)
				{
					candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, cornerCell * 9 + d));
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, end * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(abzCell))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, abzCell * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(cornerCell, end, a), new(begin, abzCell, a)];
				accumulator.Add(
					new UniqueRectangleConjugatePairStep(
						[new(Elimination, abzCell, b)],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairs[1].Line)
							]
						],
						context.Options,
						Technique.UniqueRectangle3E2,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 4X/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | b
	///        a    |
	///  abz ----- abw
	/// ]]></code>
	/// </remarks>
	private partial void Check4X3SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		var link1Map = corner1.AsCellMap() + corner2;
		foreach (var (a, b) in ((d1, d2), (d2, d1)))
		{
			if (!IsConjugatePair(a, in link1Map, link1Map.SharedLine))
			{
				continue;
			}

			var abwCell = GetDiagonalCell(urCells, corner1);
			var abzCell = (otherCellsMap - abwCell)[0];
			foreach (var (head, begin, end, extra) in ((corner2, corner1, abzCell, abwCell), (corner1, corner2, abwCell, abzCell)))
			{
				var link2Map = begin.AsCellMap() + end;
				if (!IsConjugatePair(b, in link2Map, link2Map.SharedLine))
				{
					continue;
				}

				var link3Map = end.AsCellMap() + extra;
				if (!IsConjugatePair(a, in link3Map, link3Map.SharedLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>(2);
				if (CandidatesMap[b].Contains(head))
				{
					conclusions.Add(new(Elimination, head, b));
				}
				if (CandidatesMap[b].Contains(extra))
				{
					conclusions.Add(new(Elimination, extra, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (var d in grid.GetCandidates(head))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, head * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(extra))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, extra * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(head, begin, a), new(begin, end, b), new(end, extra, a)];
				accumulator.Add(
					new UniqueRectangleConjugatePairStep(
						[.. conclusions],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairs[1].Line),
								new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[2].Line)
							]
						],
						context.Options,
						Technique.UniqueRectangle4X3,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 4C/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | a
	///        b    |
	///  abz ----- abw
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///   |    a    |
	///   | b       | a
	///   |         |
	///  abz       abw
	/// ]]></code>
	/// </para>
	/// </remarks>
	private partial void Check4C3SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		var link1Map = corner1.AsCellMap() + corner2;
		var innerMaps = (stackalloc CellMap[2]);
		foreach (var (a, b) in ((d1, d2), (d2, d1)))
		{
			if (!IsConjugatePair(a, in link1Map, link1Map.SharedLine))
			{
				continue;
			}

			var end = GetDiagonalCell(urCells, corner1);
			var extra = (otherCellsMap - end)[0];
			foreach (var (abx, aby, abw, abz) in ((corner2, corner1, extra, end), (corner1, corner2, end, extra)))
			{
				var link2Map = aby.AsCellMap() + abw;
				if (!IsConjugatePair(a, in link2Map, link2Map.SharedLine))
				{
					continue;
				}

				var link3Map1 = abw.AsCellMap() + abz;
				var link3Map2 = abx.AsCellMap() + abz;
				innerMaps[0] = link3Map1;
				innerMaps[1] = link3Map2;
				for (var i = 0; i < 2; i++)
				{
					var linkMap = innerMaps[i];
					if (!IsConjugatePair(b, in link3Map1, link3Map1.SharedLine))
					{
						continue;
					}

					if (!CandidatesMap[b].Contains(aby))
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>(7);
					foreach (var d in grid.GetCandidates(abx))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(
								new(
									i == 0 ? d == a ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal : ColorIdentifier.Auxiliary1,
									abx * 9 + d
								)
							);
						}
					}
					foreach (var d in grid.GetCandidates(abz))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(d == b ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, abz * 9 + d));
						}
					}
					foreach (var d in grid.GetCandidates(aby))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, aby * 9 + d));
						}
					}
					foreach (var d in grid.GetCandidates(abw))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, abw * 9 + d));
						}
					}
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = (Conjugate[])[new(abx, aby, a), new(aby, abw, a), new(linkMap[0], linkMap[1], b)];
					accumulator.Add(
						new UniqueRectangleConjugatePairStep(
							[new(Elimination, aby, b)],
							[
								[
									.. arMode ? GetHighlightCells(urCells) : [],
									.. candidateOffsets,
									new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[0].Line),
									new HouseViewNode(ColorIdentifier.Normal, conjugatePairs[1].Line),
									new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairs[2].Line)
								]
							],
							context.Options,
							Technique.UniqueRectangle4C3,
							d1,
							d2,
							[.. urCells],
							arMode,
							conjugatePairs,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 3x/1SL and UR + 3X/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ cornerCell
	///  (ab ) | abX
	///        |  |
	///        |  |a
	///        |  |
	///   abz  | abY  a/bz
	/// ]]></code>
	/// Suppose cell <c>abX</c> is filled with digit <c>b</c>, then a deadly pattern will be formed:
	/// <code><![CDATA[
	/// a | b
	/// b | a  z
	/// ]]></code>
	/// The pattern is called UR + 3x/1SL.
	/// </para>
	/// <para>
	/// The pattern can be extended with cell <c>a/bz</c> to a pair of cells <c>a/bS</c>,
	/// and cell <c>abz</c> extends to <c>abS</c>, which will become UR + 3X/1SL (where <c>S</c> is a subset of digits).
	/// </para>
	/// </remarks>
	private partial void Check3X1SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index)
	{
		// Test examples:
		// UR + 3x/1SL
		// 3+8.+75.+96+4......2+38.+49.+3871+5.....73.+68.+3.6...9..65.+3...+5273.+68.+19+34...+6..+16+8.7...3:142 244 548 552 156 268 288
		// .46.2.13....+1+4.....2.386.7...2.9+38+1+5.3..5..9...5.1.6+4+3.5.861.2.+2...+3...+1.14.7.35.:821 829 641 751 851 753 254 754 256 756 861 977 984 986 487 991 999
		// 6..1..2.9.+9.+6...5.....896+139...361.+5..65.49....529..+678.936.....7.+9....+62+63..7.+91:512 812 713 223 723 425 731 433 443 843 351 152 867 488
		// +51+628.+9.379.6....+5+48..95.+6.2....+95..+9+5..2.1.6+1.85...296.59.....+8.+9...6573.1.+56.9.:442 742 343 744 345 353 754 462 762 365 766 175 375 176 376
		// +2+863+975+1+4+97415623+81+5+3..+2+9+765+4.+6...+29+62.....8.+3+9..2..+6.7+6+2...+8+53+4+15+73+86+9+2+8+392651+4+7:156 456
		// 6.+7+9+8134...+4+7....9.+39+42.7.1.6..+57.+94+7...4+98..+495....+7+3...6+7+49+3.37.5+92+4+1.+94......7:521 522 227 528 628 144 844 253 258 259 667 579 193 893 394 297 898
		//
		// UR + 3X/1SL
		// ..7348+26+5+6+3+2...+7+4845+867+2+31+928..3+6.7..+7+6+814.23.+4+3+2.+7..+63+2.76....+8..+4+2+36.+7+7+64.8..32:543 147 967 173 177 578

		var cornerDigitsMask = grid.GetCandidates(cornerCell);
		if ((cornerDigitsMask & ~comparer) != 0)
		{
			// :( The corner cell can only contain the digits appeared in UR.
			return;
		}

		// Determine target cell, same-block cell (as corner) and the last cell.
		Unsafe.SkipInit(out int targetCell);
		Unsafe.SkipInit(out int sameBlockCell);
		var cells = (CellMap)urCells;
		foreach (var cell in cells - cornerCell)
		{
			if (HousesMap[cornerCell.ToHouse(HouseType.Block)].Contains(cell))
			{
				sameBlockCell = cell;
			}
			else if (PeersMap[cornerCell].Contains(cell))
			{
				targetCell = cell;
			}
		}

		// Check pattern.
		// According to pattern, there should be a strong link of digit 'a' between 'targetCell' and 'lastCell',
		// and 'sameBlockCell' can only contain one extra digit,
		// and one peer intersection cell of 'targetCell' and 'sameBlockCell' should only contain that extra digit,
		// and only digits appeared in UR pattern.
		var mapOfDigit1And2 = CandidatesMap[d1] | CandidatesMap[d2];
		var lastCell = (cells - cornerCell - targetCell - sameBlockCell)[0];
		foreach (var (conjugatePairDigit, elimDigit) in ((d1, d2), (d2, d1)))
		{
			if ((grid.GetCandidates(targetCell) >> elimDigit & 1) == 0 || (grid.GetCandidates(sameBlockCell) >> elimDigit & 1) == 0)
			{
				// :( Both cells 'targetCell' and 'sameBlockCell' must contain the elimination digit.
				continue;
			}

			var pairMap = targetCell.AsCellMap() + lastCell;
			foreach (var conjugatePairHouse in pairMap.SharedHouses)
			{
				if (!IsConjugatePair(conjugatePairDigit, in pairMap, conjugatePairHouse))
				{
					// :( Strong link of digit 'a' is required.
					continue;
				}

				// Check for cells in line of cell 'same-block', which doesn't include cell 'cornerCell'.
				// Then we should check for empty cells that doesn't overlap with UR pattern, to determine existence of subsets.
				var sameBlockHouses = 1 << sameBlockCell.ToHouse(HouseType.Row) | 1 << sameBlockCell.ToHouse(HouseType.Column);
				foreach (var house in sameBlockHouses)
				{
					if (HousesMap[house].Contains(cornerCell))
					{
						sameBlockHouses &= ~(1 << house);
						break;
					}
				}

				// Then iterate empty cells lying in the target house, to determine whether a subset can be formed.
				var subsetHouse = HouseMask.Log2(sameBlockHouses);
				var outsideCellsRange = HousesMap[subsetHouse] // Subset house that:
					& ~HousesMap[sameBlockCell.ToHouse(HouseType.Block)] // won't overlap the block with same-block cell
					& ~cells // and won't overlap with UR pattern
					& mapOfDigit1And2; // and must contain either digit 1 or digit 2
				foreach (ref readonly var outsideCells in outsideCellsRange | outsideCellsRange.Count)
				{
					var outsideCellDigitsMask = grid[in outsideCells];
					var extraDigitsMaskInOutsideCell = (Mask)(outsideCellDigitsMask & ~comparer);
					if (extraDigitsMaskInOutsideCell == 0)
					{
						// :( The cell contains at least one extra digit.
						continue;
					}

					if (Mask.PopCount(extraDigitsMaskInOutsideCell) != outsideCells.Count)
					{
						// :( The size of the extra cell must be equal to the number of extra digits.
						continue;
					}

					var extraDigitsMaskInSameBlockCell = (Mask)(grid.GetCandidates(sameBlockCell) & ~comparer);
					if (extraDigitsMaskInSameBlockCell != extraDigitsMaskInOutsideCell)
					{
						// :( The cell 'sameBlockCell' must hold the exactly number of extra digits
						//    with the number of cells 'outsideCells'.
						continue;
					}

					var subsetCellsContainingElimDigit = outsideCells & CandidatesMap[elimDigit];
					if ((subsetCellsContainingElimDigit.SharedHouses >> conjugatePairHouse & 1) == 0)
					{
						// :( All cells in outside cells containing elimination digit
						//    should share same block with conjugate pair shared.
						continue;
					}

					// Now pattern is formed. Collect view nodes.
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in cells | outsideCells)
					{
						foreach (var digit in comparer)
						{
							if ((grid.GetCandidates(cell) >> digit & 1) != 0 && (cell != targetCell || digit != elimDigit))
							{
								candidateOffsets.Add(
									new(
										(cell == targetCell || cell == lastCell) && digit == conjugatePairDigit
											? ColorIdentifier.Auxiliary1
											: ColorIdentifier.Normal,
										cell * 9 + digit
									)
								);
							}
						}
					}
					foreach (var outsideCell in outsideCells)
					{
						foreach (var extraDigitInOutsideCell in (Mask)(grid.GetCandidates(outsideCell) & extraDigitsMaskInOutsideCell))
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, outsideCell * 9 + extraDigitInOutsideCell));
						}
					}
					if (!IsIncompleteValid(arMode, AllowIncompleteUniqueRectangles, candidateOffsets, out _))
					{
						continue;
					}

					accumulator.Add(
						new UniqueRectangleConjugatePairExtraStep(
							[new(Elimination, targetCell, elimDigit)],
							[
								[
									.. candidateOffsets,
									..
									from outsideCell in outsideCells
									select new CellViewNode(ColorIdentifier.Auxiliary2, outsideCell),
									..
									from extraDigitInOutsideCell in extraDigitsMaskInOutsideCell
									let extraCandidate = sameBlockCell * 9 + extraDigitInOutsideCell
									select new CandidateViewNode(ColorIdentifier.Auxiliary2, extraCandidate),
									new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairHouse),
									new HouseViewNode(ColorIdentifier.Auxiliary2, subsetHouse)
								]
							],
							context.Options,
							outsideCells.Count == 1 ? Technique.UniqueRectangle3X1L : Technique.UniqueRectangle3X1U,
							d1,
							d2,
							in cells,
							arMode,
							[new(in pairMap, conjugatePairDigit)],
							in outsideCells,
							extraDigitsMaskInOutsideCell,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 4x/1SL and UR + 4X/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	/// Case 1:
	/// cornerCell
	///     ↓       .--------------------.
	///   (abW)-----+-abX                |
	///           a |                    |
	///             |                    |
	///   ab(yz)    | ab(yz) b(yz) b(yz) |
	///             '--------------------'
	/// 
	/// Case 2:
	/// .--------------------.
	/// |   cornerCell→(abW)-+-----abX
	/// |                    | a
	/// |                    |
	/// | a(yz) a(yz) ab(yz) |    ab(yz)
	/// '--------------------'
	/// 
	/// Case 3:
	/// .--------------.   .--------------.
	/// |      cornerCell  |              |
	/// |          ↓   |   |              |
	/// |        (abW)-+---+-abX          |
	/// |              | a |              |
	/// |              |   |              |
	/// | a(yz) ab(yz) |   | ab(yz) b(yz) |
	/// '--------------'   '--------------'
	/// ]]></code>
	/// </remarks>
	private partial void Check4X1SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index)
	{
	}

	/// <summary>
	/// Check UR + 4x/2SL and UR + 4X/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>
	/// The pattern:
	/// <code><![CDATA[
	/// corner1   corner2
	///    ↓    a    ↓
	///  (abZ)--|--(abX)
	///         |    |
	///         |    |a
	///         |    |
	///   abz   |   abY  a/bz
	/// ]]></code>
	/// Suppose cell <c>abX</c> is filled with digit <c>b</c>, then a deadly pattern will be formed:
	/// <code><![CDATA[
	/// a | b
	/// b | a  z
	/// ]]></code>
	/// The pattern is called UR + 4x/2SL.
	/// </para>
	/// <para>
	/// The pattern can be extended with cell <c>a/bz</c> to a pair of cells <c>a/bS</c>,
	/// and cell <c>abz</c> extends to <c>abS</c>, which will become UR + 4X/2SL (where <c>S</c> is a subset of digits).
	/// </para>
	/// </remarks>
	private partial void Check4X2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		// Test examples:
		// UR + 4x/2SL
		// .+23.+5.1675..2.......8.7.+29+5...684..2.8.5..6.............1.2+5+4792.4.6.+51..5..+4..+2+6:422 325 326 826 347 151 356 161 162 364 366 367 867
		// .93.5.218.42...+7+5.+5+1.+2..+46..2+6+5.3+1.7+1+3+9647+58+2.5...2+3.6+9+7+5...+82+4+284..+5+6+3+1+361.2.975:924 925 835 935 175
		// 7.5....+36..+3...89...+9..+3.+75+83+7+45+9+6+2+1+29+4+3+16+587+1+56+782+9+4+3548.3..6.3+6+2....1.+9+7+126+4+3+58:814 124 134 984 786
		// .+4.3.+6.+28.+2..4+83.+686+3..+19....48.2.6+5+61+85+79.3..5+26.48....6..+5.71..1.6+7+5.+34+7+5+1.3+6..:711 911 721 921 961 972
		// 6.+7+9+8134...+4+7....9.+39+42.7.1.6..+57.+94+7...4+98..+495....+7+3...6+7+49+3.37.5+92+4+1.+94......7:521 522 227 528 628 243 144 844 253 258 259 667 271 272 579 193 893 394 297 898
		// 
		// UR + 4X/2SL
		// 85+9+26...364..+93.+52..+25...6+9+2.843.+59.5..7.9+2...9..52...+9+8..+2...5..39.+5.2842+5+3..91.:153 458 663 167 467 867 468 669
		// +56....+7...+4.+7.59.+6.73+6...5..2+63+57...+7+5.+86...238...26+75+6152+7+8...8.+2.4.+567+4.7+5.+62+18:457 458
		// .+85+71+4..2..46395+1+89+1..+8.7+4..3..+4.+8.....+3.8..45+4+8..73..8+5.4...6.49....+28..61+89.+4..:233 641 144 246 546 149 649 251 751 653 655 957 969 373 276 779 586 396 399
		// .+9+5+4+6..2...4....+56.1+6..54+9..41+5+8+69..5.+8...+6.+46.+97+4..+8+51+62+3+78+54+9+45+7..98+6+39+8365+4...:116 717 719 326 127 367

		if ((corner1.AsCellMap() + corner2).IsInIntersection)
		{
			// :( Two corner cells shouldn't inside one same block.
			return;
		}

		foreach (var (targetCell, cornerCell) in ((corner1, corner2), (corner2, corner1)))
		{
			// Determine target cell, same-block cell (as corner) and the last cell.
			Unsafe.SkipInit(out int sameBlockCell);
			var cells = (CellMap)urCells;
			foreach (var cell in cells - cornerCell)
			{
				if (HousesMap[cornerCell.ToHouse(HouseType.Block)].Contains(cell))
				{
					sameBlockCell = cell;
					break;
				}
			}

			var mapOfDigit1And2 = CandidatesMap[d1] | CandidatesMap[d2];
			var lastCell = (cells - cornerCell - targetCell - sameBlockCell)[0];
			var pairMap1 = targetCell.AsCellMap() + cornerCell;
			var pairMap2 = targetCell.AsCellMap() + lastCell;
			foreach (var (conjugatePairDigit, elimDigit) in ((d1, d2), (d2, d1)))
			{
				if ((grid.GetCandidates(targetCell) >> elimDigit & 1) == 0
					|| (grid.GetCandidates(sameBlockCell) >> elimDigit & 1) == 0)
				{
					// :( Both cells 'targetCell' and 'sameBlockCell' must contain the elimination digit.
					continue;
				}

				// Determine whether there're two conjugate pairs, with both connected with cell 'targetCell', of same digit.
				if (!IsConjugatePair(conjugatePairDigit, in pairMap1, pairMap1.SharedLine)
					|| !IsConjugatePair(conjugatePairDigit, in pairMap2, HouseMask.TrailingZeroCount(pairMap2.SharedHouses)))
				{
					continue;
				}

				// Check for cells in line of cell 'same-block', which doesn't include cell 'cornerCell'.
				// Then we should check for empty cells that doesn't overlap with UR pattern, to determine existence of subsets.
				var sameBlockHouses = 1 << sameBlockCell.ToHouse(HouseType.Row) | 1 << sameBlockCell.ToHouse(HouseType.Column);
				foreach (var house in sameBlockHouses)
				{
					if (HousesMap[house].Contains(cornerCell))
					{
						sameBlockHouses &= ~(1 << house);
						break;
					}
				}

				// Then iterate empty cells lying in the target house, to determine whether a subset can be formed.
				var conjugatePairHouse = HouseMask.TrailingZeroCount(pairMap2.SharedHouses);
				var subsetHouse = HouseMask.Log2(sameBlockHouses);
				var outsideCellsRange = HousesMap[subsetHouse] // Subset house that:
					& ~HousesMap[sameBlockCell.ToHouse(HouseType.Block)] // won't overlap the block with same-block cell
					& ~cells // and won't overlap with UR pattern
					& mapOfDigit1And2; // and must contain either digit 1 or digit 2
				foreach (ref readonly var outsideCells in outsideCellsRange | outsideCellsRange.Count)
				{
					var outsideCellDigitsMask = grid[in outsideCells];
					var extraDigitsMaskInOutsideCell = (Mask)(outsideCellDigitsMask & ~comparer);
					if (extraDigitsMaskInOutsideCell == 0)
					{
						// :( The cell contains at least one extra digit.
						continue;
					}

					if (Mask.PopCount(extraDigitsMaskInOutsideCell) != outsideCells.Count)
					{
						// :( The size of the extra cell must be equal to the number of extra digits.
						continue;
					}

					var extraDigitsMaskInSameBlockCell = (Mask)(grid.GetCandidates(sameBlockCell) & ~comparer);
					if (extraDigitsMaskInSameBlockCell != extraDigitsMaskInOutsideCell)
					{
						// :( The cell 'sameBlockCell' must hold the exactly number of extra digits
						//    with the number of cells 'outsideCells'.
						continue;
					}

					var subsetCellsContainingElimDigit = outsideCells & CandidatesMap[elimDigit];
					if ((subsetCellsContainingElimDigit.SharedHouses >> conjugatePairHouse & 1) == 0)
					{
						// :( All cells in outside cells containing elimination digit
						//    should share same block with conjugate pair shared.
						continue;
					}

					// Now pattern is formed. Collect view nodes.
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in cells | outsideCells)
					{
						foreach (var digit in comparer)
						{
							if ((grid.GetCandidates(cell) >> digit & 1) != 0 && (cell != targetCell || digit != elimDigit))
							{
								candidateOffsets.Add(
									new(
										(cell == targetCell || cell == lastCell || cell == cornerCell) && digit == conjugatePairDigit
											? ColorIdentifier.Auxiliary1
											: ColorIdentifier.Normal,
										cell * 9 + digit
									)
								);
							}
						}
					}
					foreach (var outsideCell in outsideCells)
					{
						foreach (var extraDigitInOutsideCell in (Mask)(grid.GetCandidates(outsideCell) & extraDigitsMaskInOutsideCell))
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, outsideCell * 9 + extraDigitInOutsideCell));
						}
					}
					if (!IsIncompleteValid(arMode, AllowIncompleteUniqueRectangles, candidateOffsets, out _))
					{
						continue;
					}

					accumulator.Add(
						new UniqueRectangleConjugatePairExtraStep(
							[new(Elimination, targetCell, elimDigit)],
							[
								[
									.. candidateOffsets,
									..
									from outsideCell in outsideCells
									select new CellViewNode(ColorIdentifier.Auxiliary2, outsideCell),
									..
									from extraDigitInOutsideCell in extraDigitsMaskInOutsideCell
									let extraCandidate = sameBlockCell * 9 + extraDigitInOutsideCell
									select new CandidateViewNode(ColorIdentifier.Auxiliary2, extraCandidate),
									new HouseViewNode(ColorIdentifier.Auxiliary1, conjugatePairHouse),
									new HouseViewNode(ColorIdentifier.Auxiliary1, pairMap1.SharedLine),
									new HouseViewNode(ColorIdentifier.Auxiliary2, subsetHouse)
								]
							],
							context.Options,
							outsideCells.Count == 1 ? Technique.UniqueRectangle4X2L : Technique.UniqueRectangle4X2U,
							d1,
							d2,
							in cells,
							arMode,
							[new(in pairMap1, conjugatePairDigit), new(in pairMap2, conjugatePairDigit)],
							in outsideCells,
							extraDigitsMaskInOutsideCell,
							index
						)
					);
				}
			}
		}
	}
}
