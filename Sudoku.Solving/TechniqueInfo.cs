using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics.CodeAnalysis;
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


		[OnDeconstruction]
		public void Deconstruct(out string name, out decimal difficulty) =>
			(name, difficulty) = (Name, Difficulty);

		[OnDeconstruction]
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevels difficultyLevel) =>
			(name, difficulty, difficultyLevel) = (Name, Difficulty, DifficultyLevel);

		[OnDeconstruction]
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevels difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions) =>
			(name, difficulty, difficultyLevel, conclusions) = (Name, Difficulty, DifficultyLevel, Conclusions);
		
		[OnDeconstruction]
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevels difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions, out IReadOnlyList<View> views) =>
			(name, difficulty, difficultyLevel, conclusions, views) = (Name, Difficulty, DifficultyLevel, Conclusions, Views);

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
