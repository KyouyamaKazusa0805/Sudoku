using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Chaining
{
	/// <summary>
	/// Encapsulates an alternating inference chain (AIC) technique searcher.
	/// </summary>
	public sealed class AlernatingInferenceChainTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <inheritdoc/>
		public override int Priority { get; set; } = 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on each strong relation, and search for weak relations.
			var undoable = (UndoableGrid)grid;
			foreach (var strongRelation in GetAllStrongRelations(undoable))
			{
				// TODO: Get off to on (two cases).
			}
		}


		/// <summary>
		/// Get all strong relations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All strong relations.</returns>
		private static IReadOnlyList<FullGridMap> GetAllStrongRelations(IReadOnlyGrid grid)
		{
			var result = new List<FullGridMap>();
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					short mask = grid.GetDigitAppearingMask(digit, region);
					if (mask.CountSet() != 2)
					{
						continue;
					}

					int pos1 = mask.FindFirstSet();
					result.Add(new FullGridMap
					{
						[RegionUtils.GetCellOffset(region, pos1) * 9 + digit] = true,
						[RegionUtils.GetCellOffset(region, mask.GetNextSetBit(pos1)) * 9 + digit] = true
					});
				}
			}
			for (int cell = 0; cell < 81; cell++)
			{
				if (!grid.IsBivalueCell(cell))
				{
					continue;
				}

				short mask = grid.GetCandidatesReversal(cell);
				int digit1 = mask.FindFirstSet();
				result.Add(new FullGridMap
				{
					[cell * 9 + digit1] = true,
					[cell * 9 + mask.GetNextSetBit(digit1)] = true
				});
			}

			return result;
		}
	}
}
