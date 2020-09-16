using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) with wings technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TypeCode">The type code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	/// <param name="ExtraDigits">The extra digits.</param>
	/// <param name="Pivots">The pivot cells.</param>
	public sealed record UrWithWingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		UrTypeCode TypeCode, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		IEnumerable<int> ExtraCells, IEnumerable<int> ExtraDigits, IEnumerable<int> Pivots)
		: UrTechniqueInfo(Conclusions, Views, TypeCode, Digit1, Digit2, Cells, IsAvoidable)
	{
		/// <summary>
		/// Indicates the difficulty rating extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = { .2M, .3M, .5M };


		/// <inheritdoc/>
		public override decimal Difficulty =>
			4.4M + (IsAvoidable ? .1M : 0) +
			DifficultyExtra[TypeCode - (IsAvoidable ? UrTypeCode.AXyWing : UrTypeCode.XyWing)];

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			string pivotsStr = new GridMap(Pivots).ToString();
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string cellsStr = new GridMap(ExtraCells).ToString();
			return $"pivots: {pivotsStr}, with digits: {digitsStr} in cells {cellsStr}";
		}
	}
}
