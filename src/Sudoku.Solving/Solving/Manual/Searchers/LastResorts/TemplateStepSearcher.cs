namespace Sudoku.Solving.Manual.Searchers.LastResorts;

/// <summary>
/// Provides with a <b>Template</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Template Set</item>
/// <item>Template Delete</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class TemplateStepSearcher : ITemplateStepSearcher
{
	/// <inheritdoc/>
	public bool TemplateDeleteOnly { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(21, DisplayingLevel.C)
	{
		EnabledAreas = EnabledAreas.None,
		DisabledReason = DisabledReason.LastResort
	};

	/// <inheritdoc/>
	public Grid* Solution { get; set; }

	/// <inheritdoc/>
	public delegate*<in Grid, bool> Predicate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => null;
	}


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
				if ((distributedMapsByDigit[digit] & CandMaps[digit]) is not { IsEmpty: false } templateSetMap)
				{
					continue;
				}

				int templateSetIndex = 0;
				var templateSetConclusions = new Conclusion[templateSetMap.Count];
				foreach (int cell in templateSetMap)
				{
					templateSetConclusions[templateSetIndex++] = new(ConclusionType.Assignment, cell, digit);
				}

				var candidateOffsets = new (int, ColorIdentifier)[templateSetConclusions.Length];
				int z = 0;
				foreach (var (_, candidate) in templateSetConclusions)
				{
					candidateOffsets[z++] = (candidate, (ColorIdentifier)0);
				}

				var templateSetStep = new TemplateStep(
					ImmutableArray.Create(templateSetConclusions),
					ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
					false
				);
				if (onlyFindOne)
				{
					return templateSetStep;
				}

				accumulator.Add(templateSetStep);
			}

			// Then check template deletes.
			if (CandMaps[digit] - distributedMapsByDigit[digit] is not { IsEmpty: false } templateDeleteMap)
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
				ImmutableArray.Create<PresentationData>(),
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
