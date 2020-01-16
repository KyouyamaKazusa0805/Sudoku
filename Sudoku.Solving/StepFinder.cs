using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	public abstract class StepFinder
	{
		public void RecordSteps(Grid grid, ref List<TechniqueInfo> accumulator) =>
			accumulator.AddRange(TakeAll(grid));

		public TechniqueInfo? TakeOne(Grid grid) => TakeAll(grid).FirstOrDefault();

		public IReadOnlyList<TechniqueInfo> Take(Grid grid, int count)
		{
			Contract.Assume(count >= 1);

			// 'Take' method will never throw exceptions when
			// count is greater than the step count of the list.
			return TakeAll(grid).Take(count).ToList();
		}

		public abstract IReadOnlyList<TechniqueInfo> TakeAll(Grid grid);
	}
}
