namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Multi-sector Locked Sets</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Multi-sector Locked Sets</item>
/// </list>
/// </summary>
[StepSearcher(Technique.MultisectorLockedSets)]
[StepSearcherRuntimeName("StepSearcherName_MultisectorLockedSetsStepSearcher")]
public sealed partial class MultisectorLockedSetsStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the list initialized with the static constructor.
	/// </summary>
	private static readonly CellMap[] Patterns;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static MultisectorLockedSetsStepSearcher()
	{
		const HouseMask a = ~7, b = ~56, c = ~448;
		var sizeList = (int[][])[[3, 3], [3, 4], [4, 3], [4, 4], [4, 5], [5, 4]];
		var z = (int[])[0, 1, 2, 3, 4, 5, 6, 7, 8];
		var result = new CellMap[74601];
		var n = 0;
		for (var i = 0; i < sizeList.Length; i++)
		{
			var (rows, columns) = (sizeList[i][0], sizeList[i][1]);
			foreach (var rowList in z.GetSubsets(rows))
			{
				var (rowMask, rowMap) = ((Mask)0, CellMap.Empty);
				foreach (var row in rowList)
				{
					rowMask |= (Mask)(1 << row);
					rowMap |= HousesMap[row + 9];
				}

				if ((rowMask & a) == 0 || (rowMask & b) == 0 || (rowMask & c) == 0)
				{
					continue;
				}

				foreach (var columnList in z.GetSubsets(columns))
				{
					var (columnMask, columnMap) = ((Mask)0, CellMap.Empty);
					foreach (var column in columnList)
					{
						columnMask |= (Mask)(1 << column);
						columnMap |= HousesMap[column + 18];
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
	protected internal override unsafe Step? Collect(scoped ref AnalysisContext context)
	{
		scoped var linkForEachHouse = (stackalloc Mask[27]);
		linkForEachHouse.Clear();

		scoped var linkForEachDigit = (stackalloc CellMap[9]);
		scoped var canL = (stackalloc CellMap[9]);
		scoped ref readonly var grid = ref context.Grid;
		foreach (var pattern in Patterns)
		{
			var map = EmptyCells & pattern;
			if (pattern.Count < 12 && pattern.Count - map.Count > 1 || pattern.Count - map.Count > 2)
			{
				continue;
			}

			var n = 0;
			var count = map.Count;
			for (var digit = 0; digit < 9; digit++)
			{
				scoped ref var tempMap = ref linkForEachDigit[digit];
				tempMap = CandidatesMap[digit] & map;
				n += MathExtensions.Min(PopCount((uint)tempMap.RowMask), PopCount((uint)tempMap.ColumnMask), PopCount((uint)tempMap.BlockMask));
			}

			if (n == count)
			{
				canL.Clear();

				var conclusions = new List<Conclusion>();
				var candidateOffsets = new List<CandidateViewNode>();
				for (var digit = 0; digit < 9; digit++)
				{
					var q = (Mask)(1 << digit);
					var currentMap = linkForEachDigit[digit];
					var rMask = (uint)currentMap.RowMask;
					var cMask = (uint)currentMap.ColumnMask;
					var bMask = (uint)currentMap.BlockMask;
					var temp = MathExtensions.Min(PopCount(rMask), PopCount(cMask), PopCount(bMask));
					var elimMap = CellMap.Empty;
					var check = 0;
					if (PopCount(rMask) == temp)
					{
						check++;
						foreach (var i in rMask)
						{
							var house = i + 9;
							linkForEachHouse[house] |= q;
							elimMap |= (CandidatesMap[digit] & HousesMap[house] & map).PeerIntersection;
						}
					}
					if (PopCount(cMask) == temp)
					{
						check++;
						foreach (var i in cMask)
						{
							var house = i + 18;
							linkForEachHouse[house] |= q;
							elimMap |= (CandidatesMap[digit] & HousesMap[house] & map).PeerIntersection;
						}
					}
					if (PopCount(bMask) == temp)
					{
						check++;
						foreach (var i in bMask)
						{
							linkForEachHouse[i] |= q;
							elimMap |= (CandidatesMap[digit] & HousesMap[i] & map).PeerIntersection;
						}
					}

					elimMap &= CandidatesMap[digit];
					if (!elimMap)
					{
						continue;
					}

					foreach (var cell in elimMap)
					{
						if (map.Contains(cell))
						{
							canL[digit].Add(cell);
						}

						conclusions.Add(new(Elimination, cell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				for (var house = 0; house < 27; house++)
				{
					var linkMask = linkForEachHouse[house];
					if (linkMask == 0)
					{
						continue;
					}

					foreach (var cell in map & HousesMap[house])
					{
						var cands = (Mask)(grid.GetCandidates(cell) & linkMask);
						if (cands == 0)
						{
							continue;
						}

						foreach (var cand in cands)
						{
							if (!canL[cand].Contains(cell))
							{
								candidateOffsets.Add(
									new(
										house switch
										{
											< 9 => WellKnownColorIdentifier.Auxiliary2,
											< 18 => WellKnownColorIdentifier.Normal,
											_ => WellKnownColorIdentifier.Auxiliary1
										},
										cell * 9 + cand
									)
								);
							}
						}
					}
				}

				var step = new MultisectorLockedSetsStep([.. conclusions], [[.. candidateOffsets]], context.PredefinedOptions, in map);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}
}
