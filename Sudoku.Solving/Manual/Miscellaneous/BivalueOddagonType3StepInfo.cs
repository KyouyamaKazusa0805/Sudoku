using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Miscellaneous
{
	/// <summary>
	/// Provides a usage of <b>bi-value oddagon type 3</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ExtraDigits">All extra digits.</param>
	/// <param name="ExtraCells">All extra cells.</param>
	public sealed record BivalueOddagonType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop, int Digit1, int Digit2,
		short ExtraDigits, in Cells ExtraCells)
		: BivalueOddagonStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.0M + (ExtraCells.Count >> 1) * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BivalueOddagonType3;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: To avoid the structure {Loop} of digits {Digit1 + 1} and {Digit2 + 1} error, " +
				$"the only way is to form the subset (digits {digitsStr} in cells {ExtraCells}) => {elimStr}";
		}
	}
}
