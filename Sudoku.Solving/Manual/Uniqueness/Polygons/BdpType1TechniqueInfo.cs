using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 1</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	public sealed record BdpType1TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, GridMap Map, short DigitsMask)
		: BdpTechniqueInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BdpType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = Map.ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: {digitsStr} in cells {cellsStr} => {elimStr}";
		}
	}
}
