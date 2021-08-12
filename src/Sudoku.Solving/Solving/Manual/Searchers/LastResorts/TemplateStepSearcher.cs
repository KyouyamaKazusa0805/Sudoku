namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>template</b> technique searcher.
	/// </summary>
	public sealed class TemplateStepSearcher : LastResortStepSearcher
	{
		/// <summary>
		/// Initializes a <see cref="TemplateStepSearcher"/> instance with no arguments.
		/// </summary>
		public TemplateStepSearcher()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified solution.
		/// </summary>
		/// <param name="solution">The solution.</param>
		public TemplateStepSearcher(in SudokuGrid solution) => Solution = solution;


		/// <summary>
		/// Indicates whether the technique searcher only checks template deletes.
		/// </summary>
		public bool TemplateDeleteOnly { get; set; }

		/// <summary>
		/// Indicates the solution grid.
		/// </summary>
		public SudokuGrid Solution { get; set; }

		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(
			21, DisplayingLevel.C,
			EnabledAreas: EnabledAreas.None,
			DisabledReason: DisabledReason.LastResort
		);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(21, nameof(Technique.TemplateSet))
		{
			DisplayLevel = 3,
			IsEnabled = false,
			DisabledReason = DisabledReason.LastResort
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			if (Solution == SudokuGrid.Undefined)
			{
				return;
			}

			// Iterate on each digit.
			for (int digit = 0; digit < 9; digit++)
			{
				if (!TemplateDeleteOnly)
				{
					// Check template sets.
					var templateSetMap = Solution.ValuesMap[digit] & CandMaps[digit];
					if (templateSetMap.IsEmpty)
					{
						continue;
					}

					var templateSetConclusions = new List<Conclusion>();
					foreach (int cell in templateSetMap)
					{
						templateSetConclusions.Add(new(ConclusionType.Assignment, cell, digit));
					}

					var candidateOffsets = new DrawingInfo[templateSetConclusions.Count];
					int i = 0;
					foreach (var (_, candidate) in templateSetConclusions)
					{
						candidateOffsets[i++] = new(0, candidate);
					}

					accumulator.Add(
						new TemplateStepInfo(
							templateSetConclusions,
							new View[] { new() { Candidates = candidateOffsets } },
							false
						)
					);
				}

				// Then check template deletes.
				var templateDeleteMap = CandMaps[digit] - Solution.ValuesMap[digit];
				if (templateDeleteMap.IsEmpty)
				{
					continue;
				}

				var templateDeleteConclusions = new List<Conclusion>();
				foreach (int cell in templateDeleteMap)
				{
					templateDeleteConclusions.Add(new(ConclusionType.Elimination, cell, digit));
				}

				accumulator.Add(new TemplateStepInfo(templateDeleteConclusions, new View[] { new() }, true));
			}
		}
	}
}
