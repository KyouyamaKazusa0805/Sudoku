using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) with wings technique.
	/// </summary>
	public sealed class UrWithWingTechniqueInfo : UrTechniqueInfo
	{
		/// <summary>
		/// Indicates the difficulty rating extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = { .2M, .3M, .5M };


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="typeCode">The type code.</param>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="extraCells">The extra cells.</param>
		/// <param name="extraDigits">The extra digits.</param>
		/// <param name="pivots">The pivot cells.</param>
		/// <param name="isAr">Indicates whether the specified structure forms an AR.</param>
		public UrWithWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, UrTypeCode typeCode,
			int digit1, int digit2, int[] cells, IEnumerable<int> extraCells,
			IEnumerable<int> extraDigits, IEnumerable<int> pivots, bool isAr)
			: base(conclusions, views, typeCode, digit1, digit2, cells, isAr) =>
			(ExtraCells, ExtraDigits, Pivots) = (extraCells, extraDigits, pivots);


		/// <summary>
		/// Indicates the extra cells.
		/// </summary>
		public IEnumerable<int> ExtraCells { get; }

		/// <summary>
		/// Indicates the extra digits.
		/// </summary>
		public IEnumerable<int> ExtraDigits { get; }

		/// <summary>
		/// Indicates the pivot cells.
		/// </summary>
		public IEnumerable<int> Pivots { get; }

		/// <inheritdoc/>
		public override decimal Difficulty =>
			4.4M + (IsAr ? .1M : 0) + DifficultyExtra[TypeCode - UrTypeCode.XyWing];

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;

		/// <inheritdoc/>
		public override string Name =>
			$"{(IsAr ? "Avoidable" : "Unique")} Rectangle {TypeCode.GetCustomName()!}";


		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			string pivotsStr = new CellCollection(Pivots).ToString();
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string cellsStr = new CellCollection(ExtraCells).ToString();
			return $"pivots: {pivotsStr}, with digits: {digitsStr} in cells {cellsStr}";
		}
	}
}
