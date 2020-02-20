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
		/// <param name="conjugatePair">The conjugate pair.</param>
		public BorescoperDeadlyPatternType4DetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits, ConjugatePair conjugatePair)
			: base(cells, digits) =>
			ConjugatePair = conjugatePair;


		/// <inheritdoc/>
		public override int Type => 4;

		/// <summary>
		/// Indicates the conjugate pair used.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string cellsStr = CellCollection.ToString(Cells);
			return $"{digitsStr} in cells {cellsStr} with conjugate pair {ConjugatePair}";
		}
	}
}
