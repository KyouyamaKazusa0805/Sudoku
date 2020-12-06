using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 2</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	public sealed record UsType2TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in GridMap Cells, short DigitsMask,
		int ExtraDigit)
		: UsTechniqueInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.UsType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = Cells.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} will form a deadly pattern if " +
				$"the extra digit {ExtraDigit + 1} is all false in the pattern => {elimStr}";
		}
	}
}
