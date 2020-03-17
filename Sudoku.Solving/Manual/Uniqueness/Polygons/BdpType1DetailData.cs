using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Indicates the detail data of BDP type 1.
	/// </summary>
	public sealed class BdpType1DetailData : BdpDetailData
	{
		/// <summary>
		/// Initializes an instance with cells.
		/// </summary>
		/// <param name="cells">Cells.</param>
		/// <param name="digits">Digits.</param>
		public BdpType1DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits) : base(cells, digits)
		{
		}


		/// <inheritdoc/>
		public override int Type => 1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = CellCollection.ToString(Cells);
			string digitsStr = DigitCollection.ToString(Digits);
			return $"{digitsStr} in cells {cellsStr}";
		}
	}
}
