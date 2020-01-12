using System.Collections.Generic;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Manual.Subsets
{
	public sealed class SubsetsStepFinder : StepFinder
	{
		public override IList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			result.AddRange(TakeAllBySize(grid, 2));
			result.AddRange(TakeAllBySize(grid, 3));
			result.AddRange(TakeAllBySize(grid, 4));

			return result;
		}

		private static IList<SubsetTechniqueInfo> TakeAllBySize(Grid grid, int size)
		{
			var result = new List<SubsetTechniqueInfo>();



			return result;
		}
	}
}
