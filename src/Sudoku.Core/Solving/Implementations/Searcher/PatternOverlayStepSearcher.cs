namespace Sudoku.Solving.Implementations.Searcher;

[StepSearcher]
[StepSearcherOptions(IsDirect = true)]
internal sealed partial class PatternOverlayStepSearcher : IPatternOverlayStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var templates = IPatternOverlayStepSearcher.GetInvalidPos(grid);
		for (int digit = 0; digit < 9; digit++)
		{
			if (templates[digit] is not (var template and not []))
			{
				continue;
			}

			var step = new PatternOverlayStep(from cell in template select new Conclusion(Elimination, cell, digit));
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}
}
