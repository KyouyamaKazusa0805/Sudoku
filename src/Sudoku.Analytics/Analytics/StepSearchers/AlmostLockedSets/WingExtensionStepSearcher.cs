namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Wing Extension</b> (Extended Subset Principle) step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>WXYZ-Wing Extension</item>
/// <item>VWXYZ-Wing Extension</item>
/// <item>UVWXYZ-Wing Extension</item>
/// <item>TUVWXYZ-Wing Extension</item>
/// <item>STUVWXYZ-Wing Extension</item>
/// <item>RSTUVWXYZ-Wing Extension</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.WxyzWingExtension, Technique.VwxyzWingExtension, Technique.UvwxyzWingExtension,
	Technique.TuvwxyzWingExtension, Technique.StuvwxyzWingExtension, Technique.RstuvwxyzWingExtension)]
[StepSearcherRuntimeName("StepSearcherName_WingExtensionStepSearcher")]
public sealed partial class WingExtensionStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Must starts with an extended WXYZ-Wing (4 pattern cells and at least one elimination).
		if (EmptyCells.Count < 5)
		{
			return null;
		}

		// A valid ESP must starts with a locked candidate, in order to make no duplicate.
		scoped ref readonly var grid = ref context.Grid;
		var list = new List<CellMap>(7);
		var results = new HashSet<CellMap>();
		foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
		{
			var emptyCellsInInterMap = c & EmptyCells;

			// Add all combinations into the collection.
			list.Clear();
			list.AddRangeRef(emptyCellsInInterMap.GetAllSubsets());

			// Iterate on each intersection combination.
			foreach (var currentInterMap in list)
			{
				var selectedInterMask = grid[in currentInterMap];
				if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
				{
					// The intersection combination is an ALS or a normal subset, which is invalid in ESPs.
					continue;
				}

				var blockMap = (b | c - currentInterMap) & EmptyCells;
				var lineMap = a & EmptyCells;

				// Iterate on the number of the cells that should be selected in block.
				for (var i = 1; i < blockMap.Count; i++)
				{
					// Iterate on each combination in block.
					foreach (ref readonly var currentBlockMap in blockMap.GetSubsets(i))
					{
						// Iterate on the number of the cells that should be selected in line.
						var blockMask = grid[in currentBlockMap];
						for (var j = 1; j <= 9 - i - currentInterMap.Count && j <= lineMap.Count; j++)
						{
							// Iterate on each combination in line.
							foreach (ref readonly var currentLineMap in lineMap.GetSubsets(j))
							{
								var lineMask = grid[in currentLineMap];
								var zDigitsMask = (Mask)(blockMask & lineMask);
								if (!IsPow2(zDigitsMask))
								{
									continue;
								}

								var isolatedDigitsMask = (Mask)(selectedInterMask & ~(blockMask | lineMask));
								var p = PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)isolatedDigitsMask);
								if (currentInterMap.Count + i + j != p - 1
									|| Log2((uint)zDigitsMask) is not (var zDigit and not TrailingZeroCountFallback))
								{
									// Invalid.
									continue;
								}

								// Check for elimination.
								var pattern = currentBlockMap | currentLineMap | currentInterMap;
								var elimMap = pattern % CandidatesMap[zDigit];
								if (!elimMap)
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
												digit == zDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
												cell * 9 + digit
											)
										);
									}
								}

								if (results.Add(pattern))
								{
									var step = new WingExtensionStep(
										[.. from cell in elimMap select new Conclusion(Elimination, cell, zDigit)],
										[
											[
												.. candidateOffsets,
												new HouseViewNode(WellKnownColorIdentifier.Normal, coverSet),
												new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, baseSet)
											]
										],
										context.PredefinedOptions,
										in pattern,
										(Mask)(blockMask | lineMask),
										zDigit
									);
									if (context.OnlyFindOne)
									{
										return step;
									}

									context.Accumulator.Add(step);
								}
							}
						}
					}
				}
			}
		}

		return null;
	}
}
