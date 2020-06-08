using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 3 technique.
	/// </summary>
	public sealed class XrType3TechniqueInfo : XrTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="extraDigits">All extra digits.</param>
		/// <param name="region">The region.</param>
		public XrType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> cells, IReadOnlyList<int> digits,
			IReadOnlyList<int> extraCells, IReadOnlyList<int> extraDigits, int region)
			: base(conclusions, views, cells, digits) =>
			(ExtraCells, ExtraDigits, Region) = (extraCells, extraDigits, region);


		/// <summary>
		/// Indicates the extra cells.
		/// </summary>
		public IReadOnlyList<int> ExtraCells { get; }

		/// <summary>
		/// Indicates the extra digits.
		/// </summary>
		public IReadOnlyList<int> ExtraDigits { get; }

		/// <summary>
		/// Indicates the region.
		/// </summary>
		public int Region { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M + DifficultyExtra[Size] + .1M * ExtraDigits.Count;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.XrType3;


		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string cellsStr = new CellCollection(ExtraCells).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return $"{digitsStr} in cells {cellsStr} in {regionStr}";
		}
	}
}
