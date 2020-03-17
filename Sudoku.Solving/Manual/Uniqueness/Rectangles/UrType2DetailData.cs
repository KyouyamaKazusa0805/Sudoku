using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Indicates the detail data of UR type 2.
	/// </summary>
	public sealed class UrType2DetailData : UrDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="extraDigit">The extra digit.</param>
		/// <param name="isType5">
		/// Indicates whether this type is the variant of type 2.
		/// </param>
		public UrType2DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits, int extraDigit, bool isType5)
			: base(cells, digits) =>
			(ExtraDigit, IsType5) = (extraDigit, isType5);


		/// <summary>
		/// Indicates the extra digit in this pattern.
		/// </summary>
		public int ExtraDigit { get; }

		/// <summary>
		/// Indicates whether this type is the variant of type 2
		/// (pattern with diagonal extra digits).
		/// </summary>
		public bool IsType5 { get; }

		/// <inheritdoc/>
		public override int Type => IsType5 ? 5 : 2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			int extraDigit = ExtraDigit + 1;
			string cellsStr = CellCollection.ToString(Cells);
			return $"{digitsStr} in cells {cellsStr} with extra digit {extraDigit}";
		}
	}
}
