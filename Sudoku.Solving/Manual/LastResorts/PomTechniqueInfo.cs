using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>pattern overlay method</b> (POM) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public sealed record PomTechniqueInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: LastResortTechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override decimal Difficulty => 8.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Pom;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: Digit {digit} => {elimStr}";
		}
	}
}
