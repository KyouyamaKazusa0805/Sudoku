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
	public sealed class BdpType3TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="map">The cells used.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		/// <param name="extraDigitsMask">The extra digits mask.</param>
		public BdpType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, short digitsMask, GridMap map,
			GridMap extraCellsMap, short extraDigitsMask) : base(conclusions, views) =>
			(DigitsMask, Map, ExtraCells, ExtraDigitsMask) = (digitsMask, map, extraCellsMap, extraDigitsMask);


		/// <summary>
		/// Indicates the digits used.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates the cells used.
		/// </summary>
		public GridMap Map { get; }

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
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

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
