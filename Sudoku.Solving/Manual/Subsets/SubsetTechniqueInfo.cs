using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>subset</b> technique.
	/// </summary>
	public abstract class SubsetTechniqueInfo : TechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="regionOffset">The region offset.</param>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <param name="digits">The digits.</param>
		protected SubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, IReadOnlyList<int> cellOffsets, IReadOnlyList<int> digits)
			: base(conclusions, views) =>
			(RegionOffset, CellOffsets, Digits) = (regionOffset, cellOffsets, digits);


		/// <summary>
		/// The region offset.
		/// </summary>
		public int RegionOffset { get; }

		/// <summary>
		/// All digits.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// All cell offsets.
		/// </summary>
		public IReadOnlyList<int> CellOffsets { get; }

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;
	}
}
