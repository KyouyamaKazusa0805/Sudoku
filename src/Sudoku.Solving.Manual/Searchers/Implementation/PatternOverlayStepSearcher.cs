namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
[StepSearcherOptions(IsDirect = true)]
internal sealed unsafe partial class PatternOverlayStepSearcher : IPatternOverlayStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var templates = IPatternOverlayStepSearcher.GetInvalidPos(grid);
		for (int digit = 0; digit < 9; digit++)
		{
			if (templates[digit] is not { Count: var templateCount and not 0 } template)
			{
				continue;
			}

			var conclusions = new Conclusion[templateCount];
			int i = 0;
			foreach (int cell in template)
			{
				conclusions[i++] = new(ConclusionType.Elimination, cell, digit);
			}

			var step = new PatternOverlayStep(ImmutableArray.Create(conclusions));
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}
}
