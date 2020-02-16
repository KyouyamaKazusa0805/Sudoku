using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.SingleDigitPatterns
{
	/// <summary>
	/// Provides a usage of empty rectangle technique.
	/// </summary>
	public sealed class EmptyRectangleTechniqueInfo : SingleDigitPatternTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="block">The block.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public EmptyRectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int digit,
			int block, ConjugatePair conjugatePair)
			: base(conclusions, views, digit) =>
			(Block, ConjugatePair) = (block, conjugatePair);


		/// <summary>
		/// The block.
		/// </summary>
		public int Block { get; }

		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override string Name => "Empty Rectangle";

		/// <inheritdoc/>
		public override decimal Difficulty => 4.6m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string regionStr = RegionUtils.ToString(Block);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {digit} in {regionStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
