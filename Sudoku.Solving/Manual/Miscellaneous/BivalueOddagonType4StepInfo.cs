using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Miscellaneous
{
	/// <summary>
	/// Provides a usage of <b>bi-value oddagon type 4</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record BivalueOddagonType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop, int Digit1, int Digit2,
		in ConjugatePair ConjugatePair) : BivalueOddagonStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BivalueOddagonType4;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: The loop {Loop} of digits {Digit1 + 1} and {Digit2 + 1} " +
				$"with the conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
