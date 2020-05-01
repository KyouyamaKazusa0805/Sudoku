using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Indicates the detail data of UL type 1.
	/// </summary>
	public sealed class UlType1DetailData : UlDetailData
	{
		/// <inheritdoc/>
		public UlType1DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits) : base(cells, digits)
		{
		}


		/// <inheritdoc/>
		public override int Type => 1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Cells).ToString();
			string digitsStr = new DigitCollection(Digits).ToString();
			return $"{digitsStr} in cells {cellsStr}";
		}
	}
}
