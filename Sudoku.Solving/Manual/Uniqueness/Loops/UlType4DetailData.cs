using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Indicates the detail data of UL type 4.
	/// </summary>
	public sealed class UlType4DetailData : UlDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public UlType4DetailData(
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
		public override bool Equals(UlDetailData other) =>
			other is UlType4DetailData comparer && ConjugatePair == comparer.ConjugatePair;

		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			return $"{digitsStr} in cells {cellsStr} with conjugate pair {ConjugatePair}";
		}
	}
}
