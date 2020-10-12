using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 3</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	public sealed record UsType3TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, GridMap Cells, short DigitsMask,
		short ExtraDigitsMask, IReadOnlyList<int> ExtraCells)
		: UsTechniqueInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + ExtraDigitsMask.PopCount() * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.UsType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = Cells.ToString();
			string subsetDigitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string subsetCellsStr = new GridMap(ExtraCells).ToString();
			string subsetName = SubsetNames[ExtraCells.Count + 1];
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} can be avoid to form a deadly pattern " +
				$"if and only if the digits {subsetDigitsStr} in cells {subsetCellsStr} " +
				$"form a naked {subsetName} => {elimStr}";
		}
	}
}
