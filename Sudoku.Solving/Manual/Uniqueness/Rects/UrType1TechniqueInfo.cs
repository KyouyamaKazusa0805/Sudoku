using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) type 1 technique.
	/// </summary>
	public sealed class UrType1TechniqueInfo : UrTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="isAr">Indicates whether the instance is an AR.</param>
		public UrType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit1, int digit2, int[] cells, bool isAr)
			: base(conclusions, views, isAr ? UrTypeCode.AType1 : UrTypeCode.Type1, digit1, digit2, cells, isAr)
		{
		}


		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected override string? GetAdditional() => null;
	}
}
