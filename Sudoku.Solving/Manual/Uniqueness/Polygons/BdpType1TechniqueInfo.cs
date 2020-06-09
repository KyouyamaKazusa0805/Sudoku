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
	public sealed class BdpType1TechniqueInfo : BdpTechniqueInfo
	{
		/// <inheritdoc/>
		public BdpType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap map, short digitsMask)
			: base(conclusions, views, map, digitsMask)
		{
		}


		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BdpType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Map).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in cells {cellsStr} => {elimStr}";
		}
	}
}
