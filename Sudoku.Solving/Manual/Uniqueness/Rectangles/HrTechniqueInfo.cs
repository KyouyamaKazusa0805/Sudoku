using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of <b>hidden rectangle</b> (HR) technique.
	/// </summary>
	[Obsolete]
	public sealed class HrTechniqueInfo : RectangleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="conjugatePairs">All conjugate pairs.</param>
		public HrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> cells, IReadOnlyList<int> digits,
			IReadOnlyList<ConjugatePair> conjugatePairs)
			: base(conclusions, views, null!) =>
			(Cells, Digits, ConjugatePairs) = (cells, digits, conjugatePairs);


		/// <summary>
		/// Indicates all cells.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates all digits.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// Indicates the digit used in all conjugate pairs.
		/// </summary>
		public int Digit => ConjugatePairs[0].Digit;

		/// <summary>
		/// Indicates all conjugate pairs.
		/// </summary>
		public IReadOnlyList<ConjugatePair> ConjugatePairs { get; }

		/// <inheritdoc/>
		public override string Name => "Hidden Rectangle";

		/// <inheritdoc/>
		public override decimal Difficulty => 4.8M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string cellsStr = CellCollection.ToString(Cells);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {digitsStr} in cells {cellsStr} with conjugate pairs: {ConjugatePairs[0]} and {ConjugatePairs[1]} => {elimStr}";
		}
	}
}
