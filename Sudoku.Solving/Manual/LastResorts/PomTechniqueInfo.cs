using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>pattern overlay method</b> (POM) technique.
	/// </summary>
	public sealed class PomTechniqueInfo : LastResortTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		public PomTechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override string Name => "Pattern Overlay";

		/// <inheritdoc/>
		public override decimal Difficulty => 8.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Digit {digit} => {elimStr}";
		}
	}
}
