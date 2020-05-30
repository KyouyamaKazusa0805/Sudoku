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
	public sealed class BdpType1TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="map">The cells used.</param>
		public BdpType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, short digitsMask, GridMap map)
			: base(conclusions, views) => (DigitsMask, Map) = (digitsMask, map);


		/// <summary>
		/// Indicates the digits used.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates the cells used.
		/// </summary>
		public GridMap Map { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.3M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

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
