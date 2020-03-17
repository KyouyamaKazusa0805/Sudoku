using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Indicates the detail data of UR type 6.
	/// </summary>
	public sealed class UrType6DetailData : UrDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="conjugatePairs">The conjugate pairs.</param>
		public UrType6DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits, ConjugatePair[] conjugatePairs)
			: base(cells, digits) =>
			ConjugatePairs = conjugatePairs;


		/// <inheritdoc/>
		public override int Type => 6;

		/// <summary>
		/// Indicates all conjugate pairs.
		/// </summary>
		public ConjugatePair[] ConjugatePairs { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string cellsStr = CellCollection.ToString(Cells);
			var (cp1, cp2) = (ConjugatePairs[0], ConjugatePairs[1]);
			return $"{digitsStr} in cells {cellsStr} with conjugate pair {cp1} and {cp2}";
		}
	}
}
