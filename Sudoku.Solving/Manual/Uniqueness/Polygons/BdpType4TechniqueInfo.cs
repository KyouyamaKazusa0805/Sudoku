using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 4</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ConjugateRegion">
	/// The so-called conjugate region. If you do not know what is a "conjugate region",
	/// please read the comments in the method
	/// <see cref="BdpTechniqueSearcher.CheckType4(IList{TechniqueInfo}, Grid, Pattern, short, short, short, GridMap)"/>
	/// for more details.
	/// </param>
	/// <param name="ExtraMask">Indicates the mask of digits that is the combination.</param>
	/// <seealso cref="BdpTechniqueSearcher.CheckType4(IList{TechniqueInfo}, Grid, Pattern, short, short, short, GridMap)"/>
	public sealed record BdpType4TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, GridMap Map, short DigitsMask,
		GridMap ConjugateRegion, short ExtraMask)
		: BdpTechniqueInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BdpType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string combStr = new DigitCollection(ExtraMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Map).ToString();
			string conjRegion = new CellCollection(ConjugateRegion).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {digitsStr} in cells {cellsStr} with the conjugate region {conjRegion} " +
				$"of the extra digits {combStr} => {elimStr}";
		}
	}
}
