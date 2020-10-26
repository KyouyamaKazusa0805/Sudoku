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
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	public sealed record QdpType3TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Pattern Pattern,
		short ExtraDigitsMask, IReadOnlyList<int> ExtraCells) : QdpTechniqueInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + ExtraDigitsMask.PopCount() * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.QdpType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = Pattern.FullMap.ToString();
			string digitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string cellsStr = new GridMap(ExtraCells).ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			string subsetName = SubsetNames[ExtraCells.Count + 1].ToLower();
			return
				$"{Name}: Cells {patternStr} won't be a deadly pattern " +
				$"if and only if digits {digitsStr} in cells {cellsStr} is a naked {subsetName} => {elimStr}";
		}
	}
}
