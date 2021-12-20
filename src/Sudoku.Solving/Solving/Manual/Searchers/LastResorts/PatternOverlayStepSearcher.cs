namespace Sudoku.Solving.Manual.Searchers.LastResorts;

/// <summary>
/// Provides with a <b>Pattern Overlay</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Pattern Overlay</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class PatternOverlayStepSearcher : IPatternOverlayStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } =
		new(20, DisplayingLevel.C, EnabledAreas: EnabledAreas.Gathering, DisabledReason: DisabledReason.LastResort);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var templates = IPatternOverlayStepSearcher.GetInvalidPos(grid);
		for (int digit = 0; digit < 9; digit++)
		{
			if (templates[digit] is not { IsEmpty: false } template)
			{
				continue;
			}

			var conclusions = new Conclusion[template.Count];
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
