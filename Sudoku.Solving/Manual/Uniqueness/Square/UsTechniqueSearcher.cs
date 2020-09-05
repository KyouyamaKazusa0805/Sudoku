using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.UsType1))]
	[SearcherProperty(53)]
	public sealed partial class UsTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the patterns.
		/// </summary>
		private static readonly GridMap[] Patterns = new GridMap[162];


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
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
					foreach (var f in new delegate*<IList<TechniqueInfo>, Grid, GridMap, short, void>[]
					{
						&CheckType1, &CheckType2, &CheckType3, &CheckType4
					})
					{
						f(accumulator, grid, pattern, mask);
					}
				}
			}
		}

		private static partial void CheckType1(IList<TechniqueInfo> accumulator, Grid grid, GridMap pattern, short mask);

		private static partial void CheckType2(IList<TechniqueInfo> accumulator, Grid grid, GridMap pattern, short mask);

		private static partial void CheckType3(IList<TechniqueInfo> accumulator, Grid grid, GridMap pattern, short mask);

		private static partial void CheckType4(IList<TechniqueInfo> accumulator, Grid grid, GridMap pattern, short mask);
	}
}
