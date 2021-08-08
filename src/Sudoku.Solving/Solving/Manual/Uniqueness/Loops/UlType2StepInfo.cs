using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 2</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	public sealed record UlType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, in Cells Loop, int ExtraDigit
	) : UlStepInfo(Conclusions, Views, Digit1, Digit2, Loop)
	{
		/// <inheritdoc/>
		public override int Type => 2;

		[FormatItem]
		private string ExtraDigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (ExtraDigit + 1).ToString();
		}


		/// <inheritdoc/>
		public bool Equals(UlType2StepInfo? other) => base.Equals(other);

		/// <inheritdoc/>
		public override int GetHashCode() => base.GetHashCode();
	}
}
