namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Guardian</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Guardian</item>
/// </list>
/// </summary>
public interface IGuardianStepSearcher : INegativeRankStepSearcher
{
	/// <summary>
	/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
	/// </summary>
	/// <param name="cells">The list of cells.</param>
	/// <param name="offset">The offset. The default value is 4.</param>
	/// <returns>All links.</returns>
	protected static sealed IEnumerable<LinkViewNode> GetLinks(IReadOnlyList<int> cells, int offset = 4)
	{
		var result = new List<LinkViewNode>();

		for (int i = 0, length = cells.Count - 1; i < length; i++)
		{
			result.Add(
				new(
					DisplayColorKind.Normal,
					new(offset, Cells.Empty + cells[i]),
					new(offset, Cells.Empty + cells[i + 1]),
					Inference.Default
				)
			);
		}

		result.Add(
			new(
				DisplayColorKind.Normal,
				new(offset, Cells.Empty + cells[^1]),
				new(offset, Cells.Empty + cells[0]),
				Inference.Default
			)
		);

		return result;
	}

	/// <summary>
	/// Create the guardian map.
	/// </summary>
	/// <param name="cell1">The first cell.</param>
	/// <param name="cell2">The second cell.</param>
	/// <param name="digit">The current digit.</param>
	/// <param name="guardians">
	/// The current guardian cells.
	/// This map may not contain cells that lies in the house
	/// that <paramref name="cell1"/> and <paramref name="cell2"/> both lies in.
	/// </param>
	/// <returns>All guardians.</returns>
	protected static sealed Cells CreateGuardianMap(int cell1, int cell2, int digit, scoped in Cells guardians)
	{
		var tempMap = Cells.Empty;
		foreach (int coveredHouse in (Cells.Empty + cell1 + cell2).CoveredHouses)
		{
			tempMap |= HouseMaps[coveredHouse];
		}

		tempMap &= CandidatesMap[digit];
		tempMap |= guardians;

		return tempMap - cell1 - cell2;
	}
}

[StepSearcher]
internal sealed unsafe partial class GuardianStepSearcher : IGuardianStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Check POM eliminations first.
		var eliminationMaps = stackalloc Cells[9];
		Unsafe.InitBlock(eliminationMaps, 0, (uint)sizeof(Cells) * 9);
		var pomSteps = new List<IStep>();
		new PatternOverlayStepSearcher().GetAll(pomSteps, grid, onlyFindOne: false);
		foreach (PatternOverlayStep step in pomSteps)
		{
			var pMap = eliminationMaps + step.Digit;
			foreach (var conclusion in step.Conclusions)
			{
				pMap->Add(conclusion.Cell);
			}
		}

		var resultAccumulator = new List<GuardianStep>();
		for (int digit = 0; digit < 9; digit++)
		{
			if (eliminationMaps[digit] is not (var eliminations and not []))
			{
				continue;
			}

			foreach (int elimination in eliminations)
			{
				var loops = new List<(Cells, Cells, IEnumerable<LinkViewNode>)>();
				var tempLoop = new List<int>();
				var globalMap = CandidatesMap[digit] - (PeerMaps[elimination] + elimination);
				foreach (int cell in globalMap)
				{
					var loopMap = Cells.Empty;
					loops.Clear();
					tempLoop.Clear();
					f(cell, (HouseType)byte.MaxValue, Cells.Empty);

					if (loops.Count == 0)
					{
						continue;
					}

					foreach (var (map, guardians, links) in loops)
					{
						if ((!guardians & CandidatesMap[digit]) is not (var elimMap and not []))
						{
							continue;
						}

						var conclusions = new Conclusion[elimMap.Count];
						int i = 0;
						foreach (int c in elimMap)
						{
							conclusions[i++] = new(Elimination, c, digit);
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (int c in map)
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, c * 9 + digit));
						}
						foreach (int c in guardians)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, c * 9 + digit));
						}

						var step = new GuardianStep(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(View.Empty | candidateOffsets | links),
							digit,
							map,
							guardians
						);
						if (onlyFindOne)
						{
							return step;
						}

						resultAccumulator.Add(step);
					}


					// This function is used for recursion.
					// You can't change it to the static local function or normal methods,
					// because it'll cause stack-overflowing.
					// One example is:
					// 009050007060030080000009200100700800002400005080000040010820600000010000300007010
					void f(int cell, HouseType lastHouseType, Cells guardians)
					{
						loopMap.Add(cell);
						tempLoop.Add(cell);

						foreach (var houseType in HouseTypes)
						{
							if (houseType == lastHouseType)
							{
								continue;
							}

							int houseIndex = cell.ToHouseIndex(houseType);
							var otherCellsMap = HouseMaps[houseIndex] & globalMap - cell;
							if (otherCellsMap is not [var anotherCell])
							{
								continue;
							}

							if (tempLoop.Count >= 5 && (tempLoop.Count & 1) != 0 && tempLoop[0] == anotherCell)
							{
								loops.Add(
									(
										loopMap,
										IGuardianStepSearcher.CreateGuardianMap(cell, anotherCell, digit, guardians),
										IGuardianStepSearcher.GetLinks(tempLoop)
									)
								);
							}
							else if (!loopMap.Contains(anotherCell))
							{
								f(
									anotherCell,
									houseType,
									IGuardianStepSearcher.CreateGuardianMap(cell, anotherCell, digit, guardians)
								);
							}
						}

						loopMap -= cell;
						tempLoop.RemoveAt(tempLoop.Count - 1);
					}
				}
			}
		}

		if (resultAccumulator.Count == 0)
		{
			goto ReturnNull;
		}

		accumulator.AddRange(
			from info in IDistinctableStep<GuardianStep>.Distinct(resultAccumulator)
			orderby info.Loop.Count, info.Guardians.Count
			select info
		);

	ReturnNull:
		return null;
	}
}
