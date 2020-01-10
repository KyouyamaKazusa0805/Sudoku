using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	public abstract class SingleTechniqueInfo : TechniqueInfo
	{
		protected SingleTechniqueInfo(
			ICollection<Conclusion> conclusions, ICollection<View> views)
			: base(conclusions, views)
		{
		}


		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Easy;
	}
}
