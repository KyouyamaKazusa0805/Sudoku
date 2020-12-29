using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Miscellaneous
{
	/// <summary>
	/// Provides a usage of <b>bi-value oddagon type 1</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ExtraCell">The extra cell.</param>
	public sealed record BivalueOddagonType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop, int Digit1, int Digit2,
		int ExtraCell)
		: BivalueOddagonStepInfo(Conclusions, Views, Loop, Digit1, Digit2), IEquatable<BivalueOddagonType1StepInfo>
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.0M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BivalueOddagonType1;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public bool Equals(BivalueOddagonType1StepInfo? other) =>
			other is not null && Loop == other.Loop && Digit1 == other.Digit1 && Digit2 == other.Digit2;

		/// <inheritdoc/>
		public override int GetHashCode() => (1 << Digit1 | 1 << Digit2) ^ Loop.GetHashCode();


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new Cells { ExtraCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: If the digits {Digit1 + 1} and {Digit2 + 1} in the cell {cellStr} are both removed, " +
				$"the loop {Loop} will form an error structure => {elimStr}";
		}
	}
}
