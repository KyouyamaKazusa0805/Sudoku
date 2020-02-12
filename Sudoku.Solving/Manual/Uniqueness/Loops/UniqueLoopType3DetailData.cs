using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Indicates the detail data of UL type 3.
	/// </summary>
	public sealed class UniqueLoopType3DetailData : UniqueLoopDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="subsetDigits">All subset digits.</param>
		/// <param name="subsetCells">All subset cells.</param>
		/// <param name="isNaked">Indicates whether the subset is naked or not.</param>
		public UniqueLoopType3DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits,
			IReadOnlyList<int> subsetDigits, IReadOnlyList<int> subsetCells, bool isNaked)
			: base(cells, digits) =>
			(SubsetDigits, SubsetCells, IsNaked) = (subsetDigits, subsetCells, isNaked);


		/// <inheritdoc/>
		public override int Type => 3;

		/// <summary>
		/// Indicates all subset digits in this pattern.
		/// </summary>
		public IReadOnlyList<int> SubsetDigits { get; }

		/// <summary>
		/// Indicates all subset cells in this pattern.
		/// </summary>
		public IReadOnlyList<int> SubsetCells { get; }

		/// <summary>
		/// Indicates whether this subset is naked or not.
		/// </summary>
		public bool IsNaked { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = CellCollection.ToString(Cells);
			string digitsStr = DigitCollection.ToString(Digits);
			string subsetDigitsStr = DigitCollection.ToString(SubsetDigits);
			string subsetCellsStr = CellCollection.ToString(SubsetCells);
			string subsetType = IsNaked ? "Naked" : "Hidden";
			string subsetName = SubsetUtils.GetNameBy(SubsetCells.Count + 1);
			return $"{digitsStr} in cells {cellsStr} with {subsetType} {subsetName}: digits {subsetDigitsStr} in cells {subsetCellsStr}";
		}
	}
}
