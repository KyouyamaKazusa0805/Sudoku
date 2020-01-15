using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	public abstract class SingleTechniqueInfo : TechniqueInfo
	{
		protected SingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		public sealed override DifficultyLevels DifficultyLevel => DifficultyLevels.Easy;
	}
}
