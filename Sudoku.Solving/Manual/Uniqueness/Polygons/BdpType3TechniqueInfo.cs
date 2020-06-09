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
	public sealed class BdpType3TechniqueInfo : BdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="map">The cells used.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		/// <param name="extraDigitsMask">The extra digits mask.</param>
		public BdpType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap map, short digitsMask,
			GridMap extraCellsMap, short extraDigitsMask) : base(conclusions, views, map, digitsMask) =>
			(ExtraCells, ExtraDigitsMask) = (extraCellsMap, extraDigitsMask);


		/// <summary>
		/// Indicates the extra cells.
		/// </summary>
		public GridMap ExtraCells { get; }

		/// <summary>
		/// Indicates the extra digits mask.
		/// </summary>
		public short ExtraDigitsMask { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.2M + ExtraCells.Count * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BdpType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Map).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string exDigitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string exCellsStr = new CellCollection(ExtraCells).ToString();
			return
				$"{Name}: {digitsStr} in cells {cellsStr} with the digits {exDigitsStr} in cells {exCellsStr}" +
				$" => {elimStr}";
		}
	}
}
