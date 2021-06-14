using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>template</b> technique searcher.
	/// </summary>
	public sealed class TemplateStepSearcher : LastResortStepSearcher
	{
		/// <summary>
		/// Indicates the solution.
		/// </summary>
		private readonly SudokuGrid _solution;


		/// <summary>
		/// Initializes an instance with the specified solution.
		/// </summary>
		/// <param name="solution">The solution.</param>
		public TemplateStepSearcher(in SudokuGrid solution) => _solution = solution;


		/// <summary>
		/// Indicates whether the technique searcher only checks template deletes.
		/// </summary>
		public bool TemplateDeleteOnly { get; init; }


		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		public static TechniqueProperties Properties { get; } = new(21, nameof(Technique.TemplateSet))
		{
			DisplayLevel = 3,
			IsEnabled = false,
			DisabledReason = DisabledReason.LastResort
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Iterate on each digit.
			for (int digit = 0; digit < 9; digit++)
			{
				if (!TemplateDeleteOnly)
				{
					// Check template sets.
					var templateSetMap = _solution.ValuesMap[digit] & CandMaps[digit];
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
							false));
				}

				// Then check template deletes.
				var templateDeleteMap = CandMaps[digit] - _solution.ValuesMap[digit];
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
