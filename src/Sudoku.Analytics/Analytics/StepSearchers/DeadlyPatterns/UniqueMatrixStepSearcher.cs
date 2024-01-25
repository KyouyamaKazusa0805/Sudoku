namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Unique Matrix</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Matrix Type 1</item>
/// <item>Unique Matrix Type 2</item>
/// <item>Unique Matrix Type 3</item>
/// <item>Unique Matrix Type 4</item>
/// </list>
/// </summary>
[StepSearcher(Technique.UniqueMatrixType1, Technique.UniqueMatrixType2, Technique.UniqueMatrixType3, Technique.UniqueMatrixType4)]
[StepSearcherFlags(ConditionalFlags.Standard)]
[StepSearcherRuntimeName("StepSearcherName_UniqueMatrixStepSearcher")]
public sealed partial class UniqueMatrixStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the patterns.
	/// </summary>
	private static readonly CellMap[] Patterns;

	/// <summary>
	/// Indicates the iterator values for split chutes.
	/// </summary>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/requires-static-constructor-invocation" />
	/// </remarks>
	private static readonly int[][] ChuteIteratorValues = [
		[0, 3, 6], [0, 3, 7], [0, 3, 8], [0, 4, 6], [0, 4, 7], [0, 4, 8], [0, 5, 6], [0, 5, 7], [0, 5, 8],
		[1, 3, 6], [1, 3, 7], [1, 3, 8], [1, 4, 6], [1, 4, 7], [1, 4, 8], [1, 5, 6], [1, 5, 7], [1, 5, 8],
		[2, 3, 6], [2, 3, 7], [2, 3, 8], [2, 4, 6], [2, 4, 7], [2, 4, 8], [2, 5, 6], [2, 5, 7], [2, 5, 8]
	];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static UniqueMatrixStepSearcher()
	{
		var result = new CellMap[162];
		var length = ChuteIteratorValues.Length / 3;
		var n = 0;
		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < length; j++)
			{
				var a = ChuteIteratorValues[j][0] + i * 27;
				var b = ChuteIteratorValues[j][1] + i * 27;
				var c = ChuteIteratorValues[j][2] + i * 27;
				result[n++] = [a, b, c, a + 9, b + 9, c + 9, a + 18, b + 18, c + 18];
			}
		}

		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < length; j++)
			{
				var a = ChuteIteratorValues[j][0] * 9;
				var b = ChuteIteratorValues[j][1] * 9;
				var c = ChuteIteratorValues[j][2] * 9;
				result[n++] = [
					a + 3 * i, b + 3 * i, c + 3 * i,
					a + 1 + 3 * i, b + 1 + 3 * i, c + 1 + 3 * i,
					a + 2 + 3 * i, b + 2 + 3 * i, c + 2 + 3 * i
				];
			}
		}

		Patterns = result;
	}


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var pattern in Patterns)
		{
			if ((EmptyCells & pattern) != pattern)
			{
				continue;
			}

			var mask = grid[in pattern];
			if (CheckType1(accumulator, in grid, ref context, onlyFindOne, in pattern, mask) is { } type1Step)
			{
				return type1Step;
			}
			if (CheckType2(accumulator, onlyFindOne, ref context, in pattern, mask) is { } type2Step)
			{
				return type2Step;
			}
			if (CheckType3(accumulator, in grid, ref context, onlyFindOne, in pattern, mask) is { } type3Step)
			{
				return type3Step;
			}
			if (CheckType4(accumulator, in grid, ref context, onlyFindOne, in pattern, mask) is { } type4Step)
			{
				return type4Step;
			}
		}

		return null;
	}

	/// <summary>
	/// Searches for type 1.
	/// </summary>
	private UniqueMatrixType1Step? CheckType1(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		bool onlyFindOne,
		scoped ref readonly CellMap pattern,
		Mask mask
	)
	{
		if (PopCount((uint)mask) != 5)
		{
			goto ReturnNull;
		}

		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			var digitsMask = MaskOperations.Create(digits);
			var extraDigit = TrailingZeroCount(mask & ~digitsMask);
			var extraDigitMap = CandidatesMap[extraDigit] & pattern;
			if (extraDigitMap is not [var elimCell])
			{
				continue;
			}

			var cellMask = grid.GetCandidates(elimCell);
			var elimMask = (Mask)(cellMask & ~(1 << extraDigit));
			if (elimMask == 0)
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (var digit in elimMask)
			{
				conclusions.Add(new(Elimination, elimCell, digit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var digit in digits)
			{
				foreach (var cell in pattern - elimCell & CandidatesMap[digit])
				{
					candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
				}
			}

			var step = new UniqueMatrixType1Step([.. conclusions], [[.. candidateOffsets]], context.PredefinedOptions, in pattern, digitsMask, elimCell * 9 + extraDigit);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Searches for type 2.
	/// </summary>
	private UniqueMatrixType2Step? CheckType2(
		List<Step> accumulator,
		bool onlyFindOne,
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap pattern,
		Mask mask
	)
	{
		if (PopCount((uint)mask) != 5)
		{
			goto ReturnNull;
		}

		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			var digitsMask = MaskOperations.Create(digits);
			var extraDigit = TrailingZeroCount(mask & ~digitsMask);
			if (pattern % CandidatesMap[extraDigit] is not (var elimMap and not []))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (var cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var digit in digits)
			{
				foreach (var cell in CandidatesMap[digit] & pattern)
				{
					candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
				}
			}
			foreach (var cell in CandidatesMap[extraDigit] & pattern)
			{
				candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + extraDigit));
			}

			var step = new UniqueMatrixType2Step([.. conclusions], [[.. candidateOffsets]], context.PredefinedOptions, in pattern, digitsMask, extraDigit);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Searches for type 3.
	/// </summary>
	private UniqueMatrixType3Step? CheckType3(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		bool onlyFindOne,
		scoped ref readonly CellMap pattern,
		Mask mask
	)
	{
		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			var digitsMask = MaskOperations.Create(digits);
			var extraDigitsMask = (Mask)(mask & ~digitsMask);
			var tempMap = CellMap.Empty;
			foreach (var digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			if (tempMap.InOneHouse(out _))
			{
				continue;
			}

			foreach (var house in tempMap.CoveredHouses)
			{
				var allCells = (HousesMap[house] & EmptyCells) - pattern;
				for (var size = PopCount((uint)extraDigitsMask) - 1; size < allCells.Count; size++)
				{
					foreach (ref readonly var cells in allCells.GetSubsets(size))
					{
						var tempMask = grid[in cells];
						if (PopCount((uint)tempMask) != size + 1 || (tempMask & extraDigitsMask) != extraDigitsMask)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (var digit in tempMask)
						{
							foreach (var cell in (allCells - cells) & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in pattern)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(tempMask >> digit & 1) != 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
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

						var step = new UniqueMatrixType3Step(
							[.. conclusions],
							[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
							context.PredefinedOptions,
							in pattern,
							digitsMask,
							in cells,
							extraDigitsMask
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

		return null;
	}

	/// <summary>
	/// Searches for type 4.
	/// </summary>
	private UniqueMatrixType4Step? CheckType4(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		bool onlyFindOne,
		scoped ref readonly CellMap pattern,
		Mask mask
	)
	{
		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			var digitsMask = MaskOperations.Create(digits);
			var extraDigitsMask = (Mask)(mask & ~digitsMask);
			var tempMap = CellMap.Empty;
			foreach (var digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			if (tempMap.InOneHouse(out _))
			{
				continue;
			}

			foreach (var house in tempMap.CoveredHouses)
			{
				var d1 = -1;
				var d2 = -1;
				var count = 0;
				var conjugateMap = HousesMap[house] & pattern;
				foreach (var digit in digits)
				{
					if ((conjugateMap | HousesMap[house] & CandidatesMap[digit]) == conjugateMap)
					{
						switch (count++)
						{
							case 0:
							{
								d1 = digit;
								break;
							}
							case 1:
							{
								d2 = digit;
								goto Finally;
							}
						}
					}
				}

			Finally:
				var comparer = (Mask)(1 << d1 | 1 << d2);
				var otherDigitsMask = (Mask)(digitsMask & ~comparer);
				var conclusions = new List<Conclusion>();
				foreach (var digit in otherDigitsMask)
				{
					foreach (var cell in conjugateMap & CandidatesMap[digit])
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in pattern - conjugateMap)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}
				}
				foreach (var cell in conjugateMap & CandidatesMap[d1])
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + d1));
				}
				foreach (var cell in conjugateMap & CandidatesMap[d2])
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + d2));
				}

				var step = new UniqueMatrixType4Step(
					[.. conclusions],
					[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
					context.PredefinedOptions,
					in pattern,
					digitsMask,
					d1,
					d2,
					in conjugateMap
				);
				if (onlyFindOne)
				{
					return step;
				}
				accumulator.Add(step);
			}
		}

		return null;
	}
}
