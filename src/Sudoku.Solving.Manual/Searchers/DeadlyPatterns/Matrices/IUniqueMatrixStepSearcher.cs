namespace Sudoku.Solving.Manual.Searchers;

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
public interface IUniqueMatrixStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates the patterns.
	/// </summary>
	protected static readonly Cells[] Patterns = new Cells[UniqueSquareTemplatesCount];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IUniqueMatrixStepSearcher()
	{
		int[,] ChuteIterator =
		{
			{ 0, 3, 6 }, { 0, 3, 7 }, { 0, 3, 8 }, { 0, 4, 6 }, { 0, 4, 7 }, { 0, 4, 8 },
			{ 0, 5, 6 }, { 0, 5, 7 }, { 0, 5, 8 },
			{ 1, 3, 6 }, { 1, 3, 7 }, { 1, 3, 8 }, { 1, 4, 6 }, { 1, 4, 7 }, { 1, 4, 8 },
			{ 1, 5, 6 }, { 1, 5, 7 }, { 1, 5, 8 },
			{ 2, 3, 6 }, { 2, 3, 7 }, { 2, 3, 8 }, { 2, 4, 6 }, { 2, 4, 7 }, { 2, 4, 8 },
			{ 2, 5, 6 }, { 2, 5, 7 }, { 2, 5, 8 }
		};

		int length = ChuteIterator.Length / 3, n = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int a = ChuteIterator[j, 0] + i * 27;
				int b = ChuteIterator[j, 1] + i * 27;
				int c = ChuteIterator[j, 2] + i * 27;
				Patterns[n++] = Cells.Empty
					+ a + b + c
					+ (a + 9) + (b + 9) + (c + 9)
					+ (a + 18) + (b + 18) + (c + 18);
			}
		}

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int a = ChuteIterator[j, 0] * 9;
				int b = ChuteIterator[j, 1] * 9;
				int c = ChuteIterator[j, 2] * 9;
				Patterns[n++] = Cells.Empty
					+ (a + 3 * i) + (b + 3 * i) + (c + 3 * i)
					+ (a + 1 + 3 * i) + (b + 1 + 3 * i) + (c + 1 + 3 * i)
					+ (a + 2 + 3 * i) + (b + 2 + 3 * i) + (c + 2 + 3 * i);
			}
		}
	}
}

[StepSearcher]
[SukakuNotSupported]
internal sealed unsafe partial class UniqueMatrixStepSearcher : IUniqueMatrixStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		foreach (var pattern in IUniqueMatrixStepSearcher.Patterns)
		{
			if ((EmptyCells & pattern) != pattern)
			{
				continue;
			}

			short mask = grid.GetDigitsUnion(pattern);
			if (CheckType1(accumulator, grid, onlyFindOne, pattern, mask) is { } type1Step)
			{
				return type1Step;
			}
			if (CheckType2(accumulator, onlyFindOne, pattern, mask) is { } type2Step)
			{
				return type2Step;
			}
			if (CheckType3(accumulator, grid, onlyFindOne, pattern, mask) is { } type3Step)
			{
				return type3Step;
			}
			if (CheckType4(accumulator, grid, onlyFindOne, pattern, mask) is { } type4Step)
			{
				return type4Step;
			}
		}

		return null;
	}

	private IStep? CheckType1(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in Cells pattern, short mask)
	{
		if (PopCount((uint)mask) != 5)
		{
			goto ReturnNull;
		}

		foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (int digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			int extraDigit = TrailingZeroCount(mask & ~digitsMask);
			var extraDigitMap = CandidatesMap[extraDigit] & pattern;
			if (extraDigitMap is not [var elimCell])
			{
				continue;
			}

			short cellMask = grid.GetCandidates(elimCell);
			short elimMask = (short)(cellMask & ~(1 << extraDigit));
			if (elimMask == 0)
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (int digit in elimMask)
			{
				conclusions.Add(new(Elimination, elimCell, digit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (int digit in digits)
			{
				foreach (int cell in pattern - elimCell & CandidatesMap[digit])
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}

			var step = new UniqueMatrixType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(View.Empty | candidateOffsets),
				pattern,
				digitsMask,
				elimCell * 9 + extraDigit
			);
			if (onlyFindOne)
			{
				return step;
			}
			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private IStep? CheckType2(
		ICollection<IStep> accumulator, bool onlyFindOne, scoped in Cells pattern, short mask)
	{
		if (PopCount((uint)mask) != 5)
		{
			goto ReturnNull;
		}

		foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (int digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			int extraDigit = TrailingZeroCount(mask & ~digitsMask);
			if (pattern % CandidatesMap[extraDigit] is not (var elimMap and not []))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (int digit in digits)
			{
				foreach (int cell in CandidatesMap[digit] & pattern)
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}
			foreach (int cell in CandidatesMap[extraDigit] & pattern)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + extraDigit));
			}

			var step = new UniqueMatrixType2Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(View.Empty | candidateOffsets),
				pattern,
				digitsMask,
				extraDigit
			);
			if (onlyFindOne)
			{
				return step;
			}
			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private IStep? CheckType3(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in Cells pattern, short mask)
	{
		foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (int digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			short extraDigitsMask = (short)(mask & ~digitsMask);
			var tempMap = Cells.Empty;
			foreach (int digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			if (tempMap.InOneHouse)
			{
				continue;
			}

			foreach (int house in tempMap.CoveredHouses)
			{
				var allCells = (HousesMap[house] & EmptyCells) - pattern;
				for (int size = PopCount((uint)extraDigitsMask) - 1, count = allCells.Count; size < count; size++)
				{
					foreach (var cells in allCells & size)
					{
						short tempMask = grid.GetDigitsUnion(cells);
						if (PopCount((uint)tempMask) != size + 1
							|| (tempMask & extraDigitsMask) != extraDigitsMask)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in tempMask)
						{
							foreach (int cell in (allCells - cells) & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (int cell in pattern)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(tempMask >> digit & 1) != 0
											? DisplayColorKind.Auxiliary1
											: DisplayColorKind.Normal,
										cell * 9 + digit
									)
								);
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
							}
						}

						var step = new UniqueMatrixType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(
								View.Empty
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, house)
							),
							pattern,
							digitsMask,
							extraDigitsMask,
							cells
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

	private IStep? CheckType4(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in Cells pattern, short mask)
	{
		foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (int digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			short extraDigitsMask = (short)(mask & ~digitsMask);
			var tempMap = Cells.Empty;
			foreach (int digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			if (tempMap.InOneHouse)
			{
				continue;
			}

			foreach (int house in tempMap.CoveredHouses)
			{
				int d1 = -1, d2 = -1, count = 0;
				var compareMap = HousesMap[house] & pattern;
				foreach (int digit in digits)
				{
					if ((compareMap | HousesMap[house] & CandidatesMap[digit]) == compareMap)
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
				short comparer = (short)(1 << d1 | 1 << d2);
				short otherDigitsMask = (short)(digitsMask & ~comparer);
				var conclusions = new List<Conclusion>();
				foreach (int digit in otherDigitsMask)
				{
					foreach (int cell in compareMap & CandidatesMap[digit])
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int cell in pattern - compareMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}
				foreach (int cell in compareMap & CandidatesMap[d1])
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d1));
				}
				foreach (int cell in compareMap & CandidatesMap[d2])
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d2));
				}

				var step = new UniqueMatrixType4Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new HouseViewNode(DisplayColorKind.Normal, house)
					),
					pattern,
					digitsMask,
					d1,
					d2,
					compareMap
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
