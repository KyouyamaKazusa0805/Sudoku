using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	public sealed partial class UsTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// The function list.
		/// </summary>
		private static readonly unsafe delegate*<IList<TechniqueInfo>, in SudokuGrid, in GridMap, short, void>[] FunctionList =
		{
			&CheckType1,
			&CheckType2,
			&CheckType3,
			&CheckType4
		};

		/// <summary>
		/// Indicates the patterns.
		/// </summary>
		private static readonly GridMap[] Patterns = new GridMap[162];


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(53, nameof(TechniqueCode.UsType1)) { DisplayLevel = 2 };


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
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

				unsafe
				{
					foreach (var act in FunctionList)
					{
						act(accumulator, grid, pattern, mask);
					}
				}
			}
		}

		private static partial void CheckType1(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);
		private static partial void CheckType2(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);
		private static partial void CheckType3(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);
		private static partial void CheckType4(IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask);
	}
}
