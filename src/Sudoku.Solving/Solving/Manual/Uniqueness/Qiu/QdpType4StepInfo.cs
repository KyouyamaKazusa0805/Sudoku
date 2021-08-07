using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 4</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record QdpType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Pattern Pattern, in ConjugatePair ConjugatePair
	) : QdpStepInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .2M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.QdpType4;

		[FormatItem]
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePair.ToString();
		}


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: Cells {PatternStr} will be a deadly pattern if another digit in either cells lying on the conjugate pair {ConjStr} is true => {ElimStr}";
	}
}
