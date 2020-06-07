using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of empty rectangle technique.
	/// </summary>
	public sealed class EmptyRectangleTechniqueInfo : SdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit">The digit.</param>
		/// <param name="block">The block.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public EmptyRectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int digit,
			int block, ConjugatePair conjugatePair) : base(conclusions, views, digit) =>
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
		public override decimal Difficulty => 4.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.EmptyRectangle;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string regionStr = new RegionCollection(Block).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digit} in {regionStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
