namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Remote Pair</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Remote Pair</item>
/// <item>Complex Remote Pair</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_RemotePairStepSearcher", Technique.RemotePair, Technique.ComplexRemotePair)]
public sealed partial class RemotePairStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		// Try to collect digits that contain at least one unfilled state.
		var mask = (Mask)0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (CandidatesMap[digit])
			{
				mask |= (Mask)(1 << digit);
			}
		}

		// Iterate on each pair of digits, to check whether a remote pair pattern can be formed.
		var accumulator = new HashSet<RemotePairStep>();
		ref readonly var grid = ref context.Grid;
		for (var d1 = 0; d1 < 8; d1++)
		{
			for (var d2 = d1 + 1; d2 < 9; d2++)
			{
				var suchCells = BivalueCells & CandidatesMap[d1] & CandidatesMap[d2];
				if (suchCells.Count < 4)
				{
					continue;
				}

				// Iterate on each component of the graph.
				var pairMask = (Mask)(1 << d1 | 1 << d2);
				var graph = new UndirectedCellGraph(in suchCells);
				var components = graph.Components;
				var lastCells = suchCells;
				while (lastCells)
				{
					foreach (var componentStartCell in lastCells.ToArray())
					{
						var component = components.First(graph => graph.Contains(componentStartCell));

						// Iterate on cells whose degree is 1.
						foreach (var cellDegree1 in component.GetCellsOfDegree(1))
						{
							graph.GetComponentOf(cellDegree1, out var depths);
							var nodeGroups =
								from depth in depths
								let depthKey = (depth.Depth & 1) == 1
								group depth by depthKey into depthGroup
								let depthCells = (from depth in depthGroup select depth.Cell).AsCellMap()
								select (DepthValueIsOdd: depthGroup.Key, Cells: depthCells);

							// There must be 2 cases. Now we should iterate two collections to get contradiction.
							var conflictedCells = CellMap.Empty;
							var conflictedPair = new List<(Cell Left, Cell Right)>();
							foreach (var cell1 in nodeGroups[0].Cells)
							{
								foreach (var cell2 in nodeGroups[1].Cells)
								{
									var intersection = (cell1.AsCellMap() + cell2).PeerIntersection;
									var currentConflictCells = intersection & (CandidatesMap[d1] | CandidatesMap[d2]);
									if (currentConflictCells)
									{
										conflictedPair.Add((cell1, cell2));
										conflictedCells |= currentConflictCells;
									}
								}
							}
							if (!conflictedCells)
							{
								// There're no cells can be used as elimination.
								continue;
							}

							var conclusions = new List<Conclusion>();
							foreach (var cell in conflictedCells)
							{
								if ((grid.GetCandidates(cell) >> d1 & 1) != 0)
								{
									conclusions.Add(new(Elimination, cell, d1));
								}
								if ((grid.GetCandidates(cell) >> d2 & 1) != 0)
								{
									conclusions.Add(new(Elimination, cell, d2));
								}
							}
							if (conclusions.Count == 0)
							{
								// There're no eliminations found.
								continue;
							}

							var candidateOffsets = new List<CandidateViewNode>();
							var cellOffsets = new HashSet<CellViewNode>();
							var babaGroupingOffsets = new List<BabaGroupViewNode>();
							foreach (var cell in component.Map)
							{
								candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + d1));
								candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + d2));
							}
							foreach (var (c1, c2) in conflictedPair)
							{
								cellOffsets.Add(new(ColorIdentifier.Auxiliary1, c1));
								cellOffsets.Add(new(ColorIdentifier.Auxiliary1, c2));
							}
							foreach (var (isMarkX, cells) in nodeGroups)
							{
								foreach (var cell in cells)
								{
									babaGroupingOffsets.Add(new(cell, isMarkX ? 'x' : 'y', pairMask));
								}
							}

							var step = new RemotePairStep(
								[.. conclusions],
								[[.. candidateOffsets, .. cellOffsets], [.. cellOffsets, .. babaGroupingOffsets]],
								context.Options,
								in component.Map,
								nodeGroups[0].Cells.Count != nodeGroups[1].Cells.Count,
								pairMask
							);
							if (context.OnlyFindOne)
							{
								return step;
							}

							accumulator.Add(step);
						}

						// Break out of the loop, and iterate other cells which are not inside the current component.
						lastCells &= ~component.Map;
						break;
					}
				}
			}
		}

		if (!context.OnlyFindOne && accumulator.Count != 0)
		{
			context.Accumulator.AddRange(accumulator);
		}
		return null;
	}
}
