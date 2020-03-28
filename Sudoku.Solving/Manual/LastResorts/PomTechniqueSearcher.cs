using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using static Sudoku.Solving.Utils.PatternOverlayMethodUtils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>pattern overlay method</b> (POM) technique searcher.
	/// </summary>
	[TechniqueDisplay("Pattern Overlay Method")]
	public sealed class PomTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 80;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = false;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				int count = 0;
				var valueCells = new List<int>();
				for (int cell = 0; cell < 81; cell++)
				{
					if (grid.GetCellStatus(cell) != CellStatus.Empty && grid[cell] == digit)
					{
						valueCells.Add(cell);
						count++;
					}
				}

				if (count == 9)
				{
					// All placements of a single digit are values.
					continue;
				}

				// Enumerate on all 46656 templates.
				// 46656 = 9 * 6 * 3 * 6 * 4 * 2 * 3 * 2 * 1.
				var filters = new List<GridMap>();
				foreach (var template in Templates)
				{
					var templateCells = template.Offsets;
					if (valueCells.Any(c => !templateCells.Contains(c)))
					{
						continue;
					}

					if (templateCells.Any(c => grid.GetCellStatus(c) != CellStatus.Empty && grid[c] != digit))
					{
						continue;
					}

					filters.Add(template);
				}

				// Iterate on filters.
				var unionMap = GridMap.Empty;
				foreach (var filter in filters)
				{
					unionMap |= filter;
				}

				// Search for all uncover cells.
				var conclusions = new List<Conclusion>();
				foreach (int cell in (~unionMap).Offsets)
				{
					if (grid.GetCellStatus(cell) != CellStatus.Empty || grid[cell, digit])
					{
						continue;
					}

					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, cell, digit));
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				accumulator.Add(
					new PomTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: null,
								regionOffsets: null,
								links: null)
						}));
			}
		}
	}
}
