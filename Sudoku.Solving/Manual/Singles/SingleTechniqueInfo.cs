using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Provides a usage of <b>single</b> technique.
	/// </summary>
	public abstract class SingleTechniqueInfo : TechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		protected SingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int cell, int digit)
			: base(conclusions, views) => (Cell, Digit) = (cell, digit);


		/// <summary>
		/// Indicates the cell offset.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the difficulty level.
		/// </summary>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Easy;
	}
}
