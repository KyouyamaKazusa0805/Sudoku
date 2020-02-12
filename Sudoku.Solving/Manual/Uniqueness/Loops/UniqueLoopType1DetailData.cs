using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Indicates the detail data of UL type 1.
	/// </summary>
	public sealed class UniqueLoopType1DetailData : UniqueLoopDetailData
	{
		/// <inheritdoc/>
		public UniqueLoopType1DetailData(
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
