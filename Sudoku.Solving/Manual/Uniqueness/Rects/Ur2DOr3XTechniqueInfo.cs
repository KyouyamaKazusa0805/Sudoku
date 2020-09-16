using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle + 2D (or 3X)</b> or
	/// <b>avoidable rectangle + 2D (or 3X)</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TypeCode">The type code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ConjugatePairs">All conjugate pairs.</param>
	/// <param name="XDigit">The X digit.</param>
	/// <param name="YDigit">The Y digit.</param>
	/// <param name="XyCell">The cell that only contains X and Y digit.</param>
	public sealed record Ur2DOr3XTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		UrTypeCode TypeCode, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		int XDigit, int YDigit, int XyCell)
		: UrTechniqueInfo(Conclusions, Views, TypeCode, Digit1, Digit2, Cells, IsAvoidable)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.7M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string xyCellStr = new GridMap { XyCell }.ToString();
			return $"X = {XDigit + 1}, Y = {YDigit + 1} and a bi-value cell {xyCellStr}";
		}
	}
}
