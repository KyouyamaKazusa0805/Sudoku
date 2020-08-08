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
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
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

				CheckType1(accumulator, grid, pattern, mask);
				CheckType2(accumulator, pattern, mask);
				CheckType3(accumulator, grid, pattern, mask);
				CheckType4(accumulator, grid, pattern, mask);
			}
		}

		partial void CheckType1(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask);

		partial void CheckType2(IList<TechniqueInfo> accumulator, GridMap pattern, short mask);

		partial void CheckType3(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask);

		partial void CheckType4(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask);
	}
}
