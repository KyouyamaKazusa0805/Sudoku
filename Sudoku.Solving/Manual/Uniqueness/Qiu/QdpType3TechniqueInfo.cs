using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 3</b> (QDP) technique.
	/// </summary>
	public sealed class QdpType3TechniqueInfo : QdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="pattern">The pattern.</param>
		/// <param name="extraDigitsMask">The extra digits mask.</param>
		/// <param name="extraCells">The extra cells.</param>
		public QdpType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Pattern pattern,
			short extraDigitsMask, IReadOnlyList<int> extraCells) : base(conclusions, views, pattern) =>
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
		public override TechniqueCode TechniqueCode => TechniqueCode.QdpType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = new CellCollection(Pattern.FullMap).ToString();
			string digitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(ExtraCells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string subsetName = SubsetNames[ExtraCells.Count + 1].ToLower();
			return
				$"{Name}: Cells {patternStr} will not be a deadly pattern " +
				$"if and only if digits {digitsStr} in cells {cellsStr} is a naked {subsetName} => {elimStr}";
		}
	}
}
