using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>grouped bi-value oddagon</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ExtraDigits">The extra digits.</param>
	public sealed record GroupedBivalueOddagonStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Loop, int Digit1, int Digit2, short ExtraDigits
	) : RankTheoryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.3M;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.BivalueOddagon;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.GroupedBivalueOddagon;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string concStr = new ConclusionCollection(Conclusions).ToString();
			string cellsStr = Loop.ToString();
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			return $"The cells {cellsStr} about digits {Digit1 + 1} and {Digit2 + 1} will form an invalid bi-value oddagon structure to make the puzzle no solution when all positions about a digit in digits {digitsStr} in this structure is removed => {concStr}";
		}
	}
}
