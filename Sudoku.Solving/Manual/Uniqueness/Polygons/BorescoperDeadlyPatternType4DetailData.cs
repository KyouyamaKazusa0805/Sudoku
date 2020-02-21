using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Indicates the detail data of UR type 4.
	/// </summary>
	public sealed class BorescoperDeadlyPatternType4DetailData : BorescoperDeadlyPatternDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="region">The so-called "conjugate region".</param>
		public BorescoperDeadlyPatternType4DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits, int region)
			: base(cells, digits) => Region = region;


		/// <inheritdoc/>
		public override int Type => 4;

		/// <summary>
		/// Indicates the conjugate pair used.
		/// </summary>
		public int Region { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string cellsStr = CellCollection.ToString(Cells);
			string regionStr = RegionUtils.ToString(Region);
			return $"{digitsStr} in cells {cellsStr} with a region {regionStr}";
		}
	}
}
