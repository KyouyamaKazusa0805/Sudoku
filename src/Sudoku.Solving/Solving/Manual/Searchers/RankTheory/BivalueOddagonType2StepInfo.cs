using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>bi-value oddagon type 2</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	public sealed record BivalueOddagonType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Loop, int Digit1, int Digit2, int ExtraDigit
	) : BivalueOddagonStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BivalueOddagonType2;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		[FormatItem]
		private string ExtraDigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (ExtraDigit + 1).ToString();
		}

		[FormatItem]
		private string LoopStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Loop.ToString();
		}
	}
}
