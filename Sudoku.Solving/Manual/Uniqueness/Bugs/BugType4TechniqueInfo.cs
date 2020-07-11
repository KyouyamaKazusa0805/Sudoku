using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 4 technique.
	/// </summary>
	public sealed class BugType4TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digits">All digits.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public BugType4TechniqueInfo(
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
		public override decimal Difficulty => 5.7M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BugType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in cells {cellsStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
