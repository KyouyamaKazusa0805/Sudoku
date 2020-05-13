using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 1 technique.
	/// </summary>
	public sealed class XrType1TechniqueInfo : XrTechniqueInfo
	{
		/// <inheritdoc/>
		public XrType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int typeCode,
			string typeName, IReadOnlyList<int> cells, IReadOnlyList<int> digits)
			: base(conclusions, views, typeCode, typeName, cells, digits)
		{
		}


		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M + DifficultyExtra[Cells.Count];

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.XrType1;


		/// <inheritdoc/>
		protected override string? GetAdditional() => null;
	}
}
