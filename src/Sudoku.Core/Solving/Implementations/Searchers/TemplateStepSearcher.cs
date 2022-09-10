namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
internal sealed partial class TemplateStepSearcher : ITemplateStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool TemplateDeleteOnly { get; set; }

	/// <inheritdoc/>
	public Grid Solution { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Iterate on each digit.
		var distributedMapsByDigit = Solution.ValuesMap;
		for (var digit = 0; digit < 9; digit++)
		{
			if (!TemplateDeleteOnly)
			{
				// Check template sets.
				if ((distributedMapsByDigit[digit] & CandidatesMap[digit]) is not (var templateSetMap and not []))
				{
					continue;
				}

				var templateSetConclusions = from cell in templateSetMap select new Conclusion(Assignment, cell, digit);
				var candidateOffsets = new CandidateViewNode[templateSetConclusions.Length];
				var z = 0;
				foreach (var (_, candidate) in templateSetConclusions)
				{
					candidateOffsets[z++] = new(DisplayColorKind.Normal, candidate);
				}

				var templateSetStep = new TemplateStep(
					templateSetConclusions,
					ImmutableArray.Create(View.Empty | candidateOffsets),
					false
				);
				if (onlyFindOne)
				{
					return templateSetStep;
				}

				accumulator.Add(templateSetStep);
			}

			// Then check template deletes.
			if (CandidatesMap[digit] - distributedMapsByDigit[digit] is not (var templateDeleteMap and not []))
			{
				continue;
			}

			var templateDeleteStep = new TemplateStep(
				from cell in templateDeleteMap select new Conclusion(Elimination, cell, digit),
				ViewList.Empty,
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
