using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square</b> (US) technique.
	/// </summary>
	public abstract class UsTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">The cells.</param>
		/// <param name="digitsMask">The digits mask.</param>
		protected UsTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap cells, short digitsMask)
			: base(conclusions, views) => (Cells, DigitsMask) = (cells, digitsMask);


		/// <summary>
		/// Indicates the cells.
		/// </summary>
		public GridMap Cells { get; }

		/// <summary>
		/// Indicates the digits.
		/// </summary>
		public short DigitsMask { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.3M;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public abstract override TechniqueCode TechniqueCode { get; }


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
