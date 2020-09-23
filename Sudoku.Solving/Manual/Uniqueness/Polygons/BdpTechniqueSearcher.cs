using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.BdpType1))]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		/// <remarks>
		/// All possible heptagons and octagons are in here.
		/// </remarks>
		private static readonly Pattern[] Patterns = new Pattern[14580];


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(53);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			unsafe
			{
				var funcs = new delegate*<IList<TechniqueInfo>, Grid, Pattern, short, short, short, GridMap, void>[]
				{
					&CheckType1,
					&CheckType2,
					&CheckType3,
					&CheckType4
				};

				for (int i = 0, end = EmptyMap.Count == 7 ? 14580 : 11664; i < end; i++)
				{
					var pattern = Patterns[i];
					if ((EmptyMap | pattern.Map) != EmptyMap)
					{
						// The pattern contains non-empty cells.
						continue;
					}

					short cornerMask1 = BitwiseOrMasks(grid, pattern.Pair1Map);
					short cornerMask2 = BitwiseOrMasks(grid, pattern.Pair2Map);
					short centerMask = BitwiseOrMasks(grid, pattern.CenterCellsMap);
					var map = pattern.Map;

					foreach (var func in funcs)
					{
						func(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask, map);
					}
				}
			}
		}

		private static partial void CheckType1(
			IList<TechniqueInfo> accumulator, Grid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);

		private static partial void CheckType2(
			IList<TechniqueInfo> accumulator, Grid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);

		private static partial void CheckType3(
			IList<TechniqueInfo> accumulator, Grid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);

		private static partial void CheckType4(
			IList<TechniqueInfo> accumulator, Grid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);
	}
}
