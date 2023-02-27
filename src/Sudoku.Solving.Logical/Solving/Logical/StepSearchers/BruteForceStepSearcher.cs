namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherMetadata(IsOptionsFixed = true, IsDirect = true)]
internal sealed partial class BruteForceStepSearcher : IBruteForceStepSearcher
{
	/// <inheritdoc/>
	public Grid Solution { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		if (Solution.IsUndefined)
		{
			goto ReturnNull;
		}

		scoped ref readonly var grid = ref context.Grid;
		foreach (var offset in BruteForceTryAndErrorOrder)
		{
			if (grid.GetStatus(offset) == CellStatus.Empty)
			{
				var cand = offset * 9 + Solution[offset];
				var step = new BruteForceStep(
					ImmutableArray.Create(new Conclusion(Assignment, cand)),
					ImmutableArray.Create(View.Empty | new CandidateViewNode(DisplayColorKind.Normal, cand))
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

	ReturnNull:
		return null;
	}
}
