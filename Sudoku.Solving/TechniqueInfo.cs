using System;
using System.Collections.Generic;
using System.Globalization;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.Drawing;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates all information after searched a solving step,
	/// which include the conclusion, the difficulty and so on.
	/// </summary>
	public abstract class TechniqueInfo
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views of this solving step.</param>
		protected TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views) =>
			(Conclusions, Views) = (conclusions, views);


		/// <summary>
		/// Indicates the technique name.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// The difficulty or this step.
		/// </summary>
		public abstract decimal Difficulty { get; }

		/// <summary>
		/// The difficulty level of this step.
		/// </summary>
		public abstract DifficultyLevels DifficultyLevel { get; }

		/// <summary>
		/// All conclusions found in this technique step.
		/// </summary>
		public IReadOnlyList<Conclusion> Conclusions { get; }

		/// <summary>
		/// All views to display on the GUI.
		/// </summary>
		public IReadOnlyList<View> Views { get; }


		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="name">(out parameter) The name.</param>
		/// <param name="difficulty">(out parameter) The difficulty.</param>
		[OnDeconstruction]
		public void Deconstruct(out string name, out decimal difficulty) =>
			(name, difficulty) = (Name, Difficulty);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="name">(out parameter) The name.</param>
		/// <param name="difficulty">(out parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(out parameter) The difficulty level.</param>
		[OnDeconstruction]
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevels difficultyLevel) =>
			(name, difficulty, difficultyLevel) = (Name, Difficulty, DifficultyLevel);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="name">(out parameter) The name.</param>
		/// <param name="difficulty">(out parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(out parameter) The difficulty level.</param>
		/// <param name="conclusions">(out parameter) All conclusions.</param>
		[OnDeconstruction]
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevels difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions) =>
			(name, difficulty, difficultyLevel, conclusions) = (Name, Difficulty, DifficultyLevel, Conclusions);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="name">(out parameter) The name.</param>
		/// <param name="difficulty">(out parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(out parameter) The difficulty level.</param>
		/// <param name="conclusions">(out parameter) All conclusions.</param>
		/// <param name="views">(out parameter) All views.</param>
		[OnDeconstruction]
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevels difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions, out IReadOnlyList<View> views) =>
			(name, difficulty, difficultyLevel, conclusions, views) = (Name, Difficulty, DifficultyLevel, Conclusions, Views);

		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public void ApplyTo(Grid grid)
		{
			foreach (var conclusion in Conclusions)
			{
				conclusion.ApplyTo(grid);
			}
		}

		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
