using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>hidden unique rectangle</b> (HUR) or
	/// <b>hidden avoidable rectangle</b> (HAR) technique.
	/// </summary>
	public sealed class HiddenUrTechniqueInfo : UrPlusTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="conjugatePairs">The conjugate pairs.</param>
		/// <param name="isAr">Indicates whether the specified structure is an AR.</param>
		public HiddenUrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit1, int digit2, int[] cells, IReadOnlyList<ConjugatePair> conjugatePairs,
			bool isAr)
			: base(conclusions, views, UrTypeCode.Hidden, digit1, digit2, cells, conjugatePairs, isAr)
		{
		}


		/// <inheritdoc/>
		public override string Name => $"Hidden {(IsAr ? "Avoidable" : "Unique")} Rectangle";
	}
}
