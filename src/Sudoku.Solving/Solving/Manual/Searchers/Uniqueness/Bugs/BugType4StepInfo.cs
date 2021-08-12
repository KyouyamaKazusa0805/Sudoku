using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 4 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record BugType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		IReadOnlyList<int> Digits, IReadOnlyList<int> Cells, in ConjugatePair ConjugatePair
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType4;

		[FormatItem]
		private string DigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(Digits).ToString();
		}

		[FormatItem]
		private string CellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(Cells).ToString();
		}

		[FormatItem]
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePair.ToString();
		}
	}
}
