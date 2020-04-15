using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) type 1 technique.
	/// </summary>
	public sealed class UrType1TechniqueInfo : UrTechniqueInfo
	{
		/// <inheritdoc/>
		public UrType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, string typeName,
			int typeCode, int digit1, int digit2, int[] cells, bool isAr)
			: base(conclusions, views, typeName, typeCode, digit1, digit2, cells, isAr)
		{
		}


		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected override string GetAdditional() => string.Empty;
	}
}
