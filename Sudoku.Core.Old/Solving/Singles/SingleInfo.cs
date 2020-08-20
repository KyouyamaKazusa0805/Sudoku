using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Singles
{
	public abstract class SingleInfo : TechniqueInfo
	{
		protected SingleInfo(Conclusion conclusion, ICollection<View> views) : base(conclusion, views)
		{
		}


		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Easy;
	}
}
