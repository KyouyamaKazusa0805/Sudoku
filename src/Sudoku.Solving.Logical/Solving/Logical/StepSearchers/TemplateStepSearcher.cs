﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherMetadata(PuzzleNotRelying = true)]
internal sealed partial class TemplateStepSearcher : ITemplateStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool TemplateDeleteOnly { get; set; }

	/// <inheritdoc/>
	public Grid Solution { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
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

				var templateSetStep = new TemplateStep(templateSetConclusions, new[] { View.Empty | candidateOffsets }, false);
				if (context.OnlyFindOne)
				{
					return templateSetStep;
				}

				context.Accumulator.Add(templateSetStep);
			}

			// Then check template deletes.
			if (CandidatesMap[digit] - distributedMapsByDigit[digit] is not (var templateDelete and not []))
			{
				continue;
			}

			var templateDeleteStep = new TemplateStep(from cell in templateDelete select new Conclusion(Elimination, cell, digit), null, true);
			if (context.OnlyFindOne)
			{
				return templateDeleteStep;
			}

			context.Accumulator.Add(templateDeleteStep);
		}

		return null;
	}
}
