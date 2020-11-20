using System;
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
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, in GridMap Loop,
		in GridMap Guardians) : SdpTechniqueInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M + .1M * (Loop.Count + (Guardians.Count >> 1) >> 1);

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Guardian;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public bool Equals(GuardianTechniqueInfo? other) =>
			other is not null && (Loop, Guardians, Digit) == (other.Loop, other.Guardians, other.Digit);

		/// <inheritdoc/>
		public override int GetHashCode() => HashCode.Combine(Loop, Guardians, Digit);


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = Loop.ToString();
			string guardians = Guardians.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string guardianSingularOrPlural = Guardians.Count == 1 ? "a guardian" : "guardians";
			return $"{Name}: Cells {cellsStr} with {guardianSingularOrPlural} {guardians} => {elimStr}";
		}
	}
}
