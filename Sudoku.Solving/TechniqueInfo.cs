using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving
{
	public abstract class TechniqueInfo
	{
		protected TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views) =>
			(Conclusions, Views) = (conclusions, views);


		public abstract string Name { get; }

		public abstract decimal Difficulty { get; }

		public abstract DifficultyLevels DifficultyLevel { get; }

		public IReadOnlyList<Conclusion> Conclusions { get; }

		public IReadOnlyList<View> Views { get; }


		public void ApplyTo(Grid grid)
		{
			foreach (var conclusion in Conclusions)
			{
				conclusion.ApplyTo(grid);
			}
		}

		public abstract override string ToString();
	}
}
