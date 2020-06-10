using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 2</b> (US) technique.
	/// </summary>
	public sealed class UsType2TechniqueInfo : UsTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">The cells.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="extraDigit">The extra digit.</param>
		public UsType2TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap cells, short digitsMask,
			int extraDigit) : base(conclusions, views, cells, digitsMask) => ExtraDigit = extraDigit;


		/// <summary>
		/// Indicates the extra digit.
		/// </summary>
		public int ExtraDigit { get; }

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.UsType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} will form a deadly pattern if " +
				$"the extra digit {ExtraDigit + 1} is all false in the pattern => {elimStr}";
		}
	}
}
