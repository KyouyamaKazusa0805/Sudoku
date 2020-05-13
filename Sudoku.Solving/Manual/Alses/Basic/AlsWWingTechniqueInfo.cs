using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets W-Wing</b> (ALS-W-Wing) technique.
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
		/// <param name="conjugatePair">The conjugate pair.</param>
		/// <param name="wDigitsMask">The W digits mask.</param>
		/// <param name="x">The X digit.</param>
		public AlsWWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Als als1, Als als2, ConjugatePair conjugatePair, short wDigitsMask, int x) : base(conclusions, views) =>
			(Als1, Als2, ConjugatePair, WDigitsMask, XDigit) = (als1, als2, conjugatePair, wDigitsMask, x);


		/// <summary>
		/// Indicates the ALS 1.
		/// </summary>
		public Als Als1 { get; }

		/// <summary>
		/// Indicates the ALS 2.
		/// </summary>
		public Als Als2 { get; }

		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <summary>
		/// Indicates the W digits mask.
		/// </summary>
		public short WDigitsMask { get; }

		/// <summary>
		/// Indicates the X digit.
		/// </summary>
		public int XDigit { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 6.2M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.AlsWWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string wStr = new DigitCollection(WDigitsMask.GetAllSets()).ToString();
			return
				$"{Name}: Two ALSes {Als1}, {Als2} connected by " +
				$"{ConjugatePair}, W = {wStr}, X = {XDigit + 1} => {elimStr}";
		}
	}
}
