namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Guardian</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Guardian</item>
/// </list>
/// </summary>
[StepSearcher(Technique.BrokenWing, Flags = ConditionalFlags.TimeComplexity | ConditionalFlags.SpaceComplexity)]
public sealed partial class GuardianStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the base searcher.
	/// </summary>
	private static readonly PatternOverlayStepSearcher ElimsSearcher = new();


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Check POM eliminations first.
		scoped ref readonly var grid = ref context.Grid;
		scoped var eliminationMaps = (stackalloc CellMap[9]);
		eliminationMaps.Fill(CellMap.Empty);

		var pomSteps = new List<Step>();
		scoped var pomContext = new AnalysisContext(pomSteps, grid, false);
		ElimsSearcher.Collect(ref pomContext);

		foreach (var step in pomSteps.Cast<PatternOverlayStep>())
		{
			scoped ref var currentMap = ref eliminationMaps[step.Digit];
			foreach (var conclusion in step.Conclusions)
			{
				currentMap.Add(conclusion.Cell);
			}
		}

		var resultAccumulator = new List<GuardianStep>();
		for (var digit = 0; digit < 9; digit++)
		{
			if (!eliminationMaps[digit])
			{
				// Using global view, we cannot find any eliminations for this digit.
				// Just skip to the next loop.
				continue;
			}

			var foundData = Cached.GatherGuardianLoops(digit);
			if (foundData.Length == 0)
			{
				continue;
			}

			foreach (var (loop, guardians, _) in foundData)
			{
				if ((guardians.PeerIntersection & CandidatesMap[digit]) is not (var elimMap and not []))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var c in loop)
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, c * 9 + digit));
				}
				foreach (var c in guardians)
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, c * 9 + digit));
				}

				// Add found step into the collection.
				// To be honest I can return the step if 'LogicalAnalysisContext.OnlyFindOne' is true,
				// but due to the limit of the algorithm, the method always gets the longer guardian loops instead of shorter ones.
				// If we just return the first found step, we will miss steps being more elegant.
				resultAccumulator.Add(
					new GuardianStep(
						[.. from c in elimMap select new Conclusion(Elimination, c, digit)],
						[[.. candidateOffsets]],
						digit,
						loop,
						guardians
					)
				);
			}
		}

		if (resultAccumulator.Count == 0)
		{
			return null;
		}

		var tempAccumulator = from step in resultAccumulator.Distinct() orderby step.LoopCells.Count, step.Guardians.Count select step;
		if (context.OnlyFindOne)
		{
			return tempAccumulator.First();
		}

		context.Accumulator.AddRange(tempAccumulator);

		return null;
	}
}

/// <summary>
/// Represents a cached gathering operation set.
/// </summary>
file static unsafe class Cached
{
	/// <summary>
	/// Try to gather all possible loops which should satisfy the specified condition.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// </returns>
	public static Guardian[] GatherGuardianLoops(Digit digit)
	{
		var condition = (delegate*<in CellMap, bool>)&GuardianOrBivalueOddagonSatisfyingPredicate;

		var result = new List<Guardian>();
		foreach (var cell in CandidatesMap[digit])
		{
			DepthFirstSearching_Guardian(cell, cell, 0, CellsMap[cell], CellMap.Empty, digit, condition, result);
		}

		return [.. result.Distinct()];
	}

	/// <summary>
	/// Checks for guardian loops using recursion.
	/// </summary>
	private static void DepthFirstSearching_Guardian(
		Cell startCell,
		Cell lastCell,
		House lastHouse,
		scoped in CellMap currentLoop,
		scoped in CellMap currentGuardians,
		Digit digit,
		delegate*<in CellMap, bool> condition,
		List<Guardian> result
	)
	{
		foreach (var houseType in HouseTypes)
		{
			var house = lastCell.ToHouseIndex(houseType);
			if ((lastHouse >> house & 1) != 0)
			{
				continue;
			}

			var cellsToBeChecked = CandidatesMap[digit] & HousesMap[house];
			if (cellsToBeChecked.Count < 2 || (currentLoop & HousesMap[house]).Count > 2)
			{
				continue;
			}

			foreach (var tempCell in cellsToBeChecked)
			{
				if (tempCell == lastCell)
				{
					continue;
				}

				var housesUsed = 0;
				foreach (var tempHouseType in HouseTypes)
				{
					if (tempCell.ToHouseIndex(tempHouseType) == lastCell.ToHouseIndex(tempHouseType))
					{
						housesUsed |= 1 << lastCell.ToHouseIndex(tempHouseType);
					}
				}

				var tempGuardians = (CandidatesMap[digit] & HousesMap[house]) - tempCell - lastCell;
				if (tempCell == startCell && condition(currentLoop)
					&& !!((currentGuardians | tempGuardians).PeerIntersection & CandidatesMap[digit]))
				{
					result.Add(new(currentLoop, currentGuardians | tempGuardians, digit));

					// Exit the current of this recursion frame.
					return;
				}

				if ((currentLoop | currentGuardians).Contains(tempCell)
					|| !((currentGuardians | tempGuardians).PeerIntersection & CandidatesMap[digit])
					|| (HousesMap[house] & currentLoop).Count > 1)
				{
					continue;
				}

				DepthFirstSearching_Guardian(
					startCell, tempCell, lastHouse | housesUsed, currentLoop + tempCell,
					currentGuardians | tempGuardians, digit, condition, result
				);
			}
		}
	}

	/// <summary>
	/// Defines a templating method that can determine whether a loop is a valid guardian.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool GuardianOrBivalueOddagonSatisfyingPredicate(scoped in CellMap loop) => loop.Count is var l && (l & 1) != 0 && l >= 5;
}
