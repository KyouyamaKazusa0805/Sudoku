using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of bivalue universal grave (BUG) type 4 technique.
	/// </summary>
	public sealed class BivalueUniversalGraveType4TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public BivalueUniversalGraveType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> digits, IReadOnlyList<int> cells, ConjugatePair conjugatePair)
			: base(conclusions, views) =>
			(Digits, Cells, ConjugatePair) = (digits, cells, conjugatePair);


		/// <summary>
		/// Indicates all digits.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// Indicates all cells.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override string Name => "Bivalue Universal Grave (Type 4)";

		/// <inheritdoc/>
		public override decimal Difficulty => 5.7m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string cellsStr = CellCollection.ToString(Cells);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {digitsStr} in cells {cellsStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
