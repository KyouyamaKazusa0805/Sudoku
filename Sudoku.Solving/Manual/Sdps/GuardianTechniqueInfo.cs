using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of guardian technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="Guardians">All guardians.</param>
	public sealed record GuardianTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, GridMap Loop, GridMap Guardians)
		: SdpTechniqueInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M + .1M * (Loop.Count >> 1);

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Guardian;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = Loop.ToString();
			string guardians = Guardians.ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			string guardianSingularOrPlural = Guardians.Count == 1 ? "a guardian" : "guardians";
			return $"{Name}: Cells {cellsStr} with {guardianSingularOrPlural} {guardians} => {elimStr}";
		}
	}
}
