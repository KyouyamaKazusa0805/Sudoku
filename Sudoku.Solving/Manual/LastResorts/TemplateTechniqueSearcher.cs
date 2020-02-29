using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a template technique searcher.
	/// </summary>
	public sealed class TemplateTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the searcher checks template deletes.
		/// </summary>
		private readonly bool _templateDeleteOnly;


		/// <summary>
		/// Initializes an instance with the specified <see cref="bool"/> value.
		/// </summary>
		/// <param name="templateDeleteOnly">
		/// Indicates whether the technique searcher checks template deletes.
		/// </param>
		public TemplateTechniqueSearcher(bool templateDeleteOnly) =>
			_templateDeleteOnly = templateDeleteOnly;


		/// <inheritdoc/>
		public override int Priority => 80;


		/// <inheritdoc/>
		/// <exception cref="WrongHandlingException">
		/// Throws when the puzzle is not unique.
		/// </exception>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (grid.IsUnique(out var solution))
			{
				(_, _, var digitDistributions) = grid;
				if (!_templateDeleteOnly)
				{
					GetAllTemplateSet(accumulator, solution, digitDistributions);
				}
				GetAllTemplateDelete(accumulator, solution, digitDistributions);
			}
			else
			{
				throw new WrongHandlingException(grid);
			}
		}


		/// <summary>
		/// Get all template sets.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="digitDistributions">All digit distributions.</param>
		/// <returns>All template sets.</returns>
		private static void GetAllTemplateSet(
			IBag<TechniqueInfo> result, IReadOnlyGrid solution, GridMap[] digitDistributions)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				var map = GridMap.CreateInstance(solution, digit);
				var resultMap = map & digitDistributions[digit];
				var conclusions = new List<Conclusion>();
				foreach (int cell in resultMap.Offsets)
				{
					conclusions.Add(new Conclusion(ConclusionType.Assignment, cell, digit));
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				result.Add(
					new TemplateTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets:
									new List<(int, int)>(
										from cand in conclusions
										select (0, cand.CellOffset * 9 + cand.Digit)),
								regionOffsets: null,
								linkMasks: null)
						},
						isTemplateDeletion: false));
			}
		}

		/// <summary>
		/// Get all template deletes.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="digitDistributions">All digit distributions.</param>
		/// <returns>All template deletes.</returns>
		private static void GetAllTemplateDelete(
			IBag<TechniqueInfo> result, IReadOnlyGrid solution, GridMap[] digitDistributions)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				var map = GridMap.CreateInstance(solution, digit);
				var resultMap = digitDistributions[digit] - map;
				var conclusions = new List<Conclusion>();
				foreach (int cell in resultMap.Offsets)
				{
					conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, digit));
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				result.Add(
					new TemplateTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets:
									new List<(int, int)>(
										from cand in conclusions
										select (0, cand.CellOffset * 9 + cand.Digit)),
								regionOffsets: null,
								linkMasks: null)
						},
						isTemplateDeletion: true));
			}
		}
	}
}
