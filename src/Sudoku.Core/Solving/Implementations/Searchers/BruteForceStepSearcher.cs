namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
[StepSearcherOptions(IsOptionsFixed = true, IsDirect = true)]
internal sealed unsafe partial class BruteForceStepSearcher : IBruteForceStepSearcher
{
	/// <inheritdoc/>
	public Grid Solution { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (Solution.IsUndefined)
		{
			goto ReturnNull;
		}

		foreach (var offset in BruteForceTryAndErrorOrder)
		{
			if (grid.GetStatus(offset) == CellStatus.Empty)
			{
				var cand = offset * 9 + Solution[offset];
				var step = new BruteForceStep(
					ImmutableArray.Create(new Conclusion(Assignment, cand)),
					ImmutableArray.Create(View.Empty | new CandidateViewNode(DisplayColorKind.Normal, cand))
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

	ReturnNull:
		return null;
	}
}
