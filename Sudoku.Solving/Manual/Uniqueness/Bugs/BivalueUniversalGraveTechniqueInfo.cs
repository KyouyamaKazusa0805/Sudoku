using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of bivalue universal grave (BUG) technique.
	/// </summary>
	public sealed class BivalueUniversalGraveTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		public BivalueUniversalGraveTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		/// <inheritdoc/>
		public override string Name => "Bivalue Universal Grave (Type 1)";

		/// <inheritdoc/>
		public override decimal Difficulty => 5.6m;

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {elimStr}";
		}
	}
}
