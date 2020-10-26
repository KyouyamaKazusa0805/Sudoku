using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Provides a usage of <b>W-Wing</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="StartCell">The start cell.</param>
	/// <param name="EndCell">The end cell.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record WWingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int StartCell, int EndCell,
		in ConjugatePair ConjugatePair) : IrregularWingTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.4M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.WWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string startCellStr = new GridMap { StartCell }.ToString();
			string endCellStr = new GridMap { EndCell }.ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: {startCellStr} to {endCellStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
