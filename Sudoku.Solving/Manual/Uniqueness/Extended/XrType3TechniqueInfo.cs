using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 3 technique.
	/// </summary>
	public sealed class XrType3TechniqueInfo : XrTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="typeCode">The type code.</param>
		/// <param name="typeName">The type name.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="extraDigits">All extra digits.</param>
		/// <param name="region">The region.</param>
		public XrType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int typeCode,
			string typeName, IReadOnlyList<int> cells, IReadOnlyList<int> digits,
			IReadOnlyList<int> extraCells, IReadOnlyList<int> extraDigits, int region)
			: base(conclusions, views, typeCode, typeName, cells, digits) =>
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
		protected override string GetAdditional()
		{
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string cellsStr = new CellCollection(ExtraCells).ToString();
			string regionStr = RegionUtils.ToString(Region);
			return $"{digitsStr} in cells {cellsStr} in {regionStr}";
		}
	}
}
