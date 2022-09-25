namespace Sudoku.Solving.Logical.Implementations.Searchers;

[StepSearcher]
[StepSearcherOptions(IsDirect = true)]
internal sealed partial class PatternOverlayStepSearcher : IPatternOverlayStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		var templates = IPatternOverlayStepSearcher.GetInvalidPos(context.Grid);
		for (var digit = 0; digit < 9; digit++)
		{
			if (templates[digit] is not (var template and not []))
			{
				continue;
			}

			var step = new PatternOverlayStep(from cell in template select new Conclusion(Elimination, cell, digit));
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}
}
