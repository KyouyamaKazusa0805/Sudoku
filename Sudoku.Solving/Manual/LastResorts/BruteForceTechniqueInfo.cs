using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>brute force</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public sealed record BruteForceTechniqueInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: LastResortTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 20.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BruteForce;


		/// <inheritdoc/>
		public override string ToString()
		{
			using var elims = new ConclusionCollection(Conclusions);
			return $"{Name}: {elims.ToString()}";
		}
	}
}
