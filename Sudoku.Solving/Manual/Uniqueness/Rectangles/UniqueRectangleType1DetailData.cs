using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Indicates the detail data of UR type 1.
	/// </summary>
	public sealed class UniqueRectangleType1DetailData : UniqueRectangleDetailData
	{
		/// <summary>
		/// Initializes an instance with cells.
		/// </summary>
		/// <param name="cells">Cells.</param>
		/// <param name="digits">Digits.</param>
		public UniqueRectangleType1DetailData(int[] cells, int[] digits) : base(cells, digits)
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
