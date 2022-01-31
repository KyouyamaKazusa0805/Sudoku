using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Steps;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Multi-sector Locked Sets</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Multi-sector Locked Sets</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class MultisectorLockedSetsStepSearcher : IMultisectorLockedSetsStepSearcher
{
	/// <summary>
	/// Indicates the list initialized with the static constructor.
	/// </summary>
	private static readonly IReadOnlyList<Cells> Patterns;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static MultisectorLockedSetsStepSearcher()
	{
		const int a = ~7, b = ~56, c = ~448;
		int[,] sizeList = { { 3, 3 }, { 3, 4 }, { 4, 3 }, { 4, 4 }, { 4, 5 }, { 5, 4 } };
		int[] z = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		var result = new Cells[74601];
		int n = 0;
		for (int i = 0, iterationLength = sizeList.Length >> 1; i < iterationLength; i++)
		{
			int rows = sizeList[i, 0], columns = sizeList[i, 1];
			foreach (int[] rowList in z.GetSubsets(rows))
			{
				short rowMask = 0;
				var rowMap = Cells.Empty;
				foreach (int row in rowList)
				{
					rowMask |= (short)(1 << row);
					rowMap |= RegionMaps[row + 9];
				}

				if ((rowMask & a) == 0 || (rowMask & b) == 0 || (rowMask & c) == 0)
				{
					continue;
				}

				foreach (int[] columnList in z.GetSubsets(columns))
				{
					short columnMask = 0;
					var columnMap = Cells.Empty;
					foreach (int column in columnList)
					{
						columnMask |= (short)(1 << column);
						columnMap |= RegionMaps[column + 18];
					}

					if ((columnMask & a) == 0 || (columnMask & b) == 0 || (columnMask & c) == 0)
					{
						continue;
					}

					result[n++] = rowMap & columnMap;
				}
			}
		}

		Patterns = result;
	}


	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(33, DisplayingLevel.D);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		short* linkForEachRegion = stackalloc short[27];
		var linkForEachDigit = stackalloc Cells[9];
		for (int globalIndex = 0, iterationCount = Patterns.Count; globalIndex < iterationCount; globalIndex++)
		{
			Cells pattern = Patterns[globalIndex], map = EmptyMap & pattern;
			if (pattern.Count < 12 && pattern.Count - map.Count > 1 || pattern.Count - map.Count > 2)
			{
				continue;
			}

			int n = 0, count = map.Count;
			for (int digit = 0; digit < 9; digit++)
			{
				var pMap = linkForEachDigit + digit;
				*pMap = CandMaps[digit] & map;
				n += MathExtensions.Min(
					PopCount((uint)pMap->RowMask),
					PopCount((uint)pMap->ColumnMask),
					PopCount((uint)pMap->BlockMask)
				);
			}

			if (n == count)
			{
				var canL = new Cells[9];
				var conclusions = new List<Conclusion>();
				var candidateOffsets = new List<(int, ColorIdentifier)>();
				for (int digit = 0; digit < 9; digit++)
				{
					short q = (short)(1 << digit);
					var currentMap = linkForEachDigit[digit];
					uint
						rMask = (uint)currentMap.RowMask,
						cMask = (uint)currentMap.ColumnMask,
						bMask = (uint)currentMap.BlockMask;
					int temp = MathExtensions.Min(PopCount(rMask), PopCount(cMask), PopCount(bMask));
					var elimMap = Cells.Empty;
					int check = 0;
					if (PopCount(rMask) == temp)
					{
						check++;
						foreach (int i in rMask)
						{
							int region = i + 9;
							linkForEachRegion[region] |= q;
							elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
						}
					}
					if (PopCount(cMask) == temp)
					{
						check++;
						foreach (int i in cMask)
						{
							int region = i + 18;
							linkForEachRegion[region] |= q;
							elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
						}
					}
					if (PopCount(bMask) == temp)
					{
						check++;
						foreach (int i in bMask)
						{
							linkForEachRegion[i] |= q;
							elimMap |= (CandMaps[digit] & RegionMaps[i] & map).PeerIntersection;
						}
					}

					elimMap &= CandMaps[digit];
					if (elimMap.IsEmpty)
					{
						continue;
					}

					foreach (int cell in elimMap)
					{
						if (map.Contains(cell))
						{
							canL[digit].AddAnyway(cell);
						}

						conclusions.Add(new(ConclusionType.Elimination, cell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				for (int region = 0; region < 27; region++)
				{
					short linkMask = linkForEachRegion[region];
					if (linkMask == 0)
					{
						continue;
					}

					foreach (int cell in map & RegionMaps[region])
					{
						short cands = (short)(grid.GetCandidates(cell) & linkMask);
						if (cands == 0)
						{
							continue;
						}

						foreach (int cand in cands)
						{
							if (!canL[cand].Contains(cell))
							{
								candidateOffsets.Add(
									(
										cell * 9 + cand,
										(ColorIdentifier)(region switch { < 9 => 2, < 18 => 0, _ => 1 })
									)
								);
							}
						}
					}
				}

				var step = new MultisectorLockedSetsStep(
					conclusions.ToImmutableArray(),
					ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
					map
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
