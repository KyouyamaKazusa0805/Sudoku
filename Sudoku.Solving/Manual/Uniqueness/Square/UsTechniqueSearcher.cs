using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.UsType1))]
	public sealed partial class UsTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the patterns.
		/// </summary>
		private static readonly GridMap[] Patterns = new GridMap[162];


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(53);


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			foreach (var pattern in Patterns)
			{
				if ((EmptyMap | pattern) != EmptyMap)
				{
					continue;
				}

				short mask = 0;
				foreach (int cell in pattern)
				{
					mask |= grid.GetCandidateMask(cell);
				}

				var funcs = stackalloc delegate* managed<
					IList<TechniqueInfo>, in SudokuGrid, in GridMap, short, void>[]
				{
					&CheckType1,
					&CheckType2,
					&CheckType3,
					&CheckType4
				};
				for (int i = 0; i < 4; i++)
				{
					funcs[i](accumulator, grid, pattern, mask);
				}
			}
		}

		private static partial void CheckType1(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);

		private static partial void CheckType2(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);

		private static partial void CheckType3(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);

		private static partial void CheckType4(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);
	}
}
