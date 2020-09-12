using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Provides a usage of <b>single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public abstract record SingleTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit)
		: TechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the difficulty level.
		/// </summary>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Easy;
	}
}
