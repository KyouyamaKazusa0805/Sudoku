namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Template</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Template Set</item>
/// <item>Template Delete</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class TemplateStepSearcher : ITemplateStepSearcher
{
	/// <inheritdoc/>
	public bool TemplateDeleteOnly { get; set; }

	/// <inheritdoc/>
	public Grid* Solution { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// Iterate on each digit.
		var distributedMapsByDigit = Solution->ValuesMap;
		for (int digit = 0; digit < 9; digit++)
		{
			if (!TemplateDeleteOnly)
			{
				// Check template sets.
				if ((distributedMapsByDigit[digit] & CandMaps[digit]) is not { Count: not 0 } templateSetMap)
				{
					continue;
				}

				int templateSetIndex = 0;
				var templateSetConclusions = new Conclusion[templateSetMap.Count];
				foreach (int cell in templateSetMap)
				{
					templateSetConclusions[templateSetIndex++] = new(ConclusionType.Assignment, cell, digit);
				}

				var candidateOffsets = new CandidateViewNode[templateSetConclusions.Length];
				int z = 0;
				foreach (var (_, candidate) in templateSetConclusions)
				{
					candidateOffsets[z++] = new(0, candidate);
				}

				var templateSetStep = new TemplateStep(
					ImmutableArray.Create(templateSetConclusions),
					ImmutableArray.Create(View.Empty + candidateOffsets),
					false
				);
				if (onlyFindOne)
				{
					return templateSetStep;
				}

				accumulator.Add(templateSetStep);
			}

			// Then check template deletes.
			if (CandMaps[digit] - distributedMapsByDigit[digit] is not { Count: not 0 } templateDeleteMap)
			{
				continue;
			}

			var templateDeleteConclusions = new Conclusion[templateDeleteMap.Count];
			int templateDeleteIndex = 0;
			foreach (int cell in templateDeleteMap)
			{
				templateDeleteConclusions[templateDeleteIndex++] = new(ConclusionType.Elimination, cell, digit);
			}

			var templateDeleteStep = new TemplateStep(
				ImmutableArray.Create(templateDeleteConclusions),
				ImmutableArray<View>.Empty,
				true
			);
			if (onlyFindOne)
			{
				return templateDeleteStep;
			}

			accumulator.Add(templateDeleteStep);
		}

		return null;
	}
}
