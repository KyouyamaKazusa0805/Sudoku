namespace Sudoku.Solving.Implementations.Searcher;

[StepSearcher]
internal sealed unsafe partial class GuardianStepSearcher : IGuardianStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Check POM eliminations first.
		scoped var eliminationMaps = (stackalloc CellMap[9]);
		eliminationMaps.Fill(CellMap.Empty);
		var pomSteps = new List<IStep>();
		new PatternOverlayStepSearcher().GetAll(pomSteps, grid, onlyFindOne: false);
		foreach (var step in pomSteps.Cast<PatternOverlayStep>())
		{
			scoped ref var currentMap = ref eliminationMaps[step.Digit];
			foreach (var conclusion in step.Conclusions)
			{
				currentMap.Add(conclusion.Cell);
			}
		}

		var resultAccumulator = new List<GuardianStep>();
		for (int digit = 0; digit < 9; digit++)
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
				if ((+guardians & CandidatesMap[digit]) is not (var elimMap and not []))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int c in loop)
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, c * 9 + digit));
				}
				foreach (int c in guardians)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, c * 9 + digit));
				}

				// Add found step into the collection.
				// To be honest I can return the step if 'onlyFindOne' is true,
				// but due to the limit of the algorithm, the method always gets the longer guardian loops
				// instead of shorter ones.
				// If we just return the first found step, we will miss steps being more elegant.
				resultAccumulator.Add(
					new GuardianStep(
						from c in elimMap select new Conclusion(Elimination, c, digit),
						ImmutableArray.Create(View.Empty | candidateOffsets),
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

		var tempCollection = IDistinctableStep<GuardianStep>.Distinct(
			(
				from info in resultAccumulator
				orderby info.Loop.Count, info.Guardians.Count
				select info
			).ToList()
		);
		if (onlyFindOne)
		{
			return tempCollection.First();
		}

		accumulator.AddRange(tempCollection);

		return null;
	}
}
