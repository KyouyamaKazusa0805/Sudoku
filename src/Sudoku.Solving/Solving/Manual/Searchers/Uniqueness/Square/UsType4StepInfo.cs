using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 4</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ConjugateRegion">The so-called conjugate region.</param>
	public sealed record UsType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Cells, short DigitsMask, int Digit1, int Digit2, in Cells ConjugateRegion
	) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.UsType4;

		[FormatItem]
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugateRegion.ToString();
		}

		[FormatItem]
		private string Digit1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit1 + 1).ToString();
		}

		[FormatItem]
		private string Digit2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit2 + 1).ToString();
		}
	}
}
