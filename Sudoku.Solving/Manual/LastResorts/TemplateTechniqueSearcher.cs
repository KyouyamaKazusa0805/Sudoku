using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Runtime;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Checking;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>template</b> technique searcher.
	/// </summary>
	[DisplayLevel(3)]
	[TechniqueDisplay(nameof(TechniqueCode.TemplateSet))]
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
		public TemplateTechniqueSearcher(bool templateDeleteOnly) => _templateDeleteOnly = templateDeleteOnly;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55) { IsEnabled = false };


		/// <inheritdoc/>
		/// <exception cref="WrongHandlingException">
		/// Throws when the puzzle is not unique.
		/// </exception>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			_ = !grid.IsValid(out SudokuGrid solution) ? throw new WrongHandlingException(grid) : 0;

			if (!_templateDeleteOnly)
			{
				GetAllTemplateSet(accumulator, solution);
			}

			GetAllTemplateDelete(accumulator, solution);
		}


		/// <summary>
		/// Get all template sets.
		/// </summary>
		/// <param name="result">(<see langword="in"/> parameter) The result.</param>
		/// <param name="solution">The solution.</param>
		/// <returns>All template sets.</returns>
		private static void GetAllTemplateSet(IList<TechniqueInfo> result, in SudokuGrid solution)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				var map = CreateInstance(solution, digit);
				var resultMap = map & CandMaps[digit];
				var conclusions = new List<Conclusion>();
				foreach (int cell in resultMap)
				{
					conclusions.Add(new(Assignment, cell, digit));
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				result.Add(
					new TemplateTechniqueInfo(
						conclusions,
						new View[]
						{
							new((
								from conclusion in conclusions
								select new DrawingInfo(0, conclusion.Cell * 9 + conclusion.Digit)).ToArray())
						},
						false));
			}
		}

		/// <summary>
		/// Get all template deletes.
		/// </summary>
		/// <param name="result">(<see langword="in"/> parameter) The result.</param>
		/// <param name="solution">The solution.</param>
		/// <returns>All template deletes.</returns>
		private static void GetAllTemplateDelete(IList<TechniqueInfo> result, in SudokuGrid solution)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				var map = CreateInstance(solution, digit);
				var resultMap = CandMaps[digit] - map;
				var conclusions = new List<Conclusion>();
				foreach (int cell in resultMap)
				{
					conclusions.Add(new(Elimination, cell, digit));
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				result.Add(new TemplateTechniqueInfo(conclusions, View.DefaultViews, true));
			}
		}

		/// <summary>
		/// Create a <see cref="GridMap"/> instance with the specified solution.
		/// If the puzzle has been solved, this method will create a grid map of
		/// distribution of a single digit in this solution.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="digit">The digit to search.</param>
		/// <returns>
		/// The grid map that contains all cells of a digit appearing
		/// in the solution.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Throws when the puzzle has not been solved.
		/// </exception>
		private static GridMap CreateInstance(in SudokuGrid grid, int digit)
		{
			_ = grid.HasSolved ? 0 : throw new ArgumentException("The specified sudoku grid has not been solved.");

			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid[cell] == digit)
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}
	}
}
