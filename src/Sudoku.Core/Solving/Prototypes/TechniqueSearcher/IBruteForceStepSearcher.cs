namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Brute Force</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Brute Force</item>
/// </list>
/// </summary>
public interface IBruteForceStepSearcher : ILastResortStepSearcher, IStepSearcherRequiresSolution
{
}

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

		foreach (int offset in BruteForceTryAndErrorOrder)
		{
			if (grid.GetStatus(offset) == CellStatus.Empty)
			{
				int cand = offset * 9 + Solution[offset];
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
