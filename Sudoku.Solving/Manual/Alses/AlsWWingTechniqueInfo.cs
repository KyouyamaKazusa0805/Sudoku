using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets W-Wing</b> technique.
	/// </summary>
	public sealed class AlsWWingTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="als1">The ALS 1.</param>
		/// <param name="als2">The ALS 2.</param>
		/// <param name="w">The W digit.</param>
		/// <param name="x">The X digit.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public AlsWWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Als als1, Als als2, int w, int x, ConjugatePair conjugatePair)
			: base(conclusions, views) =>
			(Als1, Als2, WDigit, XDigit, ConjugatePair) = (als1, als2, w, x, conjugatePair);


		/// <summary>
		/// Indicates the ALS 1.
		/// </summary>
		public Als Als1 { get; }

		/// <summary>
		/// Indicates the ALS 2.
		/// </summary>
		public Als Als2 { get; }

		/// <summary>
		/// Indicates the W digit.
		/// </summary>
		public int WDigit { get; }

		/// <summary>
		/// Indicates the X digit.
		/// </summary>
		public int XDigit { get; }

		/// <summary>
		/// Indicates the conjugate pair in the structure.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override string Name => "Almost Locked Sets W-Wing";

		/// <inheritdoc/>
		public override decimal Difficulty => 6.2M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToSimpleString(Conclusions);
			return
				$"{Name}: Two ALSes {Als1}, {Als2} " +
				$"with conjugate pair {ConjugatePair} (W = {WDigit + 1}, X = {XDigit + 1}) => {elimStr}";
		}
	}
}
