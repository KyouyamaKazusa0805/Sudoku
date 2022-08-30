namespace Sudoku.Solving.Manual.Searchers;

using GatheredData = Dictionary</*Cell*/ int, Dictionary</*Digit*/ int, List<AlmostLockedSet>>>;

/// <summary>
/// Provides with a <b>Death Blossom</b> step searcher.
/// <!--
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Death Blossom Cell Type</item>
/// <item>Death Blossom Region Type (House Type)</item>
/// </list>
/// -->
/// </summary>
public interface IDeathBlossomStepSearcher : IAlmostLockedSetsStepSearcher
{
	/// <summary>
	/// <para>Gathers almost locked sets and groups them by cell related.</para>
	/// <para>
	/// For example, <c>r1c12(123)</c> is an almost locked set, and cells <c>{ r1c3, r23c123 }</c>
	/// can be related to that ALS.
	/// </para>
	/// </summary>
	/// <param name="grid">The grid used.</param>
	/// <returns>The dictionary of grouped result.</returns>
	protected internal static sealed GatheredData GatherGroupedByCell(scoped in Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new GatheredData();
		foreach (int cell in BivalueCells)
		{
			var als = new AlmostLockedSet(grid.GetCandidates(cell), CellMap.Empty + cell, PeersMap[cell] & EmptyCells);
			foreach (int peerCell in PeersMap[cell])
			{
				append(als, peerCell, result);
			}
		}

		// Get all non-bi-value-cell ALSes.
		for (int houseIndex = 0; houseIndex < 27; houseIndex++)
		{
			if ((HousesMap[houseIndex] & EmptyCells) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (int size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					short blockMask = map.BlockMask;
					if (IsPow2(blockMask) && houseIndex >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					short digitsMask = 0;
					foreach (int cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					int coveredLine = map.CoveredLine;

					var als = new AlmostLockedSet(
						digitsMask,
						map,
						houseIndex < 9 && coveredLine is >= 9 and not InvalidValidOfTrailingZeroCountMethodFallback
							? ((HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells) - map
							: tempMap - map
					);
					foreach (int digit in digitsMask)
					{
						foreach (int peerCell in map % CandidatesMap[digit])
						{
							append(als, peerCell, result);
						}
					}
				}
			}
		}

		return result;


		static void append(AlmostLockedSet structure, int cell, GatheredData gatheredData)
		{
			foreach (int digit in structure.DigitsMask)
			{
				if (gatheredData.TryGetValue(cell, out var structuresGroupedByCell))
				{
					if (structuresGroupedByCell.TryGetValue(digit, out var structuresGroupedByCandidate))
					{
						structuresGroupedByCandidate.Add(structure);
					}
					else
					{
						structuresGroupedByCell.Add(digit, new() { structure });
					}
				}
				else
				{
					gatheredData.Add(cell, new() { { digit, new() { structure } } });
				}
			}
		}
	}
}

[StepSearcher]
internal sealed partial class DeathBlossomStepSearcher : IDeathBlossomStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
#if false
		var dic = IDeathBlossomStepSearcher.GatherGroupedByCell(grid);

		// Cell Type.
		foreach (int hubCell in EmptyCells)
		{
			if (IsPow2(grid.GetCandidates(hubCell)))
			{
				// Optimization: If the cell only contains one candidate, it will be a naked single. We should ignore it.
				continue;
			}

			if (!dic.TryGetValue(hubCell, out var alsGroupedByCell))
			{
				// The current cell does not contain any possible related ALS structures.
				continue;
			}

			short digitsMask = grid.GetCandidates(hubCell);
			var alsesGroupedByDigit = new List<AlmostLockedSet[]>(PopCount((uint)digitsMask));
			bool containsDigitDoesNotContainRelatedAlses = false;
			foreach (int digit in digitsMask)
			{
				if (!alsGroupedByCell.TryGetValue(digit, out var list)/* || list.Count == 0*/)
				{
					containsDigitDoesNotContainRelatedAlses = true;
					break;
				}

				alsesGroupedByDigit.Add(list.ToArray());
			}
			if (containsDigitDoesNotContainRelatedAlses)
			{
				continue;
			}

			foreach (var combination in Combinatorics.GetExtractedCombinations(alsesGroupedByDigit.ToArray()))
			{
				bool isDuplicated = false;
				for (int i = 0; i < combination.Length - 1; i++)
				{
					var a = combination[i];
					for (int j = i + 1; j < combination.Length; j++)
					{
						var b = combination[j];
						switch (a, b)
						{
							case ({ Map: [var aa] }, { Map: [var bb] })
							when aa.ToHouseIndex(HouseType.Block) == bb.ToHouseIndex(HouseType.Block)
								|| aa.ToHouseIndex(HouseType.Row) == bb.ToHouseIndex(HouseType.Row)
								|| aa.ToHouseIndex(HouseType.Column) == bb.ToHouseIndex(HouseType.Column):
							case ({ House: var ah }, { House: var bh }) when ah == bh:
							case (_, _) when a == b:
							{
								isDuplicated = true;
								goto CheckDuplicate;
							}
						}
					}
				}
			CheckDuplicate:
				if (isDuplicated)
				{
					continue;
				}

				short elimDigitsMask = Grid.MaxCandidatesMask;
				foreach (var als in combination)
				{
					elimDigitsMask &= als.DigitsMask;
				}
				elimDigitsMask &= (short)~grid.GetCandidates(hubCell);

				if (elimDigitsMask == 0)
				{
					// No digits can be eliminated.
					continue;
				}

				var conclusions = new List<Conclusion>();
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int elimDigit in elimDigitsMask)
				{
					var cellsHavingElimDigit = CellMap.Empty;
					foreach (var als in combination)
					{
						cellsHavingElimDigit |= als.Map & CandidatesMap[elimDigit];
					}

					if (((CandidatesMap[elimDigit] & +cellsHavingElimDigit) - hubCell) is not (var elimMap and not []))
					{
						// No eliminations found.
						continue;
					}

					foreach (int elimCell in elimMap)
					{
						conclusions.Add(new(Elimination, elimCell, elimDigit));
					}
					foreach (int cellHavingElimDigit in cellsHavingElimDigit)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cellHavingElimDigit * 9 + elimDigit));
					}
				}
				if (conclusions.Count == 0)
				{
					// No eliminations found.
					continue;
				}

				byte alsIdentifier = 0;
				foreach (var als in combination)
				{
					foreach (int alsCell in als.Map)
					{
						foreach (int digit in (short)(grid.GetCandidates(alsCell) & ~elimDigitsMask))
						{
							candidateOffsets.Add(new((DisplayColorKind)alsIdentifier, alsCell * 9 + digit));
						}
					}

					alsIdentifier = (byte)(DisplayColorKind.AlmostLockedSet1 + (byte)((alsIdentifier + 1) % 5));
				}

				var step = new DeathBlossomCellTypeStep(
					conclusions.ToImmutableArray(),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new CellViewNode(DisplayColorKind.Normal, hubCell)
					),
					hubCell,
					elimDigitsMask,
					combination
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}
#endif

		return null;
	}
}
