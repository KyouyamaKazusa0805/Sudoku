using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving
{
	public abstract class TechniqueInfo
	{
		protected TechniqueInfo(ICollection<Conclusion> conclusions, ICollection<View> views) =>
			(Conclusions, Views) = (conclusions, views);


		public abstract string Name { get; }

		public abstract decimal Difficulty { get; }

		public abstract DifficultyLevels DifficultyLevel { get; }

		public ICollection<Conclusion> Conclusions { get; }

		public ICollection<View> Views { get; }


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
