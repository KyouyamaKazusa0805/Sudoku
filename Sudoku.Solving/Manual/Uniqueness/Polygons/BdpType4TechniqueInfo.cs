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
	public sealed class BdpType4TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="map">The cells used.</param>
		/// <param name="conjugateRegion">The conjugate region.</param>
		/// <param name="extraMask">The extra mask.</param>
		public BdpType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, short digitsMask, GridMap map,
			GridMap conjugateRegion, short extraMask) : base(conclusions, views) =>
			(DigitsMask, ExtraMask, Map, ConjugateRegion) = (digitsMask, extraMask, map, conjugateRegion);


		/// <summary>
		/// Indicates the digits used.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates the mask of digits that is the combination.
		/// </summary>
		public short ExtraMask { get; }

		/// <summary>
		/// Indicates the cells used.
		/// </summary>
		public GridMap Map { get; }

		/// <summary>
		/// The so-called conjugate region. If you do not know what is a "conjugate region",
		/// please read the comments in the method <see cref="BdpTechniqueSearcher.CheckType4(IBag{TechniqueInfo}, IReadOnlyGrid, Pattern, short, short, short, GridMap)"/> for more details.
		/// </summary>
		/// <seealso cref="BdpTechniqueSearcher.CheckType4(IBag{TechniqueInfo}, IReadOnlyGrid, Pattern, short, short, short, GridMap)"/>
		public GridMap ConjugateRegion { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

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
