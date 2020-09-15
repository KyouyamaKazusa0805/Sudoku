using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>hidden unique rectangle</b> (HUR) or
	/// <b>hidden avoidable rectangle</b> (HAR) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TypeCode">The type code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ConjugatePairs">All conjugate pairs.</param>
	public sealed record HiddenUrTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, int[] Cells, bool IsAvoidable, IReadOnlyList<ConjugatePair> ConjugatePairs)
		: UrPlusTechniqueInfo(
			Conclusions, Views, IsAvoidable ? UrTypeCode.AHidden : UrTypeCode.Hidden,
			Digit1, Digit2, Cells, IsAvoidable, ConjugatePairs);
}
