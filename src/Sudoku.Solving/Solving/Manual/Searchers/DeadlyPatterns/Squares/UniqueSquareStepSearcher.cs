using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Steps;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;
using static Sudoku.Solving.Manual.Constants;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Unique Square</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Square Type 1</item>
/// <item>Unique Square Type 2</item>
/// <item>Unique Square Type 3</item>
/// <item>Unique Square Type 4</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class UniqueSquareStepSearcher : IUniqueSquareStepSearcher
{
	/// <summary>
	/// Indicates the patterns.
	/// </summary>
	private static readonly Cells[] Patterns = new Cells[UniqueSquareTemplatesCount];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static UniqueSquareStepSearcher()
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
				Patterns[n++] = new() { a, b, c, a + 9, b + 9, c + 9, a + 18, b + 18, c + 18 };
			}
		}

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int a = ChuteIterator[j, 0] * 9;
				int b = ChuteIterator[j, 1] * 9;
				int c = ChuteIterator[j, 2] * 9;
				Patterns[n++] = new()
				{
					a + 3 * i,
					b + 3 * i,
					c + 3 * i,
					a + 1 + 3 * i,
					b + 1 + 3 * i,
					c + 1 + 3 * i,
					a + 2 + 3 * i,
					b + 2 + 3 * i,
					c + 2 + 3 * i
				};
			}
		}
	}


	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(16, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		foreach (var pattern in Patterns)
		{
			if ((EmptyMap & pattern) != pattern)
			{
				continue;
			}

			short mask = 0;
			foreach (int cell in pattern)
			{
				mask |= grid.GetCandidates(cell);
			}

			if (CheckType1(accumulator, grid, onlyFindOne, pattern, mask) is { } type1Step) return type1Step;
			if (CheckType2(accumulator, onlyFindOne, pattern, mask) is { } type2Step) return type2Step;
			if (CheckType3(accumulator, grid, onlyFindOne, pattern, mask) is { } type3Step) return type3Step;
			if (CheckType4(accumulator, grid, onlyFindOne, pattern, mask) is { } type4Step) return type4Step;
		}

		return null;
	}

	private Step? CheckType1(
		ICollection<Step> accumulator,
		in Grid grid,
		bool onlyFindOne,
		in Cells pattern,
		short mask
	)
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
			var extraDigitMap = CandMaps[extraDigit] & pattern;
			if (extraDigitMap.Count != 1)
			{
				continue;
			}

			int elimCell = extraDigitMap[0];
			short cellMask = grid.GetCandidates(elimCell);
			short elimMask = (short)(cellMask & ~(1 << extraDigit));
			if (elimMask == 0)
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (int digit in elimMask)
			{
				conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>();
			foreach (int digit in digits)
			{
				foreach (int cell in pattern - elimCell & CandMaps[digit])
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
				}
			}

			var step = new UniqueSquareType1Step(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
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

	private Step? CheckType2(ICollection<Step> accumulator, bool onlyFindOne, in Cells pattern, short mask)
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
			var elimMap = pattern % CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>();
			foreach (int digit in digits)
			{
				foreach (int cell in CandMaps[digit] & pattern)
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
				}
			}
			foreach (int cell in CandMaps[extraDigit] & pattern)
			{
				candidateOffsets.Add((cell * 9 + extraDigit, (ColorIdentifier)1));
			}

			var step = new UniqueSquareType2Step(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
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

	private Step? CheckType3(
		ICollection<Step> accumulator,
		in Grid grid,
		bool onlyFindOne,
		in Cells pattern,
		short mask
	)
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
				tempMap |= CandMaps[digit];
			}
			if (tempMap.InOneRegion)
			{
				continue;
			}

			foreach (int region in tempMap.CoveredRegions)
			{
				var allCells = (RegionMaps[region] & EmptyMap) - pattern;
				for (int size = PopCount((uint)extraDigitsMask) - 1, count = allCells.Count; size < count; size++)
				{
					foreach (var cells in allCells & size)
					{
						short tempMask = 0;
						foreach (int cell in cells)
						{
							tempMask |= grid.GetCandidates(cell);
						}

						if (PopCount((uint)tempMask) != size + 1 || (tempMask & extraDigitsMask) != extraDigitsMask)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in tempMask)
						{
							foreach (int cell in (allCells - cells) & CandMaps[digit])
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, ColorIdentifier)>();
						foreach (int cell in pattern)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									(cell * 9 + digit, (ColorIdentifier)((tempMask >> digit & 1) != 0 ? 1 : 0))
								);
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
							}
						}

						var step = new UniqueSquareType3Step(
							conclusions.ToImmutableArray(),
							ImmutableArray.Create(new PresentationData
							{
								Candidates = candidateOffsets,
								Regions = new[] { (region, (ColorIdentifier)0) }
							}),
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

	private Step? CheckType4(
		ICollection<Step> accumulator,
		in Grid grid,
		bool onlyFindOne,
		in Cells pattern,
		short mask
	)
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
				tempMap |= CandMaps[digit];
			}
			if (tempMap.InOneRegion)
			{
				continue;
			}

			foreach (int region in tempMap.CoveredRegions)
			{
				int d1 = -1, d2 = -1, count = 0;
				var compareMap = RegionMaps[region] & pattern;
				foreach (int digit in digits)
				{
					if ((compareMap | RegionMaps[region] & CandMaps[digit]) == compareMap)
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
					foreach (int cell in compareMap & CandMaps[digit])
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, ColorIdentifier)>();
				foreach (int cell in pattern - compareMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
					}
				}
				foreach (int cell in compareMap & CandMaps[d1])
				{
					candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)1));
				}
				foreach (int cell in compareMap & CandMaps[d2])
				{
					candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)1));
				}

				var step = new UniqueSquareType4Step(
					conclusions.ToImmutableArray(),
					ImmutableArray.Create(new PresentationData
					{
						Candidates = candidateOffsets,
						Regions = new[] { (region, (ColorIdentifier)0) }
					}),
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
