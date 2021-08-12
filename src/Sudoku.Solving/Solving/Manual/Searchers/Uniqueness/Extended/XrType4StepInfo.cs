using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 4 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record XrType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Cells, short DigitsMask, in ConjugatePair ConjugatePair
	) : XrStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.XrType4;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		[FormatItem]
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePair.ToString();
		}
	}
}
