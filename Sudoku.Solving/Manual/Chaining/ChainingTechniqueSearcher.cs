using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using System.Collections.Generic;
using System.Linq;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a <b>chain</b> technique searcher.
	/// </summary>
	public abstract class ChainingTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Get all available weak links.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="p">(<see langword="in"/> parameter) The current node.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <returns>All possible weak links.</returns>
		protected internal static ISet<Node> GetOnToOff(in SudokuGrid grid, in Node p, bool yEnabled)
		{
			var result = new Set<Node>();

			if (yEnabled)
			{
				// First rule: Other candidates for this cell get off.
				for (int digit = 0; digit < 9; digit++)
				{
					if (digit != p.Digit && grid.Exists(p.Cell, digit) is true)
					{
						result.Add(new(p.Cell, digit, false, p));
					}
				}
			}

			// Second rule: Other positions for this digit get off.
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = GetRegion(p.Cell, label);
				for (int pos = 0; pos < 9; pos++)
				{
					int cell = RegionCells[region][pos];
					if (cell != p.Cell && grid.Exists(cell, p.Digit) is true)
					{
						result.Add(new(cell, p.Digit, false, p));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get all available strong links.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="p">(<see langword="in"/> parameter) The current node.</param>
		/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <returns>All possible strong links.</returns>
		protected internal static ISet<Node> GetOffToOn(
			in SudokuGrid grid, in Node p, bool xEnabled, bool yEnabled)
		{
			var result = new Set<Node>();
			if (yEnabled)
			{
				// First rule: If there's only two candidates in this cell, the other one gets on.
				if (BivalueMap[p.Cell] &&
					(short)(grid.GetCandidateMask(p.Cell) & ~(1 << p.Digit)) is var mask && mask.IsPowerOfTwo())
				{
					var pOn = new Node(p.Cell, mask.FindFirstSet(), true, p);
					//AddHiddenParentsOfCell(pOn, grid, offNodes);
					result.Add(pOn);
				}
			}

			if (xEnabled)
			{
				// Second rule: If there's only two positions for this candidate, the other ont gets on.
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = GetRegion(p.Cell, label);
					if (new GridMap(CandMaps[p.Digit] & RegionMaps[region]) { ~p.Cell } is { Count: 1 } cells)
					{
						var pOn = new Node(cells.First, p.Digit, true, p);
						//AddHiddenParentsOfRegion(pOn, region, offNodes);
						result.Add(pOn);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Remove duplicate information instances and sort them.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns>The result list.</returns>
		protected static IQueryable<ChainingTechniqueInfo> SortInfo(
			IEnumerable<ChainingTechniqueInfo> accumulator) =>
		(
			from info in new Set<ChainingTechniqueInfo>(accumulator)
			orderby info.Difficulty, info.Complexity, info.SortKey
			select info
		).AsQueryable();
	}
}
