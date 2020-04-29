using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Checking;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>template</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Template")]
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
		/// Indicates whether the technique searcher checks template deletes only.
		/// </param>
		public TemplateTechniqueSearcher(bool templateDeleteOnly) =>
			_templateDeleteOnly = templateDeleteOnly;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 80;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = false;


		/// <inheritdoc/>
		/// <exception cref="WrongHandlingException">
		/// Throws when the puzzle is not unique.
		/// </exception>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (grid.IsValid(out var solution))
			{
				(_, _, var candMaps, _) = grid;
				if (!_templateDeleteOnly)
				{
					GetAllTemplateSet(accumulator, solution, candMaps);
				}
				GetAllTemplateDelete(accumulator, solution, candMaps);
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
				var map = CreateInstance(solution, digit);
				var resultMap = map & digitDistributions[digit];
				var conclusions = new List<Conclusion>();
				foreach (int cell in resultMap.Offsets)
				{
					conclusions.Add(new Conclusion(Assignment, cell, digit));
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
										from conclusion in conclusions
										select (0, conclusion.CellOffset * 9 + conclusion.Digit)),
								regionOffsets: null,
								links: null)
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
				var map = CreateInstance(solution, digit);
				var resultMap = digitDistributions[digit] - map;
				var conclusions = new List<Conclusion>();
				foreach (int cell in resultMap.Offsets)
				{
					conclusions.Add(new Conclusion(Elimination, cell, digit));
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				result.Add(
					new TemplateTechniqueInfo(
						conclusions,
						views: new[] { new View() },
						isTemplateDeletion: true));
			}
		}

		/// <summary>
		/// Create a <see cref="GridMap"/> instance with the specified solution.
		/// If the puzzle has been solved, this method will create a grid map of
		/// distribution of a single digit in this solution.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit to search.</param>
		/// <returns>
		/// The grid map that contains all cells of a digit appearing
		/// in the solution.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Throws when the puzzle has not been solved.
		/// </exception>
		private static GridMap CreateInstance(IReadOnlyGrid grid, int digit)
		{
			if (!grid.HasSolved)
			{
				throw new ArgumentException($"The specified sudoku grid has not been solved.");
			}

			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid[cell] == digit)
				{
					result.Add(cell);
				}
			}
			return result;
		}
	}
}
