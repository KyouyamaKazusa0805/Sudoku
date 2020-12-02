using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;

namespace Sudoku.Solving
{
	partial record TechniqueInfo
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		public void Deconstruct(out string name, out decimal difficulty)
		{
			name = Name;
			difficulty = Difficulty;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		public void Deconstruct(out string name, out decimal difficulty, out DifficultyLevel difficultyLevel)
		{
			name = Name;
			difficulty = Difficulty;
			difficultyLevel = DifficultyLevel;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		/// <param name="conclusions">(<see langword="out"/> parameter) All conclusions.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions)
		{
			name = Name;
			difficulty = Difficulty;
			difficultyLevel = DifficultyLevel;
			conclusions = Conclusions;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		/// <param name="conclusions">(<see langword="out"/> parameter) All conclusions.</param>
		/// <param name="views">(<see langword="out"/> parameter) All views.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions, out IReadOnlyList<View> views)
		{
			name = Name;
			difficulty = Difficulty;
			difficultyLevel = DifficultyLevel;
			conclusions = Conclusions;
			views = Views;
		}
	}
}
