namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.HighMemoryAllocation)]
internal sealed unsafe partial class GuardianStepSearcher : IGuardianStepSearcher
{
	private static readonly PatternOverlayStepSearcher ElimsSearcher = new();


	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		// Check POM eliminations first.
		scoped ref readonly var grid = ref context.Grid;
		scoped var eliminationMaps = (stackalloc CellMap[9]);
		eliminationMaps.Fill(CellMap.Empty);

		var pomSteps = new List<IStep>();
		scoped var pomContext = new LogicalAnalysisContext(pomSteps, grid, false);
		ElimsSearcher.GetAll(ref pomContext);

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
			if (eliminationMaps[digit] is not (var baseElimMap and not []))
			{
				// Using global view, we cannot find any eliminations for this digit.
				// Just skip to the next loop.
				continue;
			}

			var foundData = ICellLinkingLoopStepSearcher.GatherGuardianLoops(digit);
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
					candidateOffsets.Add(new(DisplayColorKind.Normal, c * 9 + digit));
				}
				foreach (var c in guardians)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, c * 9 + digit));
				}

				/**
					Add found step into the collection.
					To be honest I can return the step if <see cref="LogicalAnalysisContext.OnlyFindOne"/> is true,
					but due to the limit of the algorithm, the method always gets the longer guardian loops instead of shorter ones.
					If we just return the first found step, we will miss steps being more elegant.
				*/
				resultAccumulator.Add(
					new GuardianStep(
						from c in elimMap select new Conclusion(Elimination, c, digit),
						new[] { View.Empty | candidateOffsets },
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

		var tempAccumulator =
			from info in IDistinctableStep<GuardianStep>.Distinct(resultAccumulator)
			orderby info.Loop.Count, info.Guardians.Count
			select info;

		if (context.OnlyFindOne)
		{
			return tempAccumulator.First();
		}

		context.Accumulator.AddRange(tempAccumulator);

		return null;
	}
}
