using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of pattern overlay method (POM) technique.
	/// </summary>
	public sealed class PatternOverlayMethodTechniqueInfo : LastResortTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		public PatternOverlayMethodTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override string Name => "Pattern overlay method";

		/// <inheritdoc/>
		public override decimal Difficulty => 8.5m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: Digit {digit} => {elimStr}";
		}
	}
}
