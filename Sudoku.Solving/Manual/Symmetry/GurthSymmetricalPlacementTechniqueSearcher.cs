using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a Gurth's symmetrical placement technique searcher.
	/// </summary>
	public sealed partial class GurthSymmetricalPlacementTechniqueSearcher : SymmetryTechniqueSearcher
	{
		/// <inheritdoc/>
		public override int Priority => 0;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<GurthSymmetricalPlacementTechniqueInfo>();

			CheckCentral(result, grid);
			// TODO: Check diagonal|anti-diagonal.
			// TODO: Check X-axis|Y-axis.

			return result;
		}


		#region Gsp utils
		private static void CheckCentral(
			IList<GurthSymmetricalPlacementTechniqueInfo> result, Grid grid)
		{
			int?[] mapping = new int?[9];
			if (grid.GetCellStatus(40) != CellStatus.Empty)
			{
				// Has no conclusion even though the grid may be symmetrical.
				return;
			}

			for (int cell = 0; cell < 40; cell++)
			{
				int anotherCell = 80 - cell;
				if (grid.GetCellStatus(cell) == CellStatus.Empty
					^ grid.GetCellStatus(anotherCell) == CellStatus.Empty)
				{
					// One of two cell is empty, not central symmetrical type.
					return;
				}

				if (grid.GetCellStatus(cell) != CellStatus.Empty)
				{
					if (grid.GetCellStatus(anotherCell) == CellStatus.Empty)
					{
						return;
					}

					int d1 = grid[cell], d2 = grid[anotherCell];
					if (d1 == d2)
					{
						int? o1 = mapping[d1];
						if (o1 is null)
						{
							mapping[d1] = d1;
							continue;
						}

						if (o1 != d1)
						{
							return;
						}
					}
					else
					{
						int? o1 = mapping[d1], o2 = mapping[d2];
						if (o1 is null ^ o2 is null)
						{
							return;
						}

						if (o1 is null && o2 is null)
						{
							mapping[d1] = d2;
							mapping[d2] = d1;
							continue;
						}

						// 'o1' and 'o2' are both not null.
						if (o1 != d2 || o2 != d1)
						{
							return;
						}
					}
				}
			}

			for (int i = 0; i < 9; i++)
			{
				if (mapping[i] == i || mapping[i] is null)
				{
					result.Add(
						new GurthSymmetricalPlacementTechniqueInfo(
							conclusions: new[]
							{
								new Conclusion(ConclusionType.Assignment, 360 + i)
							},
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets: null,
									regionOffsets: null,
									linkMasks: null)
							},
							symmetricalType: SymmetricalType.Central,
							mappingTable: mapping));

					return;
				}
			}
		}
		#endregion
	}
}
