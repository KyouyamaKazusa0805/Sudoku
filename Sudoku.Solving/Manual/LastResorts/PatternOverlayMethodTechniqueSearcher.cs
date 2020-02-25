using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using static Sudoku.Solving.Utils.PatternOverlayMethodUtils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a pattern overlay method (POM) technique searcher.
	/// </summary>
	[Slow]
	public sealed class PatternOverlayMethodTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <inheritdoc/>
		public override int Priority => 80;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<PatternOverlayMethodTechniqueInfo>();

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

				result.Add(
					new PatternOverlayMethodTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: null,
								regionOffsets: null,
								linkMasks: null)
						}));
			}

			return result;
		}
	}
}
