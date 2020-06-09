using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 2</b> (QDP) technique.
	/// </summary>
	public sealed class QdpType2TechniqueInfo : QdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="pattern">The pattern.</param>
		/// <param name="extraDigit">The extra digit.</param>
		public QdpType2TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Pattern pattern, int extraDigit)
			: base(conclusions, views, pattern) => ExtraDigit = extraDigit;


		/// <summary>
		/// Indicates the extra digit.
		/// </summary>
		public int ExtraDigit { get; }

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.QdpType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = new CellCollection(Pattern.FullMap).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Cells will be a deadly pattern {patternStr} " +
				$"if the extra digits {ExtraDigit + 1} in pair cells is both false => {elimStr}";
		}
	}
}
