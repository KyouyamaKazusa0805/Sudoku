using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	/// <summary>
	/// Provides a usage of <b>reverse bi-value universal grave type 1</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">All cells used.</param>
	/// <param name="Digit1">The digit 1 used.</param>
	/// <param name="Digit2">The digit 2 used.</param>
	/// <param name="Anchor">Indicates the anchor cell that produces the conclusion.</param>
	/// <param name="AnchorLastDigit">
	/// Indicates the digit when the anchor cell is filled with it, then the structure may
	/// form a deadly pattern.
	/// </param>
	public sealed record ReverseBugType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop,
		int Digit1, int Digit2, int Anchor, int AnchorLastDigit
	) : ReverseBugStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.6M;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			Loop.Count == 4 ? Technique.ReverseUrType1 : Technique.ReverseUlType1;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		[FormatItem]
		private string AnchorStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { Anchor }.ToString();
		}

		[FormatItem]
		private string AnchorDigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (AnchorLastDigit + 1).ToString();
		}
	}
}
