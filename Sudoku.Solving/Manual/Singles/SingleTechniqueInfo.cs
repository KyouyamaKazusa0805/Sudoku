using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Provides a usage of single technique.
	/// </summary>
	public abstract class SingleTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected SingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		/// <summary>
		/// Indicates the difficulty level.
		/// </summary>
		public sealed override DifficultyLevels DifficultyLevel => DifficultyLevels.Easy;
	}
}
