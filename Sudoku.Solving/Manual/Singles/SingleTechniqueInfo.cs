using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Provides a usage of single technique.
	/// </summary>
	public abstract class SingleTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views of this solving step.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		protected SingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int cellOffset, int digit)
			: base(conclusions, views) => (CellOffset, Digit) = (cellOffset, digit);


		/// <summary>
		/// Indicates the cell offset.
		/// </summary>
		public int CellOffset { get; }

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
