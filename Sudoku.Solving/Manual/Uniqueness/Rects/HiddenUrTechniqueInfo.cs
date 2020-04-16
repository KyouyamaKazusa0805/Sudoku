using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>hidden unique rectangle</b> (HUR) or
	/// <b>hidden avoidable rectangle</b> (HAR) technique.
	/// </summary>
	public sealed class HiddenUrTechniqueInfo : UrPlusTechniqueInfo
	{
		/// <inheritdoc/>
		public HiddenUrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, string typeName,
			int typeCode, int digit1, int digit2, int[] cells,
			IReadOnlyList<ConjugatePair> conjugatePairs, bool isAr)
			: base(conclusions, views, typeName, typeCode, digit1, digit2, cells, conjugatePairs, isAr)
		{
		}


		/// <inheritdoc/>
		public override string Name => $"Hidden {(IsAr ? "Avoidable" : "Unique")} Rectangle {TypeName}";
	}
}
