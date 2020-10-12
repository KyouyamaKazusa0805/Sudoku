using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 3</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	public sealed record BdpType3TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, GridMap Map, short DigitsMask,
		GridMap ExtraCells, short ExtraDigitsMask)
		: BdpTechniqueInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.2M + ExtraCells.Count * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BdpType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = Map.ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			string exDigitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string exCellsStr = ExtraCells.ToString();
			return
				$"{Name}: {digitsStr} in cells {cellsStr} with the digits {exDigitsStr} in cells {exCellsStr}" +
				$" => {elimStr}";
		}
	}
}
