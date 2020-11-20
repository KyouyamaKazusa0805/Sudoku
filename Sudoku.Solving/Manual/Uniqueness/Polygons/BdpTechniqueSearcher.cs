using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.BdpType1))]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// The function list.
		/// </summary>
		private static readonly unsafe delegate*<IList<TechniqueInfo>, in SudokuGrid, in Pattern, short, short, short, in GridMap, void>[] FunctionsList =
		{
			&CheckType1,
			&CheckType2,
			&CheckType3,
			&CheckType4
		};

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
		public override unsafe void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			for (int i = 0, end = EmptyMap.Count == 7 ? 14580 : 11664; i < end; i++)
			{
				var pattern = Patterns[i];
				if ((EmptyMap | pattern.Map) != EmptyMap)
				{
					// The pattern contains non-empty cells.
					continue;
				}

				short cornerMask1 = grid.BitwiseOrMasks(pattern.Pair1Map);
				short cornerMask2 = grid.BitwiseOrMasks(pattern.Pair2Map);
				short centerMask = grid.BitwiseOrMasks(pattern.CenterCellsMap);
				var map = pattern.Map;
				for (int j = 0; j < 4; j++)
				{
					FunctionsList[j](accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask, map);
				}
			}
		}

		private static partial void CheckType1(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in Pattern pattern, short cornerMask1, short cornerMask2, short centerMask, in GridMap map);
		private static partial void CheckType2(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in Pattern pattern, short cornerMask1, short cornerMask2, short centerMask, in GridMap map);
		private static partial void CheckType3(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in Pattern pattern, short cornerMask1, short cornerMask2, short centerMask, in GridMap map);
		private static partial void CheckType4(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in Pattern pattern, short cornerMask1, short cornerMask2, short centerMask, in GridMap map);
	}
}
