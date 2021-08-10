using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Text;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 4</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record UlType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, in Cells Loop, in ConjugatePair ConjugatePair
	) : UlStepInfo(Conclusions, Views, Digit1, Digit2, Loop)
	{
		/// <inheritdoc/>
		public override int Type => 4;

		[FormatItem]
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePair.ToString();
		}


		/// <inheritdoc/>
		public bool Equals(UlType4StepInfo? other) => base.Equals(other) && ConjugatePair == other.ConjugatePair;

		/// <inheritdoc/>
		public override int GetHashCode() => base.GetHashCode();
	}
}
