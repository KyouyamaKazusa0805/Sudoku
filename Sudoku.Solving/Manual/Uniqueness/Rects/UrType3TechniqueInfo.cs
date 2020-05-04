using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) type 3 technique.
	/// </summary>
	public sealed class UrType3TechniqueInfo : UrTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="extraDigits">All extra digits.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="region">The region.</param>
		/// <param name="isNaked">Indicates whether the subset is naked.</param>
		/// <param name="isAr">Indicates whether the specified structure is an AR.</param>
		public UrType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit1, int digit2, int[] cells, IReadOnlyList<int> extraDigits,
			IReadOnlyList<int> extraCells, int region, bool isNaked, bool isAr)
			: base(conclusions, views, UrTypeCode.Type3, digit1, digit2, cells, isAr) =>
			(ExtraDigits, ExtraCells, Region, IsNaked) = (extraDigits, extraCells, region, isNaked);


		/// <summary>
		/// Indicates the extra digits.
		/// </summary>
		public IReadOnlyList<int> ExtraDigits { get; }

		/// <summary>
		/// Indicates the extra cells.
		/// </summary>
		public IReadOnlyList<int> ExtraCells { get; }

		/// <summary>
		/// Indicates whether the specified subset is naked.
		/// </summary>
		public bool IsNaked { get; }

		/// <summary>
		/// Indicates the current region.
		/// </summary>
		public int Region { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => (IsNaked ? 4.5M : 4.6M) + .1M * ExtraDigits.Count;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string cellsStr = new CellCollection(ExtraCells).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return $"{digitsStr} in {(IsNaked ? string.Empty : "only ")}cells {cellsStr} in {regionStr}";
		}
	}
}
