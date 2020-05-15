using System.Collections.Generic;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Indicates the detail data of UR type 4.
	/// </summary>
	public sealed class BdpType4DetailData : BdpDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="region">The so-called "conjugate region".</param>
		public BdpType4DetailData(IReadOnlyList<int> cells, IReadOnlyList<int> digits, int region)
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
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return $"{digitsStr} in cells {cellsStr} with a region {regionStr}";
		}
	}
}
