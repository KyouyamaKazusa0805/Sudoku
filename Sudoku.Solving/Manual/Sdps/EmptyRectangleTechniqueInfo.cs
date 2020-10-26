using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of empty rectangle technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="Block">The block that the empty rectangle lies in.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record EmptyRectangleTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, int Block,
		in ConjugatePair ConjugatePair)
		: SdpTechniqueInfo(Conclusions, Views, Digit)
	{
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
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: {digit} in {regionStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
