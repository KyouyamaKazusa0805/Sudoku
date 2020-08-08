using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Annotations;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.BdpType1))]
	[SearcherProperty(53)]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		/// <remarks>
		/// All possible heptagons and octagons are in here.
		/// </remarks>
		private static readonly Pattern[] Patterns = new Pattern[14580];


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			var funcs = new Action<IList<TechniqueInfo>, IReadOnlyGrid, Pattern, short, short, short, GridMap>[]
			{
				CheckType1,
				CheckType2,
				CheckType3,
				CheckType4
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

		partial void CheckType1(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);

		partial void CheckType2(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);

		partial void CheckType3(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);

		partial void CheckType4(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map);
	}
}
