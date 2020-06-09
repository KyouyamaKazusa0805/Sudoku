using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 2</b> (BDP) technique.
	/// </summary>
	public sealed class BdpType2TechniqueInfo : BdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="map">The cells used.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="extraDigit">The extra digit.</param>
		public BdpType2TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap map, short digitsMask,
			int extraDigit) : base(conclusions, views, map, digitsMask) => ExtraDigit = extraDigit;


		/// <summary>
		/// Indicates the extra digit.
		/// </summary>
		public int ExtraDigit { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.4M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BdpType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Map).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in cells {cellsStr} with the extra digit {ExtraDigit + 1} => {elimStr}";
		}
	}
}
