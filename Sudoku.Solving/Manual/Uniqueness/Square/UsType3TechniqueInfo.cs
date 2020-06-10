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
	public sealed class UsType3TechniqueInfo : UsTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">The cells.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="extraDigitsMask">The extra digits mask.</param>
		/// <param name="extraCells">The extra cells.</param>
		public UsType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap cells, short digitsMask,
			short extraDigitsMask, IReadOnlyList<int> extraCells) : base(conclusions, views, cells, digitsMask) =>
			(ExtraDigitsMask, ExtraCells) = (extraDigitsMask, extraCells);


		/// <summary>
		/// Indicates the extra digits mask.
		/// </summary>
		public short ExtraDigitsMask { get; }

		/// <summary>
		/// Indicates the extra cells.
		/// </summary>
		public IReadOnlyList<int> ExtraCells { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + ExtraDigitsMask.CountSet() * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.UsType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string subsetDigitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string subsetCellsStr = new CellCollection(ExtraCells).ToString();
			string subsetName = SubsetNames[ExtraCells.Count + 1];
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} can be avoid to form a deadly pattern " +
				$"if and only if the digits {subsetDigitsStr} in cells {subsetCellsStr} " +
				$"form a naked {subsetName} => {elimStr}";
		}
	}
}
