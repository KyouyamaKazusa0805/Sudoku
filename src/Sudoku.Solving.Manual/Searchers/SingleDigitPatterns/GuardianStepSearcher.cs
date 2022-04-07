namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Guardian</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Guardian</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class GuardianStepSearcher : IGuardianStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// Check POM eliminations first.
		var eliminationMaps = stackalloc Cells[9];
		Unsafe.InitBlock(eliminationMaps, 0, (uint)sizeof(Cells) * 9);
		var pomSteps = new List<Step>();
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
			if (eliminationMaps[digit] is not { Count: not 0 } eliminations)
			{
				continue;
			}

			foreach (int elimination in eliminations)
			{
				var loops = new List<(Cells, Cells, IEnumerable<LinkViewNode>)>();
				var tempLoop = new List<int>();
				var globalMap = CandMaps[digit] - new Cells(elimination);
				foreach (int cell in globalMap)
				{
					var loopMap = Cells.Empty;
					loops.Clear();
					tempLoop.Clear();
					f(cell, (Region)byte.MaxValue, Cells.Empty);

					if (loops.Count == 0)
					{
						continue;
					}

					foreach (var (map, guardians, links) in loops)
					{
						if ((!guardians & CandMaps[digit]) is not { Count: not 0 } elimMap)
						{
							continue;
						}

						var conclusions = new Conclusion[elimMap.Count];
						int i = 0;
						foreach (int c in elimMap)
						{
							conclusions[i++] = new(ConclusionType.Elimination, c, digit);
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (int c in map)
						{
							candidateOffsets.Add(new(0, c * 9 + digit));
						}
						foreach (int c in guardians)
						{
							candidateOffsets.Add(new(1, c * 9 + digit));
						}

						var step = new GuardianStep(
							conclusions.ToImmutableArray(),
							ImmutableArray.Create(View.Empty + candidateOffsets + links),
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
					void f(int cell, Region lastRegion, Cells guardians)
					{
						loopMap.Add(cell);
						tempLoop.Add(cell);

						foreach (var region in Regions)
						{
							if (region == lastRegion)
							{
								continue;
							}

							int regionIndex = cell.ToRegionIndex(region);
							var otherCellsMap = RegionMaps[regionIndex] & globalMap - cell;
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
									region,
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
