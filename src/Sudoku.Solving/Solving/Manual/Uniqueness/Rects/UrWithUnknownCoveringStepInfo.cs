using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle + unknown covering</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="TargetCell">The target cell.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	/// <param name="Cells">All UR cells.</param>
	/// <param name="AbsoluteOffset">The absolute offset.</param>
	public sealed record UrWithUnknownCoveringStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, int TargetCell, int ExtraDigit, int[] Cells, int AbsoluteOffset
	) : UrStepInfo(Conclusions, Views, Technique.UrUnknownCovering, Digit1, Digit2, Cells, false, AbsoluteOffset)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.9M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string digitsStr = new DigitCollection(stackalloc int[] { Digit1, Digit2 }).ToString(" or ");
			string targetCellStr = new Cells { TargetCell }.ToString();
			string extraDigitStr = (ExtraDigit + 1).ToString();
			return
				$"unknown covering: Suppose {targetCellStr} is filled with the unknown digit X (X is {digitsStr}), " +
				$"then 4 cells form a UR deadly pattern of digit X and {extraDigitStr}";
		}
	}
}
